using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;
using Respuesta;

namespace Configuraciones
{
    public abstract class BaseEntidad
    {

        //los parametros para las operaciones
        public static Dictionary<string, object> parametros { get; set; }
        public static string _classLocal;
        public static string _methodLocal;
        public static string _Error;
        public static string _Conexion;
        public static ap_configuraciones configs = null;
        public static ap_mensajes smessages = null;



        //OBTIENE EL NOMBRE DE CLASE INSTANCIA
        public void init()
        {
            parametros = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(_classLocal))
                _classLocal = this.GetType().Name;
        }
        //OBTIENE EL NOMBRE DE STATICO
        public static void InitMethod()
        {
            if (parametros == null)
                parametros = new Dictionary<string, object>();

            parametros.Clear();
            var tme = MethodBase.GetCurrentMethod();
            if (string.IsNullOrEmpty(_classLocal))
            {
                if (!tme.IsStatic)
                {
                    _classLocal = MethodBase.GetCurrentMethod()?.DeclaringType?.Name;
                }
                _methodLocal = MethodBase.GetCurrentMethod()?.Name;
            }
            if (string.IsNullOrEmpty(_methodLocal))
            {
                _methodLocal = MethodBase.GetCurrentMethod()?.Name;
            }
        }



        public static bool initConfig(string modulo)
        {
            _Error = string.Empty;
            if (configs == null)
            {
                configs = ap_configuraciones.CrearInstancia("BRBK");
            }


            ResultadoOperacion<bool> r = null;
            configs = ap_configuraciones.CrearInstancia("BRBK");
            if ( configs.Lista == null || configs.Lista.Count <= 0 || configs.Lista.Where(s=>s.modulo.ToLower().Equals(modulo.ToLower())).Count() <=0)
            {
                r = configs.CargarConfiguraciones(modulo);
                smessages.CargarMensajes(modulo);
                _Error = r.Exitoso ? string.Empty : r.MensajeProblema;
                return r.Exitoso;
            }

   
            return true;
        }


    }

}
