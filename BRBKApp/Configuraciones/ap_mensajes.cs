using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;
using Respuesta;

namespace Configuraciones
{
    public class ap_mensajes : BaseEntidad
    {

        public List<ap_mensaje> Lista { get; set; }
        private string _con = string.Empty;
        private static string _error = null;


        public bool Verificada { get; private set; }

        public static ap_mensajes CrearInstancia(string _config)
        {
            if (smessages == null)
                smessages = new ap_mensajes(_config);
            return smessages;
        }

        private ap_mensajes(string db_conf_name)
        {
            this.Lista = new List<ap_mensaje>();
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


        public Respuesta.ResultadoOperacion<bool> CargarMensajes(string app_name)
        {
            this.Lista = new List<ap_mensaje>();
            InitMethod();
            parametros.Clear();
            parametros.Add("apk", app_name);
            var rp = BDOpe.ComandoSelectALista<ap_mensaje>(_con, "Bill.mensajes_leer", parametros);
            Lista = rp.Resultado;
            return rp.Exitoso ? ResultadoOperacion<bool>.CrearResultadoExitoso(true) : ResultadoOperacion<bool>.CrearFalla(rp.MensajeProblema);
          }

        //retorna una configuracion.
        public ResultadoOperacion<ap_mensaje> ObtenerMensaje(string codigo)
        {
            var p = Lista?.Where(s => s.codigo.ToLower().Equals(codigo?.Trim().ToLower())).FirstOrDefault();
            if (p != null)
            {
                return ResultadoOperacion<ap_mensaje>.CrearResultadoExitoso(p);
            }
            return ResultadoOperacion<ap_mensaje>.CrearFalla(string.Format("Mensaje [{0}] no encontrado", codigo));

        }
    }
}
