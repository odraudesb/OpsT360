using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static ViewModel.Enumerados;
using ViewModel;
using ReferencialVie = ViewModel;
using ApiModels.AppModels;
using BRBKWebApiData;
using System.IO;
using System.Drawing;
using System.Xml.Linq;
using System.ServiceModel;
using BRBKWebApi.ServicioAduana;

namespace MiWebApi.Controllers.P2D
{
    public class TransporteController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpPost]
        [Route("api/servicio_paletizado")]
        [ValidateModelAttribute]
        public RespuestaViewModel<bool> Servicio_Paletizado([FromBody] ParametrosPallet.ParametrosPaletizado pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<bool> respuesta = new RespuestaViewModel<bool>();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    //valida que no exista el despacho del BL
                    if (OnError != string.Empty)
                    {
                        Mensaje.Add(string.Format("Error al cargar servicio de paletizado, No se pudo carga servicio a la carga:{0} - {1}", pTarea.numero_carga, OnError));
                        Valido = false;
                    }
                    else
                    {

                        //saco el número de gkey de la unidad
                        var Entity = Gkey_Cargacfs.GetGkey(pTarea.numero_carga);
                        if (Entity == null)
                        {
                            Mensaje.Add(string.Format("No existe información con el número de carga : {0}, no se puede cargar servicio de paletizado", pTarea.numero_carga));
                            Valido = false;
                        }
                        else
                        {


                            XDocument XMLContenedores = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                                new XElement("CONTENEDORES", new XElement("CONTENEDOR",
                                                     new XAttribute("gkey", Entity.GKEY == null ? "0" : Entity.GKEY.ToString()),
                                                     new XAttribute("contenedor", pTarea.contenedor == null ? "" : pTarea.contenedor.Trim())
                                                     )));


                            List<ValidaPaletizado> ListTransp = Valida_Paletizado.Validacion_Peletizado(XMLContenedores.ToString(), out OnError);
                            if (!String.IsNullOrEmpty(OnError))
                            {

                                Mensaje.Add(string.Format("No existe información con el número de carga : {0}, no se puede validar servicio de paletizado", pTarea.numero_carga));
                                Valido = false;
                            }
                            else 
                            {
                                var LinqTransp = (from TblFact in ListTransp.Where(TblFact => TblFact.gkey != 0 && TblFact.servicio == 0)
                                                  select new
                                                  {
                                                      gkey = TblFact.gkey,
                                                      contenedor = TblFact.contenedor,
                                                      servicio = TblFact.servicio
                                                  }).Distinct().FirstOrDefault();

                                if (LinqTransp != null)
                                {
                                    var n4 = N4Ws.Entidad.Servicios.PonerEvento_Paletizado(LinqTransp.gkey.Value, pTarea.usuario, pTarea.cantidad);
                                    if (!n4.Exitoso)
                                    {
                                        string CuerpoMensaje = string.Format("Se presentaron los siguientes problemas al cargar servicio paletizado: {0}", n4.MensajeProblema);
                                        Mensaje.Add(string.Format("{0}", CuerpoMensaje));
                                        Valido = false;
                                    }
                                    else
                                    {
                                        IdGenerado = 1;
                                        Mensaje.Add(string.Format("Se cargo servicio de paletizado con éxito: {0} ", n4?.MensajeInformacion));
                                        Valido = true;

                                    }
                                }
                                else 
                                {
                                    IdGenerado = 1;
                                    Mensaje.Add(string.Format("El servicio de paletizado ya consta como cargado al # de carga: {0}", pTarea.numero_carga));
                                    Valido = true;
                                }
                                    
                            }


                            //   var Entity = Valida_Paletizado.Validacion_Peletizado(pObj.numeroPase);

                        }

                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Servicio Paletizado";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = Valido;
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Servicio_Paletizado), "api/servicio_paletizado", false, null, null, ex.StackTrace, ex);

                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("EXCEPCION NO CONTROLADA # {0}: {1}", lm, ex.Message.ToString()));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;
        }

    }
}
