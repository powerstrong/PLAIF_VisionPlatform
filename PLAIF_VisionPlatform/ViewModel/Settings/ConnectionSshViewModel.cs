using Microsoft.Toolkit.Mvvm.Input;
using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Utilities;
using PLAIF_VisionPlatform.ViewModel.Command;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    class ConnectionSshViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public RelayCommand<Window> OKWindowCommand { get; private set; }

        public ConnectionSshViewModel()
        {
            OKWindowCommand = new RelayCommand<Window>(OKWindow);
            var userinfo = Document.Instance.userinfo;
            IpAddress = userinfo.ip_address;
            Username = userinfo.username;
            Password = userinfo.password;
        }

        private void OKWindow(Window window)
        {
            if (PowershellUtil.ValidateIPv4(_ipAddress) == false)
            { 
                MessageBox.Show("입력한 IP 주소가 형식에 맞지 않습니다");
                return;
            }
            //if(_username== null)
            //{
            //    MessageBox.Show("사용자 이름을 입력하세요");
            //    return;
            //}

            var userinfo = Document.Instance.userinfo;
            userinfo.ip_address = _ipAddress;
            userinfo.username = _username;
            userinfo.password = _password;

            // local/appdata에 ip, username 저장
            string path_appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PLAIF\\AI Vision";
            Directory.CreateDirectory(path_appdata);
            string path_userinfo = path_appdata + "\\userinfo.ini";
            string data = String.Format("{0}, {1}", _ipAddress, _username);
            File.WriteAllText(path_userinfo, data);

            // powershell script 실행해서 연결 수립
            PowershellUtil.RunPowershellFile(@"./scripts/ssh-connector.ps1", _ipAddress, _username, _password);

            if (window != null)
            {
                window.Close();
            }
        }

        private string _ipAddress;

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        
        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
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
