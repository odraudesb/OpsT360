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
using MiWebApi;

namespace BRBKWebApi.Controllers.Pesaje
{
    public class PesajeController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = 1;

        //////////////////////////////////////////
        //REGISTRO DE PESO DE CONTENEDOR
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/registra_pesaje")]
        [ValidateModelAttribute]
        public RespuestaViewModel<pesaje> RegistraPesaje([FromBody] ParametrosPesaje.ParametrosRegistraPeso pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string CuerpoMensaje = string.Empty;

            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<pesaje> respuesta = new RespuestaViewModel<pesaje>();
            pesaje ObjTarea = new pesaje();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    pesaje eObj = new pesaje();
                    eObj.id = 0;
                    eObj.gkey = pTarea.gkey;
                    eObj.container = pTarea.container;
                    eObj.peso = pTarea.peso;
                    eObj.estado = pTarea.estado;
                    eObj.ip = pTarea.ip;
                    eObj.mensaje = pTarea.mensaje;
                    eObj.usuarioCrea = pTarea.Create_user;

                    pesajeDA oSeal = new pesajeDA();
                    long? resultado = oSeal.Save_Update(eObj, out OnError);

                    if (resultado != null)
                    {
                        var result = pesajeDA.GetPesajePorId(resultado);

                        if (string.IsNullOrEmpty(CuerpoMensaje)) { CuerpoMensaje = "CNTR: " + result.container; }
                        if (result.estado)
                        {
                            IdGenerado = 1;
                            Mensaje.Add(string.Format("Registro de Peso con éxito: [{0}], [{1}] - [{2}] ", result.id, result.mensaje, CuerpoMensaje));
                            Valido = true;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("Registro de Peso con novedades: [{0}], [{1}] ", result.mensaje, CuerpoMensaje));
                            Valido = false;
                        }

                        ObjTarea = result;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se pudo grabar el registro de peso: {0} - {1}", OnError, CuerpoMensaje));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar Peso Containers";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new pesaje();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraPesaje), "api/registra_pesaje", false, null, null, ex.StackTrace, ex);

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