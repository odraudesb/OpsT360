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
            var manager = JNIEnv.CallStaticObjectMethod(managerClass, getInstance);
            if (manager == IntPtr.Zero)
                return RfidReadResult.Fail("No fue posible inicializar el módulo RFID (getInstance = null).");

            var setPower = JNIEnv.GetMethodID(managerClass, "setPower", "(II)Lcom/uhf/api/cls/Reader$READER_ERR;");
            if (setPower != IntPtr.Zero)
            {
                var powerArgs = new JValue[] { new(30), new(30) };
                JNIEnv.CallObjectMethod(manager, setPower, powerArgs);
            }

            var inventoryMethod = JNIEnv.GetMethodID(managerClass, "tagInventoryByTimer", "(S)Ljava/util/List;");
            if (inventoryMethod == IntPtr.Zero)
                return RfidReadResult.Fail("Método tagInventoryByTimer no disponible en el SDK.");

            for (var i = 0; i < 6; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var timerArgs = new JValue[] { new((short)120) };
                var tagsList = JNIEnv.CallObjectMethod(manager, inventoryMethod, timerArgs);
                if (tagsList == IntPtr.Zero)
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }

                var epc = TryExtractFirstEpc(tagsList);
                if (!string.IsNullOrWhiteSpace(epc))
                    return RfidReadResult.Ok(epc);

                await Task.Delay(100, cancellationToken);
            }

            return RfidReadResult.Fail("No se detectó EPC. Acerca el sello y vuelve a gatillar.");
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
