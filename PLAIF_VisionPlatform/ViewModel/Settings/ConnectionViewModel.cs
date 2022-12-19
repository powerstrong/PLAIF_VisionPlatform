using Microsoft.Toolkit.Mvvm.Input;
using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Utilities;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    class ConnectionViewModel : INotifyPropertyChanged, Observer
    {
        public ICommand? LoginClick { get; set; }
        public ICommand? ConnectClick { get; set; }

        public ConnectionViewModel()
        {
            Document.Instance.updater.Add(this);
            LoginClick = new RelayCommand<object>(LoginCommand, CanExcute_LoginButton);
            //SshDeleteClick = new AsyncRelayCommand(SshDeleteCommand);
            ConnectClick = new RelayCommand<object>(ConnectCommand, CanExcute_ConnectionButton);

            Document.Instance.CheckConnection();
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

        private void LoginCommand(object parameter)
        {
            sshViewService.CreateWindow();
        }
        public bool CanExcute_LoginButton(object parameter)
        {
            return Document.Instance.IsConnected ? false : true;
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

            if(RosbridgeMgr.Instance.IsConnected == true)
            {
                //Import config_file.yaml
                Document.Instance.IsImported = SSHUtil.DownloadFile(Document.Instance.userinfo.ip_address,
                                    Document.Instance.userinfo.username,
                                    Document.Instance.userinfo.password,
                                    @"config_file.yaml",
                                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PLAIF\\AI Vision");

                string Filtpath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PLAIF\\AI Vision\\config_file.yaml";
                Document.Instance.jsonUtil.Load(Filtpath, JsonUtil.FileType.Type_Yaml);
                Document.Instance.updater.Notify(Observer.Cmd.UpdateFromJson);
                Document.Instance.updater.Notify(Observer.Cmd.UpdateView);
            }
            else
            {
                Document.Instance.IsImported = false;
                Document.Instance.updater.Notify(Observer.Cmd.UpdateView);
            }
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

        public void Update(Observer.Cmd cmd)
        { 
        }
    }
}
