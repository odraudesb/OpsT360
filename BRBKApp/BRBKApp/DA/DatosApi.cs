using ApiDatos;
using ApiModels;
using ApiModels.AppModels;
using ApiModels.Resultados;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using System.Linq;
using Newtonsoft.Json;
using Android.Net.Wifi.Aware;

namespace BRBKApp.DA
{
    public class DatosApi
    {
        public tarjaDet oDet { get; set; }
        public BAN_Stowage_Plan_Aisv oDetAisv { get; set; }
        public BAN_Stowage_Movimiento oDetMovimiento { get; set; }
        public long? idResultado {  get; set; }
        public int arrastre { get; set; }
        public async Task<User> Login(string user, string pass, string imei)
        {
            User _user;
            UserDB _userDB;
            Datos datos = new Datos();
            RespuestaViewModel<User> result = new RespuestaViewModel<User>();

            var res = await datos.Login(user, pass, imei).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _user = new User();
                _user = result.Respuesta;
                _user.UserId = (int)result.Respuesta.Id;
                _user.response = result.Resultado.Respuesta;
                _user.messages = result.Resultado.Mensajes[0].Trim();

                try
                {
                    _userDB = new UserDB()
                    {
                        Id = (long)_user.Id,
                        UserId = (long)_user.Id,
                        Identification = _user.Identification.Trim(),
                        Username = _user.Username.Trim(),
                        Names = _user.Names.Trim(),
                        Password = _user?.Password?.Trim(),
                        Phone = _user?.Phone?.Trim(),
                        Create_user = _user?.Create_user?.Trim(),
                        Modifie_user = _user?.Modifie_user?.Trim(),
                        Create_date = _user.Create_date,
                        Modifie_date = _user.Modifie_date,
                        Status = _user.Status,
                        PositionId = (long)_user?.PositionId,
                        RoleId = (long)_user?.RoleId,
                        CompanyId = _user?.CompanyId,
                        IdUserLeader = _user.IdUserLeader,
                        Email = _user?.Email,
                        WorkDate = _user.WorkDate,
                        Timer = (int)_user.Timer,
                        response = result.Resultado.Respuesta,
                        messages = result.Resultado.Mensajes[0].Trim()
                    };
                }
                catch
                {
                    _userDB = new UserDB()
                    {
                        Id = (long)_user.Id,
                        UserId = (long)_user.Id,
                        Identification = _user.Identification.Trim(),
                        Username = result.Respuesta.Username.Trim(),
                        Names = result.Respuesta.Names.Trim(),
                        PositionId = (int)result.Respuesta.PositionId,
                        RoleId = (int)result.Respuesta.RoleId,
                        Timer = (int)result.Respuesta.Timer,
                        response = result.Resultado.Respuesta,
                        messages = result.Resultado.Mensajes[0].Trim()
                    };
                }

                _user.UsuarioDB = _userDB;
            }
            else
            {
                _user = new User()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _user;
        }
              
        public async Task<ObservableCollection<Tasks>> GetListaBL(string _mrn, string _lugar,int UserId)
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<tarjaDet>> result = new RespuestaViewModel<List<tarjaDet>>();

            try
            {
                var res = await datos.BLInProgresses(_mrn,_lugar).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    //resps = new ObservableCollection<Tasks>();
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Tasks
                        {
                            Topic = item.Consignatario,
                            Duration = item.carga,
                            Color = "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.cantidad,
                            Arrastre = (Int32)item.arrastre,
                            Saldo = (Int32)item.pendiente,
                            Id = item.idTarjaDet.ToString(),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.producto?.nombre,
                                    Time = "Commodity"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0}",item.Ubicaciones?.nombre),
                                    Time = "Planned Location"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0}",item.producto?.Items?.nombre),
                                    Time = "Item"
                                },
                                new Detail
                                {
                                    Name = item.arrastre.ToString(),
                                    Time = "Drag"
                                },
                                new Detail
                                {
                                    Name = item.pendiente.ToString(),
                                    Time = "Pending Qty"
                                },
                                //new Detail
                                //{
                                //    Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                //    Time = "Date"
                                //}
                            }
                        });
                    }
                }
            }
            catch 
            {

            }
            return resps;
        }

        public async Task<ObservableCollection<Tasks>> GetListaRecepciones(long _idTarjaDet, int UserId,string _lugar)
        {
            oDet = new tarjaDet();
            arrastre = 0;
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<recepcion>> result = new RespuestaViewModel<List<recepcion>>();
            RespuestaViewModel<tarjaDet> resultDet = new RespuestaViewModel<tarjaDet>();
            try
            {
                //SE OBTIENE EL LISTADO DE RECEPCIONES
                var res = await datos.RecepcionInProgresses(_idTarjaDet, _lugar).ConfigureAwait(false);
                result = res;

                //REFRESCA LOS DATOS DEL DETALLE SELECCIONADO
                var resDet = await datos.GetTarjaDet(_idTarjaDet);
                resultDet = resDet;
                oDet = resultDet.Respuesta;

                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    foreach (var item in result.Respuesta)
                    {
                        arrastre = arrastre + (Int32)item.cantidad;
                        resps.Add(new Tasks
                        {
                            Topic = item.TarjaDet.Consignatario,
                            Duration = item.TarjaDet.carga,
                            Color = "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.cantidad,
                            Arrastre = 0,
                            Saldo = 0,
                            Id = item.idRecepcion.ToString(),
                            Details = new ObservableCollection<Detail>
                        {
                            new Detail
                            {
                                Name = item.usuarioCrea,
                                Time = "Clerk"
                            },
                            //new Detail
                            //{
                            //    Name = string.Format("{0}",item.Ubicaciones?.nombre),
                            //    Time = "Winery Location"
                            //},
                            new Detail
                            {
                                Name = item.cantidad.ToString(),
                                Time = "Qty"
                            },
                            new Detail
                            {
                                Name = item.Estados.nombre,
                                Time = "Status"
                            },
                            new Detail
                            {
                                Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                Time = "Upload Date"
                            }
                        }
                        });
                    }
                }
            }
            catch 
            {

            }
            return resps;
        }

        public async Task<recepcion> GetRecepcion(long idRecepcion, string UserName)
        {
            RespuestaViewModel<tarjaDet> resultDet = new RespuestaViewModel<tarjaDet>();
            recepcion _rec;
            Datos datos = new Datos();
            RespuestaViewModel<recepcion> result = new RespuestaViewModel<recepcion>();

            var res = await datos.GetRecepcion(idRecepcion).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new recepcion();
                _rec = result.Respuesta;
            }
            else
            {
                _rec = new recepcion()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<recepcion> GetRecepcion(long idRecepcion, long idTarjaDet, string UserName)
        {
            RespuestaViewModel<tarjaDet> resultDet = new RespuestaViewModel<tarjaDet>();
            recepcion _rec;
            Datos datos = new Datos();
            RespuestaViewModel<tarjaDet> result = new RespuestaViewModel<tarjaDet>();

            var resDet = await datos.GetTarjaDet(idTarjaDet).ConfigureAwait(false);
            result = resDet;
            if (result.Resultado.Respuesta)
            {
                _rec = new recepcion();
                _rec.idRecepcion = 0;
                _rec.TarjaDet = result.Respuesta;
            }
            else
            {
                _rec = new recepcion()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<List<Combo>> GetListaUbicaciones()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<ubicacion>> result = new RespuestaViewModel<List<ubicacion>>();
            try
            {
                var res = await datos.GetUbicaciones().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id ,
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch 
            {
                return resps;
            }

        }

        private async void GrabarTareas(List<ResultadoWorkInProgresses> respuesta)
        {
            int grabado;
            List<TasksBD> tareas = new List<TasksBD>();
            foreach (var item in respuesta)
            {
                tareas.Add(new TasksBD
                {
                    Action = item.Action,
                    BlockId = item.BlockId,
                    Block_name = item.Block_name,
                    Booking = item.Booking,
                    Complete = item.Complete,
                    Container = item.Container,
                    Date = (DateTime)item.Date,
                    HeaderWorkId = (long)item.HeaderWorkId,
                    Id_Word = item.Id_Word,
                    Reference = item.Reference,
                    TaskId = item.TaskId,
                    Task_name = item.Task_name,
                    Temperature = item.Temperature,
                    Type = item.Type,
                    UnitId = item.UnitId,
                    Word_status = item.Word_status,
                    response = true
                });
            }
            foreach (TasksBD item in tareas)
            {
                grabado = await App.Database.DeleteTareas(item);
                grabado = await App.Database.RegistroTareas(item);
            }
        }
  
        public async Task<ApiModels.AppModels.Base> RegistraNovedad(novedad oNovedad, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistersNovedad(oNovedad, photo1, photo2, photo3, photo4).ConfigureAwait(false);
                RespuestaViewModel<novedad> result = res;
                if (result != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar la novedad: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }

            }
            catch (Exception)
            {
                resp.messages = "Error al registrar la novedad";
                resp.response = false;
            }
            return resp;
        }
        
        public async Task<ApiModels.AppModels.Base> RegistraRecepcion(recepcion oRecepcion, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistersRecepcion(oRecepcion, photo1, photo2, photo3, photo4).ConfigureAwait(false);
                RespuestaViewModel<recepcion> result = res;
                if (result != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar la recepción: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception ex)
            {
                resp.messages = "Error al registrar la recepción: " + ex.Message;
                resp.response = false;
            }
            return resp;
        }

        public async Task<ApiModels.AppModels.Base> ActualizarRecepcion(recepcion oRecepcion)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.UpdateRecepcion(oRecepcion).ConfigureAwait(false);
                RespuestaViewModel<recepcion> result = res;
                if (result != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al actualizar la recepción: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception)
            {
                resp.messages = "Error al actualizar la recepción";
                resp.response = false;
            }
            return resp;
        }

        public async Task<pasePuerta> GetEPass(string numPass)
        {
            pasePuerta _rec;
            Datos datos = new Datos();
            RespuestaViewModel<pasePuerta> result = new RespuestaViewModel<pasePuerta>();

            var res = await datos.GetPasePuerta(numPass).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new pasePuerta();
                _rec = result.Respuesta;
            }
            else
            {
                _rec = new pasePuerta()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<ApiModels.AppModels.Base> RegistraDespacho(despacho oDespacho, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistersDespacho(oDespacho, photo1, photo2, photo3, photo4).ConfigureAwait(false);
                RespuestaViewModel<despacho> result = res;
                if (result != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar el despacho: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }

            }
            catch (Exception)
            {
                resp.messages = "Error al registrar el despacho";
                resp.response = false;
            }
            return resp;
        }

        public async Task<sealValidation> GetValidateSeal(string numContainer, string seal, string _position, long _gkey, bool _bloqueo, bool _impedimento, string _impedimentod, string _user, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            sealValidation _rec;
            Datos datos = new Datos();
            RespuestaViewModel<sealValidation> result = new RespuestaViewModel<sealValidation>();

            var res = await datos.GetSeal(numContainer,seal, _position,  _gkey,  _bloqueo,  _impedimento,  _impedimentod, _user, photo1, photo2, photo3, photo4).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new sealValidation();
                _rec = result.Respuesta;
                _rec.response = result.Resultado.Respuesta;
                _rec.messages = result.Resultado.Mensajes[0].Trim();
            }
            else
            {
                _rec = new sealValidation()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<sealAsignacionMuelle> SetSealRegisters(sealAsignacionMuelle objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            sealAsignacionMuelle _rec;
            Datos datos = new Datos();
            RespuestaViewModel<sealAsignacionMuelle> result = new RespuestaViewModel<sealAsignacionMuelle>();

            var res = await datos.SetSealRegisters(objSeal, photo1, photo2, photo3, photo4).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new sealAsignacionMuelle();
                _rec = result.Respuesta;
                _rec.response = result.Resultado.Respuesta;
                _rec.messages = result.Resultado.Mensajes[0].Trim();
            }
            else
            {
                _rec = new sealAsignacionMuelle()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<List<opcionesRoles>> GetPermisosPorRol(long idRol)
        {
            List<opcionesRoles> _rec = new List<opcionesRoles>();
            Datos datos = new Datos();
            RespuestaViewModel<List<opcionesRoles>> result = new RespuestaViewModel<List<opcionesRoles>>();

            var res = await datos.GetPermisosXRol(idRol).ConfigureAwait(false);
            result = res;

            if (result.Resultado.Respuesta)
            {
                foreach (var item in result.Respuesta)
                {
                    _rec.Add(new opcionesRoles
                    {
                        Id_Option = item.Id_Option,
                        Name_Option = item.Name_Option,
                        Selection = item.Selection,
                        Order = item.Order,
                        RoleId = item.RoleId,
                        Role_Name = item.Role_Name,
                    });
                }
            }
           // return resps;
            //if (result.Resultado.Respuesta)
            //{
            //    _rec = new opcionesRoles();
            //    _rec = result.Respuesta;
            //}
            //else
            //{
            //    _rec = new pasePuerta()
            //    {
            //        response = result.Resultado.Respuesta,
            //        messages = result.Resultado.Mensajes[0].Trim()
            //    };
            //}

            return _rec;
        }

        public async Task<List<Combo>> GetListPOWN4()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<workPositionN4List>> result = new RespuestaViewModel<List<workPositionN4List>>();
            try
            {
                var res = await datos.GetListPOWN4().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.codigo,
                            Descripcion = item.nave
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }

        public async Task<ObservableCollection<workPosition>> GetPOW(string _idposition, string _usuario, string _containers)
        {
            ObservableCollection<workPosition> resps = new ObservableCollection<workPosition>();
            Datos datos = new Datos();
            RespuestaViewModel<workPosition> result = new RespuestaViewModel<workPosition>();
            try
            {
                var res = await datos.GetPOW(_idposition, _usuario, _containers).ConfigureAwait(false);
                result = res;
                
                if (result.Resultado.Respuesta)
                {
                    resps = new ObservableCollection<workPosition>();
                    workPosition oPOW;
                    oPOW = result.Respuesta;
                    oPOW.response = result.Resultado.Respuesta;
                    oPOW.messages = result.Resultado.Mensajes[0].Trim();
                    resps.Add(oPOW);
                }
                else
                {
                    resps = new ObservableCollection<workPosition>();
                    workPosition oPOW = new workPosition();
                    oPOW.dataContenedor = result.Respuesta?.dataContenedor;
                    oPOW.response = result.Resultado.Respuesta;
                    oPOW.messages = result.Resultado.Mensajes[0].Trim();
                    resps.Add(oPOW);
                }
                return resps;
            }
            catch
            {
               // resps = new workPosition();
                return resps;
            }
        }

        public async Task<workPosition> SetPOWRegisters(workPosition objPOW)
        {
            workPosition _rec;
            Datos datos = new Datos();
            RespuestaViewModel<workPosition> result = new RespuestaViewModel<workPosition>();

            var res = await datos.SetPOWRegisters(objPOW).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new workPosition();
                _rec = result.Respuesta;
                _rec.response = result.Resultado.Respuesta;
                _rec.messages = result.Resultado.Mensajes[0].Trim();
            }
            else
            {
                _rec = new workPosition()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<ObservableCollection<dataContainers>> GetDataConteinersImpo(string _containers)
        {
            ObservableCollection<dataContainers> resps = new ObservableCollection<dataContainers>();
            Datos datos = new Datos();
            RespuestaViewModel<dataContainers> result = new RespuestaViewModel<dataContainers>();
            try
            {
                var res = await datos.GetDataContainersImpo(_containers).ConfigureAwait(false);
                result = res;

                if (result.Resultado.Respuesta)
                {
                    resps = new ObservableCollection<dataContainers>();
                    dataContainers oDataContainersImpo;
                    oDataContainersImpo = result.Respuesta;
                    oDataContainersImpo.response = result.Resultado.Respuesta;
                    oDataContainersImpo.messages = result.Resultado.Mensajes[0].Trim();
                    resps.Add(oDataContainersImpo);
                }
                else
                {
                    resps = new ObservableCollection<dataContainers>();
                    dataContainers odataContainers = new dataContainers();
                    odataContainers = result.Respuesta;
                    odataContainers.response = result.Resultado.Respuesta;
                    odataContainers.messages = result.Resultado.Mensajes[0].Trim();
                    resps.Add(odataContainers);
                }
                return resps;
            }
            catch
            {
                // resps = new workPosition();
                return resps;
            }
        }

        public async Task<downloadConfirmation> SetDownloadConfirmation(downloadConfirmation obj)
        {
            downloadConfirmation _rec;
            Datos datos = new Datos();
            RespuestaViewModel<downloadConfirmation> result = new RespuestaViewModel<downloadConfirmation>();

            var res = await datos.SetDownloadConfirmation(obj).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new downloadConfirmation();
                _rec = result.Respuesta;
                _rec.response = result.Resultado.Respuesta;
                _rec.messages = result.Resultado.Mensajes[0].Trim();
            }
            else
            {
                _rec = new downloadConfirmation()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<sealRegistroPreEmbarqueYaforo> SetSealPreEmbarque(sealRegistroPreEmbarqueYaforo objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            sealRegistroPreEmbarqueYaforo _rec;
            Datos datos = new Datos();
            RespuestaViewModel<sealRegistroPreEmbarqueYaforo> result = new RespuestaViewModel<sealRegistroPreEmbarqueYaforo>();

            var res = await datos.SetSealPreEmbarque(objSeal, photo1, photo2, photo3, photo4).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new sealRegistroPreEmbarqueYaforo();
                _rec = result.Respuesta;
                _rec.response = result.Resultado.Respuesta;
                _rec.messages = result.Resultado.Mensajes[0].Trim();
            }
            else
            {
                _rec = new sealRegistroPreEmbarqueYaforo()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<sealRegistroPreEmbarqueYaforo> SetSealAforo(sealRegistroPreEmbarqueYaforo objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            sealRegistroPreEmbarqueYaforo _rec;
            Datos datos = new Datos();
            RespuestaViewModel<sealRegistroPreEmbarqueYaforo> result = new RespuestaViewModel<sealRegistroPreEmbarqueYaforo>();

            var res = await datos.SetSealAforo(objSeal, photo1, photo2, photo3, photo4).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new sealRegistroPreEmbarqueYaforo();
                _rec = result.Respuesta;
                _rec.response = result.Resultado.Respuesta;
                _rec.messages = result.Resultado.Mensajes[0].Trim();
            }
            else
            {
                _rec = new sealRegistroPreEmbarqueYaforo()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<AsignaSealExpo> SetSealAssignsExpo(AsignaSealExpo objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            AsignaSealExpo _rec;
            Datos datos = new Datos();
            RespuestaViewModel<AsignaSealExpo> result = new RespuestaViewModel<AsignaSealExpo>();

            var res = await datos.SetSealAssignsExpo(objSeal, photo1, photo2, photo3, photo4).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new AsignaSealExpo();
                _rec = result.Respuesta;
                _rec.response = result.Resultado.Respuesta;
                _rec.messages = result.Resultado.Mensajes[0].Trim();
            }
            else
            {
                _rec = new AsignaSealExpo()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<ValidaSealPatio> SetSealValidationYard(ValidaSealPatio objSeal, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            ValidaSealPatio _rec;
            Datos datos = new Datos();
            RespuestaViewModel<ValidaSealPatio> result = new RespuestaViewModel<ValidaSealPatio>();

            var res = await datos.SetSealValidationYard(objSeal, photo1, photo2, photo3, photo4).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new ValidaSealPatio();
                _rec = result.Respuesta;
                _rec.response = result.Resultado.Respuesta;
                _rec.messages = result.Resultado.Mensajes[0].Trim();
            }
            else
            {
                _rec = new ValidaSealPatio()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        //####################
        //  VBS BODEGA
        //####################
        public async Task<ObservableCollection<Tasks>> GetListaAISV(string _estado, string _aisv, long? _idStowageDet, int UserId)
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Stowage_Plan_Aisv>> result = new RespuestaViewModel<List<BAN_Stowage_Plan_Aisv>>();

            try
            {
                var res = await datos.AisvInProgresses(_estado, _aisv, _idStowageDet).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    //resps = new ObservableCollection<Tasks>();
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Tasks
                        {
                            Topic =  item.oStowage_Plan_Det?.oExportador?.nombre,
                            Duration = string.Format("[{0}] - [{1}]",item.aisv, item.placa),
                            Status = (item.IIEAutorizada == true ? "IIE AUTORIZADO" : "IIE PENDIENTE") + "\n" + (string.IsNullOrEmpty(item.comentario.Trim())? "": item.comentario),
                            Color = item.IIEAutorizada == true ? "#00FF00" : "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.box,
                            Arrastre = (Int32)item.arrastre,
                            Saldo = (Int32)item.pendiente,
                            Id = item.idStowageAisv.ToString() + "," + item.IIEAutorizada.ToString() + "," + (string.IsNullOrEmpty(item.comentario.Trim()) ? "0" : item.comentario),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = string.Format("Cargo: {0} | Marca: {1}",item.oStowage_Plan_Det?.oCargo?.nombre,item.oStowage_Plan_Det?.oMarca?.nombre),
                                    Time = "Commodity"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0} | Bloque: {1}", item.oStowage_Plan_Det?.oBodega?.nombre, item.oStowage_Plan_Det?.oBloque?.nombre),
                                    Time = "Planned Location"
                                },
                                new Detail
                                {
                                    Name = string.Format("AISV: {0} DAE: {1} BOOKING: {2}",item.aisv, item.dae, item.booking),
                                    Time = "Item"
                                },
                                new Detail
                                {
                                    Name = item.arrastre.ToString(),
                                    Time = "Drag"
                                },
                                new Detail
                                {
                                    Name = item.pendiente.ToString(),
                                    Time = "Pending Qty"
                                },
                                //new Detail
                                //{
                                //    Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                //    Time = "Date"
                                //}
                            }
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return resps;
        }

        public async Task<ObservableCollection<Tasks>> GetListaMovimientos(long _idStowageAisv, int UserId)
        {
            oDetAisv = new BAN_Stowage_Plan_Aisv();
            arrastre = 0;
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Stowage_Movimiento>> result = new RespuestaViewModel<List<BAN_Stowage_Movimiento>>();
            RespuestaViewModel<BAN_Stowage_Plan_Aisv> resultDet = new RespuestaViewModel<BAN_Stowage_Plan_Aisv>();
            try
            {
                //SE OBTIENE EL LISTADO DE RECEPCIONES
                var res = await datos.AisvRecepcionInProgresses(_idStowageAisv).ConfigureAwait(false);
                result = res;

                //REFRESCA LOS DATOS DEL DETALLE SELECCIONADO
                var resDet = await datos.GetStowage_Plan_Aisv(_idStowageAisv);
                resultDet = resDet;
                oDetAisv = resultDet.Respuesta;

                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    foreach (var item in result.Respuesta)
                    {
                        item.oStowage_Plan_Aisv = oDetAisv;
                        arrastre = arrastre + (Int32)item.cantidad;
                        resps.Add(new Tasks
                        {
                            Topic = "MODALIDAD : " + item.oModalidad?.nombre,
                            Duration = item.barcode,
                            Color = "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.cantidad,
                            Arrastre = 0,
                            Saldo = 0,
                            Id = item.idMovimiento.ToString(),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.usuarioCrea,
                                    Time = "Clerk"
                                },
                                //new Detail
                                //{
                                //    Name = string.Format("{0}",item.Ubicaciones?.nombre),
                                //    Time = "Winery Location"
                                //},
                                new Detail
                                {
                                    Name = item.cantidad.ToString(),
                                    Time = "Qty"
                                },
                                new Detail
                                {
                                    Name = item.oEstado?.nombre,
                                    Time = "Status"
                                },
                                new Detail
                                {
                                    Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                    Time = "Upload Date"
                                }
                            }
                        });
                    }
                }
            }
            catch
            {

            }
            return resps;
        }

        public async Task<List<Combo>> GetListaModalidad()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Catalogo_Modalidad>> result = new RespuestaViewModel<List<BAN_Catalogo_Modalidad>>();
            try
            {
                var res = await datos.GetModalidad().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }

        }

        public async Task<ApiModels.AppModels.Base> RegistraMovmientoVBS(BAN_Stowage_Movimiento oRecepcion, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistersMovimiento(oRecepcion, photo1, photo2, photo3, photo4).ConfigureAwait(false);
                RespuestaViewModel<BAN_Stowage_Movimiento> result = res;
                idResultado = res.Respuesta.idMovimiento;
                if (result.Respuesta != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar la recepción: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception ex)
            {
                resp.messages = "Error al registrar la recepción: " + ex.Message;
                resp.response = false;
            }
            return resp;
        }

        public async Task<BAN_Stowage_Movimiento> GetRecepcionAisv(long idStowageAisv, string UserName)
        {
            RespuestaViewModel<BAN_Stowage_Plan_Aisv> resultDet = new RespuestaViewModel<BAN_Stowage_Plan_Aisv>();
            BAN_Stowage_Movimiento _rec;
            Datos datos = new Datos();
            RespuestaViewModel<BAN_Stowage_Plan_Aisv> result = new RespuestaViewModel<BAN_Stowage_Plan_Aisv>();

            var resDet = await datos.GetStowage_Plan_Aisv(idStowageAisv).ConfigureAwait(false);
            result = resDet;
            if (result.Resultado.Respuesta)
            {
                _rec = new BAN_Stowage_Movimiento();
                _rec.idMovimiento = 0;
                _rec.oStowage_Plan_Aisv = result.Respuesta;
            }
            else
            {
                _rec = new BAN_Stowage_Movimiento()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<BAN_Stowage_Movimiento> GetRecepcionAisvPorBarcode(string barcodeMovimiento)
        {
            BAN_Stowage_Movimiento _rec;
            Datos datos = new Datos();
            RespuestaViewModel<BAN_Stowage_Movimiento> result = new RespuestaViewModel<BAN_Stowage_Movimiento>();

            var res = await datos.GetReccepcionPorBarcode(barcodeMovimiento).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new BAN_Stowage_Movimiento();
                _rec = result.Respuesta;
            }
            else
            {
                _rec = new BAN_Stowage_Movimiento()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<BAN_Stowage_Movimiento> GetRecepcionAisvPorId(long _id)
        {
            BAN_Stowage_Movimiento _rec;
            Datos datos = new Datos();
            RespuestaViewModel<BAN_Stowage_Movimiento> result = new RespuestaViewModel<BAN_Stowage_Movimiento>();

            var res = await datos.GetReccepcionPorId(_id).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new BAN_Stowage_Movimiento();
                _rec = result.Respuesta;
            }
            else
            {
                _rec = new BAN_Stowage_Movimiento()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<ApiModels.AppModels.Base> ActualizarMovimientoVBS(BAN_Stowage_Movimiento oRecepcion)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.UpdateMovimiento(oRecepcion).ConfigureAwait(false);
                RespuestaViewModel<BAN_Stowage_Movimiento> result = res;
                if (result != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al actualizar la recepción: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception)
            {
                resp.messages = "Error al actualizar la recepción";
                resp.response = false;
            }
            return resp;
        }

        public async Task<ApiModels.AppModels.Base> AnularMovimientoVBS(BAN_Stowage_Movimiento oRecepcion)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.AnularMovimiento(oRecepcion).ConfigureAwait(false);
                RespuestaViewModel<BAN_Stowage_Movimiento> result = res;
                if (result != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al actualizar la recepción: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception)
            {
                resp.messages = "Error al actualizar la recepción";
                resp.response = false;
            }
            return resp;
        }

        public async Task<List<Combo>> GetListaProfundidad(int _idBodega, int _idBloque, int _idFila, int _idAltura)
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Catalogo_Profundidad>> result = new RespuestaViewModel<List<BAN_Catalogo_Profundidad>>();
            try
            {
                var res = await datos.GetProfundidad( _idBodega, _idBloque, _idFila, _idAltura).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.descripcion
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }

        }

        public async Task<BAN_Catalogo_Ubicacion> GetUbicacionPorBarcode(string barcodeUbicacion)
        {
            BAN_Catalogo_Ubicacion _rec;
            Datos datos = new Datos();
            RespuestaViewModel<BAN_Catalogo_Ubicacion> result = new RespuestaViewModel<BAN_Catalogo_Ubicacion>();

            var res = await datos.GetUbicacionPorBarcode(barcodeUbicacion).ConfigureAwait(false);
            result = res;
            if (result.Resultado.Respuesta)
            {
                _rec = new BAN_Catalogo_Ubicacion();
                _rec = result.Respuesta;
            }
            else
            {
                _rec = new BAN_Catalogo_Ubicacion()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        //Orden Despacho

        public async Task<ObservableCollection<Tasks>> GetListaMovimientosPorFiltros(string _idNave, string _idBodega, string _idExportador, string _booking, string _barcode)
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Stowage_Movimiento>> result = new RespuestaViewModel<List<BAN_Stowage_Movimiento>>();

            try
            {
                var res = await datos.GetListaMovimientos(_idNave, _idBodega, _idExportador, _booking, _barcode).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    //resps = new ObservableCollection<Tasks>();
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Tasks
                        {
                            Topic = item.oStowage_Plan_Aisv.oStowage_Plan_Det.oExportador.nombre,
                            Duration = item.barcode,
                            Status = item.oStowage_Plan_Aisv.booking,
                            Color =  "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.cantidad,
                            Arrastre = (Int32)item.cantidad,//(Int32)item.arrastre,
                            Saldo = (Int32)item.cantidad,//(Int32)item.pendiente,
                            Id = item.idStowageAisv.ToString(),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = string.Format("Cargo: {0} | Marca: {1}",item.oStowage_Plan_Aisv.oStowage_Plan_Det?.oCargo?.nombre,item.oStowage_Plan_Aisv.oStowage_Plan_Det?.oMarca?.nombre),
                                    Time = "Commodity"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0} \nBloque: {1} \nRack: {2} \nAltura: {3} \nSlot: {4}", item.oUbicacion?.oBodega?.nombre, item.oUbicacion?.oBloque?.nombre,item.oUbicacion?.oFila?.descripcion,item.oUbicacion?.oAltura?.descripcion, item.oUbicacion?.oProfundidad?.descripcion),
                                    Time = "Location"
                                },
                                new Detail
                                {
                                    Name = string.Format("AISV: {0} DAE: {1} BOOKING: {2}",item.oStowage_Plan_Aisv.aisv, item.oStowage_Plan_Aisv.dae, item.oStowage_Plan_Aisv.booking),
                                    Time = "Item"
                                },
                               new Detail
                                {
                                    Name = item.oEstado?.nombre,
                                    Time = "Status"
                                },
                                new Detail
                                {
                                    Name = item.usuarioModifica.ToString(),
                                    Time = "Entered by"
                                },
                                new Detail
                                {
                                    Name = Convert.ToDateTime(item.fechaModifica,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                    Time = "Date of Admission"
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return resps;
        }

        public async Task<ObservableCollection<Tasks>> GetListaMovimientosParaDespacho(string _idNave, string _idExportador, string _idBloque)
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Stowage_Movimiento>> result = new RespuestaViewModel<List<BAN_Stowage_Movimiento>>();

            try
            {
                var res = await datos.GetListaMovimientosParaDespacho(_idNave, _idExportador, _idBloque).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    //resps = new ObservableCollection<Tasks>();
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Tasks
                        {
                            Topic = item.oExportador.nombre,
                            Duration = item.booking,
                            Status = string.Format("{0} | {1}", item.oBloque?.oBodega.nombre, item.oBloque?.nombre),
                            Color = "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.palets,
                            Arrastre = (Int32)item.palets,//(Int32)item.arrastre,
                            Saldo = (Int32)item.palets,//(Int32)item.pendiente,
                            Id = item.idNave.ToString() + "," + item.idExportador.ToString() + "," + item.idUbicacion + "," + item.booking,
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.oBloque?.oBodega.nombre,
                                    Time = "Location"
                                },
                                new Detail
                                {
                                    Name = item.oBloque?.nombre,
                                    Time = "Block"
                                },
                                new Detail
                                {
                                    Name = item.booking,
                                    Time = "Booking"
                                },
                                //new Detail
                                //{
                                //    Name = item.oModalidad?.nombre,
                                //    Time = "Modality"
                                //},
                                new Detail
                                {
                                    Name = item.cantidad.ToString(),
                                    Time = "Amount of Boxes"
                                }
                                //new Detail
                                //{
                                //    Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                //    Time = "Date"
                                //}
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return resps;
        }

        public async Task<List<Combo>> GetListaNave()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaNaves().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }

        public async Task<List<Combo>> GetListaBodegas(string _idNave)
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaBodegas(_idNave).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }

        public async Task<List<Combo>> GetListaBloquess(string _idNave)
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaBloques(_idNave).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }

        public async Task<List<Combo>> GetListaExportadoresPorNaveBodega(string _idNave, int? _idBodega)
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaExportadoresPorNaveBodega(_idNave, _idBodega).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }

        public async Task<List<Combo>> GetListaExportadoresPorNave(string _idNave)
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaExportadoresPorNave(_idNave).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }

        public async Task<ObservableCollection<Tasks>> GetListaOrdenes(string _idNave, int _idExportador, int _idBloque, string _booking)
        {
            oDetMovimiento = new BAN_Stowage_Movimiento();
            arrastre = 0;
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> result = new RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>();

            try
            {
                //SE OBTIENE EL LISTADO DE RECEPCIONES
                var res = await datos.OrdenesDespachoInProgresses(_idNave, _idExportador,  _idBloque, _booking).ConfigureAwait(false);
                result = res;

                //REFRESCA LOS DATOS DEL DETALLE SELECCIONADO
                var resDet = await datos.GetMovimientoParaDespacho(_idNave, _idExportador.ToString(), _idBloque.ToString(), _booking);
                oDetMovimiento = resDet.Respuesta;

                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    foreach (var item in result.Respuesta)
                    {
                        item.oMovimiento = oDetMovimiento;
                        arrastre = arrastre + (Int32)item.cantidadPalets;
                        resps.Add(new Tasks
                        {
                            Topic = "Order Number : " + item.idOrdenDespacho?.ToString("D8"),
                            Duration = item.fechaCreacion.ToString(),
                            Color = "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.cantidadPalets,
                            Arrastre = (Int32)item.arrastre,
                            Saldo = (Int32)item.pendiente,
                            Id = item.idOrdenDespacho.ToString(),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.usuarioCrea,
                                    Time = "Clerk"
                                },
                                new Detail
                                {
                                    Name = item.cantidadPalets.ToString(),
                                    Time = "Pallets"
                                },
                                new Detail
                                {
                                    Name = item.cantidadBox.ToString(),
                                    Time = "Boxes"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0}",item.oBloque?.nombre),
                                    Time = "Winery Location"
                                },

                                new Detail
                                {
                                    Name = item.oEstado?.nombre,
                                    Time = "Status"
                                },
                                new Detail
                                {
                                    Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                    Time = "Upload Date"
                                }
                            }
                        });
                    }
                }
            }
            catch
            {

            }
            return resps;
        }

        public async Task<BAN_Stowage_Movimiento> GetMovimientoParaDespacho(string _idNave, string _idExportador, string _idBloque, string _booking)
        {
            RespuestaViewModel<BAN_Stowage_Movimiento> resultDet = new RespuestaViewModel<BAN_Stowage_Movimiento>();
            BAN_Stowage_Movimiento _rec;
            Datos datos = new Datos();
            RespuestaViewModel<BAN_Stowage_Movimiento> result = new RespuestaViewModel<BAN_Stowage_Movimiento>();

            var resDet = await datos.GetMovimientoParaDespacho(_idNave, _idExportador, _idBloque, _booking).ConfigureAwait(false);
            result = resDet;
            if (result.Resultado.Respuesta)
            {
                _rec = new BAN_Stowage_Movimiento();
                _rec.idMovimiento = 0;
               _rec = result.Respuesta;
            }
             else
            {
                _rec = new BAN_Stowage_Movimiento()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<ApiModels.AppModels.Base> RegistraOrdenDespacho(BAN_Stowage_OrdenDespacho oOrdenDespacho)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistersOrdenDespacho(oOrdenDespacho).ConfigureAwait(false);
                RespuestaViewModel<BAN_Stowage_OrdenDespacho> result = res;
                if (result.Respuesta != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar la recepción: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception ex)
            {
                resp.messages = "Error al registrar la recepción: " + ex.Message;
                resp.response = false;
            }
            return resp;
        }

        public async Task<ApiModels.AppModels.Base> AnularOrdenDespacho(BAN_Stowage_OrdenDespacho oOrdenDespacho)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.AnulaOrdenDespacho(oOrdenDespacho).ConfigureAwait(false);
                RespuestaViewModel<BAN_Stowage_OrdenDespacho> result = res;
                if (result.Respuesta != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar la recepción: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception ex)
            {
                resp.messages = "Error al registrar la recepción: " + ex.Message;
                resp.response = false;
            }
            return resp;
        }

        //Pre-Despacho
        public async Task<List<Combo>> GetListaBodegasPreDespacho()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaBodegasPreDespacho().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }

        public async Task<ObservableCollection<Tasks>> GetListaOrdenesDespacho(string _idBodega, int UserId)
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> result = new RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>();

            try
            {
                var res = await datos.GetListaOrdenesDespacho(_idBodega).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    //resps = new ObservableCollection<Tasks>();
                    foreach (var item in result.Respuesta)
                    {
                        string json = JsonConvert.SerializeObject(item);
                        resps.Add(new Tasks
                        {
                            Topic = "Order Number : " + item.idOrdenDespacho?.ToString("D8") + "\n" + string.Format("{0}", item.oBloque?.nombre),
                            Duration = item.oExportador.nombre,
                            Color = "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.pendiente,
                            Arrastre = (Int32)item.arrastre,
                            Saldo = (Int32)item.pendiente,
                            Id = json,//item.idOrdenDespacho.ToString(),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.usuarioCrea,
                                    Time = "Clerk"
                                },
                                new Detail
                                {
                                    Name = item.cantidadPalets.ToString(),
                                    Time = "Total Pallets"
                                },
                                new Detail
                                {
                                    Name = item.arrastre.ToString(),
                                    Time = "Processed Pallets"
                                },
                                new Detail
                                {
                                    Name = item.pendiente.ToString(),
                                    Time = "Pallets Pending Processing "
                                },
                                 new Detail
                                {
                                    Name = item.cantidadBox.ToString(),
                                    Time = "Total Boxes"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0}",item.oBloque?.nombre),
                                    Time = "Winery Location"
                                },

                                new Detail
                                {
                                   Name = item.oEstado?.nombre,
                                    Time = "Status Order"
                                },
                                new Detail
                                {
                                    Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                    Time = "Upload Date"
                                }
                                /*
                                new Detail
                                {
                                    Name = item.producto?.nombre,
                                    Time = "Commodity"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0}",item.oBloque?.nombre),
                                    Time = "Planned Location"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0}",item.o?.Items?.nombre),
                                    Time = "Item"
                                },
                                new Detail
                                {
                                    Name = item.arrastre.ToString(),
                                    Time = "Drag"
                                },
                                new Detail
                                {
                                    Name = item.pendiente.ToString(),
                                    Time = "Pending Qty"
                                },*/
                                //new Detail
                                //{
                                //    Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                //    Time = "Date"
                                //}
                            }
                        });
                    }
                }
            }
            catch
            {

            }
            return resps;
        }

        public async Task<ObservableCollection<Tasks>> GetListaOrdenesDespacho()
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>> result = new RespuestaViewModel<List<BAN_Stowage_OrdenDespacho>>();

            try
            {
                var res = await datos.GetListaOrdenesDespacho().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    //resps = new ObservableCollection<Tasks>();
                    foreach (var item in result.Respuesta)
                    {
                        string json = JsonConvert.SerializeObject(item);
                        resps.Add(new Tasks
                        {
                            Topic = "Order Number : " + item.idOrdenDespacho?.ToString("D8") + "\n" + string.Format("{0}", item.oBloque?.nombre),
                            Duration = item.oExportador.nombre,
                            Color = "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.pendiente,
                            Arrastre = (Int32)item.arrastre,
                            Status =  string.Format("Despachado {0} de {1} \nPendiente: {2}", item.arrastre.ToString(), item.cantidadPalets.ToString(), item.pendiente.ToString()),
                            Saldo = (Int32)item.pendiente,
                            Id = json,//item.idOrdenDespacho.ToString(),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.usuarioCrea,
                                    Time = "Clerk"
                                },
                                new Detail
                                {
                                    Name = item.cantidadPalets.ToString(),
                                    Time = "Total Pallets"
                                },
                                new Detail
                                {
                                    Name = item.arrastre.ToString(),
                                    Time = "Processed Pallets"
                                },
                                new Detail
                                {
                                    Name = item.pendiente.ToString(),
                                    Time = "Pallets Pending Processing "
                                },
                                 new Detail
                                {
                                    Name = item.cantidadBox.ToString(),
                                    Time = "Total Boxes"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0}",item.oBloque?.nombre),
                                    Time = "Winery Location"
                                },

                                new Detail
                                {
                                   Name = item.oEstado?.nombre,
                                    Time = "Status Order"
                                },
                                new Detail
                                {
                                    Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                    Time = "Upload Date"
                                }
                            }
                        });
                    }
                }
            }
            catch
            {

            }
            return resps;
        }
        public async Task<BAN_Stowage_OrdenDespacho> GetOrdenDespacho(long _id)
        {
            RespuestaViewModel<BAN_Stowage_OrdenDespacho> resultDet = new RespuestaViewModel<BAN_Stowage_OrdenDespacho>();
            BAN_Stowage_OrdenDespacho _rec;
            Datos datos = new Datos();
            RespuestaViewModel<BAN_Stowage_OrdenDespacho> result = new RespuestaViewModel<BAN_Stowage_OrdenDespacho>();

            var resDet = await datos.GetOrdenDespacho(_id).ConfigureAwait(false);
            result = resDet;
            if (result.Resultado.Respuesta)
            {
                _rec = new BAN_Stowage_OrdenDespacho();
                //_rec.idOrdenDespacho = 0;
                _rec = result.Respuesta;
            }
            else
            {
                _rec = new BAN_Stowage_OrdenDespacho()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        public async Task<ObservableCollection<Tasks>> GetListaDetOrdenesAgrupadaXFila(BAN_Stowage_OrdenDespacho oDespacho, long _idOrdenDespacho)
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_FilasPorOrden>> result = new RespuestaViewModel<List<BAN_Consulta_FilasPorOrden>>();

            try
            {
                //SE OBTIENE EL LISTADO DE RECEPCIONES AGRUPADAS POR FILA
                var res = await datos.GetListaDetOrdenesAgrupadaXFila(_idOrdenDespacho).ConfigureAwait(false);
                result = res;

                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);

                    var vDespacho = await datos.GetOrdenDespacho(_idOrdenDespacho).ConfigureAwait(false);
                    oDespacho = vDespacho.Respuesta;
                    foreach (var item in result.Respuesta)
                    {
                        item.oOrdenDespacho = oDespacho;
                        string json = JsonConvert.SerializeObject(item);
                        resps.Add(new Tasks
                        {
                            Topic = "Rack Number: " + item.nombre,
                            Duration = "Order Number : " + item.idOrdenDespacho.ToString("D8"),
                            Color = "#E23B1B",
                            Date = DateTime.Now,
                            QTY = (Int32)item.palets,
                            Arrastre = 0,
                            Saldo = 0,
                            Id = json,//item.idFila.ToString(),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.boxes.ToString(),
                                    Time = "Boxes"
                                },
                                new Detail
                                {
                                    Name = item.palets.ToString(),
                                    Time = "Pallets"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0}",item.nombre),
                                    Time = "Winery Location"
                                },

                                new Detail
                                {
                                    Name = item.oEstado?.nombre,
                                    Time = "Status"
                                },
                               
                            }
                        });
                    }
                }
                else
                {
                    var vDespacho = await datos.GetOrdenDespacho(_idOrdenDespacho).ConfigureAwait(false);
                    oDespacho = vDespacho.Respuesta;
                }
            }
            catch
            {

            }
            return resps;
        }

        public async Task<ObservableCollection<Tasks>> GetListaMovimientosPorFila(long _idOrdenDespacho, int _idFila)
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Stowage_Movimiento>> result = new RespuestaViewModel<List<BAN_Stowage_Movimiento>>();

            try
            {
                //SE OBTIENE EL LISTADO DE RECEPCIONES AGRUPADAS POR FILA
                var res = await datos.GetListaMovimientosXFila(_idOrdenDespacho, _idFila).ConfigureAwait(false);
                result = res;

                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Tasks
                        {
                            Topic = string.Format("{0} - {1} \nRack: {2} \nLevel: {3} \nPosition: {4}", item .oBloque?.oBodega?.nombre, item.oBloque?.nombre, item.oUbicacion?.oFila?.descripcion, item.oUbicacion?.oAltura?.descripcion ,item.oUbicacion.oProfundidad.descripcion),
                            Duration = "Pallets Barcode: " + item.barcode,
                            Color = "#E23B1B",
                            Date = DateTime.Now,
                            QTY = (Int32)item.cantidad,
                            Arrastre = 0,
                            Saldo = 0,
                            Id = item.idMovimiento.ToString(),
                            Status = item.barcode,
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.idOrdenDespacho.ToString("D8"),
                                    Time = "Order Number"
                                },
                                
                                new Detail
                                {
                                    Name = item.cantidad.ToString(),
                                    Time = "Boxes"
                                },
                                new Detail
                                {
                                    Name = item.oExportador?.nombre.ToString(),
                                    Time = "Export"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0}",item.oModalidad?.nombre),
                                    Time = "Modality"
                                },

                                new Detail
                                {
                                    Name = item.oEstado?.nombre,
                                    Time = "Status"
                                },

                            }
                        });
                    }
                }
            }
            catch
            {

            }
            return resps;
        }
        public async Task<ApiModels.AppModels.Base> RegistraPreDespacho(long idMovimiento, string user)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistersPreDespacho(idMovimiento,user).ConfigureAwait(false);
                RespuestaViewModel<BAN_Stowage_Movimiento> result = res;
                if (result.Respuesta != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar la pre despacho: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception ex)
            {
                resp.messages = "Error al registrar el pre despacho: " + ex.Message;
                resp.response = false;
            }
            return resp;
        }

        public async Task<ApiModels.AppModels.Base> RegistrarDespachoVBS(BAN_Stowage_Movimiento oRecepcion, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistraDespachoVBS(oRecepcion, photo1, photo2, photo3, photo4).ConfigureAwait(false);
                RespuestaViewModel<BAN_Stowage_Movimiento> result = res;
                if (result.Respuesta != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar la recepción: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception ex)
            {
                resp.messages = "Error al registrar la recepción: " + ex.Message;
                resp.response = false;
            }
            return resp;
        }

        public async Task<BAN_Stowage_Plan_Aisv> GetBAN_Stowage_Plan_AisvXBooking(string _booking)
        {
            RespuestaViewModel<BAN_Stowage_Plan_Aisv> resultDet = new RespuestaViewModel<BAN_Stowage_Plan_Aisv>();
            BAN_Stowage_Plan_Aisv _rec;
            Datos datos = new Datos();
            RespuestaViewModel<BAN_Stowage_Plan_Aisv> result = new RespuestaViewModel<BAN_Stowage_Plan_Aisv>();

            var resDet = await datos.GetBAN_Stowage_Plan_AisvXBooking(_booking).ConfigureAwait(false);
            result = resDet;
            if (result.Resultado.Respuesta)
            {
                _rec = new BAN_Stowage_Plan_Aisv();
                //_rec.idOrdenDespacho = 0;
                _rec = result.Respuesta;
                _rec.response = result.Resultado.Respuesta;
            }
            else
            {
                _rec = new BAN_Stowage_Plan_Aisv()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }

        //EMBARQUE
        public async Task<List<Combo>> GetListaNavesVBS()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaNavesVBS().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }
        public async Task<List<Combo>> GetListaExportadoresVBS()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaExportadoresVBS().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }
        public async Task<List<Combo>> GetListaHoldVBS()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaHolds().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }
        public async Task<List<Combo>> GetListaDecksVBS()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaDecks().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }
        public async Task<List<Combo>> GetListaOrigenVBS()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaOrigen().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }
        public async Task<List<Combo>> GetListaBrandsVBS(string _idExportador)
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaBrands(_idExportador).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }
        public async Task<List<Combo>> GetListaTipoMovimientosVBS()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaTipoMovimiento().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }
        public async Task<List<Combo>> GetListaModalidadEmbarque()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Catalogo_Modalidad>> result = new RespuestaViewModel<List<BAN_Catalogo_Modalidad>>();
            try
            {
                var res = await datos.GetModalidadEmbarque().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }

        }
        public async Task<ObservableCollection<Tasks>> GetListaOrdenesEmbarque(string _idNave, string _idExportador, int UserId)
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Embarque_Cab>> result = new RespuestaViewModel<List<BAN_Embarque_Cab>>();

            try
            {
                var res = await datos.GetListaOrdenesEmbarque(_idNave, _idExportador).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);
                    //resps = new ObservableCollection<Tasks>();
                    foreach (var item in result.Respuesta)
                    {
                        string json = JsonConvert.SerializeObject(item);
                        resps.Add(new Tasks
                        {
                            Topic = "REFERENCE: " + item.idNave + "\n" + string.Format("{0} {1}", item.nave, item.barcode),
                            Duration = item.Exportador,
                            Color = "#E23B1B",
                            Date = (DateTime)item.fechaCreacion,
                            QTY = (Int32)item.box,
                            Arrastre = 0,//(Int32)item.arrastre,
                            Saldo = 0,//(Int32)item.pendiente,
                            Id = item.idEmbarqueCab.ToString(),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.usuarioCrea,
                                    Time = "Clerk"
                                },
                                new Detail
                                {
                                    Name = item.barcode.ToString(),
                                    Time = "Barcode"
                                },
                                new Detail
                                {
                                    Name = item.idNave.ToString(),
                                    Time = "Reference"
                                },
                                new Detail
                                {
                                    Name = item.nave.ToString(),
                                    Time = "Reference Name"
                                },
                                new Detail
                                {
                                    Name = item.idExportador.ToString(),
                                    Time = "RUC Exporter "
                                },
                                 new Detail
                                {
                                    Name = item.Exportador.ToString(),
                                    Time = "Exporter Name"
                                },

                                //new Detail
                                //{
                                //   Name = item.oEstado?.nombre,
                                //    Time = "Status Order"
                                //},
                                new Detail
                                {
                                    Name = Convert.ToDateTime(item.fechaCreacion,CultureInfo.InvariantCulture).ToString("dd/MM/yyyy HH:mm"),
                                    Time = "Upload Date"
                                }
                             
                            }
                        });
                    }
                }
            }
            catch
            {

            }
            return resps;
        }
        public async Task<ApiModels.AppModels.Base> RegistraEmbarqueInbox(BAN_Embarque_Cab oEntidad, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistersEmbarqueInbox(oEntidad, photo1, photo2, photo3, photo4).ConfigureAwait(false);
                RespuestaViewModel<BAN_Embarque_Cab> result = res;
                idResultado = res.Respuesta.idEmbarqueCab;
                if (result.Respuesta != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar la orden de embarque: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception ex)
            {
                resp.messages = "Error al registrar la orden de embarque: " + ex.Message;
                resp.response = false;
            }
            return resp;
        }
        public async Task<BAN_Embarque_Cab> GetEmbarqueCab(long _id)
        {
            RespuestaViewModel<BAN_Embarque_Cab> resultDet = new RespuestaViewModel<BAN_Embarque_Cab>();
            BAN_Embarque_Cab _rec;
            Datos datos = new Datos();
            RespuestaViewModel<BAN_Embarque_Cab> result = new RespuestaViewModel<BAN_Embarque_Cab>();

            var resDet = await datos.GetEmbarqueCab(_id).ConfigureAwait(false);
            result = resDet;
            if (result.Resultado.Respuesta)
            {
                _rec = new BAN_Embarque_Cab();
                //_rec.idOrdenDespacho = 0;
                _rec = result.Respuesta;
            }
            else
            {
                _rec = new BAN_Embarque_Cab()
                {
                    response = result.Resultado.Respuesta,
                    messages = result.Resultado.Mensajes[0].Trim()
                };
            }

            return _rec;
        }
        public async Task<ObservableCollection<Tasks>> GetListaMovimientosEmbarque(long _idEmbarqueCab)
        {
            ObservableCollection<Tasks> resps = new ObservableCollection<Tasks>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Embarque_Movimiento>> result = new RespuestaViewModel<List<BAN_Embarque_Movimiento>>();

            try
            {
                //SE OBTIENE EL LISTADO DE RECEPCIONES 
                var res = await datos.GetListaMovimientosEmbarque(_idEmbarqueCab).ConfigureAwait(false);
                result = res;

                if (result.Resultado.Respuesta)
                {
                    //GrabarTareas(result.Respuesta);

                    foreach (var item in result.Respuesta)
                    {
                        //item.oEmbarque_Cab = oEmbarqueCab;
                        string json = JsonConvert.SerializeObject(item);
                        resps.Add(new Tasks
                        {
                            Topic = string.Format("Box Code: {0} \n Hold: {1} \n Deck: {2}", item.codigoCaja, item.oHold?.nombre, item.oPiso?.nombre),
                            Duration = "Type : " + item.oTipoMovimiento?.descripcion == ""? "Embarque": item.oTipoMovimiento?.descripcion,//item.idEmbarqueCab?.ToString("D8"),
                            Color = "#E23B1B",
                            Date = DateTime.Now,
                            QTY = (Int32)item.box,
                            Arrastre = 0,
                            Saldo = 0,
                            Id = item.idEmbarqueMovimiento.ToString(),
                            Details = new ObservableCollection<Detail>
                            {
                                new Detail
                                {
                                    Name = item.box.ToString(),
                                    Time = "Boxes"
                                },
                                new Detail
                                {
                                    Name = item.codigoCaja,
                                    Time = "Box Code"
                                },
                                new Detail
                                {
                                    Name = string.Format("{0} | {1}",item.oHold?.nombre, item.oPiso?.nombre),
                                    Time = "Winery Location"
                                },

                                new Detail
                                {
                                    Name = item.oTipoMovimiento?.descripcion,
                                    Time = "Type"
                                },

                            }
                        }); ;
                    }
                }
                //else
                //{
                //    var vDespacho = await datos.GetOrdenDespacho(_idEmbarqueCab).ConfigureAwait(false);
                //    oEmbarqueCab = vDespacho.Respuesta;
                //}
            }
            catch
            {

            }
            return resps;
        }
        public async Task<ApiModels.AppModels.Base> RegistraMovmientoEmbarqueVBS(BAN_Embarque_Movimiento oRecepcion, byte[] photo1, byte[] photo2, byte[] photo3, byte[] photo4)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistersMovimientoEmbarque(oRecepcion, photo1, photo2, photo3, photo4).ConfigureAwait(false);
                RespuestaViewModel<BAN_Embarque_Movimiento> result = res;
                idResultado = res.Respuesta.idEmbarqueMovimiento;
                if (result.Respuesta != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar la recepción de embarque: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception ex)
            {
                resp.messages = "Error al registrar la recepción: " + ex.Message;
                resp.response = false;
            }
            return resp;
        }

        public async Task<ApiModels.AppModels.Base> RegistraAisvExterno(string _idNave, int _idHold, int _idBodega, int _idBloque, string _aisv, string user)
        {
            ApiModels.AppModels.Base resp = new ApiModels.AppModels.Base();
            try
            {
                Datos datos = new Datos();
                var res = await datos.RegistersAisvExterno(_idNave, _idHold, _idBodega, _idBloque, _aisv, user).ConfigureAwait(false);
                RespuestaViewModel<BAN_Stowage_Plan_Det> result = res;
                if (result.Respuesta != null)
                {
                    resp.messages = result.Resultado.Mensajes[0];
                    resp.response = result.Resultado.Respuesta;
                }
                else
                {
                    resp.messages = "Error al registrar transacción de aisv externo: no se obtuvo respuesta del servidor";
                    resp.response = false;
                }
            }
            catch (Exception ex)
            {
                resp.messages = "Error al registrar aisv externo: " + ex.Message;
                resp.response = false;
            }
            return resp;
        }

        public async Task<List<Combo>> GetListaNaveST()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaNavesST().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }
        public async Task<List<Combo>> GetListaBodegasVBS()
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaBodegasVBS().ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }
        public async Task<List<Combo>> GetListaBloquesVBS(int _idBodega)
        {
            List<Combo> resps = new List<Combo>();
            Datos datos = new Datos();
            RespuestaViewModel<List<BAN_Consulta_Combo>> result = new RespuestaViewModel<List<BAN_Consulta_Combo>>();
            try
            {
                var res = await datos.GetListaBloquesVBS(_idBodega).ConfigureAwait(false);
                result = res;
                if (result.Resultado.Respuesta)
                {
                    foreach (var item in result.Respuesta)
                    {
                        resps.Add(new Combo
                        {
                            Valor = item.id.ToString(),
                            Descripcion = item.nombre
                        });
                    }
                }
                return resps;
            }
            catch
            {
                return resps;
            }
        }

    }
}
