﻿using Microsoft.Toolkit.Mvvm.Input;
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
    class ConnectionViewModel : INotifyPropertyChanged
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

        private bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
        
        public async Task ConnectCommand()
        {
            string ip_address = Document.Instance.userinfo.ip_address;
            if (ValidateIPv4(ip_address) == false)
            {
                MessageBox.Show("입력한 IP 주소가 형식에 맞지 않습니다");
                return;
            }

            ConnectButtonText = RosbridgeMgr.Instance.IsConnected() ? "Disconnecting..." : "Connecting..";
            Task<bool> task = Task.Run(() =>
            {
                string uri = String.Format("ws://{0}:9090", ip_address);

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
