using Microsoft.Toolkit.Mvvm.Input;
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

        public MainViewModel() 
        {
            StartClick = new DelegateCommand(DelegateTestCommand); // AsyncRelayCommand로 하면 async 함수를 넣을 수 있다.
            StopClick = new AsyncRelayCommand(AsyncTestCommand);
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

        public event PropertyChangedEventHandler? PropertyChanged;
        //private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
