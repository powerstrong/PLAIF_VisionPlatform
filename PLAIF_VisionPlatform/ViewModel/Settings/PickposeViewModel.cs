using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class PickposeViewModel : INotifyPropertyChanged, Observer
    {
        public ICommand? ImportPlyClick;
        public ICommand? AddClick;
        public ICommand? DelClick;
        public ICommand? ModClick;
        public event PropertyChangedEventHandler? PropertyChanged;

        public PickposeViewModel()
        {
            Document.Instance.updater.Add(this);
            this.Update();
        }

        public new void Update()
        {
            
        }
    }
}
