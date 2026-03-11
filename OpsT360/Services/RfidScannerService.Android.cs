#if ANDROID
using Android.Runtime;
using Android.Util;
using System.Diagnostics;

namespace OpsT360.Services;

public partial class RfidScannerService
{
    private const string RfidImplVersion = "RFIDv2026.03.11.6";
    private const string LogTag = "OpsT360.RFID";
    private readonly SemaphoreSlim _rfidOpLock = new(1, 1);

    private partial async Task<RfidReadResult> StartAntennaPlatformAsync(CancellationToken cancellationToken)
    {
        IntPtr managerClass = IntPtr.Zero;
        IntPtr manager = IntPtr.Zero;

        await _rfidOpLock.WaitAsync(cancellationToken);
        try
        {
            var setup = TryGetReader();
            if (!setup.Success)
                return RfidReadResult.Fail(setup.Message);

            managerClass = setup.ManagerClass;
            manager = setup.Manager;

            var powerDetail = TrySetPower(managerClass, manager);
            var startDetail = TryStartReading(managerClass, manager, out var started);

            if (!started)
                return RfidReadResult.Fail($"[{RfidImplVersion}] No se pudo activar antena RFID: {startDetail}. setPower={powerDetail}");

            LogStep($"StartAntenna ok. setPower={powerDetail}, start={startDetail}");
            return RfidReadResult.Ok($"[{RfidImplVersion}] Antena RFID activada.");
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
            if (!setup.Success)
                return RfidReadResult.Fail(setup.Message);

            managerClass = setup.ManagerClass;
            manager = setup.Manager;

            var powerDetail = TrySetPower(managerClass, manager);
            var startDetail = TryStartReading(managerClass, manager, out var started);
            if (!started)
                return RfidReadResult.Fail($"[{RfidImplVersion}] No se pudo activar antena RFID: {startDetail}. setPower={powerDetail}");

            cancellationToken.ThrowIfCancellationRequested();

            var tags = TryInventory(managerClass, manager);
            var epc = tags == IntPtr.Zero ? null : TryExtractFirstEpc(tags);
            TryStopReading(managerClass, manager);

            if (string.IsNullOrWhiteSpace(epc))
                return RfidReadResult.Fail($"[{RfidImplVersion}] No se capturó sticker RFID. Acerca el sello y reintenta.");

            LogStep($"TryReadSingleEpc ok -> {epc}");
            return RfidReadResult.Ok(epc);
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
        var getInstance = JNIEnv.GetStaticMethodID(managerClass, "getInstance", "()Lcom/handheld/uhfr/UHFRManager;");
        if (getInstance == IntPtr.Zero)
            return (false, "No se encontró getInstance() en UHFRManager.", IntPtr.Zero, IntPtr.Zero);

        var managerLocal = JNIEnv.CallStaticObjectMethod(managerClass, getInstance);
        if (managerLocal == IntPtr.Zero)
            return (false, "No fue posible inicializar módulo RFID (getInstance = null).", IntPtr.Zero, IntPtr.Zero);

        var manager = JNIEnv.NewGlobalRef(managerLocal);
        return (true, string.Empty, managerClass, manager);
    }

    private static void ReleaseReaderHandles(IntPtr managerClass, IntPtr manager)
    {
        if (manager != IntPtr.Zero)
            JNIEnv.DeleteGlobalRef(manager);

        if (managerClass != IntPtr.Zero)
            JNIEnv.DeleteGlobalRef(managerClass);
    }

    private static string TrySetPower(IntPtr managerClass, IntPtr manager)
    {
        var setPower = JNIEnv.GetMethodID(managerClass, "setPower", "(II)Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (setPower == IntPtr.Zero)
            return "MethodNotFound";

        var args = new JValue[] { new(30), new(30) };
        var result = JNIEnv.CallObjectMethod(manager, setPower, args);
        return ReadEnumName(result) ?? "Unknown";
    }

    private static string TryStartReading(IntPtr managerClass, IntPtr manager, out bool started)
    {
        var noArg = JNIEnv.GetMethodID(managerClass, "asyncStartReading", "()Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (noArg != IntPtr.Zero)
        {
            var result = JNIEnv.CallObjectMethod(manager, noArg);
            var name = ReadEnumName(result) ?? "Unknown";
            started = name.Contains("MT_OK_ERR", StringComparison.OrdinalIgnoreCase);
            return name;
        }

        var timed = JNIEnv.GetMethodID(managerClass, "asyncStartReading", "(I)Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (timed != IntPtr.Zero)
        {
            var args = new JValue[] { new(240) };
            var result = JNIEnv.CallObjectMethod(manager, timed, args);
            var name = ReadEnumName(result) ?? "Unknown";
            started = name.Contains("MT_OK_ERR", StringComparison.OrdinalIgnoreCase);
            return name;
        }

        started = false;
        return "MethodNotFound";
    }

    private static void TryStopReading(IntPtr managerClass, IntPtr manager)
    {
        var stop1 = JNIEnv.GetMethodID(managerClass, "stopTagInventory", "()Z");
        if (stop1 != IntPtr.Zero)
            JNIEnv.CallBooleanMethod(manager, stop1);

        var stop2 = JNIEnv.GetMethodID(managerClass, "asyncStopReading", "()Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (stop2 != IntPtr.Zero)
            JNIEnv.CallObjectMethod(manager, stop2);
    }

    private static IntPtr TryInventory(IntPtr managerClass, IntPtr manager)
    {
        var method = JNIEnv.GetMethodID(managerClass, "tagInventoryByTimer", "(S)Ljava/util/List;");
        if (method != IntPtr.Zero)
        {
            var args = new JValue[] { new((short)180) };
            return JNIEnv.CallObjectMethod(manager, method, args);
        }

        method = JNIEnv.GetMethodID(managerClass, "tagEpcTidInventoryByTimer", "(S)Ljava/util/List;");
        if (method != IntPtr.Zero)
        {
            var args = new JValue[] { new((short)120) };
            return JNIEnv.CallObjectMethod(manager, method, args);
        }

        method = JNIEnv.GetMethodID(managerClass, "tagInventoryRealTime", "()Ljava/util/List;");
        if (method != IntPtr.Zero)
            return JNIEnv.CallObjectMethod(manager, method);

        return IntPtr.Zero;
    }

    private static string? TryExtractFirstEpc(IntPtr listHandle)
    {
        var listClass = JNIEnv.FindClass("java/util/List");
        var sizeMethod = JNIEnv.GetMethodID(listClass, "size", "()I");
        var getMethod = JNIEnv.GetMethodID(listClass, "get", "(I)Ljava/lang/Object;");
        if (sizeMethod == IntPtr.Zero || getMethod == IntPtr.Zero)
            return null;

        var size = JNIEnv.CallIntMethod(listHandle, sizeMethod);
        for (var i = 0; i < size; i++)
        {
            var getArgs = new JValue[] { new(i) };
            var tagInfo = JNIEnv.CallObjectMethod(listHandle, getMethod, getArgs);
            if (tagInfo == IntPtr.Zero)
                continue;

            var epc = TryExtractEpcFromTag(tagInfo);
            if (!string.IsNullOrWhiteSpace(epc))
                return epc;
        }

        return null;
    }

    private static string? TryExtractEpcFromTag(IntPtr tagInfo)
    {
        var tagClass = JNIEnv.GetObjectClass(tagInfo);
        var epcField = JNIEnv.GetFieldID(tagClass, "EpcId", "[B");
        if (epcField == IntPtr.Zero)
            epcField = JNIEnv.GetFieldID(tagClass, "epc", "[B");
        if (epcField == IntPtr.Zero)
            return null;

        var epcBytesArray = JNIEnv.GetObjectField(tagInfo, epcField);
        if (epcBytesArray == IntPtr.Zero)
            return null;

        var bytes = JNIEnv.GetArray<byte>(epcBytesArray);
        if (bytes is null || bytes.Length == 0)
            return null;

        return Convert.ToHexString(bytes).Trim();
    }

    private static string? ReadEnumName(IntPtr enumObj)
    {
        if (enumObj == IntPtr.Zero)
            return null;

        var enumClass = JNIEnv.GetObjectClass(enumObj);
        var nameMethod = JNIEnv.GetMethodID(enumClass, "name", "()Ljava/lang/String;");
        if (nameMethod == IntPtr.Zero)
            return null;

        var nameObj = JNIEnv.CallObjectMethod(enumObj, nameMethod);
        return nameObj == IntPtr.Zero ? null : JNIEnv.GetString(nameObj, JniHandleOwnership.DoNotTransfer);
    }

    private static void LogStep(string message)
    {
        var line = $"[{RfidImplVersion}] {message}";
        Debug.WriteLine(line);
        Log.Debug(LogTag, line);
    }
}
#endif
