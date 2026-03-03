using BRBKApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BRBKApp.ViewModels
{
    public class VHSTarjaPhotosViewModel : BaseViewModel
    {
        public ObservableCollection<VHSDataModel<TarjaPhoto>> PhotosTarja {  get; set; }

    }
}
