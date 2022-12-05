using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class CameraViewModel : INotifyPropertyChanged, Observer
    {
        public ICommand ZividSettingClick { get; set; }

        public CameraViewModel()
        {
            Document.Instance.updater.Add(this);
            ZividSettingClick = new RelayCommand(OnZividSettingClick);

            this.UpdateFromJson();
        }

        ~CameraViewModel()
        {


        }

        public void UpdateFromJson()
        {
            try
            {
                CalibrationMatrix = Document.Instance.jsonUtil.jsonVisionSetting!["Vision"]!["Cam1"]!["calibration"]!["matrix"]!.ToString();

                string strTopicColor = Document.Instance.jsonUtil.jsonVisionSetting!["Vision"]!["Cam1"]!["topics"]!["color"]!.ToString();
                string[] strTopicList = strTopicColor.Split('/');
                CameraName = strTopicList[1];
            }
            catch
            {
                MessageBox.Show("Import가 필요합니다.");
            }
        }

        public void UpdateToJson()
        {
            try
            {
                //Document.Instance.jsonUtil.jsonVisionSetting!["Vision"]!["Cam1"]!["calibration"]!["matrix"] = CalibrationMatrix;

                //make topic list from cameraname
                Dictionary<string, string> dicTopicList = new Dictionary<string, string>();

                //get topic key list
                JToken jt = Document.Instance.jsonUtil.jsonVisionSetting!["Vision"]!["Cam1"]!["topics"]!;
                foreach (JProperty j in jt)
                {
                    dicTopicList.Add(j.Name, Document.Instance.jsonUtil.jsonVisionSetting!["Vision"]!["Cam1"]!["topics"]![j.Name]!.ToString());
                }
         
                //change cameraname to topics
                string[] strarrTemp;
                foreach (KeyValuePair<string,string> item in dicTopicList)
                {
                    strarrTemp = item.Value.Split('/');
                    strarrTemp[1] = CameraName;

                    string strTemp = "";
                    for (int i = 1; i < strarrTemp.Length; i++)
                    {
                        strTemp += "/" + strarrTemp[i];
                    }
                    Document.Instance.jsonUtil.jsonVisionSetting!["Vision"]!["Cam1"]!["topics"]![item.Key] = strTemp;
                }
            }
            catch
            {

            }
        }

        private string calibrationMatrix;
        
        public string CalibrationMatrix
        {
            get { return calibrationMatrix; }
            set { calibrationMatrix = value; 
                NotifyPropertyChanged(nameof(calibrationMatrix));
            }
        }

        private string cameraName;

        public string CameraName
        {
            get { return cameraName; }
            set { cameraName = value; 
                NotifyPropertyChanged(nameof(cameraName));
            }
        }

        private string zividSettingFile;

        public string ZividSettingFile
        {
            get { return zividSettingFile; }
            set { zividSettingFile = value;
                Document.Instance.ZividSettingFile = value;
                NotifyPropertyChanged(nameof(zividSettingFile));
            }
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
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
