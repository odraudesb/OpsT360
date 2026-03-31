#if ANDROID
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Runtime;
using Android.Util;
using JClass = Java.Lang.Class;
using Throwable = Java.Lang.Throwable;
using AndroidApplication = Android.App.Application;

namespace OpsT360.Services
{
    public partial class RfidScannerService
    {
        private const string RfidImplVersion = "RFIDv2026.03.11.13";
        private const string LogTag = "OpsT360.RFID";

        private readonly SemaphoreSlim _rfidOpLock = new(1, 1);

        private static readonly object _readerSync = new();

        private static IntPtr _managerClass = IntPtr.Zero;
        private static IntPtr _manager = IntPtr.Zero;
        private static string _managerClassName = "com.handheld.uhfr.UHFRManager";

        private static readonly string[] CandidateManagerClasses =
        {
            "com.handheld.uhfr.UHFRManager"
        };

        public static string PlatformEnsureReaderOpened()
        {
            lock (_readerSync)
            {
                try
                {
                    if (_managerClass != IntPtr.Zero && _manager != IntPtr.Zero)
                        return $"[{RfidImplVersion}] Reader ya estaba abierto.";

                    if (!TryLoadManagerClass(out var javaClassName, out var managerClass, out var detail))
                        return $"[{RfidImplVersion}] No se pudo cargar clase RFID. {detail}";

                    var sigClass = javaClassName.Replace('.', '/');
                    var getInstance = JNIEnv.GetStaticMethodID(managerClass, "getInstance", $"()L{sigClass};");
                    if (getInstance == IntPtr.Zero)
                    {
                        DeleteGlobalRefSafe(managerClass);
                        return $"[{RfidImplVersion}] No existe getInstance() en {javaClassName}.";
                    }

                    var managerLocal = JNIEnv.CallStaticObjectMethod(managerClass, getInstance);
                    if (managerLocal == IntPtr.Zero)
                    {
                        DeleteGlobalRefSafe(managerClass);
                        return $"[{RfidImplVersion}] getInstance() devolvió null en {javaClassName}.";
                    }

                    _managerClassName = javaClassName;
                    _managerClass = managerClass;
                    _manager = JNIEnv.NewGlobalRef(managerLocal);
                    DeleteLocalRefSafe(managerLocal);

                    return $"[{RfidImplVersion}] Reader abierto correctamente: {_managerClassName}";
                }
                catch (Throwable ex)
                {
                    return $"[{RfidImplVersion}] Error abriendo reader: {ex.GetType().Name}: {ex.Message}";
                }
                catch (Exception ex)
                {
                    return $"[{RfidImplVersion}] Error abriendo reader: {ex.GetType().Name}: {ex.Message}";
                }
            }
        }

        public static void PlatformCloseReader()
        {
            lock (_readerSync)
            {
                try
                {
                    if (_managerClass != IntPtr.Zero && _manager != IntPtr.Zero)
                    {
                        var closeVoid = JNIEnv.GetMethodID(_managerClass, "close", "()V");
                        if (closeVoid != IntPtr.Zero)
                        {
                            JNIEnv.CallVoidMethod(_manager, closeVoid);
                        }
                        else
                        {
                            var closeBool = JNIEnv.GetMethodID(_managerClass, "close", "()Z");
                            if (closeBool != IntPtr.Zero)
                                JNIEnv.CallBooleanMethod(_manager, closeBool);
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    DeleteGlobalRefSafe(_manager);
                    DeleteGlobalRefSafe(_managerClass);
                    _manager = IntPtr.Zero;
                    _managerClass = IntPtr.Zero;
                }
            }
        }

        private partial async Task<RfidReadResult> StartAntennaPlatformAsync(CancellationToken cancellationToken)
        {
            await _rfidOpLock.WaitAsync(cancellationToken);
            try
            {
                string openMsg;
                lock (_readerSync)
                {
                    openMsg = PlatformEnsureReaderOpened();
                }

                LogStep(openMsg);

                if (_managerClass == IntPtr.Zero || _manager == IntPtr.Zero)
                    return RfidReadResult.Fail(openMsg);

                string powerDetail;
                lock (_readerSync)
                {
                    powerDetail = TrySetPower(_managerClass, _manager, 30, 30);
                }

                LogStep($"StartAntenna setPower={powerDetail}");
                return new RfidReadResult(true, "ANTENNA_READY", null, $"[{RfidImplVersion}] Antena RFID lista.");
            }
            catch (OperationCanceledException)
            {
                return RfidReadResult.Fail($"[{RfidImplVersion}] Activación RFID cancelada por timeout.");
            }
            catch (Exception ex)
            {
                return RfidReadResult.Fail($"[{RfidImplVersion}] Error activando RFID: {ex.Message}");
            }
            finally
            {
                _rfidOpLock.Release();
            }
        }

        private partial async Task<RfidReadResult> TryReadSingleEpcPlatformAsync(CancellationToken cancellationToken)
        {
            await _rfidOpLock.WaitAsync(cancellationToken);
            try
            {
                string openMsg;
                lock (_readerSync)
                {
                    openMsg = PlatformEnsureReaderOpened();
                }

                LogStep(openMsg);

                if (_managerClass == IntPtr.Zero || _manager == IntPtr.Zero)
                    return RfidReadResult.Fail(openMsg);

                cancellationToken.ThrowIfCancellationRequested();

                string powerDetail;
                (string? Epc, string? Tid) tagData;

                lock (_readerSync)
                {
                    powerDetail = TrySetPower(_managerClass, _manager, 30, 30);
                    LogStep($"setPower={powerDetail}");
                    tagData = (null, null);
                    for (var attempt = 1; attempt <= 6 && string.IsNullOrWhiteSpace(tagData.Epc); attempt++)
                    {
                        var tags = TryInventoryWithTidPreferred(_managerClass, _manager);
                        tagData = tags == IntPtr.Zero ? (null, null) : TryExtractBestTag(tags);

                        if (string.IsNullOrWhiteSpace(tagData.Epc))
                            Thread.Sleep(80);
                    }
                }

                if (string.IsNullOrWhiteSpace(tagData.Epc))
                    return RfidReadResult.Fail($"[{RfidImplVersion}] Reader abierto y potencia aplicada, pero no se leyó ningún tag. Acerca más el sticker RFID.");

                if (string.IsNullOrWhiteSpace(tagData.Tid))
                {
                    var fallbackTid = TryReadTidMemoryBank(_managerClass, _manager);
                    if (!string.IsNullOrWhiteSpace(fallbackTid))
                        tagData = (tagData.Epc, fallbackTid);
                }

                LogStep($"TryReadSingleEpc ok -> EPC={tagData.Epc} | TID={(string.IsNullOrWhiteSpace(tagData.Tid) ? "(vacío)" : tagData.Tid)}");
                return RfidReadResult.Ok(tagData.Epc!, tagData.Tid);
            }
            catch (OperationCanceledException)
            {
                return RfidReadResult.Fail($"[{RfidImplVersion}] Lectura RFID cancelada por timeout.");
            }
            catch (Exception ex)
            {
                return RfidReadResult.Fail($"[{RfidImplVersion}] Error leyendo RFID: {ex.Message}");
            }
            finally
            {
                _rfidOpLock.Release();
            }
        }

        private partial async Task<RfidBatchReadResult> TryReadDistinctEpcsPlatformAsync(int maxCount, CancellationToken cancellationToken)
        {
            if (maxCount <= 0)
                return RfidBatchReadResult.Fail($"[{RfidImplVersion}] maxCount debe ser mayor a cero.");

            await _rfidOpLock.WaitAsync(cancellationToken);
            try
            {
                string openMsg;
                lock (_readerSync)
                {
                    openMsg = PlatformEnsureReaderOpened();
                }

                LogStep(openMsg);

                if (_managerClass == IntPtr.Zero || _manager == IntPtr.Zero)
                    return RfidBatchReadResult.Fail(openMsg);

                var uniqueEpcs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var maxAttempts = Math.Max(10, maxCount * 6);

                for (var attempt = 1; attempt <= maxAttempts && uniqueEpcs.Count < maxCount; attempt++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    lock (_readerSync)
                    {
                        var powerDetail = TrySetPower(_managerClass, _manager, 30, 30);
                        LogStep($"batch/setPower[{attempt}]={powerDetail}");

                        var tags = TryInventoryWithTidPreferred(_managerClass, _manager);
                        if (tags != IntPtr.Zero)
                        {
                            foreach (var epc in TryExtractUniqueEpcs(tags, maxCount))
                            {
                                if (!string.IsNullOrWhiteSpace(epc))
                                    uniqueEpcs.Add(epc.Trim().ToUpperInvariant());

                                if (uniqueEpcs.Count >= maxCount)
                                    break;
                            }
                        }
                    }

                    if (uniqueEpcs.Count < maxCount)
                        await Task.Delay(120, cancellationToken);
                }

                if (uniqueEpcs.Count == 0)
                    return RfidBatchReadResult.Fail($"[{RfidImplVersion}] No se detectaron tags RFID en inventario.");

                var collected = uniqueEpcs.Take(maxCount).ToList();
                LogStep($"TryReadDistinctEpcs ok -> {string.Join(",", collected)}");
                return RfidBatchReadResult.Ok(collected, $"[{RfidImplVersion}] EPCs detectados: {collected.Count}/{maxCount}");
            }
            catch (OperationCanceledException)
            {
                return RfidBatchReadResult.Fail($"[{RfidImplVersion}] Lectura múltiple RFID cancelada por timeout.");
            }
            catch (Exception ex)
            {
                return RfidBatchReadResult.Fail($"[{RfidImplVersion}] Error leyendo múltiples EPC: {ex.Message}");
            }
            finally
            {
                _rfidOpLock.Release();
            }
        }

        private static bool TryLoadManagerClass(out string javaClassName, out IntPtr classHandle, out string detail)
        {
            javaClassName = string.Empty;
            classHandle = IntPtr.Zero;

            var errors = new StringBuilder();

            try
            {
                var loader = AndroidApplication.Context?.ClassLoader;
                if (loader == null)
                {
                    detail = "AndroidApplication.Context.ClassLoader es null.";
                    return false;
                }

                foreach (var candidate in CandidateManagerClasses)
                {
                    try
                    {
                        using var clazz = JClass.ForName(candidate, false, loader);
                        if (clazz == null)
                        {
                            errors.AppendLine($"{candidate} -> null");
                            continue;
                        }

                        javaClassName = candidate;
                        classHandle = JNIEnv.NewGlobalRef(clazz.Handle);
                        detail = $"Clase cargada: {candidate}";
                        return true;
                    }
                    catch (Throwable ex)
                    {
                        errors.AppendLine($"{candidate} -> {ex.GetType().Name}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine($"{candidate} -> {ex.GetType().Name}: {ex.Message}");
                    }
                }

                detail = errors.Length == 0
                    ? "No se pudo cargar ninguna clase candidata."
                    : errors.ToString();

                return false;
            }
            catch (Throwable ex)
            {
                detail = $"Error cargando clase RFID: {ex.GetType().Name}: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                detail = $"Error cargando clase RFID: {ex.GetType().Name}: {ex.Message}";
                return false;
            }
        }

        private static string TrySetPower(IntPtr managerClass, IntPtr manager, int readPower, int writePower)
        {
            var setPower = JNIEnv.GetMethodID(managerClass, "setPower", "(II)Lcom/uhf/api/cls/Reader$READER_ERR;");
            if (setPower == IntPtr.Zero)
                return "MethodNotFound";

            var args = new JValue[] { new JValue(readPower), new JValue(writePower) };
            var result = JNIEnv.CallObjectMethod(manager, setPower, args);
            var name = ReadEnumName(result) ?? "Unknown";
            DeleteLocalRefSafe(result);
            return name;
        }

        private static IntPtr TryInventorySimple(IntPtr managerClass, IntPtr manager)
        {
            var method = JNIEnv.GetMethodID(managerClass, "tagInventoryByTimer", "(S)Ljava/util/List;");
            if (method != IntPtr.Zero)
            {
                var args = new JValue[] { new JValue((short)50) };
                return JNIEnv.CallObjectMethod(manager, method, args);
            }

            method = JNIEnv.GetMethodID(managerClass, "tagEpcTidInventoryByTimer", "(S)Ljava/util/List;");
            if (method != IntPtr.Zero)
            {
                var args = new JValue[] { new JValue((short)50) };
                return JNIEnv.CallObjectMethod(manager, method, args);
            }

            return IntPtr.Zero;
        }

        private static IntPtr TryInventoryWithTidPreferred(IntPtr managerClass, IntPtr manager)
        {
            var method = JNIEnv.GetMethodID(managerClass, "tagEpcTidInventoryByTimer", "(S)Ljava/util/List;");
            if (method != IntPtr.Zero)
            {
                var args = new JValue[] { new JValue((short)50) };
                return JNIEnv.CallObjectMethod(manager, method, args);
            }

            return TryInventorySimple(managerClass, manager);
        }

        private static (string? Epc, string? Tid) TryExtractBestTag(IntPtr listHandle)
        {
            var listClass = JNIEnv.FindClass("java/util/List");
            if (listClass == IntPtr.Zero)
                return (null, null);

            var sizeMethod = JNIEnv.GetMethodID(listClass, "size", "()I");
            var getMethod = JNIEnv.GetMethodID(listClass, "get", "(I)Ljava/lang/Object;");
            if (sizeMethod == IntPtr.Zero || getMethod == IntPtr.Zero)
                return (null, null);

            var size = JNIEnv.CallIntMethod(listHandle, sizeMethod);
            for (var i = 0; i < size; i++)
            {
                var getArgs = new JValue[] { new JValue(i) };
                var tagInfo = JNIEnv.CallObjectMethod(listHandle, getMethod, getArgs);
                if (tagInfo == IntPtr.Zero)
                    continue;

                var data = TryExtractEpcAndTidFromTag(tagInfo);

                // NO liberar tagInfo manualmente aquí.
                // En este SDK/handheld estaba provocando crash nativo.

                if (!string.IsNullOrWhiteSpace(data.Epc))
                    return data;
            }

            return (null, null);
        }

        private static IReadOnlyList<string> TryExtractUniqueEpcs(IntPtr listHandle, int maxCount)
        {
            var epcs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var listClass = JNIEnv.FindClass("java/util/List");
            if (listClass == IntPtr.Zero)
                return epcs.ToList();

            var sizeMethod = JNIEnv.GetMethodID(listClass, "size", "()I");
            var getMethod = JNIEnv.GetMethodID(listClass, "get", "(I)Ljava/lang/Object;");
            if (sizeMethod == IntPtr.Zero || getMethod == IntPtr.Zero)
                return epcs.ToList();

            var size = JNIEnv.CallIntMethod(listHandle, sizeMethod);
            for (var i = 0; i < size && epcs.Count < maxCount; i++)
            {
                var getArgs = new JValue[] { new JValue(i) };
                var tagInfo = JNIEnv.CallObjectMethod(listHandle, getMethod, getArgs);
                if (tagInfo == IntPtr.Zero)
                    continue;

                var tag = TryExtractEpcAndTidFromTag(tagInfo);
                if (string.IsNullOrWhiteSpace(tag.Epc))
                    continue;

                epcs.Add(tag.Epc.Trim().ToUpperInvariant());
            }

            return epcs.ToList();
        }


        private static (string? Epc, string? Tid) TryExtractEpcAndTidFromTag(IntPtr tagInfo)
        {
            var tagClass = JNIEnv.GetObjectClass(tagInfo);
            if (tagClass == IntPtr.Zero)
                return (null, null);

            var epc = TryExtractByteArrayField(tagInfo, tagClass, "EpcId", "epc");
            var tid = TryExtractTidFromTag(tagInfo, tagClass);

            return (epc, tid);
        }

        private static string? TryExtractTidFromTag(IntPtr tagInfo, IntPtr tagClass)
        {
            var dataField = JNIEnv.GetFieldID(tagClass, "EmbededData", "[B");
            if (dataField == IntPtr.Zero)
                dataField = JNIEnv.GetFieldID(tagClass, "embededData", "[B");
            if (dataField == IntPtr.Zero)
                return null;

            var dataArray = JNIEnv.GetObjectField(tagInfo, dataField);
            if (dataArray == IntPtr.Zero)
                return null;

            var bytes = JNIEnv.GetArray<byte>(dataArray);
            if (bytes is null || bytes.Length == 0)
                return null;

            var len = TryReadLengthField(tagInfo, tagClass, "EmbededDatalen");
            if (len <= 0)
                len = TryReadLengthField(tagInfo, tagClass, "embededDatalen");

            if (len > 0 && len < bytes.Length)
                bytes = bytes[..len];

            return bytes.Length == 0 ? null : Convert.ToHexString(bytes).Trim();
        }

        private static string? TryReadTidMemoryBank(IntPtr managerClass, IntPtr manager)
        {
            try
            {
                // Banco TID = 2, desde palabra 0, 6 palabras (=12 bytes).
                var getTagData = JNIEnv.GetMethodID(managerClass, "getTagData", "(III[B[BS)Lcom/uhf/api/cls/Reader$READER_ERR;");
                if (getTagData == IntPtr.Zero)
                    return null;

                var readBuffer = new byte[12];
                var accessPwd = new byte[4];
                var readBufferArray = JNIEnv.NewArray(readBuffer);
                var accessPwdArray = JNIEnv.NewArray(accessPwd);
                var args = new JValue[]
                {
                    new(2), // TID bank
                    new(0), // start address
                    new(6), // length in words
                    new(readBufferArray),
                    new(accessPwdArray),
                    new((short)1000)
                };

                var result = JNIEnv.CallObjectMethod(manager, getTagData, args);
                var errName = ReadEnumName(result);
                DeleteLocalRefSafe(result);
                if (!string.Equals(errName, "MT_OK_ERR", StringComparison.Ordinal))
                    return null;

                var bytes = JNIEnv.GetArray<byte>(readBufferArray);
                return bytes is { Length: > 0 } ? Convert.ToHexString(bytes).Trim() : null;
            }
            catch
            {
                return null;
            }
        }

        private static string? TryExtractByteArrayField(IntPtr tagInfo, IntPtr tagClass, params string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                var field = JNIEnv.GetFieldID(tagClass, fieldName, "[B");
                if (field == IntPtr.Zero)
                    continue;

                var arrayHandle = JNIEnv.GetObjectField(tagInfo, field);
                if (arrayHandle == IntPtr.Zero)
                    continue;

                var bytes = JNIEnv.GetArray<byte>(arrayHandle);
                if (bytes is { Length: > 0 })
                    return Convert.ToHexString(bytes).Trim();
            }

            return null;
        }

        private static int TryReadLengthField(IntPtr tagInfo, IntPtr tagClass, string fieldName)
        {
            var intField = JNIEnv.GetFieldID(tagClass, fieldName, "I");
            if (intField != IntPtr.Zero)
                return JNIEnv.GetIntField(tagInfo, intField);

            var shortField = JNIEnv.GetFieldID(tagClass, fieldName, "S");
            if (shortField != IntPtr.Zero)
                return JNIEnv.GetShortField(tagInfo, shortField);

            return -1;
        }

        private static string? ReadEnumName(IntPtr enumObj)
        {
            if (enumObj == IntPtr.Zero)
                return null;

            var enumClass = JNIEnv.GetObjectClass(enumObj);
            if (enumClass == IntPtr.Zero)
                return null;

            var nameMethod = JNIEnv.GetMethodID(enumClass, "name", "()Ljava/lang/String;");
            if (nameMethod == IntPtr.Zero)
                return null;

            var nameObj = JNIEnv.CallObjectMethod(enumObj, nameMethod);
            var name = nameObj == IntPtr.Zero
                ? null
                : JNIEnv.GetString(nameObj, JniHandleOwnership.DoNotTransfer);

            DeleteLocalRefSafe(nameObj);
            DeleteLocalRefSafe(enumClass);
            return name;
        }

        private static void LogStep(string message)
        {
            var line = $"[{RfidImplVersion}] {message}";
            Debug.WriteLine(line);
            Log.Debug(LogTag, line);
        }

        private static void DeleteGlobalRefSafe(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
                JNIEnv.DeleteGlobalRef(handle);
        }

        private static void DeleteLocalRefSafe(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
                JNIEnv.DeleteLocalRef(handle);
        }
    }
}
#endif
