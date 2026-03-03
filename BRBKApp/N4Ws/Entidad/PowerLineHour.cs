using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AccesoDatos;
using Configuraciones;
using Respuesta;



namespace N4Ws.Entidad
{
    public enum UNIT_CATEG
    {
        IMPORT,EXPORT
    }

    [Serializable]
    [XmlRoot(ElementName = "PowerLineHour")]
   public class PowerLineHour:ModuloBase
    {
        [XmlAttribute(AttributeName = "gkey")]
        public Int64 gkey { get; set; }
        [XmlAttribute(AttributeName = "CUSTOMPOWERH_GKEY")]
        public Int64? CUSTOMPOWERH_GKEY { get; set; }
        [XmlAttribute(AttributeName = "CUSTOMPOWERH_LINE")]
        public string CUSTOMPOWERH_LINE { get; set; }
        [XmlAttribute(AttributeName = "CUSTOMPOWERH_HOURS")]
        public double? CUSTOMPOWERH_HOURS { get; set; }
        [XmlAttribute(AttributeName = "CUSTOMPOWERH_UNITID")]
        public string CUSTOMPOWERH_UNITID { get; set; }
        [XmlAttribute(AttributeName = "CUSTOMPOWERH_VVISIT")]
        public string CUSTOMPOWERH_VVISIT { get; set; }
        [XmlAttribute(AttributeName = "CUSTOMPOWERH_CATEG")]
        public UNIT_CATEG CUSTOMPOWERH_CATEG { get; set; }
        //para obtenerlo y validarlo nada mas
        public override void OnInstanceCreate()
        {
            this.alterClase = "N4_SERVICE";
            base.OnInstanceCreate();
            this.Accesorio.ConfiguracionBase = "BRBK";
        }
        public static ResultadoOperacion<List<PowerLineHour>> BuscarPorReferencia(string referencia, UNIT_CATEG categoria, string   usuario)
        {
            var p = new PowerLineHour();
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            string pv;
            if (!p.Accesorio.Inicializar(out pv))
            {
                return ResultadoOperacion<List<PowerLineHour>>.CrearFalla(pv);
            }
            p.Parametros.Clear();
            p.Parametros.Add("CUSTOMPOWERH_VVISIT", referencia);
            p.Parametros.Add("CUSTOMPOWERH_CATEG", categoria);
            var bcon = p.Accesorio.ObtenerConfiguracion("N5")?.valor;

#if DEBUG
            p.LogEvent(usuario, p.actualMetodo, usuario);
#endif
            var rp = BDOpe.ComandoSelectALista<PowerLineHour>(bcon, "[Bill].[BuscarPowerLineHour]", p.Parametros);
            return rp.Exitoso ? ResultadoOperacion<List<PowerLineHour>>.CrearResultadoExitoso(rp.Resultado) : ResultadoOperacion<List<PowerLineHour>>.CrearFalla(rp.MensajeProblema);
        }
        public static ResultadoOperacion<List<PowerLineHour>> BuscarPorLinea(string linea, UNIT_CATEG categoria, string usuario)
        {

            var p = new PowerLineHour();
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            string pv;
            if (!p.Accesorio.Inicializar(out pv))
            {
                return ResultadoOperacion<List<PowerLineHour>>.CrearFalla(pv);
            }
            p.Parametros.Clear();
            p.Parametros.Add("CUSTOMPOWERH_LINE", linea);
            p.Parametros.Add("CUSTOMPOWERH_CATEG", categoria);
            var bcon = p.Accesorio.ObtenerConfiguracion("N5")?.valor;
#if DEBUG
            p.LogEvent(usuario, p.actualMetodo, usuario);
#endif
            var rp = BDOpe.ComandoSelectALista<PowerLineHour>(bcon, "[Bill].[BuscarPowerLineHour]", p.Parametros);
            return rp.Exitoso ? ResultadoOperacion<List<PowerLineHour>>.CrearResultadoExitoso(rp.Resultado) : ResultadoOperacion<List<PowerLineHour>>.CrearFalla(rp.MensajeProblema);
        }
        public static ResultadoOperacion<PowerLineHour> RecuperarPorGkey(Int64 gkey,string usuario)
        {
            var p = new PowerLineHour();
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            string pv;
            if (!p.Accesorio.Inicializar(out pv))
            {
                return ResultadoOperacion<PowerLineHour>.CrearFalla(pv);
            }
            p.Parametros.Clear();
            p.Parametros.Add("CUSTOMPOWERH_GKEY", gkey);
            var bcon = p.Accesorio.ObtenerConfiguracion("N5")?.valor;
#if DEBUG
            p.LogEvent(usuario, p.actualMetodo, usuario);
#endif
            var rp = BDOpe.ComandoSelectAEntidad<PowerLineHour>(bcon, "[Bill].[BuscarPowerLineHour]", p.Parametros);
            return rp.Exitoso ? ResultadoOperacion<PowerLineHour>.CrearResultadoExitoso(rp.Resultado) : ResultadoOperacion<PowerLineHour>.CrearFalla(rp.MensajeProblema);
        }
        //retorna los que no tienen PowerReeferHour
        public static ResultadoOperacion<List<PowerLineHour>> RecuperarFaltantes(List<Int64> keys, string usuario)
        {
            //recupera por gkey una lista.

            //arma un xml--> va a la base y solo obtiene los que faltan (tabla update)
            var p = new PowerLineHour();
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            string pv;
            if (!p.Accesorio.Inicializar(out pv))
            {
                return ResultadoOperacion<List<PowerLineHour>>.CrearFalla(pv);
            }
            if (keys == null || keys.Count <= 0)
            {
                return ResultadoOperacion<List<PowerLineHour>>.CrearFalla("Lista de keys vacía");
            }
            StringBuilder sml = new StringBuilder();
            sml.Append("<cntrs>");
            keys.ForEach(v=> {
                sml.AppendFormat("<cntr id=\"{0}\" />",v);
            });
            sml.Append("</cntrs>");
            p.Parametros.Clear();
            p.Parametros.Add("UNITS", sml.ToString());
            var bcon = p.Accesorio.ObtenerConfiguracion("N5")?.valor;
            var rp = BDOpe.ComandoSelectALista<PowerLineHour>(bcon, "[Bill].[PowerLineFaltantes]", p.Parametros);
            return rp.Exitoso ? ResultadoOperacion<List<PowerLineHour>>.CrearResultadoExitoso(rp.Resultado) : ResultadoOperacion<List<PowerLineHour>>.CrearFalla(rp.MensajeProblema);
        }
        public static ResultadoOperacion<DataTable> ValidarUnidades(List<PowerLineHour> lista, string usuario)
        {
            var p = new PowerLineHour();
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            string pv;
            if (!p.Accesorio.Inicializar(out pv))
            {
                return ResultadoOperacion<DataTable>.CrearFalla(pv);
            }
            if (lista == null || lista.Count <= 0)
            {
                return ResultadoOperacion<DataTable>.CrearFalla("Lista de PW vacía");
            }
            StringBuilder sml = new StringBuilder();
            sml.Append("<cntrs>");
            lista.ForEach(v => {
                sml.AppendFormat("<cntr id=\"{0}\" linea=\"{1}\" trafico=\"{2}\" referencia=\"{3}\"  horas=\"{4}\"/>", v.CUSTOMPOWERH_UNITID,v.CUSTOMPOWERH_LINE,v.CUSTOMPOWERH_CATEG,v.CUSTOMPOWERH_VVISIT,v.CUSTOMPOWERH_HOURS);
            });
            sml.Append("</cntrs>");
            p.Parametros.Clear();
            p.Parametros.Add("UNITS", sml.ToString());
            var bcon = p.Accesorio.ObtenerConfiguracion("N5")?.valor;
            var rp = BDOpe.ComadoSelectADatatable(bcon, "[Bill].[PowerLineHourValidar]", p.Parametros);
            return rp.Exitoso ? ResultadoOperacion<DataTable>.CrearResultadoExitoso(rp.Resultado) : ResultadoOperacion<DataTable>.CrearFalla(rp.MensajeProblema);
        }
        public static ResultadoOperacion<DataTable> ProcesarPLH(List<PowerLineHour> lista, string usuario)
        {
            var dt = PWL_Log();
            //---aqui solo debemos procesar todos los registros PWH y pasarlos a N4
            if (lista.Count <= 0 || lista == null)
            {
                return ResultadoOperacion<DataTable>.CrearFalla("Tabla vacia");
            }
            if (string.IsNullOrEmpty(usuario))
            {
                return ResultadoOperacion<DataTable>.CrearFalla("Usuario nulo");
            }

            //cabecera--->insertar
            var p = new PowerLineHour();
            p.actualMetodo = MethodBase.GetCurrentMethod().Name;
            string pv;
            if (!p.Accesorio.Inicializar(out pv))
            {
                return ResultadoOperacion<DataTable>.CrearFalla(pv);
            }
            foreach (var pw in lista)
            {
                var row = dt.NewRow();
                row[nameof(pw.CUSTOMPOWERH_GKEY)] = pw.CUSTOMPOWERH_GKEY;
                row[nameof(pw.CUSTOMPOWERH_HOURS)] = pw.CUSTOMPOWERH_HOURS;
                row[nameof(pw.CUSTOMPOWERH_VVISIT)] = pw.CUSTOMPOWERH_VVISIT;
                row[nameof(pw.CUSTOMPOWERH_UNITID)] = pw.CUSTOMPOWERH_UNITID;
                row[nameof(pw.CUSTOMPOWERH_CATEG)] = pw.CUSTOMPOWERH_CATEG;
                row[nameof(pw.CUSTOMPOWERH_LINE)] = pw.CUSTOMPOWERH_LINE;
                //procesar en N4---> actualizar valor
                var n4r = Servicios.EjecutarGroovyHoras(pw, usuario);
                if (n4r.status != 1)
                {
                    var ms = n4r.messages.FirstOrDefault();
                    row["novedad"] = ms?.message_detail;
                    row["valid"] = false;
                }
                else
                {
                    row["novedad"] = "OK";
                    row["valid"] = true;
                }
                dt.Rows.Add(row);
            }
            return ResultadoOperacion < DataTable >.CrearResultadoExitoso( dt);
        }
        private static DataTable PWL_Log()
        {
            DataTable pwl = new DataTable();
            pwl.Columns.Add("gkey", typeof(Int64));
            pwl.Columns.Add("CUSTOMPOWERH_GKEY", typeof(Int64));
            pwl.Columns.Add("CUSTOMPOWERH_LINE", typeof(string));
            pwl.Columns.Add("CUSTOMPOWERH_HOURS", typeof(float));
            pwl.Columns.Add("CUSTOMPOWERH_UNITID", typeof(string));
            pwl.Columns.Add("CUSTOMPOWERH_VVISIT", typeof(string));
            pwl.Columns.Add("CUSTOMPOWERH_CATEG", typeof(string));
            pwl.Columns.Add("PWLogUID", typeof(Int64));
            pwl.Columns.Add("novedad", typeof(string));
            pwl.Columns.Add("valid", typeof(bool));

            return pwl;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<groovy class-name=\"CGSAComputeReeferExpoWS\" class-location=\"code-extension\">");
            sb.Append("<parameters>");
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />",nameof(gkey),CUSTOMPOWERH_GKEY);
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", "line", CUSTOMPOWERH_LINE);
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", "qty", CUSTOMPOWERH_HOURS);
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", "unitid", CUSTOMPOWERH_UNITID);
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", "referencia", CUSTOMPOWERH_VVISIT);
            sb.AppendFormat("<parameter id=\"{0}\" value=\"{1}\" />", "categoria", CUSTOMPOWERH_CATEG);
            sb.Append("</parameters></groovy>");

            return sb.ToString();

        }

    }
}
