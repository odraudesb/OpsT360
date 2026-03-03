using ApiModels.AppModels;
using MiWebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using BRBKWebApiData;
using System.Web.Http;
using ViewModel;
using static ViewModel.Enumerados;

namespace BRBKWebApi.Controllers.Bodega
{
    public class AisvUbicacionController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpPost]
        [Route("api/VBS_get_ubicacionPorBarcode")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Catalogo_Ubicacion> GetUbicacionPorBarcode([FromBody] ParametrosUbicacionVBS.ParametrosConsultaUbicacionPorBarcode pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            BAN_Catalogo_Ubicacion ObjEntidad = new BAN_Catalogo_Ubicacion();
            RespuestaViewModel<BAN_Catalogo_Ubicacion> respuesta = new RespuestaViewModel<BAN_Catalogo_Ubicacion>();
            string oError = string.Empty;

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entity = BAN_Catalogo_UbicacionDA.GetEntidad(pObj.barcode);//dbContext.Tasks.AsNoTracking().Where(p => p.Id == pTarea.Id).FirstOrDefault();
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el barcode de la ubicación:{0}", pObj.barcode));
                        Valido = false;
                    }
                    else
                    {


                        Entity.oBodega = BAN_Catalogo_BodegaDA.GetEntidad(Entity.idBodega);
                        Entity.oBloque = BAN_Catalogo_BloqueDA.GetEntidad(Entity.idBloque);
                        Entity.oFila = BAN_Catalogo_FilaDA.GetEntidad(Entity.idFila);
                        Entity.oAltura = BAN_Catalogo_AlturaDA.GetEntidad(Entity.idAltura);
                        Entity.oProfundidad = BAN_Catalogo_ProfundidadDA.GetEntidad(Entity.idProfundidad);

                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.id;
                        ObjEntidad = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Ubicación";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjEntidad ?? new BAN_Catalogo_Ubicacion();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetUbicacionPorBarcode), "api/VBS_get_ubicacionPorBarcode", false, null, null, ex.StackTrace, ex);

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