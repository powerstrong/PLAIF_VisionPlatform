using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class CameraViewModel : INotifyPropertyChanged, Observer
    {
        ICommand ZividSettingClick;

        public CameraViewModel()
        {
            Document.Instance.updater.Add(this);
            ZividSettingClick = new RelayCommand(OnZividSettingClick);

            this.Update();
        }

        ~CameraViewModel()
        {
            Document.Instance.jsonUtil.jsonVisionSetting["Vision"]["Cam1"]["calibration"]["matrix"] = CalibrationMatrix;

        }

        public new void Update()
        {
            CalibrationMatrix = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]["Cam1"]["calibration"]["matrix"].ToString();
            CameraName = "topic 이름에서 camera name이 들어가는 부분 변경 필요";
        }

        private string calibrationMatrix;

        public string CalibrationMatrix
        {
            get { return calibrationMatrix; }
            set { calibrationMatrix = value; }
        }

        private string cameraName;

        public string CameraName
        {
            get { return cameraName; }
            set { cameraName = value; }
        }

        private string zividSettingFile;

        public string ZividSettingFile
        {
            get { return zividSettingFile; }
            set { zividSettingFile = value; }
        }

        // to do : file open dialog로 zivid setting file 선택
        // ZividSettingFile 문자열이 있으면, 해당 파일을 scp로 리눅스에 전달함. 리눅스 파일 경로는 고정이다.
        private void OnZividSettingClick()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Zivid Setting Files(*.yml)|*.YML|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                ZividSettingFile = ofd.FileName;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
