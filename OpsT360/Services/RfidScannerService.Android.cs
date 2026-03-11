#if ANDROID
using Android.Runtime;
using Android.OS;

namespace OpsT360.Services;

public partial class RfidScannerService
{
    private const string RfidImplVersion = "RFIDv2026.03.11.5";
    private const bool SkipSdkSetPower = true;
    private readonly object _sync = new();
    private readonly SemaphoreSlim _rfidOpLock = new(1, 1);
    private string? _lastEpc;
    private DateTimeOffset _lastEpcAt = DateTimeOffset.MinValue;

    private partial async Task<RfidReadResult> StartAntennaPlatformAsync(CancellationToken cancellationToken)
    {
        IntPtr managerClass = IntPtr.Zero;
        IntPtr manager = IntPtr.Zero;
        await _rfidOpLock.WaitAsync(cancellationToken);
        try
        {
            var setup = TryGetReader();
            if (!setup.Success || setup.ManagerClass == IntPtr.Zero || setup.Manager == IntPtr.Zero)
                return RfidReadResult.Fail(setup.Message);

            managerClass = setup.ManagerClass;
            manager = setup.Manager;

            TrySetPowerSafely(setup.ManagerClass, setup.Manager);
            StopAnyInventory(setup.ManagerClass, setup.Manager);

            var started = TryStartInventory(setup.ManagerClass, setup.Manager, out var startDetail);
            if (!started)
                return RfidReadResult.Fail($"No fue posible activar antena RFID: {startDetail}");

            return RfidReadResult.Ok($"[{RfidImplVersion}] Antena RFID activada. Pulsa Read seal para capturar EPC.");
        }
        catch (Java.Lang.Throwable jex)
        {
            var detail = DescribeJavaThrowable(jex);
            return RfidReadResult.Fail($"[{RfidImplVersion}] No se pudo activar antena RFID: {detail}");
        }
        catch (System.OperationCanceledException)
        {
            return RfidReadResult.Fail($"[{RfidImplVersion}] Activación RFID cancelada por timeout.");
        }
        catch (Exception ex)
        {
            return RfidReadResult.Fail($"[{RfidImplVersion}] No se pudo activar antena RFID: {ex.Message}");
        }
        finally
        {
            ReleaseReaderHandles(managerClass, manager);
            _rfidOpLock.Release();
        }
    }

    private partial async Task<RfidReadResult> TryReadSingleEpcPlatformAsync(CancellationToken cancellationToken)
    {
        IntPtr managerClass = IntPtr.Zero;
        IntPtr manager = IntPtr.Zero;
        await _rfidOpLock.WaitAsync(cancellationToken);
        try
        {
            var setup = TryGetReader();
            if (!setup.Success || setup.ManagerClass == IntPtr.Zero || setup.Manager == IntPtr.Zero)
                return RfidReadResult.Fail(setup.Message);

            managerClass = setup.ManagerClass;
            manager = setup.Manager;

            TrySetPowerSafely(setup.ManagerClass, setup.Manager);
            StopAnyInventory(setup.ManagerClass, setup.Manager);

            if (!TryStartInventory(setup.ManagerClass, setup.Manager, out var startDetail))
                return RfidReadResult.Fail($"[{RfidImplVersion}] No fue posible iniciar inventario RFID en UHF6: {startDetail}");

            for (var i = 0; i < 35; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var tagsList = TryInventory(setup.ManagerClass, setup.Manager);
                if (tagsList != IntPtr.Zero)
                {
                    var epc = TryExtractBestEpc(tagsList);
                    if (!string.IsNullOrWhiteSpace(epc))
                    {
                        SaveLastEpc(epc);
                        StopAnyInventory(setup.ManagerClass, setup.Manager);
                        return RfidReadResult.Ok(epc);
                    }
                }

                var cached = GetRecentEpc();
                if (!string.IsNullOrWhiteSpace(cached))
                {
                    StopAnyInventory(setup.ManagerClass, setup.Manager);
                    return RfidReadResult.Ok(cached);
                }

                await Task.Delay(120, cancellationToken);
            }

            StopAnyInventory(setup.ManagerClass, setup.Manager);
            return RfidReadResult.Fail("No se detectó EPC por antena RFID. Verifica distancia, orientación y potencia.");
        }
        catch (System.OperationCanceledException)
        {
            return RfidReadResult.Fail($"[{RfidImplVersion}] Lectura RFID cancelada por timeout.");
        }
        catch (Java.Lang.Throwable jex)
        {
            var detail = DescribeJavaThrowable(jex);
            return RfidReadResult.Fail($"[{RfidImplVersion}] Error RFID Java: {detail}");
        }
  
   
        catch (Exception ex)
        {
            return RfidReadResult.Fail($"[{RfidImplVersion}] Error RFID: {ex.Message}");
        }
        finally
        {
            ReleaseReaderHandles(managerClass, manager);
            _rfidOpLock.Release();
        }
    }

    private static (bool Success, string Message, IntPtr ManagerClass, IntPtr Manager) TryGetReader()
    {
        var managerClassLocal = JNIEnv.FindClass("com/handheld/uhfr/UHFRManager");
        if (managerClassLocal == IntPtr.Zero)
            return (false, "No se encontró UHFRManager en los JAR del SDK.", IntPtr.Zero, IntPtr.Zero);

        var managerClass = JNIEnv.NewGlobalRef(managerClassLocal);
        DeleteLocalRefSafe(managerClassLocal);

        var getInstance = JNIEnv.GetStaticMethodID(managerClass, "getInstance", "()Lcom/handheld/uhfr/UHFRManager;");
        if (getInstance == IntPtr.Zero)
        {
            DeleteGlobalRefSafe(managerClass);
            return (false, "No se encontró getInstance() en UHFRManager.", IntPtr.Zero, IntPtr.Zero);
        }

        var managerLocal = JNIEnv.CallStaticObjectMethod(managerClass, getInstance);
        if (managerLocal == IntPtr.Zero)
        {
            DeleteGlobalRefSafe(managerClass);
            return (false, "No fue posible inicializar el módulo RFID (getInstance = null).", IntPtr.Zero, IntPtr.Zero);
        }

        var manager = JNIEnv.NewGlobalRef(managerLocal);
        DeleteLocalRefSafe(managerLocal);

        return (true, string.Empty, managerClass, manager);
    }

    private static void ReleaseReaderHandles(IntPtr managerClass, IntPtr manager)
    {
        if (manager != IntPtr.Zero)
            DeleteGlobalRefSafe(manager);

        if (managerClass != IntPtr.Zero)
            DeleteGlobalRefSafe(managerClass);
    }

    private static void TrySetPower(IntPtr managerClass, IntPtr manager)
    {
        var setPower = JNIEnv.GetMethodID(managerClass, "setPower", "(II)Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (setPower == IntPtr.Zero)
            return;

        var powerArgs = new JValue[] { new(30), new(30) };
        JNIEnv.CallObjectMethod(manager, setPower, powerArgs);
    }

    private static void TrySetPowerSafely(IntPtr managerClass, IntPtr manager)
    {
        // En algunos firmwares (p.ej. C6 Android 13) setPower puede abortar en capa nativa (SIGABRT).
        // Se omite por defecto para priorizar estabilidad; el lector usa potencia/configuración actual del módulo.
        if (SkipSdkSetPower)
            return;

        TrySetPower(managerClass, manager);
    }

    private static void StopAnyInventory(IntPtr managerClass, IntPtr manager)
    {
        TryInvokeBoolNoArg(managerClass, manager, "stopTagInventory");
        TryInvokeReaderErrNoArg(managerClass, manager, "asyncStopReading");
    }

    private static bool TryStartInventory(IntPtr managerClass, IntPtr manager, out string detail)
    {
        if (TryInvokeReaderErrNoArg(managerClass, manager, "asyncStartReading", out var asyncOk, out var asyncErr))
        {
            detail = asyncOk ? "asyncStartReading()" : $"asyncStartReading()={asyncErr}";
            return asyncOk;
        }

        if (TryInvokeReaderErrWithInt(managerClass, manager, "asyncStartReading", 240, out var timedOk, out var timedErr))
        {
            detail = timedOk ? "asyncStartReading(240)" : $"asyncStartReading(240)={timedErr}";
            return timedOk;
        }

        detail = "No se encontró asyncStartReading en UHFRManager";
        return false;
    }

    private static IntPtr TryInventory(IntPtr managerClass, IntPtr manager)
    {
        var method = JNIEnv.GetMethodID(managerClass, "tagInventoryByTimer", "(S)Ljava/util/List;");
        if (method != IntPtr.Zero)
        {
            var timerArgs = new JValue[] { new((short)180) };
            return JNIEnv.CallObjectMethod(manager, method, timerArgs);
        }

        method = JNIEnv.GetMethodID(managerClass, "tagEpcTidInventoryByTimer", "(S)Ljava/util/List;");
        if (method != IntPtr.Zero)
        {
            var timerArgs = new JValue[] { new((short)120) };
            return JNIEnv.CallObjectMethod(manager, method, timerArgs);
        }

        method = JNIEnv.GetMethodID(managerClass, "tagInventoryRealTime", "()Ljava/util/List;");
        if (method != IntPtr.Zero)
            return JNIEnv.CallObjectMethod(manager, method);

        return IntPtr.Zero;
    }

    private static bool TryInvokeBoolNoArg(IntPtr managerClass, IntPtr manager, string methodName)
    {
        return TryInvokeBoolNoArg(managerClass, manager, methodName, out _);
    }

    private static bool TryInvokeBoolNoArg(IntPtr managerClass, IntPtr manager, string methodName, out bool result)
    {
        var method = TryGetMethodIdSafe(managerClass, methodName, "()Z");
        if (method == IntPtr.Zero)
        {
            result = false;
            return false;
        }

        result = JNIEnv.CallBooleanMethod(manager, method);
        return true;
    }

    private static bool TryInvokeReaderErrNoArg(IntPtr managerClass, IntPtr manager, string methodName)
    {
        return TryInvokeReaderErrNoArg(managerClass, manager, methodName, out _, out _);
    }

    private static bool TryInvokeReaderErrNoArg(IntPtr managerClass, IntPtr manager, string methodName, out bool ok, out string errName)
    {
        var method = TryGetMethodIdSafe(managerClass, methodName, "()Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (method == IntPtr.Zero)
        {
            ok = false;
            errName = "MethodNotFound";
            return false;
        }

        var errObj = JNIEnv.CallObjectMethod(manager, method);
        if (errObj == IntPtr.Zero)
        {
            ok = false;
            errName = "NullReaderErr";
            return true;
        }

        errName = ReadEnumName(errObj) ?? "UnknownReaderErr";
        ok = string.Equals(errName, "MT_OK_ERR", StringComparison.OrdinalIgnoreCase);
        return true;
    }


    private static bool TryInvokeReaderErrWithInt(IntPtr managerClass, IntPtr manager, string methodName, int value, out bool ok, out string errName)
    {
        var method = TryGetMethodIdSafe(managerClass, methodName, "(I)Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (method == IntPtr.Zero)
        {
            ok = false;
            errName = "MethodNotFound";
            return false;
        }

        var args = new JValue[] { new(value) };
        var errObj = JNIEnv.CallObjectMethod(manager, method, args);
        if (errObj == IntPtr.Zero)
        {
            ok = false;
            errName = "NullReaderErr";
            return true;
        }

        errName = ReadEnumName(errObj) ?? "UnknownReaderErr";
        ok = string.Equals(errName, "MT_OK_ERR", StringComparison.OrdinalIgnoreCase);
        return true;
    }

    private static IntPtr TryGetMethodIdSafe(IntPtr classHandle, string methodName, string signature)
    {
        try
        {
            var method = JNIEnv.GetMethodID(classHandle, methodName, signature);
            ClearPendingJavaException();
            return method;
        }
        catch
        {
            ClearPendingJavaException();
            return IntPtr.Zero;
        }
    }
    private static void DeleteLocalRefSafe(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return;

        try
        {
            JNIEnv.DeleteLocalRef(handle);
        }
        catch
        {
            // Evita crash por doble liberación o ref inválida.
        }
    }

    private static void DeleteGlobalRefSafe(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return;

        try
        {
            JNIEnv.DeleteGlobalRef(handle);
        }
        catch
        {
            // Evita crash por doble liberación o ref inválida.
        }
    }

    private static void ClearPendingJavaException()
    {
        var pending = JNIEnv.ExceptionOccurred();
        if (pending != IntPtr.Zero)
        {
            JNIEnv.ExceptionClear();
            DeleteLocalRefSafe(pending);
        }
    }

    private static string? ReadEnumName(IntPtr enumObj)
    {
        var enumClass = JNIEnv.GetObjectClass(enumObj);
        var nameMethod = JNIEnv.GetMethodID(enumClass, "name", "()Ljava/lang/String;");
        if (nameMethod == IntPtr.Zero)
        {
            DeleteLocalRefSafe(enumClass);
            return null;
        }

        var nameObj = JNIEnv.CallObjectMethod(enumObj, nameMethod);
        var name = nameObj == IntPtr.Zero ? null : JNIEnv.GetString(nameObj, JniHandleOwnership.DoNotTransfer);
        if (nameObj != IntPtr.Zero)
            DeleteLocalRefSafe(nameObj);

        DeleteLocalRefSafe(enumClass);
        return name;
    }

    private static string? TryExtractBestEpc(IntPtr listHandle)
    {
        var listClass = JNIEnv.FindClass("java/util/List");
        var sizeMethod = JNIEnv.GetMethodID(listClass, "size", "()I");
        var getMethod = JNIEnv.GetMethodID(listClass, "get", "(I)Ljava/lang/Object;");
        var size = JNIEnv.CallIntMethod(listHandle, sizeMethod);
        if (size <= 0)
            return null;

        string? bestEpc = null;
        var bestRssi = int.MinValue;

        for (var i = 0; i < size; i++)
        {
            var getArgs = new JValue[] { new(i) };
            var tagInfo = JNIEnv.CallObjectMethod(listHandle, getMethod, getArgs);
            if (tagInfo == IntPtr.Zero)
                continue;

            var epc = TryExtractEpcFromTag(tagInfo);
            if (string.IsNullOrWhiteSpace(epc))
            {
                DeleteLocalRefSafe(tagInfo);
                continue;
            }

            var rssi = TryExtractRssiFromTag(tagInfo);
            DeleteLocalRefSafe(tagInfo);
            if (rssi > bestRssi)
            {
                bestRssi = rssi;
                bestEpc = epc;
            }
        }

        DeleteLocalRefSafe(listClass);

        return bestEpc;
    }

    private static int TryExtractRssiFromTag(IntPtr tagInfo)
    {
        var tagClass = JNIEnv.GetObjectClass(tagInfo);
        var rssiField = JNIEnv.GetFieldID(tagClass, "RSSI", "I");
        var value = rssiField == IntPtr.Zero ? int.MinValue : JNIEnv.GetIntField(tagInfo, rssiField);
        DeleteLocalRefSafe(tagClass);
        return value;
    }

    private static string? TryExtractEpcFromTag(IntPtr tagInfo)
    {
        var tagClass = JNIEnv.GetObjectClass(tagInfo);
        var epcField = JNIEnv.GetFieldID(tagClass, "EpcId", "[B");
        if (epcField == IntPtr.Zero)
            epcField = JNIEnv.GetFieldID(tagClass, "epc", "[B");
        if (epcField == IntPtr.Zero)
        {
            DeleteLocalRefSafe(tagClass);
            return null;
        }

        var epcBytesArray = JNIEnv.GetObjectField(tagInfo, epcField);
        if (epcBytesArray == IntPtr.Zero)
        {
            DeleteLocalRefSafe(tagClass);
            return null;
        }

        var bytes = JNIEnv.GetArray<byte>(epcBytesArray);
        DeleteLocalRefSafe(epcBytesArray);
        DeleteLocalRefSafe(tagClass);
        if (bytes is null || bytes.Length == 0)
            return null;

        return Convert.ToHexString(bytes).Trim();
    }

    private static string DescribeJavaThrowable(Java.Lang.Throwable throwable)
    {
        var message = throwable.Message ?? throwable.Class?.Name ?? "JavaThrowable";

        if (message.Contains("cn.pda.serialport.SerialPort", StringComparison.OrdinalIgnoreCase))
        {
            var abis = Android.OS.Build.SupportedAbis;
            var abi = abis is { Count: > 0 } ? string.Join(",", abis) : "unknown";
            return $"{message}. Posible incompatibilidad ABI (SDK UHF6 requiere libSerialPort 32-bit). ABIs dispositivo: {abi}";
        }

        return message;
    }

    private void SaveLastEpc(string epc)
    {
        lock (_sync)
        {
            _lastEpc = epc.Trim().ToUpperInvariant();
            _lastEpcAt = DateTimeOffset.UtcNow;
        }
    }

    private string? GetRecentEpc()
    {
        lock (_sync)
        {
            if (_lastEpcAt == DateTimeOffset.MinValue)
                return null;

            if (DateTimeOffset.UtcNow - _lastEpcAt > TimeSpan.FromSeconds(4))
                return null;

            return _lastEpc;
        }
    }
}
#endif
