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
    public class AisvOrdenDespachoController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpGet]
        [Route("api/VBS_lista_naves")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_Naves()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Consulta_Combo> ObjTarea = new List<BAN_Consulta_Combo>();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Consulta_ComboDA.ConsultarListaNave(out OnError);

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
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Naves), "api/VBS_lista_naves", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_bodega")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_Bodega([FromBody] ParametrosDespachoVBS.ParametrosConsultaCargaEnBodegaAisv pObj)
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
                    query = BAN_Consulta_ComboDA.ConsultarListaBodega(pObj.idNave, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe bodegas con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe bodegas con los criterios ingresados");
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Bodegas por Nave";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Consulta_Combo>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Bodega), "api/VBS_lista_bodega", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_bloque")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_Bloque([FromBody] ParametrosDespachoVBS.ParametrosConsultaCargaEnBodegaAisv pObj)
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
                    query = BAN_Consulta_ComboDA.ConsultarListaBloques(pObj.idNave, out OnError);

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
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Bodega), "api/VBS_lista_bloque", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_exportadorPorNaveBodega")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_ExportadoresPorNaveBodega([FromBody] ParametrosDespachoVBS.ParametrosConsultaCargaEnBodegaAisv pObj)
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
                    query = BAN_Consulta_ComboDA.ConsultarListaExportadoresPorNaveBodega(pObj.idNave, pObj.idBodega, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe exportadores con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe exportadores con los criterios ingresados");
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Exportadores por Nave y bodega";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Consulta_Combo>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_ExportadoresPorNaveBodega), "api/VBS_lista_exportadorPorNaveBodega", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_exportadorPorNave")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Consulta_Combo>> Lista_ExportadoresPorNave([FromBody] ParametrosDespachoVBS.ParametrosConsultaCargaEnBodegaAisv pObj)
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
                    query = BAN_Consulta_ComboDA.ConsultarListaExportadoresPorNave(pObj.idNave, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe exportadores con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe exportadores con los criterios ingresados");
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Exportadores por Nave y bodega";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Consulta_Combo>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_ExportadoresPorNave), "api/VBS_lista_exportadorPorNave", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_carga_enBodega")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Stowage_Movimiento>> Lista_Carga_EnBodega([FromBody] ParametrosDespachoVBS.ParametrosConsultaCargaEnBodegaAisv pObj)
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
                    query = BAN_Stowage_MovimientoDA.ConsultarListaCargaBodega(pObj.idNave, pObj.idBodega, pObj.idExportador, pObj.booking, pObj.barcode, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Recepciones con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe Recepciones con los criterios ingresados");
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
                            var oBodega = BAN_Catalogo_BodegaDA.ConsultarLista(out oError);
                            var oBloque = BAN_Catalogo_BloqueDA.ConsultarLista(null, out oError);
                            var oStowageCab = BAN_Stowage_Plan_CabDA.ConsultarLista(out oError);
                            var oFila = BAN_Catalogo_FilaDA.ConsultarLista(out oError);
                            var oAltura = BAN_Catalogo_AlturaDA.ConsultarLista(out oError);
                            var oProfundidad = BAN_Catalogo_ProfundidadDA.ConsultarLista(out oError);
                            foreach (var item in query)
                            {
                                item.oUbicacion = oUbicaciom.Where(p => p.id == item.idUbicacion).FirstOrDefault();
                                item.oModalidad = oModalidad.Where(p => p.id == item.idModalidad).FirstOrDefault();
                                item.oEstado = oEstado.Where(p => p.id == item.estado).FirstOrDefault();
                                item.oStowage_Plan_Aisv = BAN_Stowage_Plan_AisvDA.GetEntidad(item.idStowageAisv);
                                item.oStowage_Plan_Aisv.oStowage_Plan_Det = BAN_Stowage_Plan_DetDA.GetEntidad(item.oStowage_Plan_Aisv.idStowageDet);
                                item.oStowage_Plan_Aisv.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == item.oStowage_Plan_Aisv.oStowage_Plan_Det.idExportador).FirstOrDefault();

                                item.oExportador = item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.oExportador;
                                item.oStowage_Plan_Aisv.oStowage_Plan_Det.oCargo = oCargo.Where(p=> p.id == item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idCargo).FirstOrDefault();
                                item.oStowage_Plan_Aisv.oStowage_Plan_Det.oMarca = oMarca.Where(p=> p.id == item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idMarca).FirstOrDefault();
                                item.oStowage_Plan_Aisv.oStowage_Plan_Det.oStowage_Plan_Cab = oStowageCab.Where(p=> p.idStowageCab == item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idStowageCab).FirstOrDefault();

                                if (item.oUbicacion != null)
                                {
                                    item.oUbicacion.oBodega = oBodega.Where(p => p.id == item.oUbicacion?.idBodega).FirstOrDefault();
                                    item.oUbicacion.oBloque = oBloque.Where(p => p.id == item.oUbicacion?.idBloque).FirstOrDefault();
                                    item.oBloque = item.oUbicacion.oBloque;
                                    item.oBloque.oBodega = item.oUbicacion.oBodega;
                                    item.oUbicacion.oFila = oFila.Where(p => p.id == item.oUbicacion?.idFila).FirstOrDefault();
                                    item.oUbicacion.oAltura = oAltura.Where(P => P.id == item.oUbicacion?.idAltura).FirstOrDefault();
                                    item.oUbicacion.oProfundidad = oProfundidad.Where(p => p.id == item.oUbicacion?.idProfundidad).FirstOrDefault();
                                }
                            }
                        }
                    }

                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Recepciones por AISV";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Stowage_Movimiento>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Carga_EnBodega), "api/VBS_carga_enBodega", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_carga_enBodegaParaDespacho")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Stowage_Movimiento>> Lista_Carga_paraDespacho([FromBody] ParametrosDespachoVBS.ParametrosConsultaCargaEnBodegaAisv pObj)
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
                    query = BAN_Stowage_MovimientoDA.ConsultarListaCargaParaDespacho(pObj.idNave, pObj.idExportador, pObj.idBloque, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Recepciones con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add("No existe Recepciones con los criterios ingresados");
                            Valido = false;
                        }
                        else
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                            var oModalidad = BAN_Catalogo_ModalidadDA.ConsultarLista(out oError);
                            foreach (var item in query)
                            {
                                item.oModalidad = oModalidad.Where(p => p.id == item.idModalidad).FirstOrDefault();
                                item.oEstado = oEstado.Where(p => p.id == item.estado).FirstOrDefault();
                                //item.oStowage_Plan_Aisv = BAN_Stowage_Plan_AisvDA.GetEntidad(item.idStowageAisv);
                                //item.oStowage_Plan_Aisv.oStowage_Plan_Det = BAN_Stowage_Plan_DetDA.GetEntidad(item.oStowage_Plan_Aisv.idStowageDet);
                                //item.oStowage_Plan_Aisv.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == item.oStowage_Plan_Aisv.oStowage_Plan_Det.idExportador).FirstOrDefault();
                                item.oExportador = oExportador.Where(p => p.id == item.idExportador).FirstOrDefault();
                                //item.oStowage_Plan_Aisv.oStowage_Plan_Det.oBodega = BAN_Catalogo_BodegaDA.GetEntidad(item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idBodega);
                                //item.oStowage_Plan_Aisv.oStowage_Plan_Det.oBloque = BAN_Catalogo_BloqueDA.GetEntidad(item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idBloque);
                                //item.oStowage_Plan_Aisv.oStowage_Plan_Det.oStowage_Plan_Cab = BAN_Stowage_Plan_CabDA.GetEntidad(item.oStowage_Plan_Aisv.oStowage_Plan_Det.idStowageCab);
                                item.oBloque = BAN_Catalogo_BloqueDA.GetEntidad(item.idUbicacion);//en esta consulta, en el campo idUbicacion esta el codigo del bloque (ojo)
                                item.oBloque.oBodega = BAN_Catalogo_BodegaDA.GetEntidad(item.oBloque.idBodega);
                                //item.oStowage_Plan_Aisv.oStowage_Plan_Det.oCargo = BAN_Catalogo_CargoDA.GetEntidad(item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idCargo);
                                //item.oStowage_Plan_Aisv.oStowage_Plan_Det.oMarca = BAN_Catalogo_MarcaDA.GetEntidad(item.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idMarca);
                            }
                        }
                    }

                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Recepciones para despacho";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<BAN_Stowage_Movimiento>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Carga_EnBodega), "api/VBS_carga_enBodegaParaDespacho", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_carga_enBodegaParaDespachoEsp")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Movimiento> Carga_paraDespacho([FromBody] ParametrosDespachoVBS.ParametrosConsultaCargaEnBodegaAisv pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            BAN_Stowage_Movimiento query = new BAN_Stowage_Movimiento();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
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
                    query = BAN_Stowage_MovimientoDA.ConsultarListaCargaParaDespachoEsp(pObj.idNave, pObj.idExportador, pObj.idBloque, pObj.booking);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Recepcion con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query == null && Valido)
                        {
                            Mensaje.Add("No existe Recepcion con los criterios ingresados");
                            Valido = false;
                        }
                        else
                        {
                            var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                            var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                            var oModalidad = BAN_Catalogo_ModalidadDA.ConsultarLista(out oError);

                            query.oModalidad = oModalidad.Where(p => p.id == query.idModalidad).FirstOrDefault();
                            query.oEstado = oEstado.Where(p => p.id == query.estado).FirstOrDefault();
                            query.oExportador = oExportador.Where(p => p.id == query.idExportador).FirstOrDefault();
                            query.oBloque = BAN_Catalogo_BloqueDA.GetEntidad(query.idUbicacion);//en esta consulta, en el campo idUbicacion esta el codigo del bloque (ojo)
                            query.oBloque.oBodega = BAN_Catalogo_BodegaDA.GetEntidad(query.oBloque.idBodega);
                        }
                    }

                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepcion para despacho";
                respuestaVie.TotalRowsCount = query != null ? 1: 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new BAN_Stowage_Movimiento();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Carga_paraDespacho), "api/VBS_carga_enBodegaParaDespachoEsp", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_getOrdenDespacho")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_OrdenDespacho> getEntidad([FromBody] ParametrosDespachoVBS.ParametrosConsultaListaOrdenesDespachoAgrupadas pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            BAN_Stowage_OrdenDespacho objEntidad = new BAN_Stowage_OrdenDespacho();
            RespuestaViewModel<BAN_Stowage_OrdenDespacho> respuesta = new RespuestaViewModel<BAN_Stowage_OrdenDespacho>();
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
                    var Entity = BAN_Stowage_OrdenDespachoDA.GetEntidad(pObj.idOrdenDespacho);
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el Id de la Orden:{0}", pObj.idOrdenDespacho));
                        Valido = false;
                    }
                    else
                    {

                        var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                        var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                        var oModalidad = BAN_Catalogo_ModalidadDA.ConsultarLista(out oError);
                       
                        Entity.oEstado = oEstado.Where(p => p.id == Entity.estado).FirstOrDefault();
                        Entity.oEstado = oEstado.Where(p => p.id == Entity.estado).FirstOrDefault();
                        Entity.oExportador = oExportador.Where(p => p.id == Entity.idExportador).FirstOrDefault();
                        Entity.oBloque = BAN_Catalogo_BloqueDA.GetEntidad(Entity.idBloque);//en esta consulta, en el campo idUbicacion esta el codigo del bloque (ojo)
                        Entity.oBloque.oBodega = BAN_Catalogo_BodegaDA.GetEntidad(Entity.oBloque.idBodega);

                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.idOrdenDespacho;
                        objEntidad = Entity;
                    }

                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Orden de despacho";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = objEntidad ?? new BAN_Stowage_OrdenDespacho();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Carga_paraDespacho), "api/VBS_carga_enBodegaParaDespachoEsp", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_lista_ordenesDespacho")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> Lista_OrdenesDespacho([FromBody] ParametrosDespachoVBS.ParametrosConsultaListaOrdenesDespacho pObj)
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
                    query = BAN_Stowage_OrdenDespachoDA.ConsultarLista(pObj.idNave, pObj.idExportador, pObj.idBloque, pObj.booking, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Ordenes de Despacho con los criterios ingresados, Error: {0}", OnError));
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

                            foreach (var item in query)
                            {
                                item.oEstado = oEstado.Where(p => p.id == item.estado).FirstOrDefault();
                                item.oExportador = oExportador.Where(p => p.id == item.idExportador).FirstOrDefault();
                                item.oBloque = oBloque.Where(p=> p.id == item.idBloque).FirstOrDefault();//en esta consulta, en el campo idUbicacion esta el codigo del bloque (ojo)
                                item.oBloque.oBodega = oBodega.Where(p=> p.id == item.oBloque.idBodega).FirstOrDefault();
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
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_OrdenesDespacho), "api/VBS_lista_ordenesDespacho", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_save_ordenDespacho")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_OrdenDespacho> RegistraNuevaOrdenDespacho([FromBody] ParametrosDespachoVBS.ParametrosRegistrarOrdenDespacho pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<BAN_Stowage_OrdenDespacho> respuesta = new RespuestaViewModel<BAN_Stowage_OrdenDespacho>();
            BAN_Stowage_OrdenDespacho ObjTarea = new BAN_Stowage_OrdenDespacho();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    BAN_Stowage_OrdenDespacho eEntidad = new BAN_Stowage_OrdenDespacho();
                    eEntidad.idNave = pTarea.idNave;
                    eEntidad.idExportador = pTarea.idExportador;
                    eEntidad.idBodega = pTarea.idBodega;
                    eEntidad.idBloque = pTarea.idBloque;
                    eEntidad.cantidadPalets = pTarea.cantidadPalets;
                    eEntidad.cantidadBox = pTarea.cantidadBox;
                    eEntidad.arrastre = 0;
                    eEntidad.pendiente = pTarea.cantidadPalets;
                    eEntidad.estado = "NUE";
                    eEntidad.usuarioCrea = pTarea.Create_user;
                    eEntidad.usuarioModifica = null;
                    eEntidad.fechaCreacion = System.DateTime.Now;
                    eEntidad.fechaModifica = null;
                    eEntidad.booking = pTarea.booking;

                    BAN_Stowage_OrdenDespachoDA oDespacho = new BAN_Stowage_OrdenDespachoDA();
                    var result = oDespacho.Save_Update(eEntidad, out OnError);

                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se registro con éxito la Orden No: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eEntidad.idOrdenDespacho = Id;
                        ObjTarea = eEntidad;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se pudo grabar la nueva orden de despacho, verifique que la cantidad ingresada no sobrepase lo esperado; {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar Orden de Despacho";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Stowage_OrdenDespacho();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraNuevaOrdenDespacho), "api/VBS_save_ordenDespacho", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_anula_ordenDespacho")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_OrdenDespacho> AnulaOrdenDespacho([FromBody] ParametrosDespachoVBS.ParametrosRegistrarOrdenDespacho pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<BAN_Stowage_OrdenDespacho> respuesta = new RespuestaViewModel<BAN_Stowage_OrdenDespacho>();
            BAN_Stowage_OrdenDespacho ObjTarea = new BAN_Stowage_OrdenDespacho();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    BAN_Stowage_OrdenDespacho eEntidad = new BAN_Stowage_OrdenDespacho();
                    eEntidad.idOrdenDespacho = pTarea.idOrdenDespacho;
                    eEntidad.estado = "ANU";
                    eEntidad.usuarioModifica = pTarea.Modifie_user;
                    eEntidad.fechaModifica = System.DateTime.Now;

                    BAN_Stowage_OrdenDespachoDA oDespacho = new BAN_Stowage_OrdenDespachoDA();
                    var result = oDespacho.Save_Update(eEntidad, out OnError);

                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se anulo con éxito la Orden No: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eEntidad.idOrdenDespacho = Id;
                        ObjTarea = eEntidad;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se pudo anular la nueva orden de despacho, verifique el estado de la orden; {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Anula Orden de Despacho";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Stowage_OrdenDespacho();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraNuevaOrdenDespacho), "api/VBS_anula_ordenDespacho", false, null, null, ex.StackTrace, ex);

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