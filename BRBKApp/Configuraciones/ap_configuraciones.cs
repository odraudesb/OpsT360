using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccesoDatos;
using Respuesta;

namespace Configuraciones
{
    public class ap_configuraciones : BaseEntidad
    {
        #region "Singleton"
        public static ap_configuraciones CrearInstancia(string _config)
        {
            if (configs == null)
                configs = new ap_configuraciones(_config);

            if (smessages == null)
                smessages = ap_mensajes.CrearInstancia(_config);
            return configs;
        }
        #endregion
        //mantiene tpodas las configuraciones
        public List<ap_configuracion> Lista { get; private set; }

        private string _con = string.Empty;
        private static string _error = null;
  

        public bool Verificada { get; private set; }

        //el nombre de la configuracion de base de datos
        private ap_configuraciones(string db_conf_name)
        {
            this.Lista = new List<ap_configuracion>();
            //VAlidar q existan la cadena de configuracion de los seting
            var rp = ap_configuracion.ComprobarArchivoConfiguracion(db_conf_name, ConfiguracionTipo.Conexion);
            if (!rp.Exitoso)
            {
                _error = rp.MensajeProblema;
                this.Verificada = false;
            }
            else
            {
                _con = rp.MensajeInformacion;
                this.Verificada = true;
            }
           
        }

        public ResultadoOperacion<bool> CargarConfiguraciones(string app_name)
        {
            this.Lista = new List<ap_configuracion>();
            //no existe
            if (!this.Verificada)
            {
                return ResultadoOperacion<bool>.CrearFalla(_error);
            }
            InitMethod();
            parametros.Clear();
            parametros.Add("apk", app_name);
            var rp =BDOpe.ComandoSelectALista<ap_configuracion>(_con, "[brbk].BRBK_Lee_Configuraciones", null);
            Lista = rp.Resultado;
            return rp.Exitoso? ResultadoOperacion<bool>.CrearResultadoExitoso(true) : ResultadoOperacion<bool>.CrearFalla(rp.MensajeProblema);
            //pc_bil_leer_ap_configuraciones
        }

        //retorna una configuracion.
        public ResultadoOperacion<ap_configuracion> ObtenerConfiguracion(string nombre)
        {
            var p = Lista?.Where(s => s.nombre.ToLower().Equals(nombre?.Trim().ToLower())).FirstOrDefault();
            if (p != null)
            {
                return ResultadoOperacion<ap_configuracion>.CrearResultadoExitoso(p);
            }
            return ResultadoOperacion<ap_configuracion>.CrearFalla(string.Format("Configuración [{0}] no encontrada", nombre));

        }



    }
}
