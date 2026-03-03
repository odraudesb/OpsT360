
namespace MiWebApi
{
    public class ParametrosUbicacionVBS
    {

        public class ParametrosConsultaUbicacionPorBarcode
        {
            public string barcode { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(barcode))
                {
                    msg = "Especifique el barcode de la ubicación";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
    }
}