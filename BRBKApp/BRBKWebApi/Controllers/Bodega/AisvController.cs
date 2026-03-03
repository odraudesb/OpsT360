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

namespace MiWebApi.Controllers.Bodega
{
    public class AisvController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpPost]
        [Route("api/VBS_lista_AISV")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Stowage_Plan_Aisv>> Lista_AISV([FromBody] ParametrosStowagePlanAisv.ParametrosConsultaListaStowagePlanAisv pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Stowage_Plan_Aisv> query = new List<BAN_Stowage_Plan_Aisv>();
            RespuestaViewModel<List<BAN_Stowage_Plan_Aisv>> respuesta = new RespuestaViewModel<List<BAN_Stowage_Plan_Aisv>>();
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
                    query = BRBKWebApiData.BAN_Stowage_Plan_AisvDA.ConsultarLista(pObj.estado, pObj.aisv, pObj.idStowageDet, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe información de los(el) AISV con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add(string.Format("No existe información de los(el) AISV  con los criterios ingresados."));
                            Valido = false;
                        }
                        else
                        {
                            try
                            {
                                var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                                var oBodega = BAN_Catalogo_BodegaDA.ConsultarLista(out oError);
                                var oBloque = BAN_Catalogo_BloqueDA.ConsultarLista(null, out oError);
                                var oCargo = BAN_Catalogo_CargoDA.ConsultarListaCargos(out oError);
                                var oConsignatario = BAN_Catalogo_ConsignatarioDA.ConsultarListaConsignatarios("CGSA", out oError);
                                var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                                var oHold = BAN_Catalogo_HoldDA.ConsultarListaHold(out oError);
                                var oMarca = BAN_Catalogo_MarcaDA.ConsultarListaMarca("CGSA", out oError);

                                foreach (var item in query)
                                {
                                    item.oEstados = oEstado.Where(p => p.id == item.estado).FirstOrDefault();
                                    item.oListaStowage_Movimiento = BAN_Stowage_MovimientoDA.ConsultarLista(item.idStowageAisv, out oError);
                                    var oDetalle = BAN_Stowage_Plan_DetDA.GetEntidad(item.idStowageDet);
                                    item.oStowage_Plan_Det = oDetalle;
                                    if (!(item.oStowage_Plan_Det is null))
                                    {
                                        item.oStowage_Plan_Det.oBodega = oBodega.Where(p => p.id == item.oStowage_Plan_Det.idBodega).FirstOrDefault();
                                        item.oStowage_Plan_Det.oBloque = oBloque.Where(p => p.id == item.oStowage_Plan_Det.idBloque).FirstOrDefault();
                                        item.oStowage_Plan_Det.oCargo = oCargo.Where(p => p.id == item.oStowage_Plan_Det.idCargo).FirstOrDefault();
                                        item.oStowage_Plan_Det.oConsignatario = oConsignatario.Where(p => p.id == item.oStowage_Plan_Det.idConsignatario).FirstOrDefault();
                                        item.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == item.oStowage_Plan_Det.idExportador).FirstOrDefault();
                                        item.oStowage_Plan_Det.oHold = oHold.Where(p => p.id == item.oStowage_Plan_Det.idBloque).FirstOrDefault();
                                        item.oStowage_Plan_Det.oMarca = oMarca.Where(p => p.id == item.oStowage_Plan_Det.idMarca).FirstOrDefault();
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de AISV por criterios";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Stowage_Plan_Aisv>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_AISV), "api/lista_AISV", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_getStowagePlanAisvPorId")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Plan_Aisv> GetEntidad([FromBody] ParametrosStowagePlanAisv.ParametrosGetStowagPlanAisv pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            BAN_Stowage_Plan_Aisv oStowagePlanAisv = new BAN_Stowage_Plan_Aisv();
            RespuestaViewModel<BAN_Stowage_Plan_Aisv> respuesta = new RespuestaViewModel<BAN_Stowage_Plan_Aisv>();
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
                    var Entity = BAN_Stowage_Plan_AisvDA.GetEntidad(long.Parse(pObj.id.ToString()));
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el Id de Detalle:{0}", pObj.id));
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.idStowageAisv;

                        try
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            var oBodega = BAN_Catalogo_BodegaDA.ConsultarLista(out oError);
                            var oBloque = BAN_Catalogo_BloqueDA.ConsultarLista(null, out oError);
                            var oDetalle = BAN_Stowage_Plan_DetDA.GetEntidad(Entity.idStowageDet);
                            var oCargo = BAN_Catalogo_CargoDA.ConsultarListaCargos(out oError);
                            var oConsignatario = BAN_Catalogo_ConsignatarioDA.ConsultarListaConsignatarios("CGSA", out oError);
                            var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                            var oHold = BAN_Catalogo_HoldDA.ConsultarListaHold(out oError);
                            var oMarca = BAN_Catalogo_MarcaDA.ConsultarListaMarca("CGSA", out oError);

                            Entity.oEstados = oEstado.Where(p => p.id == Entity.estado).FirstOrDefault();
                            Entity.oListaStowage_Movimiento = BAN_Stowage_MovimientoDA.ConsultarLista(Entity.idStowageAisv, out oError);
                            Entity.oStowage_Plan_Det = oDetalle;
                            if (!(Entity.oStowage_Plan_Det is null))
                            {
                                Entity.oStowage_Plan_Det.oBodega = oBodega.Where(p => p.id == Entity.oStowage_Plan_Det.idBodega).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oBloque = oBloque.Where(p => p.id == Entity.oStowage_Plan_Det.idBloque).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oCargo = oCargo.Where(p => p.id == Entity.oStowage_Plan_Det.idCargo).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oConsignatario = oConsignatario.Where(p => p.id == Entity.oStowage_Plan_Det.idConsignatario).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == Entity.oStowage_Plan_Det.idExportador).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oHold = oHold.Where(p => p.id == Entity.oStowage_Plan_Det.idBloque).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oMarca = oMarca.Where(p => p.id == Entity.oStowage_Plan_Det.idMarca).FirstOrDefault();
                            }
                        }
                        catch { }

                        oStowagePlanAisv = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = oStowagePlanAisv ?? new BAN_Stowage_Plan_Aisv();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetEntidad), "api/getStowagePlanAisvPorId", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_getStowage_Plan_AisvXBooking")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Plan_Aisv> GetEntidadXBooking([FromBody] ParametrosStowagePlanAisv.ParametrosGetStowagPlanAisvXBooking pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            BAN_Stowage_Plan_Aisv oStowagePlanAisv = new BAN_Stowage_Plan_Aisv();
            RespuestaViewModel<BAN_Stowage_Plan_Aisv> respuesta = new RespuestaViewModel<BAN_Stowage_Plan_Aisv>();
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
                    var Entity = BAN_Stowage_Plan_AisvDA.GetEntidad(pObj.booking);
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el booking de Detalle:{0}", pObj.booking));
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.idStowageAisv;

                        try
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            var oBodega = BAN_Catalogo_BodegaDA.ConsultarLista(out oError);
                            var oBloque = BAN_Catalogo_BloqueDA.ConsultarLista(null, out oError);
                            var oDetalle = BAN_Stowage_Plan_DetDA.GetEntidad(Entity.idStowageDet);
                            var oCargo = BAN_Catalogo_CargoDA.ConsultarListaCargos(out oError);
                            var oConsignatario = BAN_Catalogo_ConsignatarioDA.ConsultarListaConsignatarios("CGSA", out oError);
                            var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                            var oHold = BAN_Catalogo_HoldDA.ConsultarListaHold(out oError);
                            var oMarca = BAN_Catalogo_MarcaDA.ConsultarListaMarca("CGSA", out oError);

                            Entity.oEstados = oEstado.Where(p => p.id == Entity.estado).FirstOrDefault();
                            Entity.oListaStowage_Movimiento = BAN_Stowage_MovimientoDA.ConsultarLista(Entity.idStowageAisv, out oError);
                            Entity.oStowage_Plan_Det = oDetalle;
                            if (!(Entity.oStowage_Plan_Det is null))
                            {
                                Entity.oStowage_Plan_Det.oBodega = oBodega.Where(p => p.id == Entity.oStowage_Plan_Det.idBodega).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oBloque = oBloque.Where(p => p.id == Entity.oStowage_Plan_Det.idBloque).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oCargo = oCargo.Where(p => p.id == Entity.oStowage_Plan_Det.idCargo).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oConsignatario = oConsignatario.Where(p => p.id == Entity.oStowage_Plan_Det.idConsignatario).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == Entity.oStowage_Plan_Det.idExportador).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oHold = oHold.Where(p => p.id == Entity.oStowage_Plan_Det.idBloque).FirstOrDefault();
                                Entity.oStowage_Plan_Det.oMarca = oMarca.Where(p => p.id == Entity.oStowage_Plan_Det.idMarca).FirstOrDefault();
                            }
                        }
                        catch { }

                        oStowagePlanAisv = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = oStowagePlanAisv ?? new BAN_Stowage_Plan_Aisv();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetEntidad), "api/VBS_getStowage_Plan_AisvXBooking", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_registra_aisv_externo")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Plan_Det> RegistraDespacho([FromBody] ParametrosStowagePlanAisv.ParametrosRegistrarAisvExterno pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<BAN_Stowage_Plan_Det> respuesta = new RespuestaViewModel<BAN_Stowage_Plan_Det>();
            BAN_Stowage_Plan_Det ObjTarea = new BAN_Stowage_Plan_Det();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    //BUSCA CAB
                    var oCab = BAN_Stowage_Plan_CabDA.GetEntidad(pTarea.idNave);
                    if (oCab == null)
                    {
                        Mensaje.Add(string.Format("Configuración de Stowage plan por Nave no se encontró, verifique por favor el siguiente ID StowageCab: {0} ", pTarea.idNave));
                        Valido = false;
                    }

                    var oAisv = BAN_AISV_GeneradosDA.ConsultarListadoAISV(pTarea.aisv, out OnError).FirstOrDefault();

                    if ((OnError != string.Empty) || oAisv is null)
                    {
                        Mensaje.Add(string.Format("Solicitud no fue procesado, AISV no encontrado, verifique el estado del AISV: {0} ", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (oAisv != null)
                        {
                            var oExportador = BAN_Catalogo_ExportadorDA.GetExportaadorPorRucLinea(oCab.idLinea, oAisv.aisv_codig_clte);
                            if (oExportador == null)
                            {
                                Mensaje.Add(string.Format("Solicitud no fue procesado, El exportador del AISV selecconado no existe, favor verificar: {0} ", OnError));
                                Valido = false;
                            }
                            else
                            {
                                //valida que aisv no se repita 
                                var oAisvExiste = BAN_Stowage_Plan_AisvDA.ConsultarListaXAISV(pTarea.aisv, out OnError);
                                if (oAisvExiste?.Count > 0)
                                {
                                    Mensaje.Add(string.Format("El AISV seleccionado ya ha sido registrado, favor verificar: {0} ", OnError));
                                    Valido = false;
                                }
                                else
                                {
                                    if (oAisv.idNave == pTarea.idNave)
                                    {
                                        BAN_Stowage_Plan_Det eDet = new BAN_Stowage_Plan_Det();
                                        eDet.idStowageCab = oCab.idStowageCab;
                                        eDet.idHold = pTarea.idHold;
                                        eDet.piso = "N/N";
                                        eDet.boxSolicitado = oAisv.vbs_box;
                                        eDet.boxAutorizado = oAisv.vbs_box;
                                        eDet.idCargo = -1;
                                        eDet.idExportador = oExportador.id;
                                        eDet.idMarca = -1;
                                        eDet.idConsignatario = -1;
                                        eDet.idBodega = pTarea.idBodega;
                                        eDet.idBloque = pTarea.idBloque;
                                        eDet.reservado = 0;
                                        eDet.disponible = oAisv.vbs_box;
                                        eDet.comentario = "INGRESO AUTOMÁTICO POR APPS DE AISV EXTERNO";
                                        eDet.fechaDocumento = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                                        eDet.estado = "PBL";
                                        eDet.usuarioCrea = pTarea.Create_user;

                                        BAN_Stowage_Plan_DetDA oTransaccion = new BAN_Stowage_Plan_DetDA();
                                        BAN_Stowage_Plan_Aisv eAisv = new BAN_Stowage_Plan_Aisv();
                                        eAisv.fecha = DateTime.Now;
                                        eAisv.idHoraInicio = 1;
                                        eAisv.horaInicio = "00:00";
                                        eAisv.idHoraFin = 1;
                                        eAisv.horaFin = "01:00";
                                        eAisv.box = oAisv.vbs_box;
                                        eAisv.comentario = "INGRESO AUTOMÁTICO DE AISV (APP) DE OTRA PROCEDENCIA";
                                        eAisv.aisv = oAisv.aisv_codigo.Trim();
                                        eAisv.dae = oAisv.aisv_dae?.Trim();
                                        eAisv.booking = oAisv.aisv_numero_booking?.Trim();
                                        eAisv.IIEAutorizada = true;
                                        eAisv.daeAutorizada = true;
                                        eAisv.placa = oAisv.aisv_placa_vehi;
                                        eAisv.idChofer = oAisv.aisv_cedul_chof;
                                        eAisv.chofer = oAisv.aisv_nombr_chof;
                                        eAisv.estado = "ING";
                                        eAisv.usuarioCrea = pTarea.Create_user;
                                        eDet.ListaAISV = new List<BAN_Stowage_Plan_Aisv>();
                                        eDet.ListaAISV.Add(eAisv);
                                        long? idAisv = null;
                                        var result = oTransaccion.SaveAisvExterno(eDet, out OnError, out idAisv);
                                        IdGenerado = result;

                                        if (OnError != string.Empty)
                                        {
                                            throw new Exception(OnError);
                                        }

                                        if (IdGenerado.HasValue)
                                        {
                                            Mensaje.Add(string.Format("Se registro con éxito el aisv externo: [{0}] ", IdGenerado.Value));
                                            Valido = true;

                                            Int64 Id = IdGenerado.Value;

                                            eDet.idStowageDet = Id;
                                            eDet.ListaAISV.FirstOrDefault().idStowageAisv = long.Parse(idAisv.ToString());
                                            ObjTarea = eDet;
                                        }
                                        else
                                        {
                                            Mensaje.Add(string.Format("aisv externo no fue procesado, verifique el siguiente mensaje: {0} ", OnError));
                                            Valido = false;
                                        }
                                    }
                                    else
                                    {
                                        Mensaje.Add(string.Format("La referencia de la nave debe ser igual al seleccionado en la lista, favor verificar: Referencia AISV {0} ", oAisv.idNave));
                                        Valido = false;
                                    }
                                }
                            }
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registro de aisv externo";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Stowage_Plan_Det();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraDespacho), "api/VBS_registra_aisv_externo", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_bodegasVBS")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_BodegasVBS()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaBodegasVBS(out OnError);

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
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_BodegasVBS), "api/VBS_lista_bodegasVBS", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_bloquesVBS")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_BloquesVBS([FromBody] ParametrosDespachoVBS.ParametrosConsultaListaOrdenesDespachoPorBodega pObj)
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
                    query = BAN_Consulta_ComboDA.ConsultarListaBloquesVBS(pObj.idBodega, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe bloque con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe bloque con los criterios ingresados");
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Bloques por Nave";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Consulta_Combo>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_BloquesVBS), "api/VBS_lista_bloquesVBS", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_navesST")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_NavesST()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaNaveST(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de Naves");
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
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_NavesST), "api/VBS_lista_navesST", false, null, null, ex.StackTrace, ex);

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
