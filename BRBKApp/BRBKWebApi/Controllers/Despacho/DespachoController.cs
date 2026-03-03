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
using System.Xml.Linq;
using System.ServiceModel;
using BRBKWebApi.ServicioAduana;

namespace MiWebApi.Controllers.Despacho
{
    public class DespachoController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpPost]
        [Route("api/consulta_pase")]
        [ValidateModelAttribute]
        public RespuestaViewModel<pasePuerta> GetPasePuerta([FromBody] ParametrosDespacho.ParametrosConsultaPasePuerta pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            pasePuerta ObjPasePuerta = new pasePuerta();
            RespuestaViewModel<pasePuerta> respuesta = new RespuestaViewModel<pasePuerta>();

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entity = pasePuertaDA.GetPasePuerta(pObj.numeroPase);
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el número del pase de puerta : {0}", pObj.numeroPase));
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = 1;

                        ObjPasePuerta = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Pase Puerta";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjPasePuerta ?? new pasePuerta();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetPasePuerta), "api/consulta_recepcion", false, null, null, ex.StackTrace, ex);

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
        [Route("api/registra_despacho")]
        [ValidateModelAttribute]
        public RespuestaViewModel<despacho> RegistraDespacho([FromBody] ParametrosDespacho.ParametrosRegistraDespacho pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<despacho> respuesta = new RespuestaViewModel<despacho>();
            despacho ObjTarea = new despacho();
            string v_resultado = string.Empty;
            string v_solicitud = string.Empty;
            string v_respuesta = string.Empty;

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    //valida que no exista el despacho del BL
                    var objDespacho = despachoDA.listadoDespachos(pTarea.idTarjaDet, out OnError);
                    var objDesp = objDespacho.Where(a => a.pase == pTarea.pase).ToList();
                    if (OnError != string.Empty)
                    {
                        Mensaje.Add(string.Format("Error al validar despacho, No se pudo grabar la transacción con pase:{0} - {1}", pTarea.pase, OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (objDesp.Count > 0)
                        {
                            Mensaje.Add(string.Format("No se pudo grabar el despacho del pase:{0} - {1}", pTarea.pase, "Ya se ha realizado el despacho"));
                            Valido = false;
                        }
                        else
                        {
                            var n4 = N4Ws.Entidad.Servicios.DESPACHO_LOADTRUCK(string.Format("{0}-{1}-{2}", pTarea.mrn, pTarea.msn, pTarea.hsn), pTarea.placa, ((int)pTarea.cantidad).ToString(), pTarea.Create_user);

                            if (!n4.Exitoso)
                            {
                                string CuerpoMensaje = string.Format("Se presentaron los siguientes problemas DESPACHO_LOADTRUCK: {0}", n4.MensajeProblema);
                                Mensaje.Add(string.Format("{0}", CuerpoMensaje));
                                Valido = false;
                            }
                            else
                            {
                                //###############
                                //  GENERA SMDT
                                //###############
                                string V_SCOPE = recepcionDA.GetConfiguracion("APP", "SMDT_SCOPE");
                                string V_URL = recepcionDA.GetConfiguracion("APP", "SW_ECUAPASS");
                                string V_VALOR_GENERA_SMDT = recepcionDA.GetConfiguracion("APP", "SMDT_GENERAR").ToString().Trim();
                                bool V_GENERAR_SMDT = V_VALOR_GENERA_SMDT=="1"? true:false;
                                string v_mensaje = string.Empty;
                                string v_smdt_xml = string.Empty;
                                var bultos = ((int)pTarea.cantidad).ToString();
                                var peso = 1;
                                var carga = string.Format("{0}-{1}-{2}", pTarea.mrn, pTarea.msn, pTarea.hsn);
                                var numero_pase_n4 = pTarea.delivery;

                                var binding = new BasicHttpBinding
                                {
                                    MaxBufferPoolSize = 2147483647,
                                    MaxBufferSize = 2147483647,
                                    MaxReceivedMessageSize = 2147483647
                                };
                                binding.ReaderQuotas.MaxArrayLength = 2147483647;
                                binding.ReaderQuotas.MaxDepth = 2147483647;
                                binding.ReaderQuotas.MaxStringContentLength = 2147483647;

                                if (V_GENERAR_SMDT)
                                {
                                    var wsSMDT = new n4ServiceSoapClient(binding, new EndpointAddress(V_URL));

                                    XElement xmlDoc = (new XElement("smdt",
                                                        new XElement("tipo", "B"),
                                                        new XElement("usuario", pTarea.Create_user),
                                                        new XElement("validar", "1"),
                                                        new XElement("parametros",
                                                            new XElement("pase", numero_pase_n4),
                                                            new XElement("bl", carga),
                                                            new XElement("bultos", bultos),
                                                            new XElement("peso", Convert.ToInt64(peso))
                                                           )
                                                           ));
                                    XElement n4Client = XElement.Parse(wsSMDT.basicInvoke(V_SCOPE, xmlDoc.ToString()));
                                    v_smdt_xml = xmlDoc.ToString();
                                    var Code = n4Client.Descendants("Code").FirstOrDefault().Value;

                                    if (Code == "0")
                                    {
                                        var ticket = n4Client.Descendants("ticket").FirstOrDefault().Value;
                                        v_mensaje = " - SMDT Generado exitosamente,\n" + ticket;
                                        
                                    }
                                    else
                                    {
                                        var errors = n4Client.Descendants("errors").FirstOrDefault();
                                        var error = errors.Element("error").Value;
                                        var mensaje = " - Error al Generar SMDT,\n" + error;
                                        v_mensaje = mensaje;
                                    }
                                }

                                despacho eDespacho = new despacho();
                                eDespacho.idTarjaDet = pTarea.idTarjaDet;
                                eDespacho.pase = pTarea.pase;
                                eDespacho.mrn = pTarea.mrn;
                                eDespacho.msn = pTarea.msn;
                                eDespacho.hsn = pTarea.hsn;
                                eDespacho.placa = pTarea.placa;
                                eDespacho.idchofer = pTarea.idchofer;
                                eDespacho.chofer = pTarea.chofer;
                                eDespacho.cantidad = pTarea.cantidad;
                                eDespacho.observacion = pTarea.observacion + " - " + n4?.MensajeInformacion + v_mensaje;
                                eDespacho.estado = "NUE";
                                eDespacho.usuarioCrea = pTarea.Create_user;
                                eDespacho.usuarioModifica = null;
                                eDespacho.fechaCreacion = System.DateTime.Now;
                                eDespacho.fechaModifica = null;
                                eDespacho.delivery = pTarea.delivery;
                                eDespacho.PRE_GATE_ID = pTarea.PRE_GATE_ID;
                                eDespacho.SMDT_xml = v_smdt_xml;

                                List<fotoDespacho> oFoto = new List<fotoDespacho>();
                                eDespacho.Fotos = oFoto;

                                foreach (ParametrosDespacho.ParametrosRegistraFotoDespacho oParam in pTarea.Fotos)
                                {
                                    v_ruta = string.Empty;
                                    Carga_Imagenes(oParam.foto, out v_ruta);
                                    oParam.ruta = v_ruta;
                                    eDespacho.Fotos.Add(new fotoDespacho
                                    {
                                        idDespacho = 0,
                                        ruta = oParam.ruta,
                                        estado = oParam.estado,
                                        usuarioCrea = oParam.Create_user
                                    });
                                }

                                despachoDA oDespacho = new despachoDA();
                                var result = oDespacho.Save_Update(eDespacho, out OnError);

                                IdGenerado = result;

                                if (IdGenerado.HasValue)
                                {
                                    Mensaje.Add(string.Format("Se registro con éxito el despacho: [{0}], {1} {2} ", IdGenerado.Value, n4?.MensajeInformacion, v_mensaje));
                                    Valido = true;

                                    Int64 Id = IdGenerado.Value;

                                    eDespacho.idDespacho = Id;
                                    eDespacho.tarjaDet = tarjaDetDA.GetTarjaDet(pTarea.idTarjaDet);
                                    ObjTarea = eDespacho;
                                }
                                else
                                {
                                    Mensaje.Add(string.Format("No se pudo grabar el despacho del pase:{0} - {1}", pTarea.pase, OnError));
                                    Valido = false;
                                }
                            }
                        }
                       
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar Despacho";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new despacho();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraDespacho), "api/registra_despacho", false, null, null, ex.StackTrace, ex);

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
        [Route("api/load_truck_n4")]
        [ValidateModelAttribute]
        public RespuestaViewModel<bool> LoadTruckAutomaticoN4([FromBody] ParametrosDespacho.ParametrosLoadTruckN4 pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<bool> respuesta = new RespuestaViewModel<bool>();
          
            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    //valida que no exista el despacho del BL
                    if (OnError != string.Empty)
                    {
                        Mensaje.Add(string.Format("Error al validar despacho, No se pudo grabar la transacción con pase:{0} - {1}", pTarea.pase, OnError));
                        Valido = false;
                    }
                    else
                    {
                        var n4 = N4Ws.Entidad.Servicios.DESPACHO_LOADTRUCK(pTarea.bl, pTarea.placa, ((int)pTarea.cantidad).ToString(), pTarea.Create_user);

                        if (!n4.Exitoso)
                        {
                            string CuerpoMensaje = string.Format("Se presentaron los siguientes problemas DESPACHO_LOADTRUCK: {0}", n4.MensajeProblema);
                            Mensaje.Add(string.Format("{0}", CuerpoMensaje));
                            Valido = false;
                        }
                        else
                        {
                            IdGenerado = 1;
                            Mensaje.Add(string.Format("Se registro con éxito el despacho en N4: {0} ",n4?.MensajeInformacion));
                            Valido = true;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Generar Load Truck N4";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = Valido ;
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraDespacho), "api/registra_despacho", false, null, null, ex.StackTrace, ex);

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

        
    }
}