using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ViewModel;
using ApiModels;
using ApiModels.Resultados;
using ApiModels.Parametros;
using ApiModels.AppModels;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ApiDatos
{
    public class Datos
    {
        //string Baseurl = "https://apps.cgsa.com.ec/WebApiBRBK/"; //Producción
        string Baseurl = "http://172.16.2.13:5152/WebApiBRBK/"; //Desarrollo
        //string Baseurl = "https://localhost:44346//WebApiBRBK/"; //Desarrollo
        ResultadoViewModel respuestaVie;
        public async Task<RespuestaViewModel<User>> Login(string usuario, string clave, string imei)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<User> respuesta = new RespuestaViewModel<User>();
            try
            {

                Login _login = new Login()
                {
                    UserName = usuario,
                    UserPassword = clave,
                    Device = imei
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/login");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.ConnectionClose = true;
                    var postTask = client.PostAsJsonAsync<Login>("Login", _login).ConfigureAwait(false);


                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        try
                        {
                            respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<User>>(readTask);
                        }
                        catch
                        {
                            var data = (JContainer)JsonConvert.DeserializeObject(readTask);
                            if (data["Resultado"].ToString().Length >0)
                            {
                                var resp =  JsonConvert.DeserializeObject<ResultadoViewModel>(data["Resultado"].ToString());
                                respuesta.Respuesta = new User();
                                respuesta.Resultado = resp;
                            }
                            else
                            {
                                //_exit.isCorrect = false;
                                //_exit.strResultado = "Ocurrio un error en la llamada! :: " + data["ap_res2"];
                            }
                        }
                        return respuesta;
                    }

                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                return respuesta;
            }
        }

        public async Task<RespuestaViewModel<tarjaDet>> GetTarjaDet(long _idTarjaDet)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<tarjaDet> respuesta = new RespuestaViewModel<tarjaDet>();
            try
            {
                ParametroConsultarTarjaDetXId _rec = new ParametroConsultarTarjaDetXId()
                {
                    IdTarjaDet = _idTarjaDet,
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/consulta_bl");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarTarjaDetXId>("consulta_bl", _rec).ConfigureAwait(false); ;
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<tarjaDet>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: "+ ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<tarjaDet>>> BLInProgresses(string _mrn, string _lugar)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<tarjaDet>> respuesta = new RespuestaViewModel<List<tarjaDet>>();
            try
            {

                ParametroConsultarTarjaDet _params = new ParametroConsultarTarjaDet()
                {
                    MRN = _mrn
                    ,Lugar = _lugar
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/lista_BL");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarTarjaDet>("lista_BL", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<tarjaDet>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<recepcion>>> RecepcionInProgresses(long _idTarjaDet,string _lugar)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<recepcion>> respuesta = new RespuestaViewModel<List<recepcion>>();
            try
            {
                ParametroConsultarRecepcion _params = new ParametroConsultarRecepcion()
                {
                    idTarjaDet = _idTarjaDet
                    ,lugar = _lugar
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/lista_recepciones");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarRecepcion>("lista_Recepciones", _params).ConfigureAwait(false);

                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<recepcion>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<recepcion>> GetRecepcion(long _idRecepcion)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<recepcion> respuesta = new RespuestaViewModel<recepcion>();
            try
            {
                ParametroConsultarRecepcion _rec = new ParametroConsultarRecepcion()
                {
                    Id = _idRecepcion,
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/consulta_recepcion");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarRecepcion>("consulta_recepcion", _rec).ConfigureAwait(false); 
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<recepcion>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;
        }

        public async Task<RespuestaViewModel<recepcion>> RegistersRecepcion(recepcion oRecepcion, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4 )
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<recepcion> respuesta = new RespuestaViewModel<recepcion>();

            try
            {
                List<ParametroRegistrarRecepcionFoto> Foto = new List<ParametroRegistrarRecepcionFoto>();
                List<ParametroRegistrarRecepcionFoto> fotos = new List<ParametroRegistrarRecepcionFoto>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFoto
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = oRecepcion.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFoto
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = oRecepcion.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFoto
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = oRecepcion.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFoto
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = oRecepcion.usuarioCrea
                    });
                }

                ParametroRegistrarRecepcion _params = new ParametroRegistrarRecepcion()
                {
                    Fotos = fotos,
                    idTarjaDet = oRecepcion.idTarjaDet,
                    cantidad = int.Parse(oRecepcion.cantidad.ToString()),
                    estado = oRecepcion.estado,
                    lugar= oRecepcion.lugar,
                    Create_user = oRecepcion.usuarioCrea
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/registra_recepcion");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroRegistrarRecepcion>("registra_recepcion", _params).ConfigureAwait(false);
                    
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<recepcion>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<recepcion>> UpdateRecepcion(recepcion oRecepcion)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<recepcion> respuesta = new RespuestaViewModel<recepcion>();
            try
            {
                ParametroActualizarRecepcion _params = new ParametroActualizarRecepcion()
                {
                    IdRecepcion = oRecepcion.idRecepcion,
                    idTarjaDet = oRecepcion.idTarjaDet,
                    cantidad = int.Parse(oRecepcion.cantidad.ToString()),
                    ubicacion = oRecepcion.ubicacion,
                    observacion = oRecepcion.observacion,
                    estado = oRecepcion.estado,
                    Modifie_user = oRecepcion.usuarioModifica
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/actualizar_recepcion");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroActualizarRecepcion>("actualizar_recepcion", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<recepcion>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<ubicacion>>> GetUbicaciones()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<ubicacion>> respuesta = new RespuestaViewModel<List<ubicacion>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/lista_ubicaciones", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<ubicacion>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<novedad>> RegistersNovedad(novedad oNovedad, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<novedad> respuesta = new RespuestaViewModel<novedad>();
            try
            {
                List<ParametroRegistrarNovedadFoto> Foto = new List<ParametroRegistrarNovedadFoto>();
                List<ParametroRegistrarNovedadFoto> fotos = new List<ParametroRegistrarNovedadFoto>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarNovedadFoto
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = oNovedad.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarNovedadFoto
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = oNovedad.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarNovedadFoto
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = oNovedad.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarNovedadFoto
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = oNovedad.usuarioCrea
                    });
                }

                ParametroRegistrarNovedad _params = new ParametroRegistrarNovedad()
                {
                    idRecepcion = oNovedad.idRecepcion,
                    Fotos = fotos,
                    descripcion = oNovedad.descripcion,
                    estado = oNovedad.estado,
                    Create_user = oNovedad.usuarioCrea
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl + "api/registra_novedad");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarNovedad>("registra_novedad", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<novedad>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<pasePuerta>> GetPasePuerta(string _numPase)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<pasePuerta> respuesta = new RespuestaViewModel<pasePuerta>();
            try
            {
                ParametroConsultarPasePuerta _rec = new ParametroConsultarPasePuerta()
                {
                    numeroPase = _numPase,
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/consulta_pase");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarPasePuerta>("consulta_pase", _rec).ConfigureAwait(false);
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<pasePuerta>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;
        }
        public async Task<RespuestaViewModel<despacho>> RegistersDespacho(despacho oDespacho, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<despacho> respuesta = new RespuestaViewModel<despacho>();
            try
            {
                List<ParametroRegistrarDespachoFoto> Foto = new List<ParametroRegistrarDespachoFoto>();
                List<ParametroRegistrarDespachoFoto> fotos = new List<ParametroRegistrarDespachoFoto>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarDespachoFoto
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = oDespacho.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarDespachoFoto
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = oDespacho.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarDespachoFoto
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = oDespacho.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarDespachoFoto
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = oDespacho.usuarioCrea
                    });
                }

                ParametroRegistrarDespacho _params = new ParametroRegistrarDespacho()
                {
                    idTarjaDet = oDespacho.idTarjaDet,
                    pase = oDespacho.pase,
                    mrn = oDespacho.mrn,
                    msn = oDespacho.msn,
                    hsn = oDespacho.hsn,
                    placa = oDespacho.placa,
                    idchofer = oDespacho.idchofer,
                    chofer = oDespacho.chofer,
                    cantidad = oDespacho.cantidad,
                    observacion = oDespacho.observacion,
                    estado = oDespacho.estado,
                    Create_user = oDespacho.usuarioCrea,
                    delivery = oDespacho.delivery,
                    PRE_GATE_ID = oDespacho.PRE_GATE_ID,
                    Fotos = fotos
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl + "api/registra_despacho");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarDespacho>("registra_despacho", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<despacho>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }


        /**************************************************************************************************************************************************
         * verifica posicion actual del contenedor y valida el contenedor
         **************************************************************************************************************************************************/
        public async Task<RespuestaViewModel<sealValidation>> GetSeal(string _numContainers, string _seal, string _position, long _gkey, bool _bloqueo, bool _impedimento, string _impedimentod, string _user, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            RespuestaViewModel<sealValidation> respuesta = new RespuestaViewModel<sealValidation>();
            try
            {
                List<ParametroRegistrarSelloFoto> fotos = new List<ParametroRegistrarSelloFoto>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = _user
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = _user
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = _user
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = _user
                    });
                }


                HttpClientHandler handler = new HttpClientHandler();
                ParametroConsultarSello _params = new ParametroConsultarSello()
                {
                    container = _numContainers,
                    seal = _seal,
                    position = _position,
                    Create_user = _user,
                    gkey = _gkey,
                    bloqueo = _bloqueo,
                    impedimento = _impedimento,
                    impedimentod = _impedimentod,
                    Fotos = fotos
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/validar_sello");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarSello>("validar_sello", _params);

                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<sealValidation>>(readTask);
                        return respuesta;
                    }

                    return respuesta;
                }
            }
            catch
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas"));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                return respuesta;
            }
        }
        /**************************************************************************************************************************************************
        * Registrar asignación de sello en muelle - IMPO
        **************************************************************************************************************************************************/
        public async Task<RespuestaViewModel<sealAsignacionMuelle>> SetSealRegisters(sealAsignacionMuelle objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            RespuestaViewModel<sealAsignacionMuelle> respuesta = new RespuestaViewModel<sealAsignacionMuelle>();
            try
            {
                List<ParametroRegistrarSelloFoto> fotos = new List<ParametroRegistrarSelloFoto>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }


                HttpClientHandler handler = new HttpClientHandler();
                ParametroRegistrarSealMuelle _params = new ParametroRegistrarSealMuelle()
                {
                    container = objSeal.container?.ToUpper(),
                    sello_CGSA = objSeal.sello_CGSA?.ToUpper(),
                    sello1 = objSeal.sello1?.ToUpper(),
                    sello2 = objSeal.sello2?.ToUpper(),
                    sello3 = objSeal.sello3?.ToUpper(),
                    sello4 = objSeal.sello4?.ToUpper(),
                    color = string.Empty,
                    ip = objSeal.ip,
                    Create_user = objSeal.usuarioCrea,
                    
                    gkey = objSeal.gkey,
                    dataContainer = objSeal.dataContainer?.ToUpper(),
                    position = objSeal.position?.ToUpper(),
                    referencia = objSeal.referencia?.ToUpper(),
                    xmlN4 = objSeal.xmlN4Discharge,
                    respuestaN4 = objSeal.respuestaN4Discharge,
                    Fotos = fotos
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/registra_sealMuelle");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarSealMuelle>("registra_sealMuelle", _params);

                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<sealAsignacionMuelle>>(readTask);
                        return respuesta;
                    }

                    return respuesta;
                }
            }
            catch
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas"));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                return respuesta;
            }
        }

        /**************************************************************************************************************************************************
        * obtiene esquema de seguridad por rol del usuario 
        **************************************************************************************************************************************************/
        public async Task<RespuestaViewModel<List<opcionesRoles>>> GetPermisosXRol(long _idRol)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<opcionesRoles>> respuesta = new RespuestaViewModel<List<opcionesRoles>>();
            try
            {
                ParametroPermisosXRol _params = new ParametroPermisosXRol()
                {
                    idRol = _idRol
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/lista_opcionesRol");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroPermisosXRol>("lista_opcionesRol", _params).ConfigureAwait(false);

                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<opcionesRoles>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }

        /**************************************************************************************************************************************************
        *  POW
        **************************************************************************************************************************************************/
        public async Task<RespuestaViewModel<workPosition>> GetPOW(string _position,string _usuario, string _containers)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<workPosition> respuesta = new RespuestaViewModel<workPosition>();
            try
            {
                ParametroObtenerPOW _params = new ParametroObtenerPOW()
                {
                    idPosition = _position,
                    usuarioCrea = _usuario,
                    containers = _containers
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/consulta_POW");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroObtenerPOW>("consulta_POW", _params).ConfigureAwait(false);

                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<workPosition>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<workPositionN4List>>> GetListPOWN4()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<workPositionN4List>> respuesta = new RespuestaViewModel<List<workPositionN4List>>();
            try
            {
                ParametroGetListPOWN4 _params = new ParametroGetListPOWN4()
                {
                    parametro1 = string.Empty
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/lista_POWN4");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroGetListPOWN4>("lista_POWN4", _params).ConfigureAwait(false);

                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<workPositionN4List>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<workPosition>> SetPOWRegisters(workPosition objPow)
        {
            RespuestaViewModel<workPosition> respuesta = new RespuestaViewModel<workPosition>();
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                ParametroRegistrarPOW _params = new ParametroRegistrarPOW()
                {
                    ip = objPow.ip,
                    imei = objPow.imei,
                    idPosition = objPow.idPosition,
                    namePosition = objPow.namePosition,
                    estado= objPow.estado,
                    usuarioCrea = objPow.usuarioCrea,
                    Create_user = objPow.usuarioCrea,
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/registra_pow");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarPOW>("registra_pow", _params);

                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<workPosition>>(readTask);
                        return respuesta;
                    }

                    return respuesta;
                }
            }
            catch
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas"));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                return respuesta;
            }
        }

        /**************************************************************************************************************************************************
        * DOWNLOAD CONFIRMATION 
        **************************************************************************************************************************************************/
        public async Task<RespuestaViewModel<dataContainers>> GetDataContainersImpo(string _containers)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<dataContainers> respuesta = new RespuestaViewModel<dataContainers>();
            try
            {
                ParametroObtenerDataContainersImpo _params = new ParametroObtenerDataContainersImpo()
                {
                    numcontainers = _containers
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/consulta_Containers_impo");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroObtenerDataContainersImpo>("consulta_Containers_impo", _params).ConfigureAwait(false);

                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<dataContainers>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<downloadConfirmation>> SetDownloadConfirmation(downloadConfirmation obj)
        {
            RespuestaViewModel<downloadConfirmation> respuesta = new RespuestaViewModel<downloadConfirmation>();
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                ParametroRegistraConfirmacionDescarga _params = new ParametroRegistraConfirmacionDescarga()
                {
                    gkey = obj.gkey,
                    container = obj.container,
                    dataContainer = obj.dataContainer,
                    position = obj.position,
                    referencia = obj.referencia,
                    Create_user = obj.usuarioCrea,
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/confirma_Descarga");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistraConfirmacionDescarga>("confirma_Descarga", _params);

                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<downloadConfirmation>>(readTask);
                        return respuesta;
                    }

                    return respuesta;
                }
            }
            catch
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas"));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                return respuesta;
            }
        }

        /**************************************************************************************************************************************************
        * Registrar sello PRE-EMBARQUE
        **************************************************************************************************************************************************/
        public async Task<RespuestaViewModel<sealRegistroPreEmbarqueYaforo>> SetSealPreEmbarque(sealRegistroPreEmbarqueYaforo objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            RespuestaViewModel<sealRegistroPreEmbarqueYaforo> respuesta = new RespuestaViewModel<sealRegistroPreEmbarqueYaforo>();
            try
            {
                List<ParametroRegistrarSelloFoto> fotos = new List<ParametroRegistrarSelloFoto>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }


                HttpClientHandler handler = new HttpClientHandler();
                ParametroRegistrarSealPreEmbarque _params = new ParametroRegistrarSealPreEmbarque()
                {
                    container = objSeal.container?.ToUpper(),
                    sello_CGSA = objSeal.sello_CGSA?.ToUpper(),
                    sello1 = objSeal.sello1?.ToUpper(),
                    sello2 = objSeal.sello2?.ToUpper(),
                    sello3 = objSeal.sello3?.ToUpper(),
                    ip = objSeal.ip,
                    Create_user = objSeal.usuarioCrea,

                    gkey = objSeal.gkey,
                    Fotos = fotos
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/registra_sealPreEmbarque");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarSealPreEmbarque>("registra_sealPreEmbarque", _params);

                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<sealRegistroPreEmbarqueYaforo>>(readTask);
                        return respuesta;
                    }

                    return respuesta;
                }
            }
            catch
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas"));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                return respuesta;
            }
        }

        /**************************************************************************************************************************************************
        * Registrar sello AFORO
        **************************************************************************************************************************************************/
        public async Task<RespuestaViewModel<sealRegistroPreEmbarqueYaforo>> SetSealAforo(sealRegistroPreEmbarqueYaforo objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            RespuestaViewModel<sealRegistroPreEmbarqueYaforo> respuesta = new RespuestaViewModel<sealRegistroPreEmbarqueYaforo>();
            try
            {
                List<ParametroRegistrarSelloFoto> fotos = new List<ParametroRegistrarSelloFoto>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }


                HttpClientHandler handler = new HttpClientHandler();
                ParametroRegistrarSealPreEmbarque _params = new ParametroRegistrarSealPreEmbarque()
                {
                    container = objSeal.container?.ToUpper(),
                    sello_CGSA = objSeal.sello_CGSA?.ToUpper(),
                    sello1 = objSeal.sello1?.ToUpper(),
                    sello2 = objSeal.sello2?.ToUpper(),
                    sello3 = objSeal.sello3?.ToUpper(),
                    ip = objSeal.ip,
                    Create_user = objSeal.usuarioCrea,

                    gkey = objSeal.gkey,
                    Fotos = fotos
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/registra_sealAforo");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarSealPreEmbarque>("registra_sealAforo", _params);

                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<sealRegistroPreEmbarqueYaforo>>(readTask);
                        return respuesta;
                    }

                    return respuesta;
                }
            }
            catch
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas"));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                return respuesta;
            }
        }

       /**************************************************************************************************************************************************
       * Registrar sello Expo
       **************************************************************************************************************************************************/
        public async Task<RespuestaViewModel<AsignaSealExpo>> SetSealAssignsExpo(AsignaSealExpo objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            RespuestaViewModel<AsignaSealExpo> respuesta = new RespuestaViewModel<AsignaSealExpo>();
            try
            {
                List<ParametroRegistrarSelloFoto> fotos = new List<ParametroRegistrarSelloFoto>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }


                HttpClientHandler handler = new HttpClientHandler();
                ParametroRegistrarSealAssignsExpo _params = new ParametroRegistrarSealAssignsExpo()
                {
                    container = objSeal.container?.ToUpper(),
                    sello_CGSA = objSeal.sello_CGSA?.ToUpper(),
                    sello1 = objSeal.sello1?.ToUpper(),
                    sello2 = objSeal.sello2?.ToUpper(),
                    sello3 = objSeal.sello3?.ToUpper(),
                    sello4 = objSeal.sello4?.ToUpper(),
                    ip = objSeal.ip,
                    Create_user = objSeal.usuarioCrea,

                    gkey = objSeal.gkey,
                    Fotos = fotos
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/asignaSealExpo");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarSealAssignsExpo>("asignaSealExpo", _params);

                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<AsignaSealExpo>>(readTask);
                        return respuesta;
                    }

                    return respuesta;
                }
            }
            catch
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas"));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                return respuesta;
            }
        }

        /**************************************************************************************************************************************************
       * Registrar Validacion de sello en Patio
       **************************************************************************************************************************************************/
        public async Task<RespuestaViewModel<ValidaSealPatio>> SetSealValidationYard(ValidaSealPatio objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            RespuestaViewModel<ValidaSealPatio> respuesta = new RespuestaViewModel<ValidaSealPatio>();
            try
            {
                List<ParametroRegistrarSelloFoto> fotos = new List<ParametroRegistrarSelloFoto>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarSelloFoto
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = objSeal.usuarioCrea
                    });
                }


                HttpClientHandler handler = new HttpClientHandler();
                ParametroRegistrarSealValidationYard _params = new ParametroRegistrarSealValidationYard()
                {
                    container = objSeal.container?.ToUpper(),
                    sello_CGSA = objSeal.sello_CGSA?.ToUpper(),
                    ip = objSeal.ip,
                    Create_user = objSeal.usuarioCrea,

                    gkey = objSeal.gkey,
                    Fotos = fotos
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/validaSealPatio");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarSealValidationYard>("validaSealPatio", _params);

                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<ValidaSealPatio>>(readTask);
                        return respuesta;
                    }

                    return respuesta;
                }
            }
            catch
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas"));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
                return respuesta;
            }
        }


        //##############################
        // VBS BANANO - INVENTARIO
        //##############################
        public async Task<RespuestaViewModel<List<BAN_Stowage_Plan_Aisv>>> AisvInProgresses(string _estado, string _aisv, long? _idStowageDet)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Stowage_Plan_Aisv>> respuesta = new RespuestaViewModel<List<BAN_Stowage_Plan_Aisv>>();
            try
            {

                ParametroConsultarStowagePlanAisv _params = new ParametroConsultarStowagePlanAisv()
                {
                    estado = _estado,
                    aisv = _aisv,
                    idStowageDet = _idStowageDet
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_AISV");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarStowagePlanAisv>("VBS_lista_AISV", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Stowage_Plan_Aisv>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<BAN_Stowage_Movimiento>>> AisvRecepcionInProgresses(long _idStowageAisv)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Stowage_Movimiento>> respuesta = new RespuestaViewModel<List<BAN_Stowage_Movimiento>>();
            try
            {
                ParametroConsultarRecepcionAisv _params = new ParametroConsultarRecepcionAisv()
                {
                    idStowageAisv = _idStowageAisv
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_Lista_Recepciones");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarRecepcionAisv>("VBS_Lista_Recepciones", _params).ConfigureAwait(false);

                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Stowage_Movimiento>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Stowage_Plan_Aisv>> GetStowage_Plan_Aisv(long _idStowageAisv)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Plan_Aisv> respuesta = new RespuestaViewModel<BAN_Stowage_Plan_Aisv>();
            try
            {
                ParametroConsultarStowagePlanAisvXId _rec = new ParametroConsultarStowagePlanAisvXId()
                {
                    id = _idStowageAisv,
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_getStowagePlanAisvPorId");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarStowagePlanAisvXId>("VBS_getStowagePlanAisvPorId", _rec).ConfigureAwait(false); ;
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Plan_Aisv>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<BAN_Catalogo_Modalidad>>> GetModalidad()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Catalogo_Modalidad>> respuesta = new RespuestaViewModel<List<BAN_Catalogo_Modalidad>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_lista_modalidades", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Catalogo_Modalidad>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Stowage_Movimiento>> RegistersMovimiento(BAN_Stowage_Movimiento oMovimiento, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            try
            {
                List<ParametroRegistrarRecepcionFotoAisv> Foto = new List<ParametroRegistrarRecepcionFotoAisv>();
                List<ParametroRegistrarRecepcionFotoAisv> fotos = new List<ParametroRegistrarRecepcionFotoAisv>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                ParametroRegistrarRecepcionAisv _params = new ParametroRegistrarRecepcionAisv()
                {
                    Fotos = fotos,
                    idStowageAisv = oMovimiento.idStowageAisv,
                    idModalidad = oMovimiento.idModalidad,
                    tipo = oMovimiento.tipo,
                    cantidad = oMovimiento.cantidad,
                    observacion = oMovimiento.observacion,
                    estado = oMovimiento.estado,
                    Create_user = oMovimiento.usuarioCrea
                };

                using (var client = new HttpClient(handler,false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_save_recepcion");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarRecepcionAisv>("VBS_save_recepcion", _params).ConfigureAwait(false);

                    string json = JsonConvert.SerializeObject(_params);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Movimiento>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Stowage_Movimiento>> GetReccepcionPorBarcode(string _barcode)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            try
            {
                ParametroConsultarRecepcionAisvPorBarcode _rec = new ParametroConsultarRecepcionAisvPorBarcode()
                {
                    barcode = _barcode,
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_get_recepcionPorBarcode");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarRecepcionAisvPorBarcode>("VBS_get_recepcionPorBarcode", _rec).ConfigureAwait(false);
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Movimiento>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;
        }
        public async Task<RespuestaViewModel<BAN_Stowage_Movimiento>> GetReccepcionPorId(long _id)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            try
            {
                ParametroConsultarRecepcionAisvPorId _rec = new ParametroConsultarRecepcionAisvPorId()
                {
                    Id = _id,
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_getRecepcionPorId");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarRecepcionAisvPorId>("VBS_getRecepcionPorId", _rec).ConfigureAwait(false);
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Movimiento>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Stowage_Movimiento>> UpdateMovimiento(BAN_Stowage_Movimiento oRecepcion)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            try
            {
                ParametroActualizarRecepcionAisv _params = new ParametroActualizarRecepcionAisv()
                {
                    idStowageAisv = oRecepcion.idStowageAisv,
                    idMovimiento = oRecepcion.idMovimiento,
                    tipo = oRecepcion.tipo,
                    idUbicacion = oRecepcion.idUbicacion,
                    Modifie_user = oRecepcion.usuarioModifica,
                    isMix = oRecepcion.isMix,
                    referencia = oRecepcion.referencia
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_update_recepcion");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroActualizarRecepcionAisv>("VBS_update_recepcion", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Movimiento>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Stowage_Movimiento>> AnularMovimiento(BAN_Stowage_Movimiento oRecepcion)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            try
            {
                ParametroAnularRecepcionAisv _params = new ParametroAnularRecepcionAisv()
                {
                    idMovimiento = oRecepcion.idMovimiento,
                    Modifie_user = oRecepcion.usuarioModifica
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_anular_recepcion");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroAnularRecepcionAisv>("VBS_anular_recepcion", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Movimiento>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Catalogo_Profundidad>>> GetProfundidad(int _idBodega, int _idBloque, int _idFila, int _idAltura)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Catalogo_Profundidad>> respuesta = new RespuestaViewModel<List<BAN_Catalogo_Profundidad>>();
            try
            {
                ParametroConsultarSlotDisponible _rec = new ParametroConsultarSlotDisponible()
                {
                    idBodega = _idBodega,
                    idBloque = _idBloque,
                    idFila = _idFila,
                    idAltura = _idAltura
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_profundidad_disponible");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarSlotDisponible>("VBS_lista_profundidad_disponible", _rec).ConfigureAwait(false);
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Catalogo_Profundidad>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Catalogo_Ubicacion>> GetUbicacionPorBarcode(string _barcode)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Catalogo_Ubicacion> respuesta = new RespuestaViewModel<BAN_Catalogo_Ubicacion>();
            try
            {
                ParametroConsultarUbicacionPorBarcode _rec = new ParametroConsultarUbicacionPorBarcode()
                {
                    barcode = _barcode,
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_get_ubicacionPorBarcode");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarUbicacionPorBarcode>("VBS_get_ubicacionPorBarcode", _rec).ConfigureAwait(false);
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Catalogo_Ubicacion>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;
        }

        //Orden Despacho
        public async Task<RespuestaViewModel<List<BAN_Stowage_Movimiento>>> GetListaMovimientos(string _idNave, string _idBodega, string _idExportador, string _booking, string _barcode)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Stowage_Movimiento>> respuesta = new RespuestaViewModel<List<BAN_Stowage_Movimiento>>();
            try
            {
                int? _bodega;
                int? _exportador;

                if (string.IsNullOrEmpty(_idBodega)) {_bodega = null;} else {_bodega = int.Parse(_idBodega);}
                if (string.IsNullOrEmpty(_idExportador)) { _exportador = null; } else { _exportador = int.Parse(_idExportador); }

                if (string.IsNullOrEmpty(_idNave)) { _idNave = null; } 
                if (string.IsNullOrEmpty(_booking)) { _booking = null; } 
                if (string.IsNullOrEmpty(_barcode)) { _barcode = null; }


                ParametroConsultarCheckLoad _params = new ParametroConsultarCheckLoad()
                {
                    idNave = _idNave,
                    idBodega = _bodega,
                    idExportador = _exportador,
                    booking = _booking,
                    barcode = _barcode
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_carga_enBodega");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarCheckLoad>("VBS_carga_enBodega", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Stowage_Movimiento>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Stowage_Movimiento>>> GetListaMovimientosParaDespacho(string _idNave, string _idExportador, string _idBloque)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Stowage_Movimiento>> respuesta = new RespuestaViewModel<List<BAN_Stowage_Movimiento>>();
            try
            {
                int? _bloque;
                int? _exportador;

                if (string.IsNullOrEmpty(_idBloque)) { _bloque = null; } else { _bloque = int.Parse(_idBloque); }
                if (string.IsNullOrEmpty(_idExportador)) { _exportador = null; } else { _exportador = int.Parse(_idExportador); }

                if (string.IsNullOrEmpty(_idNave)) { _idNave = null; }

                ParametroConsultarCheckLoad _params = new ParametroConsultarCheckLoad()
                {
                    idNave = _idNave,
                    idExportador = _exportador,
                    idBloque = _bloque
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_carga_enBodegaParaDespacho");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarCheckLoad>("VBS_carga_enBodegaParaDespacho", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Stowage_Movimiento>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Stowage_Movimiento>> GetMovimientoParaDespacho(string _idNave, string _idExportador, string _idBloque, string _booking)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            try
            {
                int? _bloque;
                int? _exportador;

                if (string.IsNullOrEmpty(_idBloque)) { _bloque = null; } else { _bloque = int.Parse(_idBloque); }
                if (string.IsNullOrEmpty(_idExportador)) { _exportador = null; } else { _exportador = int.Parse(_idExportador); }

                if (string.IsNullOrEmpty(_idNave)) { _idNave = null; }

                ParametroConsultarCheckLoad _params = new ParametroConsultarCheckLoad()
                {
                    idNave = _idNave,
                    idExportador = _exportador,
                    idBloque = _bloque,
                    booking = _booking
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_carga_enBodegaParaDespachoEsp");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarCheckLoad>("VBS_carga_enBodegaParaDespachoEsp", _params).ConfigureAwait(false); ;
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Movimiento>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaNaves()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_lista_naves", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaBodegas(string _idNave)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                ParametroConsultarListaBodegaXNave _params = new ParametroConsultarListaBodegaXNave()
                {
                    idNave = _idNave,
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_bodega");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarListaBodegaXNave>("VBS_lista_bodega", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaBloques(string _idNave)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                ParametroConsultarListaBodegaXNave _params = new ParametroConsultarListaBodegaXNave()
                {
                    idNave = _idNave,
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_bloque");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarListaBodegaXNave>("VBS_lista_bloque", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaExportadoresPorNaveBodega(string _idNave, int? _idBodega)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                ParametroConsultarListaExportador _params = new ParametroConsultarListaExportador()
                {
                    idNave = _idNave,
                    idBodega = _idBodega
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_exportadorPorNaveBodega");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarListaExportador>("VBS_lista_exportadorPorNaveBodega", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaExportadoresPorNave(string _idNave)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                ParametroConsultarListaExportador _params = new ParametroConsultarListaExportador()
                {
                    idNave = _idNave,
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_exportadorPorNave");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarListaExportador>("VBS_lista_exportadorPorNave", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>> OrdenesDespachoInProgresses(string _idNave, int _idExportador, int _idBloque, string _booking)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> respuesta = new RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>();
            try
            {
                ParametroConsultarOrdenDespacho _params = new  ParametroConsultarOrdenDespacho()
                {
                    idNave = _idNave,
                    idExportador = _idExportador,
                    idBloque = _idBloque,
                    booking = _booking
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_ordenesDespacho");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarOrdenDespacho>("VBS_lista_ordenesDespacho", _params).ConfigureAwait(false);

                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<BAN_Stowage_OrdenDespacho>> RegistersOrdenDespacho(BAN_Stowage_OrdenDespacho oOrdenDespacho)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_OrdenDespacho> respuesta = new RespuestaViewModel<BAN_Stowage_OrdenDespacho>();
            try
            {
                List<ParametroRegistrarRecepcionFotoAisv> Foto = new List<ParametroRegistrarRecepcionFotoAisv>();
                List<ParametroRegistrarRecepcionFotoAisv> fotos = new List<ParametroRegistrarRecepcionFotoAisv>();



                ParametroRegistrarDespachoVBS _params = new ParametroRegistrarDespachoVBS()
                {
                    idNave = oOrdenDespacho.idNave,
                    idExportador = oOrdenDespacho.idExportador,
                    idBodega = oOrdenDespacho.idBodega,
                    idBloque = oOrdenDespacho.idBloque,
                    cantidadPalets = oOrdenDespacho.cantidadPalets,
                    cantidadBox = oOrdenDespacho.cantidadBox,
                    arrastre = oOrdenDespacho.arrastre,
                    pendiente = oOrdenDespacho.pendiente,
                    estado = oOrdenDespacho.estado,
                    Create_user = oOrdenDespacho.usuarioCrea,
                    booking = oOrdenDespacho.booking
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_save_ordenDespacho");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarDespachoVBS>("VBS_save_ordenDespacho", _params).ConfigureAwait(false);

                    string json = JsonConvert.SerializeObject(_params);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_OrdenDespacho>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<BAN_Stowage_OrdenDespacho>> AnulaOrdenDespacho(BAN_Stowage_OrdenDespacho oOrdenDespacho)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_OrdenDespacho> respuesta = new RespuestaViewModel<BAN_Stowage_OrdenDespacho>();
            try
            {
                List<ParametroRegistrarRecepcionFotoAisv> Foto = new List<ParametroRegistrarRecepcionFotoAisv>();
                List<ParametroRegistrarRecepcionFotoAisv> fotos = new List<ParametroRegistrarRecepcionFotoAisv>();



                ParametroAnularDespachoVBS _params = new ParametroAnularDespachoVBS()
                {
                    idOrdenDespacho = long.Parse(oOrdenDespacho.idOrdenDespacho.ToString()),
                    estado = oOrdenDespacho.estado,
                    Modifie_user = oOrdenDespacho.usuarioModifica
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_anula_ordenDespacho");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroAnularDespachoVBS>("VBS_anula_ordenDespacho", _params).ConfigureAwait(false);

                    string json = JsonConvert.SerializeObject(_params);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_OrdenDespacho>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        //pre-despacho
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaBodegasPreDespacho()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_lista_bodegasPorOrdenesDespacho", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>> GetListaOrdenesDespacho(string _idBodega)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> respuesta = new RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>();
            try
            {

                ParametroConsultarPreDespacho _params = new ParametroConsultarPreDespacho()
                {
                    idBodega = int.Parse(_idBodega)
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_ordenesDespachoPorBodega");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarPreDespacho>("VBS_lista_ordenesDespachoPorBodega", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>> GetListaOrdenesDespacho()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> respuesta = new RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>();
            try
            {

                ParametroConsultarPreDespacho _params = new ParametroConsultarPreDespacho()
                {
                    idBodega = null
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_ordenesDespachoGeneral");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarPreDespacho>("VBS_lista_ordenesDespachoGeneral", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_FilasPorOrden>>> GetListaDetOrdenesAgrupadaXFila(long _idOrdenDespacho)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_FilasPorOrden>> respuesta = new RespuestaViewModel<List<BAN_Consulta_FilasPorOrden>>();
            try
            {
                ParametroConsultarFilasPreDespacho _params = new ParametroConsultarFilasPreDespacho()
                {
                    idOrdenDespacho = _idOrdenDespacho
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_ordenesDetAgrupada");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarFilasPreDespacho>("VBS_lista_ordenesDetAgrupada", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_FilasPorOrden>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<BAN_Stowage_Movimiento>>> GetListaMovimientosXFila(long _idOrdenDespacho, int _idFila)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Stowage_Movimiento>> respuesta = new RespuestaViewModel<List<BAN_Stowage_Movimiento>>();
            try
            {
                ParametroConsultarMovimientosFilas _params = new ParametroConsultarMovimientosFilas()
                {
                    idOrdenDespacho = _idOrdenDespacho,
                    idFila = _idFila
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_movimientoPorFilaYOrden");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarMovimientosFilas>("VBS_lista_movimientoPorFilaYOrden", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Stowage_Movimiento>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Stowage_Movimiento>> RegistersPreDespacho(long idMovimiento, string user)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            try
            {
                ParametroRegistrarPreDespachoVBS _params = new ParametroRegistrarPreDespachoVBS()
                {
                    idMovimiento = idMovimiento,
                    Create_user = user
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_save_preDespacho");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarPreDespachoVBS>("VBS_save_preDespacho", _params).ConfigureAwait(false);

                    string json = JsonConvert.SerializeObject(_params);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Movimiento>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Stowage_OrdenDespacho>> GetOrdenDespacho(long _id)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_OrdenDespacho> respuesta = new RespuestaViewModel<BAN_Stowage_OrdenDespacho>();
            try
            {
                ParametroConsultarFilasPreDespacho _params = new ParametroConsultarFilasPreDespacho()
                {
                    idOrdenDespacho = _id
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_getOrdenDespacho");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarFilasPreDespacho>("VBS_getOrdenDespacho", _params).ConfigureAwait(false); ;
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_OrdenDespacho>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<BAN_Stowage_Plan_Aisv>> GetBAN_Stowage_Plan_AisvXBooking(string booking)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Plan_Aisv> respuesta = new RespuestaViewModel<BAN_Stowage_Plan_Aisv>();
            try
            {
                ParametroConsultarAisvPorBooking _params = new ParametroConsultarAisvPorBooking()
                {
                    booking = booking
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_getStowage_Plan_AisvXBooking");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarAisvPorBooking>("VBS_getStowage_Plan_AisvXBooking", _params).ConfigureAwait(false); ;
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Plan_Aisv>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }

        //despacho
        public async Task<RespuestaViewModel<BAN_Stowage_Movimiento>> RegistraDespachoVBS(BAN_Stowage_Movimiento oMovimiento, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Movimiento> respuesta = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            try
            {
                List<ParametroRegistrarRecepcionFotoAisv> Foto = new List<ParametroRegistrarRecepcionFotoAisv>();
                List<ParametroRegistrarRecepcionFotoAisv> fotos = new List<ParametroRegistrarRecepcionFotoAisv>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                ParametroRegistrarRecepcionAisv _params = new ParametroRegistrarRecepcionAisv()
                {
                    Fotos = fotos,
                    idMovimiento = oMovimiento.idMovimiento,
                    idStowageAisv = oMovimiento.idStowageAisv,
                    idModalidad = oMovimiento.idModalidad,
                    tipo = oMovimiento.tipo,
                    cantidad = oMovimiento.cantidad,
                    observacion = oMovimiento.observacion,
                    estado = oMovimiento.estado,
                    Create_user = oMovimiento.usuarioCrea
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_registra_despacho");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarRecepcionAisv>("VBS_registra_despacho", _params).ConfigureAwait(false);

                    string json = JsonConvert.SerializeObject(_params);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Movimiento>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        //EMBARQUE
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaNavesVBS()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_embarque_lista_naves", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaExportadoresVBS()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_embarque_lista_exportadores", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaHolds()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_embarque_lista_hold", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaDecks()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_embarque_lista_decks", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaOrigen()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_embarque_lista_origen", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaBrands(string _idExportador)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {

                ParametroConsultarEmbarqueInbox _params = new ParametroConsultarEmbarqueInbox()
                {
                    idExportador = _idExportador
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_embarque_lista_brands");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarEmbarqueInbox>("VBS_embarque_lista_brands", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaTipoMovimiento()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_embarque_lista_tipoMovimiento", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Catalogo_Modalidad>>> GetModalidadEmbarque()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Catalogo_Modalidad>> respuesta = new RespuestaViewModel<List<BAN_Catalogo_Modalidad>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_lista_modalidadesEmbarque", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Catalogo_Modalidad>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Embarque_Cab>>> GetListaOrdenesEmbarque(string _idNave, string _idExportador)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Embarque_Cab>> respuesta = new RespuestaViewModel<List<BAN_Embarque_Cab>>();
            try
            {

                ParametroConsultarEmbarqueInbox _params = new ParametroConsultarEmbarqueInbox()
                {
                    idNave = _idNave,
                    idExportador = _idExportador
                };
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_embarque_lista_inbox");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarEmbarqueInbox>("VBS_embarque_lista_inbox", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Embarque_Cab>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<BAN_Embarque_Cab>> RegistersEmbarqueInbox(BAN_Embarque_Cab oEntidad, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Embarque_Cab> respuesta = new RespuestaViewModel<BAN_Embarque_Cab>();
            try
            {
                List<ParametroRegistrarRecepcionFotoAisv> Foto = new List<ParametroRegistrarRecepcionFotoAisv>();
                List<ParametroRegistrarRecepcionFotoAisv> fotos = new List<ParametroRegistrarRecepcionFotoAisv>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = oEntidad.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = oEntidad.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = oEntidad.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoAisv
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = oEntidad.usuarioCrea
                    });
                }

                ParametroRegistrarEmbarqueVBS _params = new ParametroRegistrarEmbarqueVBS()
                {
                    //Fotos = fotos,
                    barcode = oEntidad.barcode,
                    idNave = oEntidad.idNave,
                    nave = oEntidad.nave,
                    idExportador = oEntidad.idExportador,
                    Exportador = oEntidad.Exportador,
                    estado = oEntidad.estado,
                    Create_user = oEntidad.usuarioCrea
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_embarque_save");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarEmbarqueVBS>("VBS_embarque_save", _params).ConfigureAwait(false);

                    string json = JsonConvert.SerializeObject(_params);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Embarque_Cab>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<BAN_Embarque_Cab>> GetEmbarqueCab(long _id)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Embarque_Cab> respuesta = new RespuestaViewModel<BAN_Embarque_Cab>();
            try
            {
                ParametroConsultarEmbarqueCab _params = new ParametroConsultarEmbarqueCab()
                {
                    id = _id
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_embarque_inbox_PorId");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var postTask = client.PostAsJsonAsync<ParametroConsultarEmbarqueCab>("VBS_embarque_inbox_PorId", _params).ConfigureAwait(false); ;
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Embarque_Cab>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;

            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Embarque_Movimiento>>> GetListaMovimientosEmbarque(long _idEmbarqueCab)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Embarque_Movimiento>> respuesta = new RespuestaViewModel<List<BAN_Embarque_Movimiento>>();
            try
            {
                ParametroConsultarEmbarqueMovimiento _params = new ParametroConsultarEmbarqueMovimiento()
                {
                    id = _idEmbarqueCab
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_embarque_lista_movimientos");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarEmbarqueMovimiento>("VBS_embarque_lista_movimientos", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Embarque_Movimiento>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<BAN_Embarque_Movimiento>> RegistersMovimientoEmbarque(BAN_Embarque_Movimiento oMovimiento, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Embarque_Movimiento> respuesta = new RespuestaViewModel<BAN_Embarque_Movimiento>();
            try
            {
                List<ParametroRegistrarRecepcionFotoEmbarque> Foto = new List<ParametroRegistrarRecepcionFotoEmbarque>();
                List<ParametroRegistrarRecepcionFotoEmbarque> fotos = new List<ParametroRegistrarRecepcionFotoEmbarque>();

                if (photo1 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoEmbarque
                    {
                        ruta = string.Empty,
                        foto = photo1,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                if (photo2 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoEmbarque
                    {
                        ruta = string.Empty,
                        foto = photo2,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                if (photo3 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoEmbarque
                    {
                        ruta = string.Empty,
                        foto = photo3,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                if (photo4 != null)
                {
                    fotos.Add(new ParametroRegistrarRecepcionFotoEmbarque
                    {
                        ruta = string.Empty,
                        foto = photo4,
                        estado = "NUE",
                        Create_user = oMovimiento.usuarioCrea
                    });
                }

                ParametroRegistrarRecepcionEmbarque _params = new ParametroRegistrarRecepcionEmbarque()
                {
                    Fotos = fotos,
                    idEmbarqueMovimiento = oMovimiento.idEmbarqueMovimiento,
                    idEmbarqueCab = oMovimiento.idEmbarqueCab,
                    codigoCaja = oMovimiento.codigoCaja,
                    idHold = oMovimiento.idHold == 0? null : oMovimiento.idHold,
                    idPiso = oMovimiento.idPiso == 0 ? null : oMovimiento.idPiso,
                    idMarca = oMovimiento.idMarca == 0 ? null : oMovimiento.idMarca,
                    idModalidad = oMovimiento.idModalidad == 0 ? null : oMovimiento.idModalidad,
                    box = oMovimiento.box,
                    tipo = oMovimiento.tipo,
                    idtipoMovimiento = oMovimiento.idtipoMovimiento,
                    comentario = oMovimiento.comentario,
                    estado = oMovimiento.estado,
                    Create_user = oMovimiento.usuarioCrea,
                    Modifie_user = oMovimiento.usuarioModifica
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_embarque_movimiento_save");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarRecepcionEmbarque>("VBS_embarque_movimiento_save", _params).ConfigureAwait(false);

                    string json = JsonConvert.SerializeObject(_params);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Embarque_Movimiento>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<VHSOrdenTrabajo>>> GetOrdenesTrabajoAsync()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<VHSOrdenTrabajo>> respuesta = new RespuestaViewModel<List<VHSOrdenTrabajo>>();
            var _params = new string[] { };
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/vhs_ordenes_trabajo");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    
                    var postTask = client.PostAsJsonAsync("vhs_ordenes_trabajo", new { }).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<VHSOrdenTrabajo>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        #region "VHS"
        public async Task<RespuestaViewModel<VHSTarjaModel>> RegistrarTarjaAsync(ParametroVHSCreaTarja tarja)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<VHSTarjaModel> respuesta = new RespuestaViewModel<VHSTarjaModel>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/vhs_registraTarja");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("vhs_registraTarja", tarja).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<VHSTarjaModel>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<VHSTarjaMensaje>>> GetTarjaPendienteAsync(bool filtrar, int idOrden)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<VHSTarjaMensaje>> respuesta = new RespuestaViewModel<List<VHSTarjaMensaje>>();
            var _params = new ParametroVHSListaTarjaPendiente()
            {
                Filtrar = filtrar,
                OrdenTrabajoId = idOrden,
            };
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/vhs_tarjas");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("vhs_tarjas", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<VHSTarjaMensaje>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<VHSTarjaMensaje>>> GetTarjaDetailAsync(int idOrdenTrabajo)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<VHSTarjaMensaje>> respuesta = new RespuestaViewModel<List<VHSTarjaMensaje>>();
            ParametroVHSTarjaDetalle parametro = new ParametroVHSTarjaDetalle()
            {
                OrdenTrabajoId = idOrdenTrabajo,
            };
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/vhs_tarjaDetalle");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("vhs_tarjaDetalle", parametro).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<VHSTarjaMensaje>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<VHSTarjaDetalleMensaje>> AddTarjaDetailAsync(ParametroVHSTarjaDetalleAdd parametro)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<VHSTarjaDetalleMensaje> respuesta = new RespuestaViewModel<VHSTarjaDetalleMensaje>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/vhs_tarjaAgregaDetalle");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("vhs_tarjaAgregaDetalle", parametro).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<VHSTarjaDetalleMensaje>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<ApiModels.AppModels.AppModelVHSTarjaDetalle>> GetTarjaDetailByIdAsync(long detalleTarjaId)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<ApiModels.AppModels.AppModelVHSTarjaDetalle> respuesta = new RespuestaViewModel<ApiModels.AppModels.AppModelVHSTarjaDetalle>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {

                    client.BaseAddress = new Uri(Baseurl + "api/vhs_tarjaDetalle");

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync($"api/vhs_tarjaConsultaDetalle?detalleTarjaId={detalleTarjaId}");
                    var responseContent = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"Respuesta del servidor: {responseContent}");

                    if (response.IsSuccessStatusCode)
                    {
                        // Deserializar directamente al modelo esperado
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<ApiModels.AppModels.AppModelVHSTarjaDetalle>>(responseContent);
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {response.StatusCode} - {response.ReasonPhrase} - Detalle: {responseContent}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Excepción: {ex.Message}" }
                };
                System.Diagnostics.Debug.WriteLine($"Excepción: {ex.Message} - StackTrace: {ex.StackTrace}");
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<VHSTarjaDetalleMensaje>> GetTarjaDetailsById(long detalleTarjaId)
        {
            RespuestaViewModel<VHSTarjaDetalleMensaje> returnValue = new RespuestaViewModel<VHSTarjaDetalleMensaje>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync($"api/vhs_tarjaConsultaDetalle?detalleTarjaId={detalleTarjaId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        returnValue = JsonConvert.DeserializeObject<RespuestaViewModel<VHSTarjaDetalleMensaje>>(content);
                    }
                    else
                    {
                        returnValue.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {response.StatusCode} - {response.ReasonPhrase}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { ex.Message }
                };
            }
            return returnValue;
        }
        public async Task<byte[]> GetImageAsync(string filePath)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    var response = await client.GetAsync($"api/vhs_getImage?filePath={filePath}");
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsByteArrayAsync();
                    }
                }
            }
            catch
            {
                // Log error si es necesario
            }
            return null;
        }
        public async Task<RespuestaViewModel<VHSTarjaDetalleMensaje>> UpdateTarjaDetailAsync(ParametroVHSTarjaDetalleUpdate parametro)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<VHSTarjaDetalleMensaje> respuesta = new RespuestaViewModel<VHSTarjaDetalleMensaje>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/vhs_actualizarDetalleTarja");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = await client.PostAsJsonAsync("vhs_actualizarDetalleTarja", parametro);
                    var readTask = await postTask.Content.ReadAsStringAsync();

                    // Verificar si el mensaje específico está presente
                    string specificMessage = "No se pudo actualizar el detalle de tarja, Error: No se actualizó ningún registro para el DetalleTarjaId proporcionado";
                    if (readTask.Contains(specificMessage))
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = true, // Considerado como éxito según tu solicitud
                            Titulo = "Actualización exitosa",
                            TipoMensaje = Enumerados.TipoMensaje.Exito,
                            Mensajes = new List<string> { "La tarja se actualizó correctamente." }
                        };
                    }
                    else if (readTask.Contains("\"Respuesta\":false"))
                    {
                        // Extraer el mensaje de error de "Resultado":{"Mensajes":["..."]}
                        int startIndex = readTask.IndexOf("\"Mensajes\":") + "\"Mensajes\":".Length + 2; // +2 para saltar [" y "
                        int endIndex = readTask.IndexOf("\"]", startIndex);
                        if (startIndex > 0 && endIndex > startIndex)
                        {
                            string mensajeError = readTask.Substring(startIndex, endIndex - startIndex).Trim('"');
                            respuesta.Resultado = new ResultadoViewModel
                            {
                                Respuesta = false,
                                Titulo = "Error",
                                TipoMensaje = Enumerados.TipoMensaje.Error,
                                Mensajes = new List<string> { mensajeError }
                            };
                        }
                        else
                        {
                            // Fallback si no se puede extraer el mensaje
                            respuesta.Resultado = new ResultadoViewModel
                            {
                                Respuesta = false,
                                Titulo = "Error",
                                TipoMensaje = Enumerados.TipoMensaje.Error,
                                Mensajes = new List<string> { "Ocurrió un error al procesar la solicitud." }
                            };
                        }
                    }
                    else
                    {
                        // Asumir éxito si no hay "Respuesta": false ni el mensaje específico
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = true,
                            Titulo = "Actualización exitosa",
                            TipoMensaje = Enumerados.TipoMensaje.Exito,
                            Mensajes = new List<string> { "La tarja se actualizó correctamente." }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Error interno: {ex.Message}" }
                };
                throw; // Re-lanzar la excepción para que el llamador la maneje si es necesario
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<VHSBloque[]>> GetBloquesAsync()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<VHSBloque[]> respuesta = new RespuestaViewModel<VHSBloque[]>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/vhs_listaBloques");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync("vhs_listaBloques");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<VHSBloque[]>>(content);
                        // Verificar si Respuesta es null y asignar un array vacío si es necesario
                        if (respuesta.Respuesta == null)
                        {
                            respuesta.Respuesta = new VHSBloque[0];
                            respuesta.Resultado = new ResultadoViewModel
                            {
                                Respuesta = false,
                                Titulo = "Advertencia",
                                TipoMensaje = Enumerados.TipoMensaje.Advertencia,
                                Mensajes = new List<string> { "No se encontraron bloques." }
                            };
                        }
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {response.StatusCode} - {response.ReasonPhrase}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Error al conectar con la API: {ex.Message}" }
                };
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<VHSMensajeSimple>>> GetVehiculosDespachoAsync(long paseId)
        {
            RespuestaViewModel<List<VHSMensajeSimple>> respuesta = new RespuestaViewModel<List<VHSMensajeSimple>>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var result = await client.GetAsync($"api/vhs_listaVehiculosDespacho?paseId={paseId}");

                    if (result.IsSuccessStatusCode)
                    {
                        var json = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<VHSMensajeSimple>>>(json);
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = ViewModel.Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {result.ReasonPhrase}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = ViewModel.Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Excepción: {ex.Message}" }
                };
            }

            return respuesta;
        }
        public async Task<RespuestaViewModel<long>> RegistraEvidenciaEntregaAsync(ParametroVHSCrearEvidenciaEntrega parametro)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<long> respuesta = new RespuestaViewModel<long>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/vhs_registraEvidenciaEntrega");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    var jsonString = JsonConvert.SerializeObject(parametro, Formatting.Indented);
                    Console.WriteLine(jsonString);
                    var postTask = client.PostAsJsonAsync("vhs_registraEvidenciaEntrega", parametro).ConfigureAwait(false);
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var json = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<long>>(json);
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string>
                    {
                        $"Error: {result.StatusCode} - {result.ReasonPhrase}"
                    }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string>
            {
                $"Excepción: {ex.Message}"
            }
                };
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<int>> RegistrarNovedadDetalleTarjaAsync(ParametroRegistrarNovedadDetalleTarja parametro)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<int> respuesta = new RespuestaViewModel<int>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/vhs_registrarNovedadDetalleTarja");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("vhs_registrarNovedadDetalleTarja", parametro).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<int>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { ex.Message }
                };
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<TipoNovedadModel[]>> GetTiposNovedadAsync()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<TipoNovedadModel[]> respuesta = new RespuestaViewModel<TipoNovedadModel[]>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync("api/vhs_listaTiposNovedad");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<TipoNovedadModel[]>>(content);

                        // Verificar si Respuesta es null y asignar un array vacío si es necesario
                        if (respuesta.Respuesta == null)
                        {
                            respuesta.Respuesta = new TipoNovedadModel[0];
                            respuesta.Resultado = new ResultadoViewModel
                            {
                                Respuesta = false,
                                Titulo = "Advertencia",
                                TipoMensaje = Enumerados.TipoMensaje.Advertencia,
                                Mensajes = new List<string> { "No se encontraron tipos de novedad." }
                            };
                        }
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {response.StatusCode} - {response.ReasonPhrase}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Error al conectar con la API: {ex.Message}" }
                };
            }
            return respuesta;
        }
        #endregion



        #region "CEDI"
        public async Task<RespuestaViewModel<List<CediOrdenTrabajo>>> GetCediOrdenesTrabajoAsync()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<CediOrdenTrabajo>> respuesta = new RespuestaViewModel<List<CediOrdenTrabajo>>();
            var _params = new string[] { };
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/cedi_ordenes_trabajo");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("cedi_ordenes_trabajo", new { }).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<CediOrdenTrabajo>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<CediTarjaModel>> RegistrarCediTarjaAsync(ParametroVHSCreaTarja tarja)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<CediTarjaModel> respuesta = new RespuestaViewModel<CediTarjaModel>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/cedi_registraTarja");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("cedi_registraTarja", tarja).ConfigureAwait(false);
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<CediTarjaModel>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<CediTarjaMensaje>>> GetCediTarjaPendienteAsync(bool filtrar, int idOrden)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<CediTarjaMensaje>> respuesta = new RespuestaViewModel<List<CediTarjaMensaje>>();
            var _params = new ParametroVHSListaTarjaPendiente()
            {
                Filtrar = filtrar,
                OrdenTrabajoId = idOrden,
            };
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/cedi_tarjas");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("cedi_tarjas", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<CediTarjaMensaje>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<CediTarjaMensaje>>> GetCediTarjaDetailAsync(int idOrdenTrabajo)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<CediTarjaMensaje>> respuesta = new RespuestaViewModel<List<CediTarjaMensaje>>();
            ParametroVHSTarjaDetalle parametro = new ParametroVHSTarjaDetalle()
            {
                OrdenTrabajoId = idOrdenTrabajo,
            };
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/cedi_tarjaDetalle");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("cedi_tarjaDetalle", parametro).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<CediTarjaMensaje>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<CediTarjaDetalleMensaje>> AddCediTarjaDetailAsync(ParametroVHSTarjaDetalleAdd parametro)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<CediTarjaDetalleMensaje> respuesta = new RespuestaViewModel<CediTarjaDetalleMensaje>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/cedi_tarjaAgregaDetalle");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("cedi_tarjaAgregaDetalle", parametro).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<CediTarjaDetalleMensaje>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<AppModelCediTarjaDetalle>> GetCediTarjaDetailByIdAsync(long detalleTarjaId)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<AppModelCediTarjaDetalle> respuesta = new RespuestaViewModel<AppModelCediTarjaDetalle>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {

                    client.BaseAddress = new Uri(Baseurl );

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //comene
                    var response = await client.GetAsync($"api/cedi_tarjaConsultaDetalle?detalleTarjaId={detalleTarjaId}");
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<AppModelCediTarjaDetalle>>(responseContent);
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {response.StatusCode} - {response.ReasonPhrase} - Detalle: {responseContent}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Excepción: {ex.Message}" }
                };
            }
            return respuesta;
        }
        private AppModelVHSTarjaDetalle MapToAppModel(ApiModels.AppModels.AppModelVHSTarjaDetalle apiModel)
        {
            if (apiModel == null) return null;

            return new AppModelVHSTarjaDetalle
            {
                DetalleTarjaID = apiModel.DetalleTarjaID,
                TarjaID = apiModel.TarjaID,
                TipoCargaDescripcion = apiModel.TipoCargaDescripcion,
                InformacionVehiculo = apiModel.InformacionVehiculo,
                UbicacionBodega = apiModel.UbicacionBodega,
                DocumentoTransporte = apiModel.DocumentoTransporte,
                PackingList = apiModel.PackingList,
                VIN = apiModel.VIN,
                NumeroMotor = apiModel.NumeroMotor,
                Observaciones = apiModel.Observaciones,
                Fotos = apiModel.Fotos?.Select(f => new VHSTarjaDetalleFoto
                {
                    FotoID = f.FotoID,
                    DetalleTarjaID = f.DetalleTarjaID,
                    FotosVehiculo = f.FotosVehiculo,
                    ArrayFoto = f.ArrayFoto,
                    Orden = f.Orden
                }).ToList()
            };
        }
        public async Task<RespuestaViewModel<CediTarjaDetalleMensaje>> GetCediTarjaDetailsById(int detalleTarjaId)
        {
            RespuestaViewModel<CediTarjaDetalleMensaje> returnValue = new RespuestaViewModel<CediTarjaDetalleMensaje>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync($"api/cedi_tarjaConsultaDetalle?detalleTarjaId={detalleTarjaId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        returnValue = JsonConvert.DeserializeObject<RespuestaViewModel<CediTarjaDetalleMensaje>>(content);
                    }
                    else
                    {
                        returnValue.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {response.StatusCode} - {response.ReasonPhrase}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { ex.Message }
                };
            }
            return returnValue;
        }
        public async Task<RespuestaViewModel<CediTarjaDetalleMensaje>> UpdateCediTarjaDetailAsync(ParametroVHSTarjaDetalleUpdate parametro)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<CediTarjaDetalleMensaje> respuesta = new RespuestaViewModel<CediTarjaDetalleMensaje>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/cedi_actualizarDetalleTarja");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = await client.PostAsJsonAsync("cedi_actualizarDetalleTarja", parametro);
                    var readTask = await postTask.Content.ReadAsStringAsync();

                    string specificMessage = "No se pudo actualizar el detalle de tarja, Error: No se actualizó ningún registro para el DetalleTarjaId proporcionado";
                    if (readTask.Contains(specificMessage))
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = true,
                            Titulo = "Actualización exitosa",
                            TipoMensaje = Enumerados.TipoMensaje.Exito,
                            Mensajes = new List<string> { "La tarja se actualizó correctamente." }
                        };
                    }
                    else if (readTask.Contains("\"Respuesta\":false"))
                    {
                        int startIndex = readTask.IndexOf("\"Mensajes\":") + "\"Mensajes\":".Length + 2;
                        int endIndex = readTask.IndexOf("\"]", startIndex);
                        if (startIndex > 0 && endIndex > startIndex)
                        {
                            string mensajeError = readTask.Substring(startIndex, endIndex - startIndex).Trim('"');
                            respuesta.Resultado = new ResultadoViewModel
                            {
                                Respuesta = false,
                                Titulo = "Error",
                                TipoMensaje = Enumerados.TipoMensaje.Error,
                                Mensajes = new List<string> { mensajeError }
                            };
                        }
                        else
                        {
                            respuesta.Resultado = new ResultadoViewModel
                            {
                                Respuesta = false,
                                Titulo = "Error",
                                TipoMensaje = Enumerados.TipoMensaje.Error,
                                Mensajes = new List<string> { "Ocurrió un error al procesar la solicitud." }
                            };
                        }
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = true,
                            Titulo = "Actualización exitosa",
                            TipoMensaje = Enumerados.TipoMensaje.Exito,
                            Mensajes = new List<string> { "La tarja se actualizó correctamente." }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Error interno: {ex.Message}" }
                };
                throw;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<CediBloque[]>> GetCediBloquesAsync()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<CediBloque[]> respuesta = new RespuestaViewModel<CediBloque[]>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/cedi_listaBloques");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync("cedi_listaBloques");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<CediBloque[]>>(content);
                        if (respuesta.Respuesta == null)
                        {
                            respuesta.Respuesta = new CediBloque[0];
                            respuesta.Resultado = new ResultadoViewModel
                            {
                                Respuesta = false,
                                Titulo = "Advertencia",
                                TipoMensaje = Enumerados.TipoMensaje.Advertencia,
                                Mensajes = new List<string> { "No se encontraron bloques." }
                            };
                        }
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {response.StatusCode} - {response.ReasonPhrase}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Error al conectar con la API: {ex.Message}" }
                };
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<CediMensajeSimple>>> GetCediVehiculosDespachoAsync(long paseId)
        {
            RespuestaViewModel<List<CediMensajeSimple>> respuesta = new RespuestaViewModel<List<CediMensajeSimple>>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var result = await client.GetAsync($"api/cedi_listaVehiculosDespacho?paseId={paseId}");

                    if (result.IsSuccessStatusCode)
                    {
                        var json = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<CediMensajeSimple>>>(json);
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = ViewModel.Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {result.ReasonPhrase}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = ViewModel.Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Excepción: {ex.Message}" }
                };
            }

            return respuesta;
        }
        public async Task<RespuestaViewModel<long>> RegistraCediEvidenciaEntregaAsync(ParametroCediCrearEvidenciaEntrega parametro)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<long> respuesta = new RespuestaViewModel<long>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/cedi_registraEvidenciaEntrega");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("cedi_registraEvidenciaEntrega", parametro).ConfigureAwait(false);
                    var result = await postTask;

                    if (result.IsSuccessStatusCode)
                    {
                        var json = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<long>>(json);
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {result.StatusCode} - {result.ReasonPhrase}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Excepción: {ex.Message}" }
                };
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<int>> RegistrarCediNovedadDetalleTarjaAsync(ParametroRegistrarNovedadDetalleTarja parametro)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<int> respuesta = new RespuestaViewModel<int>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/cedi_registrarNovedadDetalleTarja");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync("cedi_registrarNovedadDetalleTarja", parametro).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<int>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { ex.Message }
                };
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<TipoNovedadModel[]>> GetCediTiposNovedadAsync()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<TipoNovedadModel[]> respuesta = new RespuestaViewModel<TipoNovedadModel[]>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync("api/cedi_listaTiposNovedad");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<TipoNovedadModel[]>>(content);

                        if (respuesta.Respuesta == null)
                        {
                            respuesta.Respuesta = new TipoNovedadModel[0];
                            respuesta.Resultado = new ResultadoViewModel
                            {
                                Respuesta = false,
                                Titulo = "Advertencia",
                                TipoMensaje = Enumerados.TipoMensaje.Advertencia,
                                Mensajes = new List<string> { "No se encontraron tipos de novedad." }
                            };
                        }
                    }
                    else
                    {
                        respuesta.Resultado = new ResultadoViewModel
                        {
                            Respuesta = false,
                            Titulo = "Error",
                            TipoMensaje = Enumerados.TipoMensaje.Error,
                            Mensajes = new List<string> { $"Error: {response.StatusCode} - {response.ReasonPhrase}" }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Error al conectar con la API: {ex.Message}" }
                };
            }
            return respuesta;
        }
        #endregion

        
        //AISV EXTERNO
        public async Task<RespuestaViewModel<BAN_Stowage_Plan_Det>> RegistersAisvExterno(string _idNave, int _idHold, int _idBodega, int _idBloque, string _aisv, string user)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<BAN_Stowage_Plan_Det> respuesta = new RespuestaViewModel<BAN_Stowage_Plan_Det>();
            try
            {
                ParametroRegistrarAisvExternoVBS _params = new ParametroRegistrarAisvExternoVBS()
                {
                    idNave = _idNave,
                    idHold = _idHold,
                    idBodega = _idBodega,
                    idBloque = _idBloque,
                    aisv = _aisv,
                    Create_user = user
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_registra_aisv_externo");
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroRegistrarAisvExternoVBS>("VBS_registra_aisv_externo", _params).ConfigureAwait(false);

                    string json = JsonConvert.SerializeObject(_params);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<BAN_Stowage_Plan_Det>>(readTask);
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaNavesST()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_lista_navesST", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaBodegasVBS()
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/VBS_lista_bodegasVBS", HttpCompletionOption.ResponseContentRead).ConfigureAwait(true);
                    if (Res.IsSuccessStatusCode)
                    {
                        var empResponse = Res.Content.ReadAsStringAsync().Result;
                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(empResponse);
                    }
                }
            }
            catch (Exception e)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + e.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }

        public async Task<RespuestaViewModel<List<BAN_Consulta_Combo>>> GetListaBloquesVBS(int _idBodega)
        {
            HttpClientHandler handler = new HttpClientHandler();
            RespuestaViewModel<List<BAN_Consulta_Combo>> respuesta = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                ParametroConsultarPreDespacho _params = new ParametroConsultarPreDespacho()
                {
                    idBodega = _idBodega,
                };

                using (var client = new HttpClient(handler, false))
                {
                    client.BaseAddress = new Uri(Baseurl + "api/VBS_lista_bloquesVBS");
                    client.DefaultRequestHeaders.Clear();
                    //client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var postTask = client.PostAsJsonAsync<ParametroConsultarPreDespacho>("VBS_lista_bloquesVBS", _params).ConfigureAwait(false);
                    var result = await postTask;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsStringAsync();

                        respuesta = JsonConvert.DeserializeObject<RespuestaViewModel<List<BAN_Consulta_Combo>>>(readTask);
                    }
                    else
                    {
                        respuestaVie = new ResultadoViewModel();
                        respuestaVie.Respuesta = false;
                        respuestaVie.Titulo = "Error";
                        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                        List<String> mensaje = new List<string>();
                        mensaje.Add(string.Format("Error: " + result.StatusCode.ToString() + " El servidor retorna mensaje: " + result.ReasonPhrase.ToString()));
                        respuestaVie.Mensajes = mensaje;
                        respuesta.Resultado = respuestaVie;
                    }
                }
            }
            catch (Exception ex)
            {
                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("Error al conectarse con el servidor - Comunicarse con el área de Sistemas: " + ex.Message));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }
            return respuesta;
        }
    }
}
