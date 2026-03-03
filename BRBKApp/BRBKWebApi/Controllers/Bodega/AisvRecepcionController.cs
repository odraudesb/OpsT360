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
    public class AisvRecepcionController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpPost]
        [Route("api/VBS_lista_recepciones")]
        [ValidateModelAttribute]

        public RespuestaViewModel<List<BAN_Stowage_Movimiento>> Lista_Recepcion([FromBody] ParametrosRecepcionVBS.ParametrosConsultaListaRecepcionAisv pObj)
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
                    query = BAN_Stowage_MovimientoDA.ConsultarLista(pObj.idStowageAisv, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Recepciones con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add(string.Format("No existe Recepciones con los criterios ingresados, idStowageAisv {0}", pObj.idStowageAisv));
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
                                item.oStowage_Plan_Aisv = BAN_Stowage_Plan_AisvDA.GetEntidad(item.idStowageAisv);
                                item.oStowage_Plan_Aisv.oStowage_Plan_Det = BAN_Stowage_Plan_DetDA.GetEntidad(item.oStowage_Plan_Aisv.idStowageDet);
                                item.oStowage_Plan_Aisv.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == item.oStowage_Plan_Aisv.oStowage_Plan_Det.idExportador).FirstOrDefault();
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
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Recepcion), "api/VBS_lista_recepciones", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_getRecepcionPorId")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Movimiento> GetRecepcion([FromBody] ParametrosRecepcionVBS.ParametrosConsultaRecepcionAisv pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            BAN_Stowage_Movimiento ObjRecepcion = new BAN_Stowage_Movimiento();
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
                    var Entity = BAN_Stowage_MovimientoDA.GetEntidad(long.Parse(pObj.Id.ToString()));//dbContext.Tasks.AsNoTracking().Where(p => p.Id == pTarea.Id).FirstOrDefault();
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el Id de la recepción:{0}", pObj.Id));
                        Valido = false;
                    }
                    else
                    {
                        var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                        var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                        var oModalidad = BAN_Catalogo_ModalidadDA.ConsultarLista(out oError);

                        Entity.oModalidad = oModalidad.Where(p => p.id == Entity.idModalidad).FirstOrDefault();
                        Entity.oEstado = oEstado.Where(p => p.id == Entity.estado).FirstOrDefault();
                        Entity.oStowage_Plan_Aisv = BAN_Stowage_Plan_AisvDA.GetEntidad(Entity.idStowageAisv);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det = BAN_Stowage_Plan_DetDA.GetEntidad(Entity.oStowage_Plan_Aisv?.idStowageDet);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idExportador).FirstOrDefault();

                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oBodega = BAN_Catalogo_BodegaDA.GetEntidad(Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idBodega);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oBloque = BAN_Catalogo_BloqueDA.GetEntidad(Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idBloque);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oCargo = BAN_Catalogo_CargoDA.GetEntidad(Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idCargo);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oMarca = BAN_Catalogo_MarcaDA.GetEntidad(Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idMarca);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oStowage_Plan_Cab = BAN_Stowage_Plan_CabDA.GetEntidad(Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idStowageCab);

                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.idMovimiento;
                        ObjRecepcion = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjRecepcion ?? new BAN_Stowage_Movimiento();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetRecepcion), "api/VBS_getRecepcionPorId", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_get_recepcionPorBarcode")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Movimiento> GetRecepcionPorBarcode([FromBody] ParametrosRecepcionVBS.ParametrosConsultaRecepcionAisvPorBarcode pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            BAN_Stowage_Movimiento ObjRecepcion = new BAN_Stowage_Movimiento();
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
                    var Entity = BAN_Stowage_MovimientoDA.GetEntidad(pObj.barcode);//dbContext.Tasks.AsNoTracking().Where(p => p.Id == pTarea.Id).FirstOrDefault();
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el Id de la recepción:{0}", pObj.barcode));
                        Valido = false;
                    }
                    else
                    {

                        var oEstado = BAN_Catalogo_EstadoDA.ConsultarLista(out oError);
                        var oExportador = BAN_Catalogo_ExportadorDA.ConsultarListaExportador("CGSA", out oError);
                        var oModalidad = BAN_Catalogo_ModalidadDA.ConsultarLista(out oError);

                        Entity.oModalidad = oModalidad.Where(p => p.id == Entity.idModalidad).FirstOrDefault();
                        Entity.oEstado = oEstado.Where(p => p.id == Entity.estado).FirstOrDefault();
                        Entity.oStowage_Plan_Aisv = BAN_Stowage_Plan_AisvDA.GetEntidad(Entity.idStowageAisv);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det = BAN_Stowage_Plan_DetDA.GetEntidad(Entity.oStowage_Plan_Aisv?.idStowageDet);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idExportador).FirstOrDefault();

                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oBodega = BAN_Catalogo_BodegaDA.GetEntidad(Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idBodega);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oBloque = BAN_Catalogo_BloqueDA.GetEntidad(Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idBloque);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oCargo = BAN_Catalogo_CargoDA.GetEntidad(Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idCargo);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oMarca = BAN_Catalogo_MarcaDA.GetEntidad(Entity.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idMarca);
                        Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.oStowage_Plan_Cab = BAN_Stowage_Plan_CabDA.GetEntidad(Entity.oStowage_Plan_Aisv.oStowage_Plan_Det.idStowageCab);

                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.idMovimiento;
                        ObjRecepcion = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjRecepcion ?? new BAN_Stowage_Movimiento();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetRecepcionPorBarcode), "api/VBS_get_recepcionPorBarcode", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_save_recepcion")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Movimiento> RegistraRecepcion([FromBody] ParametrosRecepcionVBS.ParametrosRegistraRecepcionAsv pTarea)
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
                    BAN_Stowage_Movimiento eRecepcion = new BAN_Stowage_Movimiento();
                    eRecepcion.idStowageAisv = pTarea.idStowageAisv;
                    eRecepcion.idUbicacion = null;
                    eRecepcion.barcode = string.Empty;
                    eRecepcion.idModalidad = pTarea.idModalidad;
                    eRecepcion.tipo = pTarea.tipo;
                    eRecepcion.cantidad = pTarea.cantidad;
                    eRecepcion.observacion = pTarea.observacion;
                    eRecepcion.estado = "NUE";
                    eRecepcion.usuarioCrea = pTarea.Create_user;
                    eRecepcion.usuarioModifica = null;
                    eRecepcion.fechaCreacion = System.DateTime.Now;
                    eRecepcion.fechaModifica = null;
                    List<fotoRecepcionVBS> oFoto = new List<fotoRecepcionVBS>();
                    eRecepcion.Fotos = oFoto;

                    foreach (ParametrosRecepcionVBS.ParametrosRegistraFotoRecepcionAisv oParam in pTarea.Fotos)
                    {
                        v_ruta = string.Empty;
                        Carga_Imagenes(oParam.foto,out v_ruta);
                        oParam.ruta = v_ruta;
                        eRecepcion.Fotos.Add(new fotoRecepcionVBS
                        {
                            idMovimiento = 0,
                            ruta = oParam.ruta,
                            estado = oParam.estado,
                            usuarioCrea = oParam.Create_user
                        });
                    }

                    BAN_Stowage_MovimientoDA oRecepcion = new BAN_Stowage_MovimientoDA();
                    var result = oRecepcion.Save_Update(eRecepcion, out OnError);
                    
                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se registro con éxito la recepción: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eRecepcion.idMovimiento = Id;
                        ObjTarea = eRecepcion;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se pudo grabar la recepción, verifique que la cantidad ingresada no sobrepase lo esperado; {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Stowage_Movimiento();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraRecepcion), "api/VBS_save_recepcion", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_update_recepcion")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Movimiento> ActualizaRecepcion([FromBody] ParametrosRecepcionVBS.ParametrosActualizaRecepcionAisv pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
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
                    BAN_Stowage_Movimiento eRecepcion = new BAN_Stowage_Movimiento();
                    eRecepcion.idMovimiento = pTarea.idMovimiento;
                    eRecepcion.idStowageAisv = pTarea.idStowageAisv;
                    eRecepcion.idModalidad = pTarea.idModalidad;
                    eRecepcion.tipo = pTarea.tipo;
                    eRecepcion.cantidad = pTarea.cantidad;
                    eRecepcion.idUbicacion = pTarea.idUbicacion;
                    eRecepcion.observacion = pTarea.observacion;
                    eRecepcion.estado = "CON";
                    eRecepcion.usuarioCrea = null;
                    eRecepcion.usuarioModifica = pTarea.Modifie_user;
                    eRecepcion.fechaCreacion = null;
                    eRecepcion.fechaModifica = System.DateTime.Now;
                    eRecepcion.isMix = pTarea.isMix;
                    eRecepcion.referencia = pTarea.referencia;

                    List<fotoRecepcionVBS> oFoto = new List<fotoRecepcionVBS>();
                    eRecepcion.Fotos = oFoto;

                    BAN_Stowage_MovimientoDA oRecepcion = new BAN_Stowage_MovimientoDA();
                    var result = oRecepcion.Save_Update(eRecepcion, out OnError);

                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se actualizó con éxito la recepción: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eRecepcion.idMovimiento = Id;
                        ObjTarea = eRecepcion;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Recepción no actualizada, verifique que la cantidad ingresada no sobrepase el total esperado; {0} ", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Actualizar Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Stowage_Movimiento();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(ActualizaRecepcion), "api/VBS_update_recepcion", false, null, null, ex.StackTrace, ex);

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
        [Route("api/VBS_anular_recepcion")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Movimiento> AnularRecepcion([FromBody] ParametrosRecepcionVBS.ParametrosAnularRecepcionAisv pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
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
                    BAN_Stowage_Movimiento eRecepcion = new BAN_Stowage_Movimiento();
                    eRecepcion.idMovimiento = pTarea.idMovimiento;
                    eRecepcion.usuarioModifica = pTarea.Modifie_user;
                    eRecepcion.fechaCreacion = null;
                    eRecepcion.fechaModifica = System.DateTime.Now;

                    BAN_Stowage_MovimientoDA oRecepcion = new BAN_Stowage_MovimientoDA();
                    var result = oRecepcion.Save_anulacion_Movimiento(eRecepcion, out OnError);

                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se anuló con éxito la recepción: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eRecepcion.idMovimiento = Id;
                        ObjTarea = eRecepcion;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Recepción no anulada, verifique el estado de la recepción; {0} ", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Anula Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Stowage_Movimiento();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(ActualizaRecepcion), "api/VBS_update_recepcion", false, null, null, ex.StackTrace, ex);

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

        //--PRE DESPACHO
        [HttpPost]
        [Route("api/VBS_lista_recepcionesPorNoOrden")]
        [ValidateModelAttribute]

        public RespuestaViewModel<List<BAN_Stowage_Movimiento>> Lista_RecepcionesPorNoOrden([FromBody] ParametrosRecepcionVBS.ParametrosConsultaListaRecepcionPorNoOrden pObj)
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
                    query = BAN_Stowage_MovimientoDA.ConsultarLista(pObj.idOrdenDespacho, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Recepciones con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add(string.Format("No existe Recepciones con los criterios ingresados, idStowageAisv {0}", pObj.idOrdenDespacho));
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
                                item.oStowage_Plan_Aisv = BAN_Stowage_Plan_AisvDA.GetEntidad(item.idStowageAisv);
                                item.oStowage_Plan_Aisv.oStowage_Plan_Det = BAN_Stowage_Plan_DetDA.GetEntidad(item.oStowage_Plan_Aisv.idStowageDet);
                                item.oStowage_Plan_Aisv.oStowage_Plan_Det.oExportador = oExportador.Where(p => p.id == item.oStowage_Plan_Aisv.oStowage_Plan_Det.idExportador).FirstOrDefault();
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
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Recepcion), "api/VBS_lista_recepciones", false, null, null, ex.StackTrace, ex);

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