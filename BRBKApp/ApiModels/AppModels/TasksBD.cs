using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class TasksBD : Base
    {
       
        [PrimaryKey]
        public Int64 Id_Word { get; set; }
        public Int64 TaskId { get; set; }
        public string Task_name { get; set; }
        public Int64 BlockId { get; set; }
        public string Block_name { get; set; }
        public Int64 UnitId { get; set; }
        public string Container { get; set; }
        public string Type { get; set; }
        public string Temperature { get; set; }
        public string Reference { get; set; }
        public string Booking { get; set; }
        public DateTime Date { get; set; }
        public bool Complete { get; set; }
        public bool Word_status { get; set; }
        public Int64 HeaderWorkId { get; set; }

        public string Action { get; set; }
    }
}
