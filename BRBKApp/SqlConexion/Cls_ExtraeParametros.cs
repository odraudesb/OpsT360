using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlConexion;

namespace SqlConexion
{
    class Cls_ExtraeParametros
    {
        private static Cls_Conexion puntero = null;
        private static List<Cls_Parametros> configuraciones { get; set; }
        private static Cls_ExtraeParametros instancia = null;

       

        private void initialize()
        {
            puntero = Cls_Conexion.Conexion();
            configuraciones = new List<Cls_Parametros>();
            string error; //hubo una novedad al cargar configuraciones
            configuraciones = puntero.ExecuteSelectControl<Cls_Parametros>(puntero.Conexion_Local, 2000, "[brbk].BRBK_Lee_Configuraciones", null, out error);
        }

     
        private static void OnInit()
        {

            puntero = (puntero == null) ? Cls_Conexion.Conexion() : puntero;
            configuraciones = new List<Cls_Parametros>();
            string error; //hubo una novedad al cargar configuraciones
            configuraciones = puntero.ExecuteSelectControl<Cls_Parametros>(puntero.Conexion_Local, 2000, "[brbk].BRBK_Lee_Configuraciones", null, out error);
        }

        public static Cls_ExtraeParametros Get_Instancia()
        {
            if (instancia == null)
                instancia = new Cls_ExtraeParametros();
            return instancia;
        }

        private Cls_ExtraeParametros()
        {
            initialize();
        }

        public static Cls_Parametros Get_Parametro(string nombre)
        {
            OnInit();
            return configuraciones?.Where(s => s.nombre.Trim().Equals(nombre)).FirstOrDefault();
            //si no hay esta nulo

        }


    }
}
