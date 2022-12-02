using Microsoft.Toolkit.Mvvm.Input;
using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Utilities;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    class ConnectionViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public RelayCommand SshCreateClick { get; set; }
        public IAsyncRelayCommand? SshDeleteClick { get; set; }
        public IAsyncRelayCommand? ConnectClick { get; set; }

        public ConnectionViewModel()
        {
            SshCreateClick = new RelayCommand(SshCreateCommand);
            //SshDeleteClick = new AsyncRelayCommand(SshDeleteCommand);
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

        ConnectionSshViewService sshViewService = new ConnectionSshViewService();

        private void SshCreateCommand()
        {
            sshViewService.CreateWindow();
        }

        public async Task ConnectCommand()
        {
            string ip_address = Document.Instance.userinfo.ip_address;
            if (PowershellUtil.ValidateIPv4(ip_address) == false)
            {
                MessageBox.Show("입력한 IP 주소가 형식에 맞지 않습니다");
                return;
            }

            ConnectButtonText = RosbridgeMgr.Instance.IsConnected() ? "Disconnecting..." : "Connecting..";
            Task<bool> task = Task.Run(() =>
            {
                string uri = String.Format("ws://{0}:9090", ip_address);
                RosbridgeMgr.Instance.Connect(uri);

                return true;
            });
            task.Wait();
            ConnectButtonText = RosbridgeMgr.Instance.IsConnected() ? "Disconnect from ROS" : "Connect to ROS";

            // 연결 수립 후 yaml 데이터 가져오기
            string cmdGetYaml = String.Format("scp {0}@{1}:~/catkin_ws/config/config_file/config_file.yaml .", Document.Instance.userinfo.username, ip_address);
            PowershellUtil.RunPowershell(cmdGetYaml);
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
