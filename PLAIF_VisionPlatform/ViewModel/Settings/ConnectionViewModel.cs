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
using System.Windows.Input;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    class ConnectionViewModel : INotifyPropertyChanged, Observer
    {
        public RelayCommand SshCreateClick { get; set; }
        public IAsyncRelayCommand? SshDeleteClick { get; set; }
        public ICommand? ConnectClick { get; set; }

        public ConnectionViewModel()
        {
            Document.Instance.updater.Add(this);
            SshCreateClick = new RelayCommand(SshCreateCommand);
            //SshDeleteClick = new AsyncRelayCommand(SshDeleteCommand);
            ConnectClick = new RelayCommand<object>(ConnectCommand, CanExcute_ConnectionButton);
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

        public void ConnectCommand(object parameter)
        {
            string ip_address = Document.Instance.userinfo.ip_address;
            if (PowershellUtil.ValidateIPv4(ip_address) == false)
            {
                MessageBox.Show("입력한 IP 주소가 형식에 맞지 않습니다");
                //return Task.CompletedTask;
            }

            ConnectButtonText = RosbridgeMgr.Instance.IsConnected ? "Disconnecting..." : "Connecting..";
            Task<bool> task = Task.Run(() =>
            {
                string uri = string.Format("ws://{0}:9090", ip_address);
                RosbridgeMgr.Instance.Connect(uri);

                return true;
            });
            task.Wait();
            ConnectButtonText = RosbridgeMgr.Instance.IsConnected ? "Disconnect from ROS" : "Connect to ROS";
            //return Task.CompletedTask;

            //Import config_file.yaml
            Document.Instance.IsImported = SSHUtil.DownloadFile(Document.Instance.userinfo.ip_address,
                                Document.Instance.userinfo.username,
                                Document.Instance.userinfo.password,
                                @"config_file.yaml",
                                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PLAIF\\AI Vision");

        }

        public bool CanExcute_ConnectionButton(object parameter)
        {
            return Document.Instance.CanConnectedSSH? true : false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateFromJson()
        {

        }

        public void UpdateToJson()
        {

        }
    }
}
