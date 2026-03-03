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
using BRBKWebApi.Models.Parametros.Embarque;
using System.IO;
using System.Drawing;

namespace MiWebApi.Controllers.Embarque
{
    public class VBSEmbarqueController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpGet]
        [Route("api/VBS_embarque_lista_naves")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_NavesVBS()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaNavesVBS(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de naves");
                    Valido = false;
                }
                else
                {
                    if (ObjTarea.Count == 0)
                    {
                        Mensaje.Add("No existen registros de naves que mostrar");
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Total de registros:{0}", ObjTarea.Count));
                        Valido = true;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Naves";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<BAN_Consulta_Combo>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_NavesVBS), "api/VBS_embarque_lista_naves", false, null, null, ex.StackTrace, ex);

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

        [HttpGet]
        [Route("api/VBS_embarque_lista_exportadores")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_ExportadoresVBS()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaExportadoresVBS(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de exportadores");
                    Valido = false;
                }
                else
                {
                    if (ObjTarea.Count == 0)
                    {
                        Mensaje.Add("No existen registros de exportadores que mostrar");
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Total de registros:{0}", ObjTarea.Count));
                        Valido = true;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de exportadores";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<BAN_Consulta_Combo>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_NavesVBS), "api/VBS_embarque_lista_exportadores", false, null, null, ex.StackTrace, ex);

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

        [HttpGet]
        [Route("api/VBS_embarque_lista_tipoMovimiento")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_TipoMovimientosVBS()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaTipoMovimientoVBS(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de tipo");
                    Valido = false;
                }
                else
                {
                    if (ObjTarea.Count == 0)
                    {
                        Mensaje.Add("No existen registros de tipos que mostrar");
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Total de registros:{0}", ObjTarea.Count));
                        Valido = true;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de tipos de movimientos";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<BAN_Consulta_Combo>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_NavesVBS), "api/VBS_embarque_lista_tipoMovimiento", false, null, null, ex.StackTrace, ex);

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

        [HttpGet]
        [Route("api/VBS_embarque_lista_hold")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_HoldVBS()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaHoldVBS(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de holds");
                    Valido = false;
                }
                else
                {
                    if (ObjTarea.Count == 0)
                    {
                        Mensaje.Add("No existen registros de holds que mostrar");
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Total de registros:{0}", ObjTarea.Count));
                        Valido = true;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de holds";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<BAN_Consulta_Combo>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_NavesVBS), "api/VBS_embarque_lista_hold", false, null, null, ex.StackTrace, ex);

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

        [HttpGet]
        [Route("api/VBS_embarque_lista_decks")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_DeckVBS()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaPisoVBS(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de deck");
                    Valido = false;
                }
                else
                {
                    if (ObjTarea.Count == 0)
                    {
                        Mensaje.Add("No existen registros de decks que mostrar");
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Total de registros:{0}", ObjTarea.Count));
                        Valido = true;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de decks";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<BAN_Consulta_Combo>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_NavesVBS), "api/VBS_embarque_lista_decks", false, null, null, ex.StackTrace, ex);

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

        [HttpGet]
        [Route("api/VBS_embarque_lista_origen")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_OrigenVBS()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaOrigenVBS(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de origen");
                    Valido = false;
                }
                else
                {
                    if (ObjTarea.Count == 0)
                    {
                        Mensaje.Add("No existen registros de origen que mostrar");
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Total de registros:{0}", ObjTarea.Count));
                        Valido = true;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de origen";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<BAN_Consulta_Combo>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_NavesVBS), "api/VBS_embarque_lista_origen", false, null, null, ex.StackTrace, ex);

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

        [HttpPost]
        [Route("api/VBS_embarque_lista_brands")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_brands_Embarque([FromBody] ParametrosEmbarque.ParametrosConsultaListaBrandsEmbarque pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> query = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
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
                    query = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaMarcaVBS(pObj.idExportador, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe marca con el criterio ingresado, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe narca con los criterios ingresados");
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Marcas";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Consulta_Combo>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Inbox_Embarque), "api/VBS_embarque_lista_brands", false, null, null, ex.StackTrace, ex);

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

        [HttpPost]
        [Route("api/VBS_embarque_lista_inbox")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Embarque_Cab>> Lista_Inbox_Embarque([FromBody] ParametrosEmbarque.ParametrosConsultaListaInboxEmbarque pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Embarque_Cab> query = new List<BAN_Embarque_Cab>();
            RespuestaViewModel<List<BAN_Embarque_Cab>> respuesta = new RespuestaViewModel<List<BAN_Embarque_Cab>>();
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
                    query = BAN_Embarque_CabDA.ConsultarLista(pObj.idNave, pObj.idExportador, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Ordenes de Embarque con el criterio ingresado, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe Ordenes de Embarque con los criterios ingresados");
                            Valido = false;
                        }
                        else
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            foreach (var item in query)
                            {
                                item.oEstado = oEstado.Where(p => p.id == item.estado).FirstOrDefault();
                            }
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de ordenes de Embarque";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Embarque_Cab>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Inbox_Embarque), "api/VBS_lista_ordenesDespachoPorBodega", false, null, null, ex.StackTrace, ex);

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

        [HttpPost]
        [Route("api/VBS_embarque_inbox_PorId")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Embarque_Cab> GetEmbarqueCab([FromBody] ParametrosEmbarque.ParametrosConsultarEmbarque pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            BAN_Embarque_Cab oEntidad = new BAN_Embarque_Cab();
            RespuestaViewModel<BAN_Embarque_Cab> respuesta = new RespuestaViewModel<BAN_Embarque_Cab>();
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
                    var Entity = BAN_Embarque_CabDA.GetEntidad(long.Parse(pObj.id.ToString()));
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el Id de Inbox:{0}", pObj.id));
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.idEmbarqueCab;

                        try
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            //var oBodega = BAN_Catalogo_BodegaDA.ConsultarLista(out oError);
                            //var oBloque = BAN_Catalogo_BloqueDA.ConsultarLista(null, out oError);
                            //var oDetalle = BAN_Stowage_Plan_DetDA.GetEntidad(Entity.idStowageDet);
                            //var oCargo = BAN_Catalogo_CargoDA.ConsultarListaCargos(out oError);
                            var oConsignatario = BAN_Catalogo_ConsignatarioDA.ConsultarListaConsignatarios("CGSA", out oError);
                            var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                            var oHold = BAN_Catalogo_HoldDA.ConsultarListaHold(out oError);
                            var oMarca = BAN_Catalogo_MarcaDA.ConsultarListaMarca("CGSA", out oError);

                            Entity.oEstado = oEstado.Where(p => p.id == Entity.estado).FirstOrDefault();
                            Entity.oMovimientos = BAN_Embarque_MovimientoDA.ConsultarLista(pObj.id, out OnError);
                            //Entity.oStowage_Plan_Det = oDetalle;
                            //if (!(Entity.oStowage_Plan_Det is null))
                            //{
                            //    Entity.oStowage_Plan_Det.oBodega = oBodega.Where(p => p.id == Entity.oStowage_Plan_Det.idBodega).FirstOrDefault();
                            //    Entity.oStowage_Plan_Det.oBloque = oBloque.Where(p => p.id == Entity.oStowage_Plan_Det.idBloque).FirstOrDefault();
                            //    Entity.oStowage_Plan_Det.oCargo = oCargo.Where(p => p.id == Entity.oStowage_Plan_Det.idCargo).FirstOrDefault();
                            //    Entity.oStowage_Plan_Det.oConsignatario = oConsignatario.Where(p => p.id == Entity.oStowage_Plan_Det.idConsignatario).FirstOrDefault();
                            //    Entity.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == Entity.oStowage_Plan_Det.idExportador).FirstOrDefault();
                            //    Entity.oStowage_Plan_Det.oHold = oHold.Where(p => p.id == Entity.oStowage_Plan_Det.idBloque).FirstOrDefault();
                            //    Entity.oStowage_Plan_Det.oMarca = oMarca.Where(p => p.id == Entity.oStowage_Plan_Det.idMarca).FirstOrDefault();
                            //}
                        }
                        catch { }

                        oEntidad = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = oEntidad ?? new BAN_Embarque_Cab();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetEmbarqueCab), "api/VBS_embarque_inbox_PorId", false, null, null, ex.StackTrace, ex);

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

        [HttpPost]
        [Route("api/VBS_embarque_save")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Embarque_Cab> RegistraEmbarqueInbox([FromBody] ParametrosEmbarque.ParametrosRegistrarEmbarque pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<BAN_Embarque_Cab> respuesta = new RespuestaViewModel<BAN_Embarque_Cab>();
            BAN_Embarque_Cab ObjTarea = new BAN_Embarque_Cab();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    BAN_Embarque_Cab eEntidad = new BAN_Embarque_Cab();
                    eEntidad.idEmbarqueCab = pTarea.idEmbarqueCab;
                    eEntidad.barcode = pTarea.barcode;
                    eEntidad.idNave = pTarea.idNave;
                    eEntidad.nave = pTarea.nave;
                    eEntidad.idExportador = pTarea.idExportador;
                    eEntidad.Exportador = pTarea.Exportador;
                    eEntidad.estado = pTarea.estado;
                    eEntidad.usuarioCrea = pTarea.Create_user;
                    eEntidad.usuarioModifica = pTarea.Modifie_user;

                    BAN_Embarque_CabDA oPreDespacho = new BAN_Embarque_CabDA();
                    var result = oPreDespacho.Save_Update(eEntidad, out OnError);

                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se registro con éxito el embarque inbox, se genero la transacción No: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eEntidad.idEmbarqueCab = Id;
                        ObjTarea = eEntidad;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se pudo grabar el embarque inbox, verifique el siguiente mensaje; {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar Orden de Embarque";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Embarque_Cab();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraEmbarqueInbox), "api/VBS_embarque_save", false, null, null, ex.StackTrace, ex);

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

        //movimientos

        [HttpPost]
        [Route("api/VBS_embarque_lista_movimientos")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Embarque_Movimiento>> Lista_Movimientos_Embarque([FromBody] ParametrosEmbarque.ParametrosConsultarEmbarque pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Embarque_Movimiento> query = new List<BAN_Embarque_Movimiento>();
            RespuestaViewModel<List<BAN_Embarque_Movimiento>> respuesta = new RespuestaViewModel<List<BAN_Embarque_Movimiento>>();
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
                    query = BAN_Embarque_MovimientoDA.ConsultarLista(pObj.id, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe movimientos de Embarque con el criterio ingresado, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe movimientos de Embarque con los criterios ingresados");
                            Valido = false;
                        }
                        else
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            var oHold = BAN_Catalogo_HoldDA.ConsultarListaHold(out oError);
                            var oPiso = BAN_Catalogo_PisoDA.ConsultarLista(out oError);
                            var oTipoMov = BAN_Catalogo_TipoMovimientoDA.ConsultarLista(out oError);
                            foreach (var item in query)
                            {
                                item.oEstado = oEstado.Where(p => p.id == item.estado).FirstOrDefault();
                                item.oHold = oHold.Where(p => p.id == item.idHold).FirstOrDefault();
                                item.oPiso = oPiso.Where(p => p.id == item.idPiso).FirstOrDefault();
                                item.oTipoMovimiento = oTipoMov.Where(p => p.idTipo == item.idtipoMovimiento).FirstOrDefault();
                            }
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de movimientos de Embarque";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Embarque_Movimiento>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Inbox_Embarque), "api/VBS_lista_ordenesDespachoPorBodega", false, null, null, ex.StackTrace, ex);

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

        [HttpPost]
        [Route("api/VBS_embarque_movimiento_PorId")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Embarque_Movimiento> GetEmbarqueMovimiento([FromBody] ParametrosEmbarque.ParametrosConsultarEmbarque pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            BAN_Embarque_Movimiento oEntidad = new BAN_Embarque_Movimiento();
            RespuestaViewModel<BAN_Embarque_Movimiento> respuesta = new RespuestaViewModel<BAN_Embarque_Movimiento>();
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
                    var Entity = BAN_Embarque_MovimientoDA.GetEntidad(long.Parse(pObj.id.ToString()));
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el Id de movimiento:{0}", pObj.id));
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.idEmbarqueCab;

                        try
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            var oHold = BAN_Catalogo_HoldDA.ConsultarListaHold(out oError);
                            var oPiso = BAN_Catalogo_PisoDA.ConsultarLista(out oError);
                            var oTipoMov = BAN_Catalogo_TipoMovimientoDA.ConsultarLista(out oError);

                            Entity.oEstado = oEstado.Where(p => p.id == Entity.estado).FirstOrDefault();
                            Entity.oHold = oHold.Where(p => p.id == Entity.idHold).FirstOrDefault();
                            Entity.oPiso = oPiso.Where(p => p.id == Entity.idPiso).FirstOrDefault();
                            Entity.oTipoMovimiento = oTipoMov.Where(p => p.idTipo == Entity.idtipoMovimiento).FirstOrDefault();
                        }
                        catch { }

                        oEntidad = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = oEntidad ?? new BAN_Embarque_Movimiento();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetEmbarqueCab), "api/VBS_embarque_movimiento_PorId", false, null, null, ex.StackTrace, ex);

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

        [HttpPost]
        [Route("api/VBS_embarque_movimiento_save")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Embarque_Movimiento> RegistraDespacho([FromBody] ParametrosEmbarque.ParametrosRegistrarMovimiento pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<BAN_Embarque_Movimiento> respuesta = new RespuestaViewModel<BAN_Embarque_Movimiento>();
            BAN_Embarque_Movimiento ObjTarea = new BAN_Embarque_Movimiento();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    BAN_Embarque_Movimiento eEntidad = new BAN_Embarque_Movimiento();
                    eEntidad.idEmbarqueMovimiento = pTarea.idEmbarqueMovimiento;
                    eEntidad.idEmbarqueCab = pTarea.idEmbarqueCab;
                    eEntidad.origen = pTarea.origen;
                    eEntidad.codigoCaja = pTarea.codigoCaja;
                    eEntidad.idHold = pTarea.idHold;
                    eEntidad.idPiso = pTarea.idPiso;
                    eEntidad.idMarca = pTarea.idMarca;
                    eEntidad.idModalidad = pTarea.idModalidad;
                    eEntidad.box = pTarea.box;
                    eEntidad.tipo = pTarea.tipo;
                    eEntidad.idtipoMovimiento = pTarea.idtipoMovimiento;
                    eEntidad.comentario = pTarea.comentario;
                    eEntidad.estado = pTarea.estado;
                    eEntidad.usuarioCrea = pTarea.Create_user;
                    eEntidad.usuarioModifica = pTarea.Modifie_user;

                    List<fotoEmbarqueVBS> oFoto = new List<fotoEmbarqueVBS>();
                    eEntidad.Fotos = oFoto;

                    foreach (ParametrosEmbarque.ParametrosRegistraFotoMovimientoEmbarqueVBS oParam in pTarea.Fotos)
                    {
                        v_ruta = string.Empty;
                        Carga_Imagenes(oParam.foto, out v_ruta);
                        oParam.ruta = v_ruta;
                        eEntidad.Fotos.Add(new fotoEmbarqueVBS
                        {
                            idMovimiento = 0,
                            ruta = oParam.ruta,
                            estado = oParam.estado,
                            usuarioCrea = oParam.Create_user
                        });
                    }

                    BAN_Embarque_MovimientoDA oRecepcion = new BAN_Embarque_MovimientoDA();
                    var result = oRecepcion.Save_Update(eEntidad, out OnError);

                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se registro con éxito el Movimiento de embarque: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eEntidad.idEmbarqueMovimiento = Id;
                        ObjTarea = eEntidad;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Movimiento de embarque no fue procesado, verifique el siguiente mensaje: {0} ", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registro de Movimiento de embarque";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Embarque_Movimiento();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraDespacho), "api/VBS_embarque_movimiento_save", false, null, null, ex.StackTrace, ex);

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

        //graba imagenes en el servidor
        private bool Carga_Imagenes(byte[] Foto, out string Nombre)
        {
            try
            {
                //SACO EL NOMBRE DE LA RUTA A GUARDAR
                string RUTA = recepcionDA.GetConfiguracion("APP", "RUTA_IMG_VBS");//dbContext.ConfigurationSet.Where(x => x.Name.Trim().Equals("RUTA_IMG")).Select(kvp => kvp.Value).FirstOrDefault();

                //SACO LA EXTESNION DE LA IMAGEN
                string EXTENSION = recepcionDA.GetConfiguracion("APP", "TIPO_IMG");//dbContext.ConfigurationSet.Where(x => x.Name.Trim().Equals("TIPO_IMG")).Select(kvp => kvp.Value).FirstOrDefault();

                string NOMBRE_IMAGEN = Guid.NewGuid().ToString().Replace("-", "");
                string CARPETA = System.DateTime.Now.Year.ToString().Trim();
                string SUBCARPETA = System.DateTime.Now.Month.ToString().Trim();
                string DIR_CARPETA = string.Format("{0}{1}", RUTA, CARPETA);
                string DIR_SUBCARPETA = string.Format("{0}{1}\\{2}", RUTA, CARPETA, SUBCARPETA);

                //CREO DIRECTORIOS EN CASO DE NO EXISTIR
                if (!(Directory.Exists(DIR_CARPETA)))
                {
                    Directory.CreateDirectory(DIR_CARPETA);
                }
                if (!(Directory.Exists(DIR_SUBCARPETA)))
                {
                    Directory.CreateDirectory(DIR_SUBCARPETA);
                }

                string RUTA_FINAL = string.Format("{0}{1}\\{2}\\{3}", RUTA.Trim(), CARPETA, SUBCARPETA, string.Format("{0}{1}", NOMBRE_IMAGEN, EXTENSION.Trim()));

                MemoryStream ms = new MemoryStream(Foto, 0, Foto.Length);
                ms.Write(Foto, 0, Foto.Length);
                Image returnImage = Image.FromStream(ms);

                System.Drawing.Imaging.ImageFormat format;
                switch (EXTENSION)
                {
                    case ".PNG":
                        format = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                    case ".GIF":
                        format = System.Drawing.Imaging.ImageFormat.Gif;
                        break;
                    default:
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                        break;
                }


                returnImage.Save(RUTA_FINAL, format);
                ms.Dispose();

                Nombre = string.Format("{0}/{1}/{2}", CARPETA, SUBCARPETA, string.Format("{0}{1}", NOMBRE_IMAGEN, EXTENSION.Trim()));

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Carga_Imagenes), "api/workinprogressregisters/Carga_Imagenes", false, null, null, ex.StackTrace, ex);
                Nombre = (string.Format("EXCEPCION NO CONTROLADA # {0}: {1}", lm, ex.Message.ToString()));
                return false;
            }

            return true;
        }
    }
}