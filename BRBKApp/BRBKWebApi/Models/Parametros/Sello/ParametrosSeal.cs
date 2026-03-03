using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiWebApi
{
    public class ParametrosSeal
    {
        public class ParametrosConsultaSello : Base
        {
            public string container { get; set; }
            public string seal { get; set; }
            public bool addPosition { get; set; }
            public string position { get; set; }
            public long? idWorkPosition { get; set; }
            public string xmlN4 { get; set; }
            public string respuestaN4 { get; set; }
            public string referencia { get; set; }
            public string grua { get; set; }
            public long? gkey { get; set; }
            public bool bloqueo { get; set; }

            public bool impedimento { get; set; }
            public string impedimentod { get; set; }
            public List<ParametrosRegistraFotoSello> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.container))
                {
                    msg = "Ingrese el numero del contenedor";
                    return 0;
                }

                if (this.gkey == null)
                {
                    msg = "No se encontro el gkey del contenedor";
                    return 0;
                }

                if (this.gkey < 0)
                {
                    msg = "No se encontro el gkey del contenedor";
                    return 0;
                }


                if (string.IsNullOrEmpty(this.seal))
                {
                    msg = "Ingrese el sello";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.position))
                {
                    this.addPosition = false;
                    //msg = "Ingrese la posición";
                    //return 0;
                }
                else
                {
                    this.addPosition = true;
                }

                if (this.impedimento)
                {
                    msg = "La unidad tiene impedimentos, favor verificar: " + this.impedimentod;
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                string oError = string.Empty;

                if (Fotos == null)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                if (Fotos.Count == 0)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                foreach (ParametrosRegistraFotoSello oParametroFotos in Fotos)
                {
                    if (oParametroFotos.PreValidationsTransaction(out oError) == 0)
                    {
                        msg = oError;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(oError))
                {
                    msg = oError;
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraFotoSello : Base
        {
            public byte[] foto { get; set; }
            public string ruta { get; set; }
            public string estado { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.estado))
                {
                    msg = "Debe especificar el estado";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraAsignacionSello: Base
        {
            public string container { get; set; }
            public string sello_CGSA { get; set; }
            public string sello1 { get; set; }
            public string sello2 { get; set; }
            public string sello3 { get; set; }
            public string sello4 { get; set; }
            public string color { get; set; }
            public string ip { get; set; }
            public string mensaje { get; set; }


           
            public long gkey { get; set; }
            public string dataContainer { get; set; }
            public string position { get; set; }
            public string referencia { get; set; }
            public string xmlN4 { get; set; }
            public string respuestaN4 { get; set; }




            public List<ParametrosRegistraFotoSello> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.container))
                {
                    msg = "Ingrese el numero del contenedor";
                    return 0;
                }

                //if (string.IsNullOrEmpty(this.sello_CGSA))
                //{
                //    msg = "Ingrese el sello CGSA";
                //    return 0;
                //}

                if (string.IsNullOrEmpty(this.sello1))
                {
                    msg = "Ingrese el sello1";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.ip))
                {
                    msg = "Ingrese la IP";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                string oError = string.Empty;

                if (Fotos == null)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                if (Fotos.Count == 0)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                foreach (ParametrosRegistraFotoSello oParametroFotos in Fotos)
                {
                    if (oParametroFotos.PreValidationsTransaction(out oError) == 0)
                    {
                        msg = oError;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(oError))
                {
                    msg = oError;
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraPOW : Base
        {
            public string ip { get; set; }
            public string imei { get; set; }
            public string idPosition { get; set; }
            public string namePosition { get; set; }
            public bool estado { get; set; }
            
            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.ip))
                {
                    msg = "Ingrese la dirección IP del dispositivo";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.imei))
                {
                    msg = "Ingrese el imei del dispositivo";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.idPosition))
                {
                    msg = "Ingrese el código de la posición";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.namePosition))
                {
                    msg = "Ingrese el nombre de la posición";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }
              

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosListaPOWN4 : Base
        {
            public string parametro1 { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                /*if (string.IsNullOrEmpty(this.parametro1))
                {
                    msg = "Ingrese el parametro1";
                    return 0;
                }*/


                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosConsultaPOW : Base
        {
            public string idPosition { get; set; }
            public string usuarioCrea { get; set; }

            public string containers { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.usuarioCrea))
                {
                    msg = "Ingrese el user";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosGetDataContainers : Base
        {
            public string numcontainers { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.numcontainers))
                {
                    msg = "Ingrese el número del contenedor";
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosRegistraConfirmacionDescarga : Base
        {
            public long gkey { get; set; }
            public string container { get; set; }
            public string dataContainer { get; set; }

            public string position { get; set; }

            public string referencia { get; set; }

            public string xmlN4 { get; set; }

            public string respuestaN4 { get; set; }

            public bool estado { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
               
                if (this.gkey <= 0)
                {
                    msg = "gkey de contenedor no existe";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.container))
                {
                    msg = "Ingrese el container";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.dataContainer))
                {
                    msg = "data del contenedor no existe";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.position))
                {
                    msg = "ingrese la posición";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }


                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosSelloPreEmbarqueYaforo : Base
        {
            public string container { get; set; }

            public string sello_CGSA { get; set; }
            public string sello1 { get; set; }
            public string sello2 { get; set; }
            public string sello3 { get; set; }
            public string ip { get; set; }
            public string mensaje { get; set; }

            public List<ParametrosRegistraFotoSello> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.container))
                {
                    msg = "Ingrese el numero del contenedor";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.sello_CGSA))
                {
                    msg = "Ingrese el sello CGSA";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.sello1))
                {
                    msg = "Ingrese el sello1";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.ip))
                {
                    msg = "Ingrese la IP";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                string oError = string.Empty;
                /*
                if (Fotos == null)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                if (Fotos.Count == 0)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }
                */
                foreach (ParametrosRegistraFotoSello oParametroFotos in Fotos)
                {
                    if (oParametroFotos.PreValidationsTransaction(out oError) == 0)
                    {
                        msg = oError;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(oError))
                {
                    msg = oError;
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }

        public class ParametrosSelloAsignaExpo : Base
        {
            public string container { get; set; }

            public string sello_CGSA { get; set; }
            public string sello1 { get; set; }
            public string sello2 { get; set; }
            public string sello3 { get; set; }
            public string sello4 { get; set; }
            public string ip { get; set; }
            public string mensaje { get; set; }

            public List<ParametrosRegistraFotoSello> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.container))
                {
                    msg = "Ingrese el numero del contenedor";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.sello_CGSA))
                {
                    msg = "Ingrese el sello CGSA";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.sello1))
                {
                    msg = "Ingrese el sello1";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.ip))
                {
                    msg = "Ingrese la IP";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                string oError = string.Empty;
                /*
                if (Fotos == null)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                if (Fotos.Count == 0)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }
                */
                foreach (ParametrosRegistraFotoSello oParametroFotos in Fotos)
                {
                    if (oParametroFotos.PreValidationsTransaction(out oError) == 0)
                    {
                        msg = oError;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(oError))
                {
                    msg = oError;
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
        public class ParametrosValidaSelloPatio : Base
        {
            public string container { get; set; }

            public string sello_CGSA { get; set; }
            public string ip { get; set; }
            public string mensaje { get; set; }

            public List<ParametrosRegistraFotoSello> Fotos { get; set; }

            public int? PreValidationsTransaction(out string msg)
            {
                if (string.IsNullOrEmpty(this.container))
                {
                    msg = "Ingrese el numero del contenedor";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.sello_CGSA))
                {
                    msg = "Ingrese el sello CGSA";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.ip))
                {
                    msg = "Ingrese la IP";
                    return 0;
                }

                if (string.IsNullOrEmpty(this.Create_user))
                {
                    msg = "Debe especificar el Create_user responsable";
                    return 0;
                }

                string oError = string.Empty;
                /*
                if (Fotos == null)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }

                if (Fotos.Count == 0)
                {
                    msg = "Debe agregar al menos una foto";
                    return 0;
                }
                */
                foreach (ParametrosRegistraFotoSello oParametroFotos in Fotos)
                {
                    if (oParametroFotos.PreValidationsTransaction(out oError) == 0)
                    {
                        msg = oError;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(oError))
                {
                    msg = oError;
                    return 0;
                }

                msg = string.Empty;
                return 1;
            }
        }
    }
}
