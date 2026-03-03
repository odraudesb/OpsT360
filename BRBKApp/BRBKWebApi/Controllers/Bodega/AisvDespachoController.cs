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
    public class AisvDespachoController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpPost]
        [Route("api/VBS_registra_despacho")]
        [ValidateModelAttribute]
        public RespuestaViewModel<BAN_Stowage_Movimiento> RegistraDespacho([FromBody] ParametrosRecepcionVBS.ParametrosRegistrarDespachoAisv pTarea)
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
                    eRecepcion.idMovimiento = pTarea.idMovimiento;
                    eRecepcion.tipo = "EGR";
                    eRecepcion.usuarioCrea = pTarea.Create_user;
                    eRecepcion.fechaCreacion = null;
                    eRecepcion.fechaModifica = System.DateTime.Now;

                    List<fotoRecepcionVBS> oFoto = new List<fotoRecepcionVBS>();
                    eRecepcion.Fotos = oFoto;

                    foreach (ParametrosRecepcionVBS.ParametrosRegistraFotoRecepcionAisv oParam in pTarea.Fotos)
                    {
                        v_ruta = string.Empty;
                        Carga_Imagenes(oParam.foto, out v_ruta);
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
                        Mensaje.Add(string.Format("Se registro con éxito el despacho: [{0}] ", IdGenerado.Value));
                        Valido = true;

                        Int64 Id = IdGenerado.Value;

                        eRecepcion.idMovimiento = Id;
                        ObjTarea = eRecepcion;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Despacho no fue procesado, verifique el siguiente mensaje: {0} ", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registro de despacho";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new BAN_Stowage_Movimiento();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraDespacho), "api/VBS_registra_despacho", false, null, null, ex.StackTrace, ex);

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