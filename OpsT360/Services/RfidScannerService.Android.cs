#if ANDROID
using Android.Runtime;

namespace OpsT360.Services;

public partial class RfidScannerService
{
    private partial async Task<RfidReadResult> TryReadSingleEpcPlatformAsync(CancellationToken cancellationToken)
    {
        try
        {
            var managerClass = JNIEnv.FindClass("com/handheld/uhfr/UHFRManager");
            if (managerClass == IntPtr.Zero)
                return RfidReadResult.Fail("No se encontró UHFRManager en los JAR del SDK.");

            var getInstance = JNIEnv.GetStaticMethodID(managerClass, "getInstance", "()Lcom/handheld/uhfr/UHFRManager;");
            if (getInstance == IntPtr.Zero)
                return RfidReadResult.Fail("No se encontró getInstance() en UHFRManager.");

            var manager = JNIEnv.CallStaticObjectMethod(managerClass, getInstance);
            if (manager == IntPtr.Zero)
                return RfidReadResult.Fail("No fue posible inicializar el módulo RFID (getInstance = null).");

            TrySetPower(managerClass, manager);
            TryInvokeNoArg(managerClass, manager, "stopTagInventory");
            TryInvokeNoArg(managerClass, manager, "stopInventory");

            for (var i = 0; i < 10; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var tagsList = TryInventory(managerClass, manager);
                if (tagsList != IntPtr.Zero)
                {
                    var epc = TryExtractFirstEpc(tagsList);
                    if (!string.IsNullOrWhiteSpace(epc))
                        return RfidReadResult.Ok(epc);
                }

                await Task.Delay(120, cancellationToken);
            }

            return RfidReadResult.Fail("No se detectó EPC por antena RFID. Acerca el sello azul al hand-held y gatilla de nuevo.");
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

    private static void TrySetPower(IntPtr managerClass, IntPtr manager)
    {
        var setPower = JNIEnv.GetMethodID(managerClass, "setPower", "(II)Lcom/uhf/api/cls/Reader$READER_ERR;");
        if (setPower == IntPtr.Zero)
            return;

        var powerArgs = new JValue[] { new(30), new(30) };
        JNIEnv.CallObjectMethod(manager, setPower, powerArgs);
    }

    private static IntPtr TryInventory(IntPtr managerClass, IntPtr manager)
    {
        var method = JNIEnv.GetMethodID(managerClass, "tagInventoryByTimer", "(S)Ljava/util/List;");
        if (method != IntPtr.Zero)
        {
            var timerArgs = new JValue[] { new((short)180) };
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

    private static void TryInvokeNoArg(IntPtr managerClass, IntPtr manager, string methodName)
    {
        var method = JNIEnv.GetMethodID(managerClass, methodName, "()V");
        if (method != IntPtr.Zero)
        {
            JNIEnv.CallVoidMethod(manager, method);
            return;
        }

        method = JNIEnv.GetMethodID(managerClass, methodName, "()Z");
        if (method != IntPtr.Zero)
            JNIEnv.CallBooleanMethod(manager, method);
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
}
#endif
