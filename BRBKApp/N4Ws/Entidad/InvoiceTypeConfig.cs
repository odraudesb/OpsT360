using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuraciones;
using Respuesta;
using AccesoDatos;
using System.Reflection;
using System.Xml.Linq;

namespace N4Ws.Entidad
{
    public class InvoiceTypeConfig:ModuloBase
    {

        public override void OnInstanceCreate()
        {
            this.alterClase = "N4_SERVICE";
            base.OnInstanceCreate();
            this.Accesorio.ConfiguracionBase = "BRBK";
        }

        private static InvoiceTypeConfig InicializaServicio(out string erno)
        {
            var p = new InvoiceTypeConfig();
            if (!p.Accesorio.Inicializar(out erno))
            {
                return null;
            }
            return p;
        }

        public static Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>> ObtenerInvoicetypes()
        {
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
              return Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>>.CrearFalla(pv);
            }
            if (!p.Accesorio.ExistenConfiguraciones)
            {
                return Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>>.CrearFalla(string.Format( "No existen configuraciones para la aplicación {0}",p.alterClase));
            }

            p.Parametros.Clear();
            var bcon = p.Accesorio.ObtenerConfiguracion("N5")?.valor;
            var result = BDOpe.ComandoSelectALista<InvoiceTypeConfig>(bcon, "[Bill].[listar_invoice_config]", p.Parametros);
            if (!result.Exitoso)
            {
                return Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>>.CrearFalla(result.MensajeProblema, result.MensajeInformacion);
            }
            return Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>>.CrearResultadoExitoso(result.Resultado);
        }


        public static Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>> ObtenerInvoicetypesFull()
        {
            string pv;
            var p = InicializaServicio(out pv);
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            if (p == null)
            {
                return Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>>.CrearFalla(pv);
            }
            if (!p.Accesorio.ExistenConfiguraciones)
            {
                return Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>>.CrearFalla(string.Format("No existen configuraciones para la aplicación {0}", p.alterClase));
            }

            p.Parametros.Clear();
            var bcon = p.Accesorio.ObtenerConfiguracion("N5")?.valor;
            var result = BDOpe.ComandoSelectALista<InvoiceTypeConfig>(bcon, "[Bill].[listar_invoice_config_full]", p.Parametros);
            if (!result.Exitoso)
            {
                return Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>>.CrearFalla(result.MensajeProblema, result.MensajeInformacion);
            }
            return Respuesta.ResultadoOperacion<List<InvoiceTypeConfig>>.CrearResultadoExitoso(result.Resultado);
        }


        public int id { get; set; }
        public string codigo { get; set; }
        public string valor { get; set; }
        public string tipo { get; set; }
        public string servicio { get; set; }



        public InvoiceTypeConfig(int _id, string _cod, string _val)
        {
            this.codigo = _cod;
            this.codigo = _cod;
            this.valor = _val;

        }
        public InvoiceTypeConfig(int _id, string _cod, string _val, string _tip, string _ser)
        {
            this.codigo = _cod;
            this.codigo = _cod;
            this.valor = _val;
            this.tipo = _tip;
            this.servicio = _ser;

        }
        public InvoiceTypeConfig()
        {

        }
    }
}
