#if ANDROID
using Android.Runtime;

namespace OpsT360.Services;

public partial class RfidScannerService
{
    private readonly object _sync = new();
    private string? _lastEpc;
    private DateTimeOffset _lastEpcAt = DateTimeOffset.MinValue;

    private partial Task<RfidReadResult> StartAntennaPlatformAsync(CancellationToken cancellationToken)
    {
        try
        {
            var setup = TryGetReader();
            if (!setup.Success || setup.ManagerClass == IntPtr.Zero || setup.Manager == IntPtr.Zero)
                return Task.FromResult(RfidReadResult.Fail(setup.Message));

            TrySetPower(setup.ManagerClass, setup.Manager);
            TryInvokeNoArg(setup.ManagerClass, setup.Manager, "stopTagInventory");
            TryInvokeNoArg(setup.ManagerClass, setup.Manager, "stopInventory");
            TryInvokeNoArg(setup.ManagerClass, setup.Manager, "asyncStopReading");

            var started = TryStartInventory(setup.ManagerClass, setup.Manager)
                          || TryInvokeNoArg(setup.ManagerClass, setup.Manager, "asyncStartReading");


            var message = started
                ? "Antena RFID activada en modo remoto. Pulsa Read seal para capturar EPC (sin gatillo)."
                : "Antena preparada. Pulsa Read seal para capturar EPC por inventario temporizado.";

            return Task.FromResult(RfidReadResult.Ok(message));
        }
        catch (Exception ex)
        {
            return Task.FromResult(RfidReadResult.Fail($"No se pudo activar antena RFID: {ex.Message}"));
        }
    }

    private partial async Task<RfidReadResult> TryReadSingleEpcPlatformAsync(CancellationToken cancellationToken)
    {
        try
        {
            var setup = TryGetReader();
            if (!setup.Success || setup.ManagerClass == IntPtr.Zero || setup.Manager == IntPtr.Zero)
                return RfidReadResult.Fail(setup.Message);

            TrySetPower(setup.ManagerClass, setup.Manager);
            TryInvokeNoArg(setup.ManagerClass, setup.Manager, "stopTagInventory");
            TryInvokeNoArg(setup.ManagerClass, setup.Manager, "stopInventory");
            var started = TryStartInventory(setup.ManagerClass, setup.Manager)
                          || TryInvokeNoArg(setup.ManagerClass, setup.Manager, "asyncStartReading");

            if (!started)
                return RfidReadResult.Fail("No fue posible iniciar inventario RFID en el SDK UHF6.");

            for (var i = 0; i < 30; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var tagsList = TryInventory(setup.ManagerClass, setup.Manager);
                if (tagsList != IntPtr.Zero)
                {
                    var epc = TryExtractFirstEpc(tagsList);
                    if (!string.IsNullOrWhiteSpace(epc))
                    {
                        SaveLastEpc(epc);
                        return RfidReadResult.Ok(epc);
                    }
                }

                var cached = GetRecentEpc();
                if (!string.IsNullOrWhiteSpace(cached))
                    return RfidReadResult.Ok(cached);

                await Task.Delay(200, cancellationToken);
            }

            return RfidReadResult.Fail("No se detectó EPC por antena RFID remota. Verifica distancia y potencia de lectura.");
        }
        catch (OperationCanceledException)
        {
            return RfidReadResult.Fail("Lectura RFID cancelada.");
        }
        catch (Exception ex)
        {
            return RfidReadResult.Fail($"Error RFID: {ex.Message}");
        }
    }

    private static (bool Success, string Message, IntPtr ManagerClass, IntPtr Manager) TryGetReader()
    {
        var managerClass = JNIEnv.FindClass("com/handheld/uhfr/UHFRManager");
        if (managerClass == IntPtr.Zero)
            return (false, "No se encontró UHFRManager en los JAR del SDK.", IntPtr.Zero, IntPtr.Zero);

        var getInstance = JNIEnv.GetStaticMethodID(managerClass, "getInstance", "()Lcom/handheld/uhfr/UHFRManager;");
        if (getInstance == IntPtr.Zero)
            return (false, "No se encontró getInstance() en UHFRManager.", IntPtr.Zero, IntPtr.Zero);

        var manager = JNIEnv.CallStaticObjectMethod(managerClass, getInstance);
        if (manager == IntPtr.Zero)
            return (false, "No fue posible inicializar el módulo RFID (getInstance = null).", IntPtr.Zero, IntPtr.Zero);

        return (true, string.Empty, managerClass, manager);
    }

    private static void TrySetPower(IntPtr managerClass, IntPtr manager)
    {
        var setPower = JNIEnv.GetMethodID(managerClass, "setPower", "(II)Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (setPower == IntPtr.Zero)
            return;

        var powerArgs = new JValue[] { new(30), new(30) };
        JNIEnv.CallObjectMethod(manager, setPower, powerArgs);
    }

    private static bool TryStartInventory(IntPtr managerClass, IntPtr manager)
    {
        return TryInvokeNoArg(managerClass, manager, "startTagInventory")
               || TryInvokeNoArg(managerClass, manager, "startInventory")
               || TryInvokeNoArg(managerClass, manager, "startInventoryTag");
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
            var timerArgs = new JValue[] { new((short)100) };
            return JNIEnv.CallObjectMethod(manager, method, timerArgs);
        }

        method = JNIEnv.GetMethodID(managerClass, "tagInventoryByTimer", "(I)Ljava/util/List;");
        if (method != IntPtr.Zero)
        {
            var timerArgs = new JValue[] { new(180) };
            return JNIEnv.CallObjectMethod(manager, method, timerArgs);
        }

        method = JNIEnv.GetMethodID(managerClass, "tagInventoryRealTime", "()Ljava/util/List;");
        if (method != IntPtr.Zero)
            return JNIEnv.CallObjectMethod(manager, method);

        method = JNIEnv.GetMethodID(managerClass, "inventoryRealTime", "()Ljava/util/List;");
        if (method != IntPtr.Zero)
            return JNIEnv.CallObjectMethod(manager, method);

        return IntPtr.Zero;
    }

    private static bool TryInvokeNoArg(IntPtr managerClass, IntPtr manager, string methodName)
    {
        var method = JNIEnv.GetMethodID(managerClass, methodName, "()V");
        if (method != IntPtr.Zero)
        {
            JNIEnv.CallVoidMethod(manager, method);
            return true;
        }

        method = JNIEnv.GetMethodID(managerClass, methodName, "()Z");
        if (method != IntPtr.Zero)
        {
            JNIEnv.CallBooleanMethod(manager, method);
            return true;
        }

        return false;
    }

    private static string? TryExtractFirstEpc(IntPtr listHandle)
    {
        var listClass = JNIEnv.FindClass("java/util/List");
        var sizeMethod = JNIEnv.GetMethodID(listClass, "size", "()I");
        var getMethod = JNIEnv.GetMethodID(listClass, "get", "(I)Ljava/lang/Object;");
        var size = JNIEnv.CallIntMethod(listHandle, sizeMethod);
        if (size <= 0)
            return null;

        var getArgs = new JValue[] { new(0) };
        var tagInfo = JNIEnv.CallObjectMethod(listHandle, getMethod, getArgs);
        if (tagInfo == IntPtr.Zero)
            return null;

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
