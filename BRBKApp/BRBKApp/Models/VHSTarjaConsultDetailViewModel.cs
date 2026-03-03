using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class VHSTarjaConsultDetailViewModel
    {
        public AppModelVHSTarjaDetalle Detalle { get; set; }

        public VHSTarjaConsultDetailViewModel(AppModelVHSTarjaDetalle detalle)
        {
            Detalle = detalle;
        }
    }
}
