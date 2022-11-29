using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.ViewModel
{
    class ConnectionViewModel : INotifyPropertyChanged
    {
        public IAsyncRelayCommand? ConnectClick { get; set; }

        public ConnectionViewModel()
        {
            ConnectClick = new AsyncRelayCommand(ConnectCommand);
        }
        private string uriText = "ws://192.168.1.75:9090";
        public string UriText
        {
            get { return uriText; }
            set
            {
                uriText = value;
                NotifyPropertyChanged(nameof(UriText));
            }
        }
        private string connectButtonText = "Connect to ROS";

        public string ConnectButtonText
        {
            get { return connectButtonText; }
            set
            {
                connectButtonText = value;
                NotifyPropertyChanged(nameof(ConnectButtonText));
            }
        }

        public async Task ConnectCommand()
        {
            ConnectButtonText = RosbridgeMgr.Instance.IsConnected() ? "Disconnecting..." : "Connecting..";
            Task<bool> task = Task.Run(() =>
            {
                RosbridgeMgr.Instance.Connect(uriText);

                return true;
            });
            task.Wait();
            ConnectButtonText = RosbridgeMgr.Instance.IsConnected() ? "Disconnect from ROS" : "Connect to ROS";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
