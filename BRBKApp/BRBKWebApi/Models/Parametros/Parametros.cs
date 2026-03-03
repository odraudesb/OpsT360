using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace MiWebApi
{
    public class Parametros
    {

        #region "Usuarios"

        public class ParametrosConsultaOpciones
        {
            public Int64 IdUsuario { get; set; }
            public string Modulo { get; set; }//APP

        }

        public class ParametrosConsultaUsuario : Base
        {
            public Int64? Id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del Usuario";
                    return 0;
                }


                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraUsuario : Base
        {
            public string Names { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Identification { get; set; }
            public string Phone { get; set; }
            public Int64? PositionId { get; set; }
            public Int64? RoleId { get; set; }

            public Int64? CompanyId { get; set; }
            public Int64? IdUserLeader { get; set; }

            public string Email { get; set; }
            public DateTime? WorkDate { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {

                if (string.IsNullOrEmpty(this.Names))
                {
                    msg = "Debe especificar los nombres del usuario ";
                    return 0;

                }

                if (string.IsNullOrEmpty(this.Username))
                {
                    msg = "Debe especificar el usuario";
                    return 0;

                }

                if (string.IsNullOrEmpty(this.Password))
                {
                    msg = "Debe especificar el password del usuario ";
                    return 0;

                }
              
                if (!this.PositionId.HasValue)
                {
                    msg = "Especifique el Áreas del usuario";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosActualizaUsuario : Base
        {
            public Int64? Id { get; set; }
            public string Names { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Identification { get; set; }
            public string Phone { get; set; }
            public Int64? PositionId { get; set; }
            public Int64? RoleId { get; set; }

            public Int64? CompanyId { get; set; }

            public Int64? IdUserLeader { get; set; }

            public string Email { get; set; }

            public DateTime? WorkDate { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del Usuario";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Names))
                {
                    msg = "Debe especificar los nombres del Usuario ";
                    return 0;

                }

                if (string.IsNullOrEmpty(this.Username))
                {
                    msg = "Debe especificar el usuario";
                    return 0;

                }

                if (string.IsNullOrEmpty(this.Password))
                {
                    msg = "Debe especificar el password del usuario ";
                    return 0;

                }

                if (!this.PositionId.HasValue)
                {
                    msg = "Especifique el Áreas del usuario";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Modifie_user))
                {
                    msg = "Debe especificar el usuario que modifica la transacción";
                    return 0;

                }

                msg = string.Empty;
                return 1;
            }
        }

        #endregion

        #region "Roles"
        public class ParametrosConsultaRol : Base
        {
            public Int64? Id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del rol";
                    return 0;
                }

              
                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraRol : Base
        {
            public string Name { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {

                if (string.IsNullOrEmpty(this.Name))
                {
                    msg = "Debe especificar los nombres del rol ";
                    return 0;

                }
             
                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosActualizaRol : Base
        {
            public Int64? Id { get; set; }
            public string Name { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del rol";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Name))
                {
                    msg = "Debe especificar los nombres del rol ";
                    return 0;

                }

                msg = string.Empty;
                return 1;
            }
        }
        #endregion

        #region "Equipos"

        public class ParametrosConsultaEquipo : Base
        {
            public Int64? Id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del equipo";
                    return 0;
                }


                msg = string.Empty;
                return 1;
            }
        }
        public class ParametrosConsultaEquipoImei : Base
        {
            public string Imei { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(Imei))
                {
                    msg = "Especifique IMEI del equipo";
                    return 0;
                }


                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraEquipo : Base
        {
            public string Names { get; set; }
            public string Imei { get; set; }
            public int? PreValidationsTransaction(out string msg)
            {

                if (string.IsNullOrEmpty(this.Names))
                {
                    msg = "Debe especificar el nombre del equipo ";
                    return 0;

                }
                if (string.IsNullOrEmpty(this.Imei))
                {
                    msg = "Debe especificar el IMEI del equipo ";
                    return 0;

                }
                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosActualizaEquipo : Base
        {
            public Int64? Id { get; set; }
            public string Names { get; set; }
            public string Imei { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del equipo";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Names))
                {
                    msg = "Debe especificar el nombre del equipo ";
                    return 0;

                }

                if (string.IsNullOrEmpty(this.Imei))
                {
                    msg = "Debe especificar el IMEI del equipo ";
                    return 0;

                }

                msg = string.Empty;
                return 1;
            }
        }
        #endregion

        #region "Departamento/Area"
        public class ParametrosConsultaDepartamento : Base
        {
            public Int64? Id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del Departamento";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraDepartamento : Base
        {
            public string Description { get; set; }
         
            public int? PreValidationsTransaction(out string msg)
            {

                if (string.IsNullOrEmpty(this.Description))
                {
                    msg = "Debe especificar el nombre del departamento ";
                    return 0;

                }
                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosActualizaDepartamento : Base
        {
            public Int64? Id { get; set; }
            public string Description { get; set; }
         

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del departamento";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Description))
                {
                    msg = "Debe especificar el nombre del departamento ";
                    return 0;

                }

                

                msg = string.Empty;
                return 1;
            }
        }
        #endregion

        #region "Compañias externas"
        public class ParametrosConsultaCompania : Base
        {
            public Int64? Id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código de la empresa externa";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraCompania : Base
        {
            public string Names { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {

                if (string.IsNullOrEmpty(this.Names))
                {
                    msg = "Debe especificar el nombre de la empresa externa ";
                    return 0;

                }
                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosActualizaCompania : Base
        {
            public Int64? Id { get; set; }
            public string Names { get; set; }


            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código de la empresa externa";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Names))
                {
                    msg = "Debe especificar el nombre de la empresa externa";
                    return 0;

                }



                msg = string.Empty;
                return 1;
            }
        }
        #endregion

        #region "Horarios"
        public class ParametrosConsultaHorario : Base
        {
            public Int64? Id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del horario";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraHorario : Base
        {
            public string Name { get; set; }
            public DateTime? Start_time { get; set; }
            public DateTime? End_time { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {

                if (string.IsNullOrEmpty(this.Name))
                {
                    msg = "Debe especificar el nombre del horario";
                    return 0;

                }

                if (!this.Start_time.HasValue)
                {
                    msg = "Debe especificar la hora de inicio del horario";
                    return 0;

                }

                if (!this.End_time.HasValue)
                {
                    msg = "Debe especificar la hora fin del horario";
                    return 0;

                }
                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosActualizaHorario : Base
        {
            public Int64? Id { get; set; }
            public string Name { get; set; }
            public DateTime? Start_time { get; set; }
            public DateTime? End_time { get; set; }


            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del horario";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Name))
                {
                    msg = "Debe especificar el nombre del horario";
                    return 0;

                }

                if (!this.Start_time.HasValue)
                {
                    msg = "Debe especificar la hora de inicio del horario";
                    return 0;

                }

                if (!this.End_time.HasValue)
                {
                    msg = "Debe especificar la hora fin del horario";
                    return 0;

                }


                msg = string.Empty;
                return 1;
            }
        }
        #endregion

        #region "Bloques"
        public class ParametrosConsultaBloque : Base
        {
            public Int64? Id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del Bloque";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraBloque : Base
        {
            public string Name { get; set; }
            public string Description { get; set; }

            public int? Limit { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {

                if (string.IsNullOrEmpty(this.Name))
                {
                    msg = "Debe especificar el nombre del bloque";
                    return 0;

                }

                if (string.IsNullOrEmpty(this.Description))
                {
                    msg = "Debe especificar la descripción del bloque";
                    return 0;

                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosActualizaBloque : Base
        {
            public Int64? Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

            public int? Limit { get; set; }
            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código del bloque";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Name))
                {
                    msg = "Debe especificar el nombre del bloque";
                    return 0;

                }

                if (string.IsNullOrEmpty(this.Description))
                {
                    msg = "Debe especificar la descripción del bloque";
                    return 0;

                }

             

                msg = string.Empty;
                return 1;
            }
        }
        #endregion

      

        #region "Tareas"
        public class ParametrosConsultaTarea : Base
        {
            public Int64? Id { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código de la tarea";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistrTarea : Base
        {
            public string Name { get; set; }
            public string Description { get; set; }

            public bool? Manually  { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {

                if (string.IsNullOrEmpty(this.Name))
                {
                    msg = "Debe especificar el nombre de la tarea";
                    return 0;

                }

                if (string.IsNullOrEmpty(this.Description))
                {
                    msg = "Debe especificar la descripción de la tarea";
                    return 0;

                }

                if (!this.Manually.HasValue)
                {
                    msg = "Debe especificar el tipo de tarea: Manual, Automática";
                    return 0;

                }


                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosActualizaTarea : Base
        {
            public Int64? Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

            public bool? Manually { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (!this.Id.HasValue)
                {
                    msg = "Especifique el código de la tarea";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Name))
                {
                    msg = "Debe especificar el nombre de la tarea";
                    return 0;

                }

                if (string.IsNullOrEmpty(this.Description))
                {
                    msg = "Debe especificar la descripción de la tarea";
                    return 0;

                }

                if (!this.Manually.HasValue)
                {
                    msg = "Debe especificar el tipo de tarea: Manual, Automática";
                    return 0;

                }

                msg = string.Empty;
                return 1;
            }
        }
        #endregion
    }


}
