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

namespace MiWebApi.Controllers.Bodega
{
    public class AisvPreDespachoController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpGet]
        [Route("api/VBS_lista_bodegasPorOrdenesDespacho")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_BodegasPreDespacho()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaBodegaXDespacho(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de bodegas");
                    Valido = false;
                }
                else
                {
                    if (ObjTarea.Count == 0)
                    {
                        Mensaje.Add("No existen registros de bodegas que mostrar");
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
                respuestaVie.Titulo = "Listado de Bodegas";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<BAN_Consulta_Combo>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_BodegasPreDespacho), "api/VBS_lista_bodegasPorOrdenesDespacho", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_ordenesDespachoPorBodega")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> Lista_OrdenesDespacho([FromBody] ParametrosDespachoVBS.ParametrosConsultaListaOrdenesDespachoPorBodega pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Stowage_OrdenDespacho> query = new List<BAN_Stowage_OrdenDespacho>();
            RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> respuesta = new RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>();
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
                    query = BAN_Stowage_OrdenDespachoDA.ConsultarListaPorBodega(pObj.idBodega, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Ordenes de Despacho con el criterio ingresado, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe Ordenes de Despacho con los criterios ingresados");
                            Valido = false;
                        }
                        else
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                            var oModalidad = BAN_Catalogo_ModalidadDA.ConsultarLista(out oError);
                            var oBodega = BAN_Catalogo_BodegaDA.ConsultarLista(out oError);
                            var oBloque = BAN_Catalogo_BloqueDA.ConsultarLista(null, out oError);
                            //var oFila = BAN_Catalogo_FilaDA.ConsultarLista(out oError);
                            //var oAltura = BAN_Catalogo_AlturaDA.ConsultarLista(out oError);
                            //var oProfundidad = BAN_Catalogo_ProfundidadDA.ConsultarLista(out oError);
                            foreach (var item in query)
                            {
                                item.oEstado = oEstado.Where(p => p.id == item.estado).FirstOrDefault();
                                item.oExportador = oExportador.Where(p => p.id == item.idExportador).FirstOrDefault();
                                item.oBloque = oBloque.Where(p => p.id == item.idBloque).FirstOrDefault(); //BAN_Catalogo_BloqueDA.GetEntidad(item.idBloque);//en esta consulta, en el campo idUbicacion esta el codigo del bloque (ojo)
                                item.oBloque.oBodega = oBodega.Where(p => p.id == item.oBloque.idBodega).FirstOrDefault(); //BAN_Catalogo_BodegaDA.GetEntidad(item.oBloque.idBodega);
                            }
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de ordenes de despacho";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Stowage_OrdenDespacho>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_OrdenesDespacho), "api/VBS_lista_ordenesDespachoPorBodega", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_ordenesDespachoGeneral")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> Lista_OrdenesDespachoGeneral([FromBody] ParametrosDespachoVBS.ParametrosConsultaListaOrdenesDespachoPorBodega pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Stowage_OrdenDespacho> query = new List<BAN_Stowage_OrdenDespacho>();
            RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> respuesta = new RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>();
            string oError = string.Empty;

            try
            {
               
                query = BAN_Stowage_OrdenDespachoDA.ConsultarListaPorBodega(pObj.idBodega, out OnError);

                if (query == null)
                {
                    Mensaje.Add(string.Format("No existe Ordenes de Despacho con el criterio ingresado, Error: {0}", OnError));
                    Valido = false;
                }
                else
                {
                    if (query?.Count <= 0 && Valido)
                    {
                        Mensaje.Add("No existe Ordenes de Despacho con los criterios ingresados");
                        Valido = false;
                    }
                    else
                    {
                        var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                        var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                        var oModalidad = BAN_Catalogo_ModalidadDA.ConsultarLista(out oError);
                        var oBodega = BAN_Catalogo_BodegaDA.ConsultarLista(out oError);
                        var oBloque = BAN_Catalogo_BloqueDA.ConsultarLista(null, out oError);
                        //var oFila = BAN_Catalogo_FilaDA.ConsultarLista(out oError);
                        //var oAltura = BAN_Catalogo_AlturaDA.ConsultarLista(out oError);
                        //var oProfundidad = BAN_Catalogo_ProfundidadDA.ConsultarLista(out oError);
                        foreach (var item in query)
                        {
                            item.oEstado = oEstado.Where(p => p.id == item.estado).FirstOrDefault();
                            item.oExportador = oExportador.Where(p => p.id == item.idExportador).FirstOrDefault();
                            item.oBloque = oBloque.Where(p => p.id == item.idBloque).FirstOrDefault(); // BAN_Catalogo_BloqueDA.GetEntidad(item.idBloque);//en esta consulta, en el campo idUbicacion esta el codigo del bloque (ojo)
                            item.oBloque.oBodega = oBodega.Where(p=> p.id == item.oBloque.idBodega).FirstOrDefault();
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de ordenes de despacho";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Stowage_OrdenDespacho>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_OrdenesDespacho), "api/VBS_lista_ordenesDespachoPorBodega", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_ordenesDetAgrupada")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_FilasPorOrden>> Lista_MovimientosPorOrdenDespacho([FromBody] ParametrosDespachoVBS.ParametrosConsultaListaOrdenesDespachoAgrupadas pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_FilasPorOrden> query = new List<BAN_Consulta_FilasPorOrden>();
            RespuestaViewModel<List<BAN_Consulta_FilasPorOrden>> respuesta = new RespuestaViewModel<List<BAN_Consulta_FilasPorOrden>>();
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
                    query = BAN_Consulta_FilasPorOrdenDA.ConsultarLista(pObj.idOrdenDespacho, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Ordenes de Despacho con el criterio ingresado, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe Ordenes de Despacho con los criterios ingresados");
                            Valido = false;
                        }
                        else
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                            var oModalidad = BAN_Catalogo_ModalidadDA.ConsultarLista(out oError);
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
                respuestaVie.Titulo = "Listado de ordenes de despacho";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Consulta_FilasPorOrden>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_MovimientosPorOrdenDespacho), "api/VBS_lista_ordenesDetAgrupada", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_movimientoPorFilaYOrden")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Stowage_Movimiento>> Lista_MovimientosPorOrdenDespachoFila([FromBody] ParametrosDespachoVBS.ParametrosConsultaListaOrdenesDespachoFila pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Stowage_Movimiento> query = new List<BAN_Stowage_Movimiento>();
            RespuestaViewModel<List<BAN_Stowage_Movimiento>> respuesta = new RespuestaViewModel<List<BAN_Stowage_Movimiento>>();
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
                    query = BAN_Stowage_MovimientoDA.ConsultarListaPorNoOrdenYFila(pObj.idOrdenDespacho, pObj.idFila,  out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe movimientos con el criterio ingresado, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe movimientos con los criterios ingresados");
                            Valido = false;
                        }
                        else
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                            var oModalidad = BAN_Catalogo_ModalidadDA.ConsultarLista(out oError);
                            var oUbicaciom = BAN_Catalogo_UbicacionDA.ConsultarLista(out oError);

                            var oCargo = BAN_Catalogo_CargoDA.ConsultarListaCargos(out oError);
                            var oMarca = BAN_Catalogo_MarcaDA.ConsultarListaMarca("CGSA", out oError);
                            var oStowageCab = BAN_Stowage_Plan_CabDA.ConsultarLista(out oError);
                            var oBodega = BAN_Catalogo_BodegaDA.ConsultarLista(out oError);
                            var oBloque = BAN_Catalogo_BloqueDA.ConsultarLista(null, out oError);
                            var oFila = BAN_Catalogo_FilaDA.ConsultarLista(out oError);
                            var oAltura = BAN_Catalogo_AlturaDA.ConsultarLista(out oError);
                            var oProfundidad = BAN_Catalogo_ProfundidadDA.ConsultarLista(out oError);

                            foreach (var item in query)
                            {
                                item.oUbicacion = oUbicaciom.Where(p => p.id == item.idUbicacion).FirstOrDefault();
                                item.oModalidad = oModalidad.Where(p => p.id == item.idModalidad).FirstOrDefault();
                                item.oEstado = oEstado.Where(p => p.id == item.estado).FirstOrDefault();
                                try { item.oStowage_Plan_Aisv = BAN_Stowage_Plan_AisvDA.GetEntidad(item.idStowageAisv); } catch { }
                                if (item.oStowage_Plan_Aisv != null)
                                {
                                    try { item.oStowage_Plan_Aisv.oStowage_Plan_Det = BAN_Stowage_Plan_DetDA.GetEntidad(item.oStowage_Plan_Aisv?.idStowageDet); } catch { }
                                    if (item.oStowage_Plan_Aisv.oStowage_Plan_Det != null)
                                    {
                                        item.oStowage_Plan_Aisv.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == item.oStowage_Plan_Aisv.oStowage_Plan_Det.idExportador).FirstOrDefault();
                                        item.oExportador = item.oStowage_Plan_Aisv.oStowage_Plan_Det.oExportador;
                                        item.oStowage_Plan_Aisv.oStowage_Plan_Det.oCargo = oCargo.Where(p=> p.id == item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idCargo).FirstOrDefault();
                                        item.oStowage_Plan_Aisv.oStowage_Plan_Det.oMarca = oMarca.Where(p=> p.id == item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idMarca).FirstOrDefault();
                                        item.oStowage_Plan_Aisv.oStowage_Plan_Det.oStowage_Plan_Cab = oStowageCab.Where(p=> p.idStowageCab == item.oStowage_Plan_Aisv.oStowage_Plan_Det.idStowageCab).FirstOrDefault();
                                    }
                                }

                                if (item.oUbicacion != null)
                                {
                                    item.oUbicacion.oBodega = oBodega.Where(p=> p.id == item.oUbicacion?.idBodega).FirstOrDefault();
                                    item.oUbicacion.oBloque = oBloque.Where(p=> p.id == item.oUbicacion?.idBloque).FirstOrDefault();
                                    item.oBloque = item.oUbicacion.oBloque;
                                    item.oBloque.oBodega = item.oUbicacion.oBodega;
                                    item.oUbicacion.oFila = oFila.Where(p=> p.id == item.oUbicacion?.idFila).FirstOrDefault();
                                    item.oUbicacion.oAltura = oAltura.Where(P=> P.id == item.oUbicacion?.idAltura).FirstOrDefault();
                                    item.oUbicacion.oProfundidad = oProfundidad.Where(p=> p.id == item.oUbicacion?.idProfundidad).FirstOrDefault();
                                }
                            }
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de movimiento en ordenes de despacho";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Stowage_Movimiento>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_MovimientosPorOrdenDespachoFila), "api/VBS_lista_movimientoPorFilaYOrden", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_save_preDespacho")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Movimiento> RegistraPreDespacho([FromBody] ParametrosDespachoVBS.ParametrosRegistrarPreDespacho pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            BAN_Stowage_Movimiento ObjTarea = new BAN_Stowage_Movimiento();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    BAN_Stowage_Movimiento eEntidad = new BAN_Stowage_Movimiento();
                    eEntidad.idMovimiento = pTarea.idMovimiento;
                    eEntidad.usuarioCrea = pTarea.Create_user;

                    BAN_Stowage_MovimientoDA oPreDespacho = new BAN_Stowage_MovimientoDA();
                    var result = oPreDespacho.Save_PreDespacho(eEntidad, out OnError);

                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se registro con éxito el pre despacho la transacción No: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eEntidad.idMovimiento = Id;
                        ObjTarea = eEntidad;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se pudo grabar el pre despacho, verifique el siguiente mensaje; {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar Orden de Despacho";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Stowage_Movimiento();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraPreDespacho), "api/VBS_save_preDespacho", false, null, null, ex.StackTrace, ex);

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