using Microsoft.Toolkit.Mvvm.Input;
using PLAIF_VisionPlatform.Model;
using PLAIF_VisionPlatform.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PLAIF_VisionPlatform.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public DelegateCommand StartClick { get; set; }
        public IAsyncRelayCommand StopClick { get; set; }
        public ICommand PauseClick { get; set; }
        public ICommand CaptureClick { get; set; }
        public IAsyncRelayCommand ConnectClick { get; set; }

        private MainModel _mainModel;

        public MainViewModel() 
        {
            StartClick = new DelegateCommand(DelegateTestCommand); // AsyncRelayCommand로 하면 async 함수를 넣을 수 있다.
            StopClick = new AsyncRelayCommand(AsyncTestCommand);
            ConnectClick = new AsyncRelayCommand(ConnectCommand);
            _mainModel = new MainModel();
        }

        //      private int progressValue;
        //      public int ProgressValue
        //      {
        //          get { return progressValue; }
        //          set
        //          {
        //              progressValue = value;
        //              NotifyPropertyChanged(nameof(ProgressValue));
        //          }
        //      }

        public void DelegateTestCommand(object msg)
        {

        }

        public async Task AsyncTestCommand()
        {
            int w = 0;

            // Task.Run을 통해 Task를 생성하고 비동기 작업을 바로 실행
            Task task = Task.Run(() =>
            {
                // 여기 안에 오래 걸리는 작업을 넣어준다
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(500);
                }
            });
            Task<int> task2 = Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(1000);
                    w = i;
                }
                return 5;
            });
            
            w = await task2;
            MessageBox.Show(w.ToString());
        }

        private string uriText = "ws://192.168.218.250:9090";

        public string UriText
        {
            get { return uriText; }
            set { uriText = value; 
				NotifyPropertyChanged(nameof(UriText));
            }
        }

        private string connectButtonText = "Connect to ROS";

        public string ConnectButtonText
        {
            get { return connectButtonText; }
            set { connectButtonText = value; 
				NotifyPropertyChanged(nameof(ConnectButtonText));
            }
        }


        public async Task ConnectCommand()
        {
            ConnectButtonText = _mainModel.IsConnected() ? "Disconnecting..." : "Connecting..";
            Task<bool> task = Task.Run(() =>
            {
                if (_mainModel.Connect(uriText))
                {
                    // to do : 메인모델의 상태가 바뀌기 전 값을 가져오고 있다. 내부에도 async가 있어서 그런듯한데...
                    ConnectButtonText = _mainModel.IsConnected() ? "Connect to ROS" : "Disconnect from ROS";
                }
                
                return true;
            });
            await task;

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
