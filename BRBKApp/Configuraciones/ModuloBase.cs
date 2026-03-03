using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;

namespace Configuraciones
{
    public class ModuloBase
    {
        public ModuloBase()
        {
            this.Parametros = new Dictionary<string, object>();
            OnInstanceCreate();
           // this.Accesorio = new ModuloAccesorios(_alter);
        }

        public virtual void OnInstanceCreate() { this.Accesorio = new ModuloAccesorios(_alter); }


        public Tuple<string, string> SetMessage(string code,string metodo, string usuario)
        {
            ap_mensaje pm = null;
            if (this.Accesorio.ExistenMensajes)
            {
                pm = this.Accesorio.ObtenerMensaje(code);
            }
            if (pm == null)
            {
                this.LogError<ApplicationException>(new ApplicationException(string.Format("No existe codigo para mensaje {0}", code)), metodo, usuario);
                return Tuple.Create<string, string>("Ha surgido un inconveniente mientras se atendía su solicitud", "Comuníquese con Servicios al Cliente.");
            }
            return Tuple.Create<string, string>(pm.mensaje_pantalla, pm.opcion_posible);
        }

        public string myClase { get { return this.GetType()?.Name; } private set {  } }

       
        public string actualMetodo { get;set;}

        public Dictionary<string,object> Parametros { get; set; }

        public ModuloAccesorios Accesorio { get; private set; }
        //para cuando el modulo no tiene el mismo nombre en el settig

        string _alter;
        public string alterClase { get { return _alter; } set { _alter = myClase = value; } }

        public Int64? LogError<T>(T ex, string metodo, string usuario) where T:Exception
        {
           
            return BDTraza.LogEvent<T>(usuario, "BRBK", metodo, false, myClase, Parametros, actualMetodo, ex);
        }

        public Int64? LogEvent(string usuario, string metodo, string nota)
        {
            Int64? r = 0;
#if DEBUG
            r= BDTraza.TraceMove(usuario, "RCC", myClase, metodo,  nota);
#endif
            return r;
        }


    }
    public class ModuloAccesorios
    {
      
        private string _modulo;
        public List<ap_configuracion> Configuraciones { get; private set; }
        public List<ap_mensaje> Mensajes { get; private set; }
        private static string confile;


        string _base;
        public string ConfiguracionBase { get{ return _base; } set { _base = value; } }
        public bool ExistenConfiguraciones { get { return this.Configuraciones != null && this.Configuraciones.Count > 0; }}
        public bool ExistenMensajes { get { return this.Mensajes != null && this.Mensajes.Count > 0; } }
        public ModuloAccesorios(string _module)
        {
            //inicialziar, configuraciones y mensajes
            _modulo = _module;
        }
        public bool Inicializar(out string _problema)
        {
            if (string.IsNullOrEmpty(_base))
            {
                _problema = "Inicialize la propiedad ConfiguracionBase que representa la cadena de conexión a datos en archivo";
                return false;
            }
            confile = !string.IsNullOrEmpty(confile) ? confile: System.Configuration.ConfigurationManager.ConnectionStrings[ConfiguracionBase?.Trim()]?.ConnectionString;
            if (string.IsNullOrEmpty(confile))
            {
                _problema = string.Format("La cadena de conexión {0} no esta registrada en archivo", ConfiguracionBase);
                return false;
            }
            var xp = new Dictionary<string, object>();
            xp.Add("apk", _modulo);

            if (!ExistenConfiguraciones)
            {
                var rp = BDOpe.ComandoSelectALista<ap_configuracion>(confile, "[brbk].BRBK_Lee_Configuraciones", null);
                this.Configuraciones = rp.Exitoso ? rp.Resultado : null;
                if (!ExistenConfiguraciones) { _problema = rp.MensajeProblema; return false; }
            }

            if (!ExistenMensajes)
            {
                //var rpp = BDOpe.ComandoSelectALista<ap_mensaje>(confile, "Bill.mensajes_leer", xp);
                //this.Mensajes = rpp.Exitoso ? rpp.Resultado : new List<ap_mensaje>();
                //if (!ExistenMensajes) { _problema = rpp.MensajeProblema; return true; }
            }

            xp = null;
            _problema = string.Empty;
            return true;
            
        }


        //Obtiene una configuracion.
        public ap_configuracion ObtenerConfiguracion(string _nombre)
        {
            if (!ExistenConfiguraciones){ return null;}
            _nombre = _nombre?.Trim().ToLower();
            return this.Configuraciones.Where(t => t.nombre.Trim().ToLower() == _nombre)?.FirstOrDefault();
        }

        //obtiene un mensaje
        public ap_mensaje ObtenerMensaje(string _code)
        {
            if (!ExistenMensajes) { return null; }
            _code = _code?.Trim().ToLower();
            return this.Mensajes.Where(t => t.codigo.ToLower() == _code)?.FirstOrDefault();
        }

    }


   

}
