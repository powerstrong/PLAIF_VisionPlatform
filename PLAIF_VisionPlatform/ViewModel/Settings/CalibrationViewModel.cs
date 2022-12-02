using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using PLAIF_VisionPlatform.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    class CalibrationViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public RelayCommand OpenClick { get; set; }
        public IAsyncRelayCommand? SendClick { get; set; }

        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                NotifyPropertyChanged(nameof(FilePath));
            }
        }


        public CalibrationViewModel()
        {
            OpenClick = new RelayCommand(OpenCommand);
            SendClick = new AsyncRelayCommand(SendCommand);
        }

        public void OpenCommand()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Calibration Matrix Files(*.ABC)|*.ABC|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                FilePath = ofd.FileName;
            }
        }

        public async Task SendCommand()
        {
            Task<bool> task = Task.Run(() =>
            {
                MessageBox.Show("To do : RosbridgeMgr.PublishMsg 활용해서 메시지 발행");
                //RosbridgeMgr.Instance.PublishMsg("chatter", "hello world", "{'key':1, 'value':'asdf'}");
                return true;
            });
            await task;
            return;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
