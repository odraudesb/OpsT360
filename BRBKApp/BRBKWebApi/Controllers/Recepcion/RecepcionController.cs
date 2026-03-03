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

namespace MiWebApi.Controllers.Recepcion
{
    public class RecepcionController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpPost]
        [Route("api/lista_recepciones")]
        [ValidateModelAttribute]

        public RespuestaViewModel<List<recepcion>> Lista_Recepcion([FromBody] ParametrosRecepcion.ParametrosConsultaListaRecepcion pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<recepcion> query = new List<recepcion>();
            RespuestaViewModel<List<recepcion>> respuesta = new RespuestaViewModel<List<recepcion>>();

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    query = BRBKWebApiData.recepcionDA.listadoRecepcion(pObj.idTarjaDet,pObj.lugar, out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Recepciones con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add(string.Format("No existe Recepciones con los criterios ingresados, idTarjaDet {0}", pObj.idTarjaDet));
                            Valido = false;
                        }
                    }
                    
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de Recepciones por BL";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<recepcion>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Recepcion), "api/lista_Recepciones", false, null, null, ex.StackTrace, ex);

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
        [Route("api/consulta_recepcion")]
        [ValidateModelAttribute]
        public RespuestaViewModel<recepcion> GetRecepcion([FromBody] ParametrosRecepcion.ParametrosConsultaRecepcion pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            recepcion ObjRecepcion = new recepcion();
            RespuestaViewModel<recepcion> respuesta = new RespuestaViewModel<recepcion>();

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entity = recepcionDA.GetRecepcion(long.Parse(pObj.Id.ToString()));//dbContext.Tasks.AsNoTracking().Where(p => p.Id == pTarea.Id).FirstOrDefault();
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el Id de la recepción:{0}", pObj.Id));
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.idRecepcion;

                        ObjRecepcion = Entity;
                        //ObjRecepcion.idRecepcion = Entity.idRecepcion;
                        //ObjRecepcion.idTarjaDet = Entity.idTarjaDet;
                        //ObjRecepcion.idGrupo = Entity.idGrupo;
                        //ObjRecepcion.lugar = Entity.lugar;
                        //ObjRecepcion.cantidad = Entity.cantidad;
                        //ObjRecepcion.ubicacion = Entity.ubicacion;
                        //ObjRecepcion.Ubicaciones = Entity.Ubicaciones;
                        //ObjRecepcion.observacion = Entity.observacion;
                        //ObjRecepcion.estado = Entity.estado;
                        //ObjRecepcion.Estados = Entity.Estados;
                        //ObjRecepcion.usuarioCrea = string.IsNullOrEmpty(Entity.usuarioCrea) ? string.Empty : Entity.usuarioCrea.Trim();
                        //ObjRecepcion.usuarioModifica = string.IsNullOrEmpty(Entity.usuarioModifica) ? string.Empty : Entity.usuarioModifica.Trim();
                        //ObjRecepcion.fechaCreacion = Entity.fechaCreacion;
                        //ObjRecepcion.fechaModifica = Entity.fechaModifica;

                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjRecepcion ?? new recepcion();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetRecepcion), "api/consulta_recepcion", false, null, null, ex.StackTrace, ex);

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
        [Route("api/registra_recepcion")]
        [ValidateModelAttribute]
        public RespuestaViewModel<recepcion> RegistraRecepcion([FromBody] ParametrosRecepcion.ParametrosRegistraRecepcion pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<recepcion> respuesta = new RespuestaViewModel<recepcion>();
            recepcion ObjTarea = new recepcion();
            //DbContextTransaction Transaction = dbContext.Database.BeginTransaction();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    recepcion eRecepcion = new recepcion();
                    eRecepcion.idTarjaDet = pTarea.idTarjaDet;
                    eRecepcion.idGrupo = 0;
                    eRecepcion.lugar = pTarea.lugar;
                    eRecepcion.cantidad = pTarea.cantidad;
                    eRecepcion.ubicacion = string.Empty;
                    eRecepcion.observacion = string.Empty;
                    eRecepcion.estado = "NUE";
                    eRecepcion.usuarioCrea = pTarea.Create_user;
                    eRecepcion.usuarioModifica = null;
                    eRecepcion.fechaCreacion = System.DateTime.Now;
                    eRecepcion.fechaModifica = null;
                    List<fotoRecepcion> oFoto = new List<fotoRecepcion>();
                    eRecepcion.Fotos = oFoto;

                    foreach (ParametrosRecepcion.ParametrosRegistraFotoRecepcion oParam in pTarea.Fotos)
                    {
                        v_ruta = string.Empty;
                        Carga_Imagenes(oParam.foto,out v_ruta);
                        oParam.ruta = v_ruta;
                        eRecepcion.Fotos.Add(new fotoRecepcion
                        {
                            idrecepcion = 0,
                            ruta = oParam.ruta,
                            estado = oParam.estado,
                            usuarioCrea = oParam.Create_user
                        });
                    }

                    recepcionDA oRecepcion = new recepcionDA();
                    var result = oRecepcion.Save_Update(eRecepcion, out OnError);
                    
                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se registro con éxito la recepción: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eRecepcion.idRecepcion = Id;
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
                respuesta.Respuesta = ObjTarea ?? new recepcion();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraRecepcion), "api/registra_recepcion", false, null, null, ex.StackTrace, ex);

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
                string RUTA = recepcionDA.GetConfiguracion("APP", "RUTA_IMG");//dbContext.ConfigurationSet.Where(x => x.Name.Trim().Equals("RUTA_IMG")).Select(kvp => kvp.Value).FirstOrDefault();

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

        [HttpPost]
        [Route("api/actualizar_recepcion")]
        [ValidateModelAttribute]
        public RespuestaViewModel<recepcion> ActualizaRecepcion([FromBody] ParametrosRecepcion.ParametrosActualizaRecepcion pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<recepcion> respuesta = new RespuestaViewModel<recepcion>();
            recepcion ObjTarea = new recepcion();
           
            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    recepcion eRecepcion = new recepcion();
                    eRecepcion.idRecepcion = pTarea.IdRecepcion;
                    eRecepcion.idTarjaDet = pTarea.idTarjaDet;
                    eRecepcion.idGrupo = 0;
                    eRecepcion.lugar = string.Empty;
                    eRecepcion.cantidad = pTarea.cantidad;
                    eRecepcion.ubicacion = pTarea.ubicacion;
                    eRecepcion.observacion = pTarea.observacion;
                    eRecepcion.estado = "CON";
                    eRecepcion.usuarioCrea = null;
                    eRecepcion.usuarioModifica = pTarea.Modifie_user;
                    eRecepcion.fechaCreacion = null;
                    eRecepcion.fechaModifica = System.DateTime.Now;

                    recepcionDA oRecepcion = new recepcionDA();
                    var result = oRecepcion.Save_Update(eRecepcion, out OnError);

                    IdGenerado = result;

                    if (IdGenerado.HasValue)
                    {
                        Mensaje.Add(string.Format("Se actualizó con éxito la recepción: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eRecepcion.idRecepcion = Id;
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
                respuesta.Respuesta = ObjTarea ?? new recepcion();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraRecepcion), "api/registra_recepcion", false, null, null, ex.StackTrace, ex);

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