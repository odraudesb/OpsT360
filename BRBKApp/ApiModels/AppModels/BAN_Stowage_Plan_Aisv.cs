using System;
using System.Collections.Generic;

namespace ApiModels.AppModels
{
    [Serializable]
    public class BAN_Stowage_Plan_Aisv : Base
    {
        #region "Propiedades"
        public long idStowageAisv { get; set; }
        public long idStowageDet { get; set; }
        public DateTime fecha { get; set; }
        public int idHoraInicio { get; set; }
        public string horaInicio { get; set; }
        public int idHoraFin { get; set; }
        public string horaFin { get; set; }
        public int box { get; set; }
        public string comentario { get; set; }
        public string aisv { get; set; }
        public string dae { get; set; }
        public string booking { get; set; }
        public bool IIEAutorizada { get; set; }
        public bool daeAutorizada { get; set; }
        public string estado { get; set; }
        public string placa { get; set; }
        public string idChofer { get; set; }
        public string chofer { get; set; }
        public long? idCapacidadHoraBodega { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime fechaCreacion { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? fechaModifica { get; set; }
        public int arrastre { get; set; }
        public int pendiente { get; set; }

        public BAN_HorarioInicial oHoraInicio { get; set; }
        public BAN_HorarioFinal oHoraFin { get; set; }
        public BAN_Stowage_Plan_Det oStowage_Plan_Det { get; set; }
        public List<BAN_Stowage_Movimiento> oListaStowage_Movimiento { get; set; }
        public BAN_Catalogo_Estado oEstados { get; set; }
        #endregion
    }
    [Serializable]
    public class BAN_AISV_Generados : Base
    {
        #region "Propiedades"
        public long vbs_id_hora_cita { get; set; }
        public string aisv_codigo { get; set; }
        public int vbs_box { get; set; }
        public string aisv_codig_clte { get; set; }
        public string aisv_dae { get; set; }
        public string aisv_numero_booking { get; set; }
        public string aisv_contenedor { get; set; }
        public string aisv_cedul_chof { get; set; }
        public string aisv_nombr_chof { get; set; }
        public string aisv_placa_vehi { get; set; }
        public string aisv_estado { get; set; }
        public string idNave { get; set; }
        #endregion
    }
}
