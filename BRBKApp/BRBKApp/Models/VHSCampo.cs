using System;
using System.Collections.Generic;
using System.Text;

namespace BRBKApp.Models
{
    public class VHSCampo
    {
        public VHSCampo() 
        {
            ColorEtiqueta = Helper.Color.ETIQUETA;
            ColorContenido = Helper.Color.CONTENIDO;
            TamanhoEtiqueta = Helper.Medida.ETIQUETA;
            TamanhoContenido = Helper.Medida.CONTENIDO;
        }

        public int Id { get; set; }
        public int GrupoEntidad { get; set; }
        public string Campo { get; set; }
        
        private bool _visible=true;
        public bool Visible 
        { 
            get{ return _visible; }
            set{ _visible = value; }
        }

        public bool EsRealacion { get; set; }
        public string EntidadRelacion { get; set; }
        public string Etiqueta { get; set; }
        public string Contenido { get; set; }
        public int Orden { get; set; }
        public string ColorEtiqueta { get; set; }
        public string ColorContenido { get; set; }
        public int TamanhoEtiqueta { get; set; }
        public int TamanhoContenido { get; set; }
        public string FuenteEtiqueta { get; set; }
        public string FuenteTexto { get; set; }
        public bool SinEtiqueta { get; set; }
        public bool EtiquetaNegrita { get; set; }
        public bool TextoNegrita { get; set; }
        public bool MostrarEtiqueta => !SinEtiqueta;
        public bool MostrarCampo => Visible&&!EsRealacion;
    }
}
