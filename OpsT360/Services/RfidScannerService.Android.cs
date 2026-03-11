#if ANDROID
using Android.Runtime;
using Android.Util;
using System.Diagnostics;

namespace OpsT360.Services;

public partial class RfidScannerService
{
    private const string RfidImplVersion = "RFIDv2026.03.11.6";
    private const string LogTag = "OpsT360.RFID";
    private static readonly bool SkipSdkSetPower = false;
    private readonly SemaphoreSlim _rfidOpLock = new(1, 1);

    private partial async Task<RfidReadResult> StartAntennaPlatformAsync(CancellationToken cancellationToken)
    {
        IntPtr managerClass = IntPtr.Zero;
        IntPtr manager = IntPtr.Zero;
        LogStep("StartAntenna: requested");
        await _rfidOpLock.WaitAsync(cancellationToken);
        try
        {
            var setup = TryGetReader();
            if (!setup.Success || setup.ManagerClass == IntPtr.Zero || setup.Manager == IntPtr.Zero)
            {
                LogStep($"StartAntenna: reader setup failed -> {setup.Message}");
                return RfidReadResult.Fail(setup.Message);
            }

            managerClass = setup.ManagerClass;
            manager = setup.Manager;

            TrySetPowerSafely(setup.ManagerClass, setup.Manager, out var powerDetail);
            StopAnyInventory(setup.ManagerClass, setup.Manager);

            var started = TryStartInventory(setup.ManagerClass, setup.Manager, out var startDetail);
            if (!started)
            {
                var reason = string.IsNullOrWhiteSpace(powerDetail) ? startDetail : $"{startDetail}. setPower: {powerDetail}";
                LogStep($"StartAntenna: start inventory failed -> {reason}");
                return RfidReadResult.Fail($"No fue posible activar antena RFID: {reason}");
            }

            LogStep("StartAntenna: success");
            return RfidReadResult.Ok($"[{RfidImplVersion}] Antena RFID activada. Pulsa Read seal para capturar EPC.");
        }
        catch (System.OperationCanceledException)
        {
            LogStep("StartAntenna: canceled/timeout");
            return RfidReadResult.Fail($"[{RfidImplVersion}] Activación RFID cancelada por timeout.");
        }
        catch (Exception ex)
        {
            LogStep($"StartAntenna: .NET error -> {ex.Message}");
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
        LogStep("TryReadSingleEpc: requested");
        await _rfidOpLock.WaitAsync(cancellationToken);
        try
        {
            var setup = TryGetReader();
            if (!setup.Success || setup.ManagerClass == IntPtr.Zero || setup.Manager == IntPtr.Zero)
            {
                LogStep($"TryReadSingleEpc: reader setup failed -> {setup.Message}");
                return RfidReadResult.Fail(setup.Message);
            }

            managerClass = setup.ManagerClass;
            manager = setup.Manager;

            TrySetPowerSafely(setup.ManagerClass, setup.Manager, out var powerDetail);
            StopAnyInventory(setup.ManagerClass, setup.Manager);

            if (!TryStartInventory(setup.ManagerClass, setup.Manager, out var startDetail))
            {
                LogStep($"TryReadSingleEpc: start inventory failed -> {startDetail}");
                return RfidReadResult.Fail($"[{RfidImplVersion}] No fue posible iniciar inventario RFID en UHF6: {startDetail}");
            }

            cancellationToken.ThrowIfCancellationRequested();
            var tagsList = TryInventory(setup.ManagerClass, setup.Manager);
            if (tagsList != IntPtr.Zero)
            {
                var epc = TryExtractFirstEpc(tagsList);
                if (!string.IsNullOrWhiteSpace(epc))
                {
                    StopAnyInventory(setup.ManagerClass, setup.Manager);
                    LogStep($"TryReadSingleEpc: EPC read -> {epc}");
                    return RfidReadResult.Ok(epc);
                }
            }

            StopAnyInventory(setup.ManagerClass, setup.Manager);
            LogStep("TryReadSingleEpc: timeout/no EPC");
            var detail = string.IsNullOrWhiteSpace(powerDetail)
                ? "No se detectó EPC por antena RFID."
                : $"No se detectó EPC por antena RFID. setPower: {powerDetail}";
            return RfidReadResult.Fail($"{detail} Verifica distancia, orientación y potencia.");
        }
        catch (System.OperationCanceledException)
        {
            LogStep("TryReadSingleEpc: canceled/timeout");
            return RfidReadResult.Fail($"[{RfidImplVersion}] Lectura RFID cancelada por timeout.");
        }
        catch (Exception ex)
        {
            LogStep($"TryReadSingleEpc: .NET error -> {ex.Message}");
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

    private static bool TrySetPower(IntPtr managerClass, IntPtr manager, out string detail)
    {
        var setPower = JNIEnv.GetMethodID(managerClass, "setPower", "(II)Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (setPower == IntPtr.Zero)
        {
            detail = "MethodNotFound";
            return false;
        }

        var powerArgs = new JValue[] { new(30), new(30) };
        var errObj = JNIEnv.CallObjectMethod(manager, setPower, powerArgs);
        if (errObj == IntPtr.Zero)
        {
            detail = "NullReaderErr";
            return false;
        }

        detail = ReadEnumName(errObj) ?? "UnknownReaderErr";
        return detail.Contains("MT_OK_ERR", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TrySetPowerSafely(IntPtr managerClass, IntPtr manager, out string detail)
    {
        if (SkipSdkSetPower)
        {
            detail = "SkippedByConfig";
            return false;
        }

        var ok = TrySetPower(managerClass, manager, out detail);
        LogStep($"TrySetPowerSafely: {(ok ? "ok" : "fail")} -> {detail}");
        return ok;
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
        ok = errName.Contains("MT_OK_ERR", StringComparison.OrdinalIgnoreCase);
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
        ok = errName.Contains("MT_OK_ERR", StringComparison.OrdinalIgnoreCase);
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
        // Mitigación de estabilidad: algunos runtimes JNI abortan proceso si el tipo real no coincide.
        // Evitamos DeleteLocalRef manual aquí; las local refs se liberan al cerrar el frame JNI.
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

    private static string? TryExtractFirstEpc(IntPtr listHandle)
    {
        var listClass = JNIEnv.FindClass("java/util/List");
        var sizeMethod = JNIEnv.GetMethodID(listClass, "size", "()I");
        var getMethod = JNIEnv.GetMethodID(listClass, "get", "(I)Ljava/lang/Object;");
        var size = JNIEnv.CallIntMethod(listHandle, sizeMethod);
        if (size <= 0)
            return null;

        for (var i = 0; i < size; i++)
        {
            var getArgs = new JValue[] { new(i) };
            var tagInfo = JNIEnv.CallObjectMethod(listHandle, getMethod, getArgs);
            if (tagInfo == IntPtr.Zero)
                continue;

            var epc = TryExtractEpcFromTag(tagInfo);
            DeleteLocalRefSafe(tagInfo);
            if (string.IsNullOrWhiteSpace(epc))
                continue;

            DeleteLocalRefSafe(listClass);
            return epc;
        }

        DeleteLocalRefSafe(listClass);

        return null;
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

    private static void LogStep(string message)
    {
        var line = $"[{RfidImplVersion}] {message}";
        Debug.WriteLine(line);
        Log.Debug(LogTag, line);
    }

}
#endif
