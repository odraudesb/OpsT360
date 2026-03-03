using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    [Serializable]
    public class sealValidation : Base
    {
        #region "Propiedades"
        public long? id { get; set; }
        public long? pregate { get; set; }
        public string chofer { get; set; }
        public string placa { get; set; }
        public string container { get; set; }
        public string seals { get; set; }
        public string tipo { get; set; }
        public string usuarioCrea { get; set; }
        public bool addposition { get; set; }
        public string position { get; set; }
        public long? idWorkPosition { get; set; }
        public string xmlN4 { get; set; }
        public string respuestaN4 { get; set; }
        public string referencia { get; set; }
        public string grua { get; set; }
        public long? gkey { get; set; }
        public bool bloqueo { get; set; }
        public List<fotoSealValidation> Fotos { get; set; }
        #endregion
    }

    [Serializable]
    public class fotoSealValidation : Base
    {
        #region "Propiedades"                 
        public long? id { get; set; }
        public long idSealValidation { get; set; }
        public sealValidation Sello { get; set; }
        public string ruta { get; set; }
        public string estado { get; set; }
        public estados Estados { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime? fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        #endregion
    }

    //Sellos de asignacion en muelle
    [Serializable]
    public class sealAsignacionMuelle : Base
    {
        #region "Propiedades"
        public long? id { get; set; }
        public string referencia { get; set; }
        public long gkey { get; set; }
        public string container { get; set; }
        public string sello_CGSA { get; set; }
        public string sello1 { get; set; }
        public string sello2 { get; set; }
        public string sello3 { get; set; }
        public string sello4 { get; set; }
        public string color { get; set; }
        public bool estado { get; set; }
        public string usuarioCrea { get; set; }
        public string ip { get; set; }
        public string mensaje { get; set; }


        public string dataContainer { get; set; }
        public string position { get; set; }
        public string xmlN4Discharge { get; set; }
        public string respuestaN4Discharge { get; set; }

        public List<fotoSealValidation> Fotos { get; set; }
        #endregion
    }

    ///////////////////////////////////////////
    //Sellos registro en Pre-Embarque y Aforo
    ///////////////////////////////////////////
    [Serializable]
    public class sealRegistroPreEmbarqueYaforo : Base
    {
        #region "Propiedades"
        public long? id { get; set; }
        public long gkey { get; set; }
        public string container { get; set; }
        public string sello_CGSA { get; set; }
        public string sello1 { get; set; }
        public string sello2 { get; set; }
        public string sello3 { get; set; }
        public bool estado { get; set; }
        public string usuarioCrea { get; set; }
        public string ip { get; set; }
        public string mensaje { get; set; }

        public List<fotoSealValidation> Fotos { get; set; }
        #endregion
    }

    /////////////////////////////////////////////////////////
    //Validacion Sellos Containers (Consolidación de carga)
    // Asigna Sello EXPO
    /////////////////////////////////////////////////////////
    [Serializable]
    public class AsignaSealExpo : Base
    {
        #region "Propiedades"
        public long? id { get; set; }
        public long gkey { get; set; }
        public string container { get; set; }
        public string sello_CGSA { get; set; }
        public string sello1 { get; set; }
        public string sello2 { get; set; }
        public string sello3 { get; set; }
        public string sello4 { get; set; }
        public bool estado { get; set; }
        public string usuarioCrea { get; set; }
        public string ip { get; set; }
        public string mensaje { get; set; }

        public List<fotoSealValidation> Fotos { get; set; }
        #endregion
    }

    //////////////////////////////////
    //Validacion Sellos Patio (Impo)
    //////////////////////////////////
    [Serializable]
    public class ValidaSealPatio : Base
    {
        #region "Propiedades"
        public long? id { get; set; }
        public long gkey { get; set; }
        public string container { get; set; }
        public string sello_CGSA { get; set; }
        public bool estado { get; set; }
        public string usuarioCrea { get; set; }
        public string ip { get; set; }
        public string mensaje { get; set; }

        public List<fotoSealValidation> Fotos { get; set; }
        #endregion
    }
}