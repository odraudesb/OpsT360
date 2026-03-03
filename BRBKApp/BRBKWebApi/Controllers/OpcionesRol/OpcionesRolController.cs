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

namespace MiWebApi.Controllers.Recepcion.OpcionesRol
{
    public class OpcionesRolController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpPost]
        [Route("api/lista_opcionesRol")]
        [ValidateModelAttribute]

        public RespuestaViewModel<List<opcionesRoles>> Lista_OpcionesRol([FromBody] ParametrosOpcionesRol.ParametrosConsultaListaOpcionesRol pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<opcionesRoles> query = new List<opcionesRoles>();
            RespuestaViewModel<List<opcionesRoles>> respuesta = new RespuestaViewModel<List<opcionesRoles>>();

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    query = BRBKWebApiData.opcionesRolesDA.listadoOpciones(pObj.idRol, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe opciónes para el rol del usuario, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query.Count <= 0 && Valido)
                        {
                            Mensaje.Add(string.Format("No existe opciónes para el rol del usuario, idRol {0}", pObj.idRol));
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Opciones por Rol";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<opcionesRoles>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_OpcionesRol), "api/lista_opcionesRol", false, null, null, ex.StackTrace, ex);

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