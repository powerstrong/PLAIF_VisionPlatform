using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class PickposeViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ICommand? AddClick;
        public ICommand? DelClick;
        public ICommand? ModClick;
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
