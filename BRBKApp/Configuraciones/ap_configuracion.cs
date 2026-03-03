using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;
using Respuesta;

namespace Configuraciones
{
    public enum ConfiguracionTipo
    {
        Aplicacion, Conexion
    }
    public class ap_configuracion
    {
        public string aplicacion { get; set; }
        public string modulo { get; set; }
        public string nombre { get; set; }
        public string valor { get; set; }
        public string tipodato { get; set; }
        //Obtiene un aconfiguracion desde archivo
        public static  ResultadoOperacion<bool> ComprobarArchivoConfiguracion(string _nombre, ConfiguracionTipo _tipo)
        {
            if (_tipo == ConfiguracionTipo.Conexion)
            {
                var x = System.Configuration.ConfigurationManager.ConnectionStrings[_nombre?.Trim()];
                if (x == null || string.IsNullOrEmpty(x.ConnectionString))
                    return ResultadoOperacion<bool>.CrearFalla(string.Format("No existe en archivo la cadena con nombre {0}",_nombre));
                return ResultadoOperacion<bool>.CrearResultadoExitoso(true,x.ConnectionString);
            }
            if (_tipo == ConfiguracionTipo.Conexion)
            {
                var x = System.Configuration.ConfigurationManager.AppSettings[_nombre?.Trim()];
                if (string.IsNullOrEmpty(x))
                    return ResultadoOperacion<bool>.CrearFalla(string.Format("No existe en archivo la configuración con nombre {0}", _nombre));
                return ResultadoOperacion<bool>.CrearResultadoExitoso(true, x);
            }
            return ResultadoOperacion<bool>.CrearFalla("Tipo de configuración no implementado");
        }
    }
}
