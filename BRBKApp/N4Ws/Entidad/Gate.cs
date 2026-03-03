using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N4Ws.Entidad
{
    public class gate
    {
        public appointment appointment { get; set; }
        public List<appointment> appointments { get; set; }
        public gate()
        {
            this.appointment = new appointment();
            this.appointments = new List<appointment>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (appointments.Count <= 0)
            {
                sb.Append("<gate><create-appointment> ");
                sb.AppendFormat("<appointment-date>{0}</appointment-date>", this.appointment.appointment_date?.ToString("yyyy-MM-dd"));


                //---Nuevo 20200415 Crear el Tiempo---///
                /*<appointment-time>19:00:00</appointment-time>*/
                var apt = this.appointment.appointment_time.HasValue?this.appointment.appointment_time.Value.ToString("HH:mm:ss") : null;
                if (!string.IsNullOrEmpty(apt))
                {
                    sb.AppendFormat("<appointment-time>{0}</appointment-time>",apt);
                }
                //apointment nuevo


                sb.AppendFormat("<gate-id>{0}</gate-id>", string.IsNullOrEmpty(this.appointment.gate_id) ? "CONTENEDORES" : this.appointment.gate_id);
                sb.AppendFormat("<driver card-id=\"{0}\" />", this.appointment.driver_id);
                sb.AppendFormat("<truck license-nbr=\"{0}\" />", this.appointment.truck_id);
                sb.AppendFormat("<tran-type>{0}</tran-type>", string.IsNullOrEmpty(this.appointment.tran_type) ? "DI" : this.appointment.tran_type);
                sb.AppendFormat("<container eqid=\"{0}\" />", this.appointment.container_id);
                sb.Append("</create-appointment></gate>");
            }
            else
            {
                sb.Append("<gate><cancel-appointment><appointments>");
                foreach (var a in this.appointments)
                {
                    if (!string.IsNullOrEmpty(a.nbr))
                        sb.AppendFormat("<appointment appointment-nbr=\"{0}\" />",a.nbr);
                }
                sb.Append(" </appointments></cancel-appointment></gate>");
            }


            return sb.ToString();
        }


    }

    public class appointment
    {
        public DateTime? appointment_date { get; set; }
        public DateTime? appointment_time { get; set; }
        public string gate_id { get; set; }
        public string driver_id { get; set; }
        public string truck_id { get; set; }
        public string tran_type  { get; set; }
        public string container_id { get; set; }
        public string nbr { get; set; }
    }

    


    /*
   
 
     
   

   
  
   
   
   
 

     
     */
}
