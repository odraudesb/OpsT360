
namespace MiWebApi
{
    public class ParametrosSlotVBS
    {

        public class ParametrosConsultaSlotDisponibles
        {
            public int idBodega { get; set; }
            public int idBloque { get; set; }
            public int idFila { get; set; }
            public int idAltura { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (idBodega <= 0)
                {
                    msg = "Especifique la bodega de la ubicación";
                    return 0;
                }

                if (idBloque <= 0)
                {
                    msg = "Especifique el bloque de la ubicación";
                    return 0;
                }

                if (idFila <= 0)
                {
                    msg = "Especifique la fila de la ubicación";
                    return 0;
                }

                if (idAltura <= 0)
                {
                    msg = "Especifique la altura de la ubicación";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
    }
}
