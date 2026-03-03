using ApiModels.AppModels;
using ApiModels.Parametros;
using BRBKWebApiData;
using Microsoft.Ajax.Utilities;
using MiWebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using ViewModel;
using static ViewModel.Enumerados;

namespace BRBKWebApi.Controllers.Cedi
{
    public class CediController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;

        [HttpPost]
        [Route("api/cedi_ordenes_trabajo")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<VHSOrdenTrabajo>> ListaOrdenesTrabajo()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<VHSOrdenTrabajo> query = new List<VHSOrdenTrabajo>();
            RespuestaViewModel<List<VHSOrdenTrabajo>> respuesta = new RespuestaViewModel<List<VHSOrdenTrabajo>>();

            try
            {
                query = CediOrdenTrabajoDA.GetOrdenes(out OnError);

                if (query == null)
                {
                    Mensaje.Add(string.Format("No existe información de ordenes de trabajos pendientes, Error: {0}", OnError));
                    Valido = false;
                }
                else
                {
                    if (query?.Count <= 0 && Valido)
                    {
                        Mensaje.Add(string.Format("No existe información de ordenes de trabajos pendientes, ERROR {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de ordenes pendientes";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<VHSOrdenTrabajo>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(ListaOrdenesTrabajo), "api/cedi_ordenes_trabajo", false, null, null, ex.StackTrace, ex);

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
        [Route("api/cedi_tarjas")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<VHSTarjaMensaje>> ListaTarjas([FromBody] ParametroVHSListaTarjaPendiente parametros)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<VHSTarjaMensaje> query = new List<VHSTarjaMensaje>();
            RespuestaViewModel<List<VHSTarjaMensaje>> respuesta = new RespuestaViewModel<List<VHSTarjaMensaje>>();

            try
            {
                query = CediTarjaDA.GetTarja(parametros.Filtrar, parametros.OrdenTrabajoId, out OnError);

                if (query == null)
                {
                    Mensaje.Add(string.Format("No existe información de tarjas pendientes, Error: {0}", OnError));
                    Valido = false;
                }
                else
                {
                    if (query?.Count <= 0 && Valido)
                    {
                        Mensaje.Add(string.Format("No existe información de tarjas pendientes, ERROR {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de tarjas pendientes";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<VHSTarjaMensaje>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(ListaTarjas), "api/cedi_tarjas", false, null, null, ex.StackTrace, ex);

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
        [Route("api/cedi_registraTarja")]
        [ValidateModelAttribute]
        public RespuestaViewModel<VHSTarjaModel> RegistraTarja([FromBody] ParametroVHSCreaTarja request)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            string v_request = string.Empty;
            string v_response = string.Empty;
            string CuerpoMensaje = string.Empty;

            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<VHSTarjaModel> respuesta = new RespuestaViewModel<VHSTarjaModel>();
            VHSTarjaModel entry = new VHSTarjaModel();

            try
            {
                entry.OrdenTrabajoId = request.OrdenId;
                entry.TarjaFotos = new List<VHSTarjaFotos>();
                entry.Contenido = request.Contenido;
                entry.Observacion = request.Observaciones;
                entry.Usuario = request.Create_user;
                foreach (var item in request.Fotos)
                {
                    entry.TarjaFotos.Add(new VHSTarjaFotos()
                    {
                        ArrayFoto = item.ArrayFoto
                    });
                }

                if (Valido)
                {
                    foreach (VHSTarjaFotos foto in entry.TarjaFotos)
                    {
                        v_ruta = string.Empty;
                        if (foto.ArrayFoto != null && foto.ArrayFoto.Length > 0)
                        {
                            Carga_Imagenes(foto.ArrayFoto, out v_ruta);
                            foto.Ruta = v_ruta;
                        }
                        else
                        {
                            foto.Ruta=string.Empty;
                        }
                    }

                    CediTarjaDA da = new CediTarjaDA();
                    var resultado = CediTarjaDA.RegistraTarjaConFotos(entry, out OnError);
                    
                    if (resultado != null)
                    {
                        Mensaje.Add(string.Format("TarjaId :{0}", resultado.TarjaId));
                        foreach (var item in resultado.TarjaFotos)
                        {
                            Mensaje.Add(string.Format("TarjafotoId :{0}", item.TarjaFotoId));
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar Tarja con fotos";
                respuestaVie.TotalRowsCount = 1;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = entry ?? new VHSTarjaModel();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraTarja), "api/cedi_registraTarja", false, null, null, ex.StackTrace, ex);

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
                string RUTA = recepcionDA.GetConfiguracion("VHS", "RUTA_IMG_VHS");
                //SACO LA EXTESNION DE LA IMAGEN
                string EXTENSION = recepcionDA.GetConfiguracion("VHS", "TIPO_IMG");

                string NOMBRE_IMAGEN = Guid.NewGuid().ToString().Replace("-", "");
                string CARPETA = DateTime.Now.Year.ToString().Trim();
                string SUBCARPETA = DateTime.Now.Month.ToString().Trim();
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
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Carga_Imagenes), "api/{cedi_registraTarja|cedi_tarjaDetalle}/Carga_Imagenes", false, null, null, ex.StackTrace, ex);
                Nombre = (string.Format("EXCEPCION NO CONTROLADA # {0}: {1}", lm, ex.Message.ToString()));
                return false;
            }

            return true;
        }

        [HttpPost]
        [Route("api/cedi_tarjaDetalle")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<VHSTarjaMensaje>> TarjaDetalle([FromBody] ParametroVHSTarjaDetalle parametro)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<VHSTarjaMensaje> query = new List<VHSTarjaMensaje>();
            RespuestaViewModel<List<VHSTarjaMensaje>> respuesta = new RespuestaViewModel<List<VHSTarjaMensaje>>();

            try
            {
                query = CediTarjaDA.GetTarjaConDetalle(true, parametro.OrdenTrabajoId, out OnError);

                if (query == null)
                {
                    Mensaje.Add(string.Format("No existe información de tarjas pendientes, Error: {0}", OnError));
                    Valido = false;
                }
                else
                {
                    if (query?.Count <= 0 && Valido)
                    {
                        Mensaje.Add(string.Format("No existe información de tarjas pendientes, ERROR {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de tarjas pendientes";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<VHSTarjaMensaje>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(TarjaDetalle), "api/cedi_tarjaDetalle", false, null, null, ex.StackTrace, ex);

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
        [Route("api/cedi_tarjaAgregaDetalle")]
        [ValidateModelAttribute]
        public RespuestaViewModel<VHSTarjaDetalleMensaje> AgregaDetalleTarja([FromBody] ParametroVHSTarjaDetalleAdd parametro)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            VHSTarjaDetalleMensaje query = new VHSTarjaDetalleMensaje();
            RespuestaViewModel<VHSTarjaDetalleMensaje> respuesta = new RespuestaViewModel<VHSTarjaDetalleMensaje>();

            try
            {
                // el campo fotosvehiculo contiene la ruta de la imagen
                foreach (var foto in parametro.Fotos)
                {
                    v_ruta = string.Empty;
                    if (foto.ArrayFoto != null && foto.ArrayFoto.Length > 0)
                    {
                        Carga_Imagenes(foto.ArrayFoto, out v_ruta);
                        foto.FotosVehiculo = v_ruta;
                    }
                    else
                    {
                        foto.FotosVehiculo=string.Empty;
                    }
                }

                query = CediTarjaDA.AddTarjaDetalleConFoto(parametro, out OnError);

                if (query == null)
                {
                    Mensaje.Add(string.Format("No existe información de tarjas pendientes, Error: {0}", OnError));
                    Valido = false;
                }
                
                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Se agrego un detalle de tarja con foto";
                respuestaVie.TotalRowsCount = 1;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new VHSTarjaDetalleMensaje();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(AgregaDetalleTarja), "api/cedi_tarjaAgregaDetalle", false, null, null, ex.StackTrace, ex);

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
        [Route("api/cedi_getImage")]
        public HttpResponseMessage GetImage(string filePath)
        {
            try
            {
                string basePath = recepcionDA.GetConfiguracion("VHS", "RUTA_IMG_VHS"); // Obtener la ruta base desde la configuración
                if (string.IsNullOrEmpty(basePath))
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Ruta de configuración no encontrada.")
                    };
                }

                string fullPath = Path.Combine(basePath, filePath.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(fullPath))
                {
                    var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(fileStream)
                    };

                    // Determinar el tipo de contenido según la extensión
                    string extension = Path.GetExtension(fullPath).ToLower();
                    string contentType = extension == ".png" ? "image/png" : extension == ".gif" ? "image/gif" : "image/jpeg";
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                    return response;
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Imagen no encontrada.")
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Error al obtener la imagen: {ex.Message}")
                };
            }
        }

        [Route("api/cedi_tarjaConsultaDetalle")]
        [HttpGet]
        public RespuestaViewModel<AppModelVHSTarjaDetalle> ConsultaDetalleTarja(long detalleTarjaId)
        {
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<AppModelVHSTarjaDetalle> respuesta = new RespuestaViewModel<AppModelVHSTarjaDetalle>();
            List<string> mensajes = new List<string>();
            bool valido = true;

            try
            {
                string onError;
                var query = CediTarjaDA.GetTarjaDetalleById(detalleTarjaId, out onError);

                if (query == null || query.DetalleTarjaID == 0)
                {
                    mensajes.Add($"No se encontró detalle de tarja con ID {detalleTarjaId}. Error: {onError}");
                    valido = false;
                }

                respuestaVie.Mensajes = mensajes;
                respuestaVie.Respuesta = valido;
                respuestaVie.TipoMensaje = valido ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = valido ? "Consulta exitosa" : "Error en consulta";
                respuestaVie.TotalRowsCount = valido ? 1 : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new AppModelVHSTarjaDetalle();
            }
            catch (Exception ex)
            {
                // Suponiendo que 'lm' y 'SqlConexion' estén definidos en tu proyecto para logging
                var lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(ConsultaDetalleTarja), "api/cedi_tarjaConsultaDetalle", false, null, null, ex.StackTrace, ex);
                respuestaVie = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"EXCEPCION NO CONTROLADA # {lm}: {ex.Message}" }
                };
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = new AppModelVHSTarjaDetalle();
            }

            return respuesta;
        }

        [HttpPost]
        [Route("api/cedi_actualizarDetalleTarja")]
        [ValidateModelAttribute]
        public RespuestaViewModel<bool> ActualizarDetalleTarja([FromBody] ParametroVHSActualizarDetalleTarja parametro)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<bool> respuesta = new RespuestaViewModel<bool>();

            try
            {
                bool resultado = CediTarjaDA.UpdateTarjaDetalleBloque(parametro.DetalleTarjaId, parametro.BloqueId, parametro.NumeroBloque, parametro.VIN, parametro.Observaciones, out OnError);

                if (!resultado)
                {
                    Mensaje.Add(string.Format("No se pudo actualizar el detalle de tarja, Error: {0}", OnError));
                    Valido = false;
                }
                else
                {
                    Mensaje.Add("Detalle de tarja actualizado exitosamente.");
                }

                respuestaVie.Mensajes = Mensaje;
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = Valido ? "Actualización exitosa" : "Error al actualizar";
                respuestaVie.TotalRowsCount = Valido ? 1 : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = resultado;
            }
            catch (Exception ex)
            {
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(ActualizarDetalleTarja), "api/cedi_actualizarDetalleTarja", false, null, null, ex.StackTrace, ex);

                respuestaVie = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"EXCEPCIÓN NO CONTROLADA # {lm}: {ex.Message}" }
                };
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = false;
            }

            return respuesta;
        }

        [HttpGet]
        [Route("api/cedi_listaBloques")]
        public RespuestaViewModel<List<Dictionary<string, object>>> ListaBloques()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<List<Dictionary<string, object>>> respuesta = new RespuestaViewModel<List<Dictionary<string, object>>>();

            try
            {
                var query = CediTarjaDA.GetBloques(out OnError);

                if (query == null || query.Count == 0)
                {
                    Mensaje.Add(string.Format("No se encontraron bloques, Error: {0}", OnError));
                    Valido = false;
                }

                respuestaVie.Mensajes = Mensaje;
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = Valido ? "Lista de bloques" : "Error al obtener bloques";
                respuestaVie.TotalRowsCount = query.Count;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query;
            }
            catch (Exception ex)
            {
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(ListaBloques), "api/cedi_listaBloques", false, null, null, ex.StackTrace, ex);

                respuestaVie = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"EXCEPCIÓN NO CONTROLADA # {lm}: {ex.Message}" }
                };
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = new List<Dictionary<string, object>>();
            }

            return respuesta;
        }

        [HttpGet]
        [Route("api/cedi_listaVehiculosDespacho")]
        public RespuestaViewModel<List<VHSMensajeSimple>> ListaVehiculosDespacho(long paseId)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<VHSMensajeSimple> query = new List<VHSMensajeSimple>();
            RespuestaViewModel<List<VHSMensajeSimple>> respuesta = new RespuestaViewModel<List<VHSMensajeSimple>>();

            try
            {
                query = CediTarjaDA.GetVehiculosDespacho(paseId, out OnError);

                if (query == null || query.Count <= 0)
                {
                    Mensaje.Add($"No existe información. Error: {OnError}");
                    Valido = false;
                }

                respuestaVie.Mensajes = Mensaje;
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Vehículos despacho";
                respuestaVie.TotalRowsCount = query?.Count ?? 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<VHSMensajeSimple>();
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                respuestaVie.Mensajes = new List<string>() { $"Error: {ex.Message}" };
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;
        }

        [HttpPost]
        [Route("api/cedi_registraEvidenciaEntrega")]
        [ValidateModelAttribute]
        public RespuestaViewModel<long> RegistraEvidenciaEntrega([FromBody] ParametroVHSCrearEvidenciaEntrega parametro)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<long> respuesta = new RespuestaViewModel<long>();

            try
            {
                string v_ruta;
                foreach (var foto in parametro.Fotos)
                {
                    v_ruta = string.Empty;
                    if (foto.ArrayFoto != null && foto.ArrayFoto.Length > 0)
                    {
                        Carga_Imagenes(foto.ArrayFoto, out v_ruta);
                        foto.FotoRuta = v_ruta;
                    }
                    else
                    {
                        foto.FotoRuta = string.Empty;
                    }
                }

                string OnError;
                long evidenciaId = CediEvidenciaEntregaDA.CrearEvidenciaEntrega(
                    parametro.VehiculoDespachadoID,
                    parametro.Observacion,
                    parametro.Usuario,
                    out OnError
                );

                if (evidenciaId <= 0)
                {
                    Mensaje.Add($"Error creando evidencia: {OnError}");
                    Valido = false;
                }
                else
                {
                    foreach (var foto in parametro.Fotos)
                    {
                        if (!string.IsNullOrEmpty(foto.FotoRuta))
                        {
                            int fotoId = CediEvidenciaEntregaDA.CrearEvidenciaEntregaFoto(evidenciaId, foto.FotoRuta, parametro.Usuario, out OnError);
                            foto.FotoID = fotoId;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje.Count > 0 ? Mensaje : new List<string> { "Evidencia registrada correctamente." };
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido ? Enumerados.TipoMensaje.Exito : Enumerados.TipoMensaje.Error;
                respuestaVie.Titulo = "Registro de Evidencia";
                respuestaVie.TotalRowsCount = 1;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = evidenciaId;
            }
            catch (Exception ex)
            {
                var lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(
                    this.User.Identity.Name,
                    nameof(RegistraEvidenciaEntrega),
                    "api/cedi_registraEvidenciaEntrega",
                    false,
                    null,
                    null,
                    ex.StackTrace,
                    ex
                );

                respuestaVie = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string>
            {
                $"EXCEPCIÓN NO CONTROLADA #{lm}: {ex.Message}"
            }
                };
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;
        }


        [HttpPost]
        [Route("api/cedi_registrarNovedadDetalleTarja")]
        [ValidateModelAttribute]
        public RespuestaViewModel<int> RegistrarNovedadDetalleTarja([FromBody] ParametroRegistrarNovedadDetalleTarja request)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;

            RespuestaViewModel<int> respuesta = new RespuestaViewModel<int>();
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            int novedadId = -1;

            try
            {
                // Insertar novedad
                novedadId = CediTarjaDA.CrearNovedad(request.DetalleTarjaID, request.TipoNovedadID, request.Descripcion, request.Usuario, out string errorNovedad);

                if (novedadId <= 0)
                {
                    Mensaje.Add($"No se pudo registrar la novedad. Error: {errorNovedad}");
                    Valido = false;
                }
                else
                {
                    Mensaje.Add($"Novedad registrada con ID: {novedadId}");

                    // Insertar fotos
                    foreach (var foto in request.Fotos)
                    {
                        if (foto.ArrayFoto != null && foto.ArrayFoto.Length > 0)
                        {
                            v_ruta = string.Empty;

                            Carga_Imagenes(foto.ArrayFoto, out v_ruta);

                            var fotoId = CediTarjaDA.CrearNovedadFoto(novedadId, v_ruta, request.Usuario, out string errorFoto);

                            Mensaje.Add($"Foto insertada ID: {fotoId}");
                        }
                    }
                }

                respuestaVie.Respuesta = Valido;
                respuestaVie.Mensajes = Mensaje;
                respuestaVie.TipoMensaje = Valido ? Enumerados.TipoMensaje.Exito : Enumerados.TipoMensaje.Error;
                respuestaVie.Titulo = "Registro de Novedad";
                respuestaVie.TotalRowsCount = 1;

                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = novedadId;
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { ex.Message }
                };
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = -1;
            }

            return respuesta;
        }



        [HttpGet]
        [Route("api/cedi_listaTiposNovedad")]
        public RespuestaViewModel<List<Dictionary<string, object>>> ListaTiposNovedad()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<List<Dictionary<string, object>>> respuesta = new RespuestaViewModel<List<Dictionary<string, object>>>();

            try
            {
                var query = CediTarjaDA.GetTiposNovedad(out OnError);

                if (query == null || query.Count == 0)
                {
                    Mensaje.Add($"No se encontraron tipos de novedad. Error: {OnError}");
                    Valido = false;
                }

                respuestaVie.Mensajes = Mensaje;
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = Valido ? "Lista de Tipos de Novedad" : "Error al obtener tipos";
                respuestaVie.TotalRowsCount = query.Count;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query;
            }
            catch (Exception ex)
            {
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(
                    this.User.Identity.Name,
                    nameof(ListaTiposNovedad),
                    "api/cedi_listaTiposNovedad",
                    false,
                    null,
                    null,
                    ex.StackTrace,
                    ex
                );

                respuestaVie = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"EXCEPCIÓN NO CONTROLADA # {lm}: {ex.Message}" }
                };
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = new List<Dictionary<string, object>>();
            }

            return respuesta;
        }


    }


}
