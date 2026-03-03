using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using ModelRcc;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Http.Description;

using static ViewModel.Enumerados;
using ViewModel;
using ReferencialVie = ViewModel;
using BRBKWebApiData;
//using System.Data.Entity;

namespace MiWebApi.Controllers
{
    [ErrorHandlingFilter]
    public class UserController : ApiController
    {
       // private RCCContainer dbContext = new RCCContainer();
        private static Int64? lm = -3;
        private string OnError;

        [HttpPost]
        [Route("api/login")]
        [ValidateModelAttribute]
        public RespuestaViewModel<ResultadoUsuario> Login([FromBody] Login pUser)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            ResultadoUsuario Usuario = new ResultadoUsuario();
            RespuestaViewModel<ResultadoUsuario> respuesta = new RespuestaViewModel<ResultadoUsuario>();

            try
            {

                //si viene nombre de equipo, validamos
                if (!string.IsNullOrEmpty(pUser.Device))
                {
                    //valida equipo
                    //var Device = dbContext.DeviceSet.AsNoTracking().Where(p => p.Imei.Trim().ToUpper().Equals(pUser.Device.Trim().ToUpper())).FirstOrDefault();
                    var Devices = deviceSetDA.listadoDeviceSet(out OnError);
                    var Device = Devices?.Where(p => p.Imei.Trim().ToUpper().Equals(pUser.Device.Trim().ToUpper())).FirstOrDefault();

                    if (Device == null)
                    {
                        Mensaje.Add("Este equipo " + pUser.Device + " no tiene acceso al sistema");
                        Valido = false;
                    }

                }

                //valido usuario y clave
                if (Valido)
                {

                    var Entitys = UserDA.listadoUser(out OnError); //dbContext.Users.AsNoTracking().Where(p => p.Username == pUser.UserName && p.Password == pUser.UserPassword).FirstOrDefault();
                    var Entity = Entitys.Where(p => p.Username.Trim() == pUser.UserName && p.Password.Trim() == pUser.UserPassword).FirstOrDefault();
                    if (Entity == null)
                    {
                        Mensaje.Add("No existe información con el usuario y password ingresado");
                        Valido = false;
                    }
                    else
                    {
                        //SACO EL TIMER PARA ENVIAR UBICACION DEL EQUIPO
                        string PTIMER = string.Empty;//dbContext.ConfigurationSet.AsNoTracking().Where(x => x.Name.Trim().Equals("TIMER")).Select(kvp => kvp.Value).FirstOrDefault();

                        Mensaje.Add("Ok");
                        Valido = true;
                        Usuario.Id = Entity.Id;
                        Usuario.Names = Entity.Names;
                        Usuario.Username = Entity.Username;
                        Usuario.Identification = Entity.Identification;
                        Usuario.PositionId = Entity.PositionId;
                        Usuario.RoleId = Entity.RoleId;
                        Usuario.Create_user = string.IsNullOrEmpty(Entity.Create_user) ? string.Empty : Entity.Create_user.Trim();
                        Usuario.Modifie_user = string.IsNullOrEmpty(Entity.Modifie_user) ? string.Empty : Entity.Modifie_user.Trim();
                        Usuario.Create_date = Entity.Create_date;
                        Usuario.Modifie_date = Entity.Modifie_date;
                        Usuario.Status = Entity.Status;
                        Usuario.CompanyId = Entity.CompanyId;
                        Usuario.Timer = (string.IsNullOrEmpty(PTIMER) ? 5 : Int64.Parse(PTIMER));
                        Usuario.IdUserLeader = Entity.IdUserLeader;
                        Usuario.Email = string.IsNullOrEmpty(Entity.Email) ? string.Empty : Entity.Email.Trim();
                        Usuario.WorkDate = Entity.WorkDate;
                        Usuario.Position = PositionDA.GetPosition(Entity.PositionId);
                        Usuario.Role = rolesDA.GetRol(long.Parse(Entity.RoleId.ToString()));
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Login";
                respuestaVie.TotalRowsCount = Usuario != null && Usuario?.Id > 0 ? Usuario.Id : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = Usuario ?? new ResultadoUsuario();

                //Usuario.RemoveAt(20);
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Login), "api/login", false, null, null, ex.StackTrace, ex);

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

        //[HttpPost]
        //[Route("api/login")]
        //[ValidateModelAttribute]
        //public RespuestaViewModel<ResultadoUsuario> Login([FromBody] Login pUser)
        //{
        //    List<string> Mensaje = new List<string>();
        //    bool Valido = true;
        //    ResultadoViewModel respuestaVie = new ResultadoViewModel();
        //    ResultadoUsuario Usuario = new ResultadoUsuario();
        //    RespuestaViewModel<ResultadoUsuario> respuesta = new RespuestaViewModel<ResultadoUsuario>();

        //    try
        //    {

        //        //si viene nombre de equipo, validamos
        //        if (!string.IsNullOrEmpty(pUser.Device))
        //        {
        //            //valida equipo
        //            //var Device = dbContext.DeviceSet.AsNoTracking().Where(p => p.Imei.Trim().ToUpper().Equals(pUser.Device.Trim().ToUpper())).FirstOrDefault();
        //            //if (Device == null)
        //            //{
        //            //    Mensaje.Add("Este equipo no tiene acceso al sistema");
        //            //    Valido = false;
        //            //}

        //        }

        //        //valido usuario y clave
        //        if (Valido)
        //        {

        //            //var Entity = dbContext.Users.AsNoTracking().Where(p => p.Username == pUser.UserName && p.Password == pUser.UserPassword).FirstOrDefault();
        //            //if (Entity == null)
        //            //{
        //            //    Mensaje.Add("No existe información con el usuario y password ingresado");
        //            //    Valido = false;
        //            //}
        //            //else
        //            //{
        //                //SACO EL TIMER PARA ENVIAR UBICACION DEL EQUIPO
        //                //string PTIMER = dbContext.ConfigurationSet.AsNoTracking().Where(x => x.Name.Trim().Equals("TIMER")).Select(kvp => kvp.Value).FirstOrDefault();

        //                Mensaje.Add("Ok");
        //                Valido = true;
        //                //Usuario.Id = Entity.Id;
        //                //Usuario.Names = Entity.Names;
        //                //Usuario.Username = Entity.Username;
        //                //Usuario.Identification = Entity.Identification;
        //                //Usuario.PositionId = Entity.PositionId;
        //                //Usuario.RoleId = Entity.RoleId;
        //                //Usuario.Create_user = string.IsNullOrEmpty(Entity.Create_user) ? string.Empty : Entity.Create_user.Trim();
        //                //Usuario.Modifie_user = string.IsNullOrEmpty(Entity.Modifie_user) ? string.Empty : Entity.Modifie_user.Trim();
        //                //Usuario.Create_date = Entity.Create_date;
        //                //Usuario.Modifie_date = Entity.Modifie_date;
        //                //Usuario.Status = Entity.Status;
        //                //Usuario.CompanyId = Entity.CompanyId;
        //                //Usuario.Timer = (string.IsNullOrEmpty(PTIMER) ? 5 : Int64.Parse(PTIMER));
        //                //Usuario.IdUserLeader = Entity.IdUserLeader;
        //                //Usuario.Email = string.IsNullOrEmpty(Entity.Email) ? string.Empty : Entity.Email.Trim();
        //                //Usuario.WorkDate = Entity.WorkDate;
        //            //}

        //        }



        //        respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
        //        respuestaVie.Respuesta = Valido;
        //        respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
        //        respuestaVie.Titulo = "Login";
        //        respuestaVie.TotalRowsCount = Usuario != null && Usuario.Id > 0 ? Usuario.Id : 0;
        //        respuesta.Resultado = respuestaVie;
        //        respuesta.Respuesta = Usuario ?? new ResultadoUsuario();

        //        //Usuario.RemoveAt(20);
        //    }
        //    catch (Exception ex)
        //    {
        //        //registro log de errores
        //        lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Login), "api/login", false, null, null, ex.StackTrace, ex);

        //        respuestaVie = new ResultadoViewModel();
        //        respuestaVie.Respuesta = false;
        //        respuestaVie.Titulo = "Error";
        //        respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
        //        List<String> mensaje = new List<string>();
        //        mensaje.Add(string.Format("EXCEPCION NO CONTROLADA # {0}: {1}", lm, ex.Message.ToString()));
        //        respuestaVie.Mensajes = mensaje;
        //        respuesta.Resultado = respuestaVie;
        //    }

        //    return respuesta;

        //}


    }
}
