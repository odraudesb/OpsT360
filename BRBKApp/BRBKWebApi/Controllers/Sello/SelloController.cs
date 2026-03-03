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
using MiWebApi;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace BRBKWebApi.Controllers.Sello
{
    public class SelloController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = 1;

        ///////////////////////////////////////
        //VALIDA SELLO DE CONTENEDORES VACIOS 
        ///////////////////////////////////////
        [HttpPost]
        [Route("api/validar_sello")]
        [ValidateModelAttribute]
        public RespuestaViewModel<sealValidation> GetSeal([FromBody] ParametrosSeal.ParametrosConsultaSello pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            sealValidation ObjSeal = new sealValidation();
            RespuestaViewModel<sealValidation> respuesta = new RespuestaViewModel<sealValidation>();
            string CuerpoMensaje = string.Empty;
            string v_request = string.Empty;
            string v_response = string.Empty;

            try
            {
                string V_HOLD = recepcionDA.GetConfiguracion("APP", "HOLD");

                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entitys = workPositionDA.GetDataContainersExpo(pObj.container.ToUpper());
                    if (Entitys != null) { pObj.container = Entitys?.cntr; }


                    sealValidation oSello = new sealValidation();
                    oSello.container = pObj.container.ToUpper();
                    oSello.seals = pObj.seal.ToUpper();
                    oSello.addposition = pObj.addPosition;
                    oSello.position = pObj.position?.ToUpper();
                    oSello.idWorkPosition = pObj.idWorkPosition;
                    oSello.xmlN4 = pObj.xmlN4;
                    oSello.respuestaN4 = pObj.respuestaN4;
                    oSello.referencia = pObj.referencia;
                    oSello.grua = pObj.grua;
                    oSello.gkey = pObj.gkey;
                    oSello.bloqueo = pObj.bloqueo;
                    oSello.usuarioCrea = pObj.Create_user.ToUpper();

                    List<fotoSealValidation> oFoto = new List<fotoSealValidation>();
                    oSello.Fotos = oFoto;

                    foreach (ParametrosSeal.ParametrosRegistraFotoSello oParam in pObj.Fotos)
                    {
                        v_ruta = string.Empty;
                        Carga_Imagenes(oParam.foto, out v_ruta);
                        oParam.ruta = v_ruta;
                        oSello.Fotos.Add(new fotoSealValidation
                        {
                            idSealValidation = 0,
                            ruta = oParam.ruta,
                            estado = oParam.estado,
                            usuarioCrea = oParam.Create_user
                        });
                    }

                    ////////////////////////////////////
                    //SE VALIDA EL SELLO POR CONTENEDOR 
                    ////////////////////////////////////
                    var Entity = sealDA.GetSello(oSello);
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información del sello para el contenedor:{0}", pObj.container.Trim()));
                        Valido = false;
                    }
                    else
                    {
                        if (Entity.pregate == -3)
                        {
                            Mensaje.Add(string.Format("No existe información del sello para el contenedor:{0}", pObj.container.Trim()));
                            Valido = false;
                        }
                        else
                        {
                            IdGenerado = Entity.id;

                            if (pObj.addPosition)
                            {
                                //////////////////////////////////////////////////////////////////
                                //SE VALIDA SI EL USUARIO TIENE EL POW REGISTRADO PREVIAMENTE
                                //////////////////////////////////////////////////////////////////
                                var oPOW = workPositionDA.GetWorkPositionXUser(pObj.Create_user);


                                ///////////////////////////////////////////////////////////////////////////
                                //SE LLAMA A GROOVY DE N4 PARA REALIZAR LA TRANSACCIÓN DE POSICIONAMIENTO
                                ///////////////////////////////////////////////////////////////////////////
                                if (oPOW != null)
                                {
                                    pObj.referencia = oPOW.idPosition;
                                    pObj.grua = oPOW.namePosition;
                                    pObj.idWorkPosition = oPOW.id;

                                    ///////////////////////////////////////////////////////////////////////
                                    //SI LA UNIDAD SE ENCUENTRA CON BLOQUEO SE LLAMA AL ICU DE DESBLOQUEO
                                    ///////////////////////////////////////////////////////////////////////
                                    if (pObj.bloqueo)
                                    {
                                        var ICU_N4 = N4Ws.Entidad.Servicios.N4ICU_HOLD(pObj.container, true, pObj.Create_user, V_HOLD);
                                        if (ICU_N4.status_id != "OK")
                                        {
                                            pObj.respuestaN4 = ICU_N4.response.ToString();
                                            CuerpoMensaje = string.Format("No se logró quitár los bloqueos para el contenedor:{0}-{1}", pObj.container.Trim(), ICU_N4.response.ToString());
                                            Valido = false;
                                        }
                                    }

                                    /////////////////////////////////////////////////////
                                    //SE REALIZA EL EMBARQUE POSITION DE LA UNIDAD EN N4
                                    /////////////////////////////////////////////////////
                                    if (Valido)
                                    {
                                        var n4 = N4Ws.Entidad.Servicios.POSITION_UNITLOAD(pObj.grua, pObj.referencia, pObj.container, pObj.position, pObj.seal, pObj.Create_user, out v_request, out v_response);
                                        pObj.xmlN4 = v_request;
                                        pObj.respuestaN4 = v_response;
                                        if (!n4.Exitoso)
                                        {
                                            CuerpoMensaje = string.Format("Se presentaron los siguientes problemas POSITION_UNITLOAD: {0}", n4.MensajeProblema);
                                            pObj.respuestaN4 = pObj.respuestaN4 + " - " + CuerpoMensaje;
                                        }
                                        else
                                        {
                                            pObj.respuestaN4 = pObj.respuestaN4 + " - " + n4.MensajeInformacion;
                                            CuerpoMensaje = n4.MensajeInformacion;
                                        }
                                    }
                                }
                                else
                                {
                                    pObj.idWorkPosition = null;
                                    pObj.xmlN4 = string.Empty;
                                    pObj.respuestaN4 = "NO SE ENCONTRO NINGUN POW HABILITADO PARA EL USER: " + pObj.Create_user;
                                    CuerpoMensaje = pObj.respuestaN4;
                                }

                                ///////////////////////////////////////////////////////////
                                //ACTUALIZA REGISTRO DE GRABADO EN LA TABLA SEALVALIDATION
                                ///////////////////////////////////////////////////////////
                                Entity.container = pObj.container.ToUpper();
                                Entity.seals = pObj.seal.ToUpper();
                                Entity.addposition = pObj.addPosition;
                                Entity.position = pObj.position.ToUpper();
                                Entity.idWorkPosition = pObj.idWorkPosition;
                                Entity.xmlN4 = pObj.xmlN4;
                                Entity.respuestaN4 = pObj.respuestaN4;
                                Entity.referencia = pObj.referencia;
                                Entity.grua = pObj.grua;
                                Entity.usuarioCrea = pObj.Create_user.ToUpper();
                                var oSelloUpdate = sealDA.UpdateSello(Entity, out OnError);

                                if (!string.IsNullOrEmpty(OnError))
                                {
                                    CuerpoMensaje = CuerpoMensaje + " - Error al Actualizar SelloValidation:" + OnError;
                                }
                            }

                            Mensaje.Add(string.Format("Sello validado con éxito: [{0}] - {1}", IdGenerado.Value, CuerpoMensaje.Trim()));
                            Valido = true;
                            IdGenerado = (long)Entity.id;
                            ObjSeal = Entity;
                        }
                    }

                }
                IdGenerado = 1;
                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Seal";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjSeal ?? new sealValidation();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetSeal), "api/validar_sello", false, null, null, ex.StackTrace, ex);

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

        //////////////////////////////////////////
        //REGISTRO DE ASIGNACIÓN DE SELLO MUELLE 
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/registra_sealMuelle")]
        [ValidateModelAttribute]
        public RespuestaViewModel<sealAsignacionMuelle> RegistraSealMuelle([FromBody] ParametrosSeal.ParametrosRegistraAsignacionSello pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            string v_request = string.Empty;
            string v_response = string.Empty;
            string CuerpoMensaje = string.Empty;

            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<sealAsignacionMuelle> respuesta = new RespuestaViewModel<sealAsignacionMuelle>();
            sealAsignacionMuelle ObjTarea = new sealAsignacionMuelle();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entitys = workPositionDA.GetDataContainersImpo(pTarea.container.ToUpper());
                    if (Entitys != null) { pTarea.container = Entitys?.cntr; }

                    sealAsignacionMuelle eSeal = new sealAsignacionMuelle();
                    eSeal.id = 0;
                    eSeal.container = pTarea.container;
                    eSeal.sello_CGSA = pTarea.sello_CGSA;
                    eSeal.sello1 = pTarea.sello1;
                    eSeal.sello2 = pTarea.sello2;
                    eSeal.sello3 = pTarea.sello3;
                    eSeal.sello4 = pTarea.sello4;
                    eSeal.color = pTarea.color;
                    eSeal.ip = pTarea.ip;
                    eSeal.usuarioCrea = pTarea.Create_user;
                    eSeal.dataContainer = pTarea.dataContainer;
                    eSeal.position = pTarea.position;
                    eSeal.xmlN4Discharge = pTarea.xmlN4;
                    eSeal.respuestaN4Discharge = pTarea.respuestaN4;

                    List<fotoSealValidation> oFoto = new List<fotoSealValidation>();
                    eSeal.Fotos = oFoto;

                    foreach (ParametrosSeal.ParametrosRegistraFotoSello oParam in pTarea.Fotos)
                    {
                        v_ruta = string.Empty;
                        Carga_Imagenes(oParam.foto, out v_ruta);
                        oParam.ruta = v_ruta;
                        eSeal.Fotos.Add(new fotoSealValidation
                        {
                            id = 0,
                            idSealValidation = 0,
                            ruta = oParam.ruta,
                            estado = oParam.estado,
                            usuarioCrea = oParam.Create_user
                        });
                    }

                    if (!string.IsNullOrEmpty(pTarea.position))
                    {
                        //////////////////////////////////
                        //llama a groovy de descarga
                        //////////////////////////////////
                        var n4 = N4Ws.Entidad.Servicios.DISCHARGE(pTarea.referencia, pTarea.container, pTarea.position, pTarea.Create_user, out v_request, out v_response);
                        eSeal.xmlN4Discharge = v_request;
                        eSeal.respuestaN4Discharge = v_response;

                        if (!n4.Exitoso)
                        {
                            CuerpoMensaje = string.Format("Se presentaron los siguientes problemas DISCHARGE: {0}", n4.MensajeProblema);
                            eSeal.respuestaN4Discharge = eSeal.respuestaN4Discharge + " - " + CuerpoMensaje;
                            //eSeal.estado = false;
                        }
                        else
                        {
                            eSeal.respuestaN4Discharge = eSeal.respuestaN4Discharge + " - " + n4.MensajeInformacion;
                            CuerpoMensaje = n4.MensajeInformacion;
                            // eSeal.estado = true;
                        }
                    }

                    /*  sealMuelleDA oSeal = new sealMuelleDA();
                      var result = oSeal.Save_Update(eSeal, out OnError);

                      if (result != null)
                      {
                          if (string.IsNullOrEmpty(CuerpoMensaje)) { CuerpoMensaje = "CNTR: " + result.container; }
                          if (result.estado)
                          {
                              IdGenerado = 1;
                              Mensaje.Add(string.Format("Registro de Sello con éxito: [{0}], [{1}] - [{2}] ", result.id, result.mensaje, CuerpoMensaje));
                              Valido = true;
                          }
                          else
                          {
                              Mensaje.Add(string.Format("Registro de Sello con novedades: [{0}], [{1}] ", result.mensaje, CuerpoMensaje));
                              Valido = false;
                          }

                          ObjTarea = result;
                      }
                      else
                      {
                          Mensaje.Add(string.Format("No se pudo grabar el registro de sello: {0} - {1}", OnError, CuerpoMensaje));
                          Valido = false;
                      }
                    */


                    //SE OBTIENE GKEY DE CONTENEDOR
                    var oDataContenedor = sealMuelleDA.consultaDataContenedorSealMuelle(eSeal.container);
                    if (oDataContenedor.Rows.Count > 0)
                    {
                        var v_gkey = oDataContenedor.Rows[0][0].ToString();
                        var v_referencia = oDataContenedor.Rows[0][8].ToString();
                        //SE GRABA SELLOS DE MUELLE EN N4
                        string mensaje = string.Empty;
                        bool diferencia = false;
                        eSeal.gkey = long.Parse(v_gkey);
                        eSeal.referencia = v_referencia;
                        try
                        {
                            sealMuelleDA.asignaSealMovil(eSeal, out mensaje, out diferencia, long.Parse(v_gkey));
                        }
                        catch (Exception ex)
                        {
                            mensaje = string.Format("NO SE LOGRÓ REALIZAR EL REGISTRO DEL SEAL; ERROR EN [mty].[SEL_PRO_ASIGNA_SEAL_BER_MOVIL_V2] - {0}", ex.Message);
                            throw new Exception(mensaje);
                        }


                        //SE GRABA SELLOS DE MUELLE EN CONTECON 4.0
                        sealMuelleDA oSeal = new sealMuelleDA();
                        long? resultado = oSeal.Save_Update(eSeal, mensaje, diferencia, out OnError);

                        //GRABA FOTO
                        if (!(resultado is null))
                        {
                            try
                            {
                                string xml = GenerarXml(eSeal.Fotos, long.Parse(resultado.ToString()), eSeal.usuarioCrea);
                                fotoSealMuelleDA oFotoSealMuelle = new fotoSealMuelleDA();
                                var dbItem = oFotoSealMuelle.Save_Update(xml, out OnError);
                                if (!dbItem.HasValue || dbItem.Value < 0)
                                {
                                    oSeal.UpdateStatus(long.Parse(resultado.ToString()), false, "NO SE PUDO GRABAR LAS FOTOS: " + OnError?.ToString(), eSeal.usuarioCrea, out OnError);
                                    OnError = "NO SE PUDO GRABAR LAS FOTOS: " + OnError?.ToString();
                                    resultado = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                oSeal.UpdateStatus(long.Parse(resultado.ToString()), false, "NO SE PUDO GRABAR LAS FOTOS: " + ex.Message?.ToString(), eSeal.usuarioCrea, out OnError);
                                OnError = "NO SE PUDO GRABAR LAS FOTOS: " + ex.Message;
                                resultado = null;
                            }
                        }

                        if (resultado != null)
                        {
                            var result = sealMuelleDA.GetSelloMuellePorId(resultado);

                            if (string.IsNullOrEmpty(CuerpoMensaje)) { CuerpoMensaje = "CNTR: " + result.container; }
                            if (result.estado)
                            {
                                IdGenerado = 1;
                                Mensaje.Add(string.Format("Registro de Sello con éxito: [{0}], [{1}] - [{2}] ", result.id, result.mensaje, CuerpoMensaje));
                                Valido = true;
                            }
                            else
                            {
                                Mensaje.Add(string.Format("Registro de Sello con novedades: [{0}], [{1}] ", result.mensaje, CuerpoMensaje));
                                Valido = false;
                            }

                            ObjTarea = result;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("No se pudo grabar el registro de sello: {0} - {1}", OnError, CuerpoMensaje));
                            Valido = false;
                        }
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se encontró el GKEY de cntr: {0} - {1}", eSeal.container, CuerpoMensaje));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar sello impo muelle";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new sealAsignacionMuelle();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraSealMuelle), "api/registra_sealMuelle", false, null, null, ex.StackTrace, ex);

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

        private string ConvertirListaAFotosXML(List<fotoSealValidation> fotos)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<fotoSealValidation>), new XmlRootAttribute("Fotos"));

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, fotos);
                return writer.ToString();
            }
        }

        public static string GenerarXml(List<fotoSealValidation> fotos, long idSealMuelle, string usuario)
        {
            var xml = new XElement("Fotos",
                fotos.Select(f => new XElement("Foto",
                    new XElement("idSealMuelle", idSealMuelle),
                    new XElement("ruta", f.ruta),
                    new XElement("estado", f.estado),
                    new XElement("usuarioCrea", usuario)
                ))
            );

            return xml.ToString(SaveOptions.DisableFormatting); // sin indentación
        }

        //graba imagenes en el servidor
        private bool Carga_Imagenes(byte[] Foto, out string Nombre)
        {
            try
            {
                //SACO EL NOMBRE DE LA RUTA A GUARDAR
                string RUTA = recepcionDA.GetConfiguracion("APP", "RUTA_IMG_SELLO");//dbContext.ConfigurationSet.Where(x => x.Name.Trim().Equals("RUTA_IMG")).Select(kvp => kvp.Value).FirstOrDefault();

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

        //////////////////////////////////////////
        //REGISTRO DE POSICIÓN DE TRABAJO
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/registra_pow")]
        [ValidateModelAttribute]
        public RespuestaViewModel<workPosition> RegistraPow([FromBody] ParametrosSeal.ParametrosRegistraPOW pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<workPosition> respuesta = new RespuestaViewModel<workPosition>();
            workPosition ObjTarea = new workPosition();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    workPosition ePOW = new workPosition();
                    ePOW.id = 0;
                    ePOW.ip = pTarea.ip;
                    ePOW.imei = pTarea.imei;
                    ePOW.idPosition = pTarea.idPosition;
                    ePOW.namePosition = pTarea.namePosition;
                    ePOW.estado = pTarea.estado;
                    ePOW.validoHasta = DateTime.Now;
                    ePOW.usuarioCrea = pTarea.Create_user;

                    workPositionDA oPOW = new workPositionDA();
                    var result = oPOW.Save_Update(ePOW, out OnError);

                    if (result != null)
                    {
                        ObjTarea.id = result;
                        Mensaje.Add(string.Format("Registro de Posición de trabajo se realizó con éxito: [{0}] ", result));
                        Valido = true;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se pudo grabar la Posición de trabajo: {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar posición de trabajo";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new workPosition();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraPow), "api/registra_pow", false, null, null, ex.StackTrace, ex);

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

        //////////////////////////////////////////
        //LISTA DE POSICIONES DE TRABAJO DESDE N4
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/lista_POWN4")]
        [ValidateModelAttribute]

        public RespuestaViewModel<List<workPositionN4List>> Lista_PowN4([FromBody] ParametrosSeal.ParametrosListaPOWN4 pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<workPositionN4List> query = new List<workPositionN4List>();
            RespuestaViewModel<List<workPositionN4List>> respuesta = new RespuestaViewModel<List<workPositionN4List>>();

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    query = BRBKWebApiData.workPositionDA.GetListaPosicionesN4(out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe Lista de posiciones con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query.Count <= 0 && Valido)
                        {
                            Mensaje.Add(string.Format("No existe  Lista de posiciones con los criterios ingresados."));
                            Valido = false;
                        }
                    }


                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de posiciones";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<workPositionN4List>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_PowN4), "api/lista_POWN4", false, null, null, ex.StackTrace, ex);

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

        //////////////////////////////////////////
        //CONSULTA REGISTRO DE POSICIÓN DE TRABAJO
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/consulta_POW")]
        [ValidateModelAttribute]
        public RespuestaViewModel<workPosition> GetPOW([FromBody] ParametrosSeal.ParametrosConsultaPOW pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            workPosition ObjPOW = new workPosition();
            RespuestaViewModel<workPosition> respuesta = new RespuestaViewModel<workPosition>();

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entity = workPositionDA.GetWorkPosition(pObj.idPosition, pObj.usuarioCrea, pObj.containers);
                    if (Entity?.id == null)
                    {
                        if (Entity?.dataContenedor != null)
                        {
                            Mensaje.Add(string.Format("No existe información del POW para el usuario:{0}", pObj.usuarioCrea));
                            Valido = false;
                            ObjPOW.dataContenedor = Entity?.dataContenedor;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("No existe información del POW para el usuario: {0}", pObj.usuarioCrea));
                            Valido = false;
                        }
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.id;
                        ObjPOW = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjPOW ?? new workPosition();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetPOW), "api/consulta_POW", false, null, null, ex.StackTrace, ex);

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

        //////////////////////////////////////////
        //CONSULTA DATOS DEL CONTENEDOR
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/consulta_Containers_expo")]
        [ValidateModelAttribute]
        public RespuestaViewModel<dataContainers> GetCONTAINERS_EXPO([FromBody] ParametrosSeal.ParametrosGetDataContainers pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            dataContainers ObjPOW = new dataContainers();
            RespuestaViewModel<dataContainers> respuesta = new RespuestaViewModel<dataContainers>();

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entity = workPositionDA.GetDataContainersExpo(pObj.numcontainers);
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información del contenedor:{0}", pObj.numcontainers));
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = 1;

                        ObjPOW = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjPOW ?? new dataContainers();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetPOW), "api/consulta_POW", false, null, null, ex.StackTrace, ex);

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

        //////////////////////////////////////////
        //CONSULTA DATOS DEL CONTENEDOR
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/consulta_Containers_impo")]
        [ValidateModelAttribute]
        public RespuestaViewModel<dataContainers> GetCONTAINERS_IMPO([FromBody] ParametrosSeal.ParametrosGetDataContainers pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            dataContainers ObjPOW = new dataContainers();
            RespuestaViewModel<dataContainers> respuesta = new RespuestaViewModel<dataContainers>();

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entity = workPositionDA.GetDataContainersImpo(pObj.numcontainers);
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información del contenedor:{0}", pObj.numcontainers));
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = 1;

                        ObjPOW = Entity;
                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjPOW ?? new dataContainers();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetPOW), "api/consulta_POW", false, null, null, ex.StackTrace, ex);

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

        //////////////////////////////////////////
        //REGISTRO DE CONFIRMACIÓN DE DESCARGA
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/confirma_Descarga")]
        [ValidateModelAttribute]
        public RespuestaViewModel<downloadConfirmation> RegistraConfirmacionDescarga([FromBody] ParametrosSeal.ParametrosRegistraConfirmacionDescarga pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<downloadConfirmation> respuesta = new RespuestaViewModel<downloadConfirmation>();
            downloadConfirmation ObjTarea = new downloadConfirmation();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entitys = workPositionDA.GetDataContainersImpo(pTarea.container.ToUpper());
                    if (Entitys != null) { pTarea.container = Entitys?.cntr; }

                    string v_request = string.Empty;
                    string v_response = string.Empty;
                    string CuerpoMensaje = string.Empty;

                    downloadConfirmation eDownload = new downloadConfirmation();
                    eDownload.id = 0;
                    eDownload.gkey = pTarea.gkey;
                    eDownload.container = pTarea.container;
                    eDownload.dataContainer = pTarea.dataContainer;
                    eDownload.position = pTarea.position;
                    eDownload.referencia = pTarea.referencia;
                    eDownload.xmlN4 = v_request;
                    eDownload.respuestaN4 = v_response;
                    eDownload.estado = pTarea.estado;
                    eDownload.usuarioCrea = pTarea.Create_user;

                    //llama a groovy de descarga
                    var n4 = N4Ws.Entidad.Servicios.DISCHARGE(pTarea.referencia, pTarea.container, pTarea.position, pTarea.Create_user, out v_request, out v_response);
                    eDownload.xmlN4 = v_request;
                    eDownload.respuestaN4 = v_response;

                    if (!n4.Exitoso)
                    {
                        CuerpoMensaje = string.Format("Se presentaron los siguientes problemas DISCHARGE: {0}", n4.MensajeProblema);
                        eDownload.respuestaN4 = eDownload.respuestaN4 + " - " + CuerpoMensaje;
                        eDownload.estado = false;
                    }
                    else
                    {
                        eDownload.respuestaN4 = eDownload.respuestaN4 + " - " + n4.MensajeInformacion;
                        CuerpoMensaje = n4.MensajeInformacion;
                        eDownload.estado = true;
                    }

                    downloadConfirmationDA oDownloadConfirmation = new downloadConfirmationDA();
                    var result = oDownloadConfirmation.Save_Update(eDownload, out OnError);

                    if (result != null)
                    {
                        ObjTarea = result;

                        if (eDownload.estado)
                        {
                            Mensaje.Add(string.Format("La confirmación de descarga se realizó con éxito: [{0}] - [{1}] ", result.id, CuerpoMensaje));
                            Valido = true;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("Trace: [{0}] - [{1}] ", result.id, CuerpoMensaje));
                            Valido = false;
                        }
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se pudo grabar la confirmación de descarga: {0}", OnError));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar confirmación de descarga";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new downloadConfirmation();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraPow), "api/confirma_Descarga", false, null, null, ex.StackTrace, ex);

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

        //////////////////////////////////////////
        //REGISTRO DE SELLO PRE-EMBARQUE (IMPO)
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/registra_sealPreEmbarque")]
        [ValidateModelAttribute]
        public RespuestaViewModel<sealRegistroPreEmbarqueYaforo> RegistraSealPreEmbarque([FromBody] ParametrosSeal.ParametrosSelloPreEmbarqueYaforo pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            string v_request = string.Empty;
            string v_response = string.Empty;
            string CuerpoMensaje = string.Empty;

            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<sealRegistroPreEmbarqueYaforo> respuesta = new RespuestaViewModel<sealRegistroPreEmbarqueYaforo>();
            sealRegistroPreEmbarqueYaforo ObjTarea = new sealRegistroPreEmbarqueYaforo();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    sealRegistroPreEmbarqueYaforo eSeal = new sealRegistroPreEmbarqueYaforo();
                    eSeal.id = 0;
                    eSeal.container = pTarea.container;
                    eSeal.sello_CGSA = pTarea.sello_CGSA;
                    eSeal.sello1 = pTarea.sello1;
                    eSeal.sello2 = pTarea.sello2;
                    eSeal.sello3 = pTarea.sello3;
                    eSeal.ip = pTarea.ip;
                    eSeal.usuarioCrea = pTarea.Create_user;

                    List<fotoSealValidation> oFoto = new List<fotoSealValidation>();
                    eSeal.Fotos = oFoto;

                    foreach (ParametrosSeal.ParametrosRegistraFotoSello oParam in pTarea.Fotos)
                    {
                        v_ruta = string.Empty;
                        Carga_Imagenes(oParam.foto, out v_ruta);
                        oParam.ruta = v_ruta;
                        eSeal.Fotos.Add(new fotoSealValidation
                        {
                            id = 0,
                            idSealValidation = 0,
                            ruta = oParam.ruta,
                            estado = oParam.estado,
                            usuarioCrea = oParam.Create_user
                        });
                    }

                    sealPreEmbarqueDA oSeal = new sealPreEmbarqueDA();
                    long? resultado = oSeal.Save_Update(eSeal, out OnError);

                    if (resultado != null)
                    {
                        var result = sealPreEmbarqueDA.GetSelloPreEmbarquePorId(resultado);

                        if (string.IsNullOrEmpty(CuerpoMensaje)) { CuerpoMensaje = "CNTR: " + result.container; }
                        if (result.estado)
                        {
                            IdGenerado = 1;
                            Mensaje.Add(string.Format("Registro de Sello con éxito: [{0}], [{1}] - [{2}] ", result.id, result.mensaje, CuerpoMensaje));
                            Valido = true;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("Registro de Sello con novedades: [{0}], [{1}] ", result.mensaje, CuerpoMensaje));
                            Valido = false;
                        }

                        ObjTarea = result;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("No se pudo grabar el registro de sello: {0} - {1}", OnError, CuerpoMensaje));
                        Valido = false;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar sello pre-embarque";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new sealRegistroPreEmbarqueYaforo();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraSealMuelle), "api/registra_sealPreEmbarque", false, null, null, ex.StackTrace, ex);

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

        //////////////////////////////////////////
        //REGISTRO DE SELLO AFORO (IMPO)
        //////////////////////////////////////////
        [HttpPost]
        [Route("api/registra_sealAforo")]
        [ValidateModelAttribute]
        public RespuestaViewModel<sealRegistroPreEmbarqueYaforo> RegistraSealAforo([FromBody] ParametrosSeal.ParametrosSelloPreEmbarqueYaforo pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            string v_request = string.Empty;
            string v_response = string.Empty;
            string CuerpoMensaje = string.Empty;

            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<sealRegistroPreEmbarqueYaforo> respuesta = new RespuestaViewModel<sealRegistroPreEmbarqueYaforo>();
            sealRegistroPreEmbarqueYaforo ObjTarea = new sealRegistroPreEmbarqueYaforo();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    sealRegistroPreEmbarqueYaforo eSeal = new sealRegistroPreEmbarqueYaforo();
                    eSeal.id = 0;
                    eSeal.container = pTarea.container;
                    eSeal.sello_CGSA = pTarea.sello_CGSA;
                    eSeal.sello1 = pTarea.sello1;
                    eSeal.sello2 = pTarea.sello2;
                    eSeal.sello3 = pTarea.sello3;
                    eSeal.ip = pTarea.ip;
                    eSeal.usuarioCrea = pTarea.Create_user;

                    List<fotoSealValidation> oFoto = new List<fotoSealValidation>();
                    eSeal.Fotos = oFoto;

                    bool? v_exigeFoto = sealAforoDA.GetExigeFoto("FSA", out OnError);

                    if (v_exigeFoto == true)
                    {
                        //if (ArrayFoto==null && ArrayFoto1 == null && ArrayFoto2 == null && ArrayFoto3 == null)
                        //{
                        //    await App.Current.MainPage.DisplayAlert("Error", "Enter at least one photo", "Close");
                        //flags = true;
                        //    return;
                        //}

                        if (pTarea.Fotos.Count > 0)
                        {
                            Valido = true;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("Enter at least one photo"));
                            Valido = false;
                        }
                    }

                    if (Valido)
                    {
                        foreach (ParametrosSeal.ParametrosRegistraFotoSello oParam in pTarea.Fotos)
                        {
                            v_ruta = string.Empty;
                            Carga_Imagenes(oParam.foto, out v_ruta);
                            oParam.ruta = v_ruta;
                            eSeal.Fotos.Add(new fotoSealValidation
                            {
                                id = 0,
                                idSealValidation = 0,
                                ruta = oParam.ruta,
                                estado = oParam.estado,
                                usuarioCrea = oParam.Create_user
                            });
                        }

                        sealAforoDA oSeal = new sealAforoDA();
                        long? resultado = oSeal.Save_Update(eSeal, out OnError);

                        if (resultado != null)
                        {
                            var result = sealAforoDA.GetSelloAforoPorId(resultado);

                            if (string.IsNullOrEmpty(CuerpoMensaje)) { CuerpoMensaje = "CNTR: " + result.container; }
                            if (result.estado)
                            {
                                IdGenerado = 1;
                                Mensaje.Add(string.Format("Registro de Sello con éxito: [{0}], [{1}] - [{2}] ", result.id, result.mensaje, CuerpoMensaje));
                                Valido = true;
                            }
                            else
                            {
                                Mensaje.Add(string.Format("Registro de Sello con novedades: [{0}], [{1}] ", result.mensaje, CuerpoMensaje));
                                Valido = false;
                            }

                            ObjTarea = result;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("No se pudo grabar el registro de sello: {0} - {1}", OnError, CuerpoMensaje));
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Registrar sello Aforo";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new sealRegistroPreEmbarqueYaforo();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraSealMuelle), "api/registra_sealAforo", false, null, null, ex.StackTrace, ex);

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

        /////////////////////////////////////////////////////////////
        //ASIGNA SEAL EXPO - VALIDACIÓN DE SELLO CONTENEDOR (IMPO)
        /////////////////////////////////////////////////////////////
        [HttpPost]
        [Route("api/asignaSealExpo")]
        [ValidateModelAttribute]
        public RespuestaViewModel<AsignaSealExpo> AsignacionSealExpo([FromBody] ParametrosSeal.ParametrosSelloAsignaExpo pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            string v_request = string.Empty;
            string v_response = string.Empty;
            string CuerpoMensaje = string.Empty;

            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<AsignaSealExpo> respuesta = new RespuestaViewModel<AsignaSealExpo>();
            AsignaSealExpo ObjTarea = new AsignaSealExpo();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    AsignaSealExpo eSeal = new AsignaSealExpo();
                    eSeal.id = 0;
                    eSeal.container = pTarea.container;
                    eSeal.sello_CGSA = pTarea.sello_CGSA;
                    eSeal.sello1 = pTarea.sello1;
                    eSeal.sello2 = pTarea.sello2;
                    eSeal.sello3 = pTarea.sello3;
                    eSeal.sello4 = pTarea.sello4;
                    eSeal.ip = pTarea.ip;
                    eSeal.usuarioCrea = pTarea.Create_user;

                    List<fotoSealValidation> oFoto = new List<fotoSealValidation>();
                    eSeal.Fotos = oFoto;

                    bool? v_exigeFoto = sealAforoDA.GetExigeFoto("FAS", out OnError);

                    if (v_exigeFoto == true)
                    {
                        //if (ArrayFoto==null && ArrayFoto1 == null && ArrayFoto2 == null && ArrayFoto3 == null)
                        //{
                        //    await App.Current.MainPage.DisplayAlert("Error", "Enter at least one photo", "Close");
                        //flags = true;
                        //    return;
                        //}

                        if (pTarea.Fotos.Count > 0)
                        {
                            Valido = true;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("Enter at least one photo"));
                            Valido = false;
                        }
                    }

                    if (Valido)
                    {
                        foreach (ParametrosSeal.ParametrosRegistraFotoSello oParam in pTarea.Fotos)
                        {
                            v_ruta = string.Empty;
                            Carga_Imagenes(oParam.foto, out v_ruta);
                            oParam.ruta = v_ruta;
                            eSeal.Fotos.Add(new fotoSealValidation
                            {
                                id = 0,
                                idSealValidation = 0,
                                ruta = oParam.ruta,
                                estado = oParam.estado,
                                usuarioCrea = oParam.Create_user
                            });
                        }

                        sealAssignsExpoDA oSeal = new sealAssignsExpoDA();
                        long? resultado = oSeal.Save_Update(eSeal, out OnError);

                        if (resultado != null)
                        {
                            var result = sealAssignsExpoDA.GetSelloPorId(resultado);

                            if (string.IsNullOrEmpty(CuerpoMensaje)) { CuerpoMensaje = "CNTR: " + result.container; }
                            if (result.estado)
                            {
                                IdGenerado = 1;
                                Mensaje.Add(string.Format("Registro de Sello con éxito: [{0}], [{1}] - [{2}] ", result.id, result.mensaje, CuerpoMensaje));
                                Valido = true;
                            }
                            else
                            {
                                Mensaje.Add(string.Format("Registro de Sello con novedades: [{0}], [{1}] ", result.mensaje, CuerpoMensaje));
                                Valido = false;
                            }

                            ObjTarea = result;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("No se pudo grabar el registro de sello: {0} - {1}", OnError, CuerpoMensaje));
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Asigna Sello Expo";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new AsignaSealExpo();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraSealMuelle), "api/registra_sealAforo", false, null, null, ex.StackTrace, ex);

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

        /////////////////////////////////////////////////////////////
        //VALIDA SEAL PATIO (EXPO E IMPO)
        /////////////////////////////////////////////////////////////
        [HttpPost]
        [Route("api/validaSealPatio")]
        [ValidateModelAttribute]
        public RespuestaViewModel<ValidaSealPatio> ValidaSealPatioExpoImpo([FromBody] ParametrosSeal.ParametrosValidaSelloPatio pTarea)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            string v_ruta = string.Empty;
            string v_request = string.Empty;
            string v_response = string.Empty;
            string CuerpoMensaje = string.Empty;

            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            RespuestaViewModel<ValidaSealPatio> respuesta = new RespuestaViewModel<ValidaSealPatio>();
            ValidaSealPatio ObjTarea = new ValidaSealPatio();

            try
            {
                if (pTarea.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    ValidaSealPatio eSeal = new ValidaSealPatio();
                    eSeal.id = 0;
                    eSeal.container = pTarea.container;
                    eSeal.sello_CGSA = pTarea.sello_CGSA;
                    eSeal.ip = pTarea.ip;
                    eSeal.usuarioCrea = pTarea.Create_user;

                    List<fotoSealValidation> oFoto = new List<fotoSealValidation>();
                    eSeal.Fotos = oFoto;

                    bool? v_exigeFoto = sealAforoDA.GetExigeFoto("FVS", out OnError);

                    if (v_exigeFoto == true)
                    {
                        //if (ArrayFoto==null && ArrayFoto1 == null && ArrayFoto2 == null && ArrayFoto3 == null)
                        //{
                        //    await App.Current.MainPage.DisplayAlert("Error", "Enter at least one photo", "Close");
                        //flags = true;
                        //    return;
                        //}

                        if (pTarea.Fotos.Count > 0)
                        {
                            Valido = true;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("Enter at least one photo"));
                            Valido = false;
                        }
                    }

                    if (Valido)
                    {
                        foreach (ParametrosSeal.ParametrosRegistraFotoSello oParam in pTarea.Fotos)
                        {
                            v_ruta = string.Empty;
                            Carga_Imagenes(oParam.foto, out v_ruta);
                            oParam.ruta = v_ruta;
                            eSeal.Fotos.Add(new fotoSealValidation
                            {
                                id = 0,
                                idSealValidation = 0,
                                ruta = oParam.ruta,
                                estado = oParam.estado,
                                usuarioCrea = oParam.Create_user
                            });
                        }

                        sealValidationYardDA oSeal = new sealValidationYardDA();
                        long? resultado = oSeal.Save_Update(eSeal, out OnError);

                        if (resultado != null)
                        {
                            var result = sealValidationYardDA.GetSelloPorId(resultado);

                            if (string.IsNullOrEmpty(CuerpoMensaje)) { CuerpoMensaje = "CNTR: " + result.container; }
                            if (result.estado)
                            {
                                IdGenerado = 1;
                                Mensaje.Add(string.Format("Registro de Sello con éxito: [{0}], [{1}] - [{2}] ", result.id, result.mensaje, CuerpoMensaje));
                                Valido = true;
                            }
                            else
                            {
                                Mensaje.Add(string.Format("Registro de Sello con novedades: [{0}], [{1}] ", result.mensaje, CuerpoMensaje));
                                Valido = false;
                            }

                            ObjTarea = result;
                        }
                        else
                        {
                            Mensaje.Add(string.Format("No se pudo grabar el registro de sello: {0} - {1}", OnError, CuerpoMensaje));
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Valida sello container Impo";
                respuestaVie.TotalRowsCount = IdGenerado != null ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new ValidaSealPatio();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(RegistraSealMuelle), "api/registra_sealAforo", false, null, null, ex.StackTrace, ex);

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