using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiModels.AppModels
{
    public class Tasks
    {
        public string Topic { get; set; }
        public string Duration { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public int QTY { get; set; }
        public int Arrastre { get; set; }
        public int Saldo { get; set; }

        public ObservableCollection<Detail> Details { get; set; }
        public string Color { get; set; }
        public string Id { get; set; }
    }

    public class Detail
    {
        public string Name { get; set; }
        public string Time { get; set; }
    }
}
