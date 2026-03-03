using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Runtime.CompilerServices; // Para CallerMemberName

namespace BRBKApp.Models
{
    public class AppModelVHSTarjaDetalle : INotifyPropertyChanged
    {
        private long _detalleTarjaID;
        public long DetalleTarjaID
        {
            get => _detalleTarjaID;
            set => SetProperty(ref _detalleTarjaID, value);
        }

        private int _tarjaID;
        public int TarjaID
        {
            get => _tarjaID;
            set => SetProperty(ref _tarjaID, value);
        }

        private string _tipoCargaDescripcion;
        public string TipoCargaDescripcion
        {
            get => _tipoCargaDescripcion;
            set => SetProperty(ref _tipoCargaDescripcion, value);
        }

        private string _informacionVehiculo;
        public string InformacionVehiculo
        {
            get => _informacionVehiculo;
            set => SetProperty(ref _informacionVehiculo, value);
        }

        private string _ubicacionBodega;
        public string UbicacionBodega
        {
            get => _ubicacionBodega;
            set => SetProperty(ref _ubicacionBodega, value);
        }

        private string _documentoTransporte;
        public string DocumentoTransporte
        {
            get => _documentoTransporte;
            set => SetProperty(ref _documentoTransporte, value);
        }

        private string _packingList;
        public string PackingList
        {
            get => _packingList;
            set => SetProperty(ref _packingList, value);
        }

        private string _vin;
        public string VIN
        {
            get => _vin;
            set => SetProperty(ref _vin, value);
        }

        private string _numeroMotor;
        public string NumeroMotor
        {
            get => _numeroMotor;
            set => SetProperty(ref _numeroMotor, value);
        }

        private string _observaciones;
        public string Observaciones
        {
            get => _observaciones;
            set => SetProperty(ref _observaciones, value);
        }

        private List<VHSTarjaDetalleFoto> _fotos;
        public List<VHSTarjaDetalleFoto> Fotos
        {
            get => _fotos;
            set => SetProperty(ref _fotos, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class VHSTarjaDetalleFoto : INotifyPropertyChanged
    {
        private int _fotoID;
        public int FotoID
        {
            get => _fotoID;
            set => SetProperty(ref _fotoID, value);
        }

        private int _detalleTarjaID;
        public int DetalleTarjaID
        {
            get => _detalleTarjaID;
            set => SetProperty(ref _detalleTarjaID, value);
        }

        private string _fotosVehiculo;
        public string FotosVehiculo
        {
            get => _fotosVehiculo;
            set => SetProperty(ref _fotosVehiculo, value);
        }

        private byte[] _arrayFoto;
        public byte[] ArrayFoto
        {
            get => _arrayFoto;
            set => SetProperty(ref _arrayFoto, value);
        }

        private int _orden;
        public int Orden
        {
            get => _orden;
            set => SetProperty(ref _orden, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}