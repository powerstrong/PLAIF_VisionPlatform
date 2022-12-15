using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.View.Settings_View;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class VisionParamViewModel : INotifyPropertyChanged, Observer
    {
		public VisionParamViewModel() 
		{
            Document.Instance.updater.Add(this);

            //this.UpdateFromJson();
        }

        private string minMatcingPoints;

        public string MinMatcingPoints
        {
            get { return minMatcingPoints; }
            set { 
                minMatcingPoints = value;
                NotifyPropertyChanged(nameof(minMatcingPoints));
            }
        }


        private string actionName = "";

		public string ActionName
        {
			get { return actionName; }
            set { 
				actionName = value; 
                NotifyPropertyChanged(nameof(actionName));
            }
        }

        private string w = "";

		public string W
		{
			get { return w; }
			set { w = value; 
                NotifyPropertyChanged(nameof(w));
            }
        }

		private string h = "";

        public string H
		{
			get { return h; }
			set { h = value; 
                NotifyPropertyChanged(nameof(h));
            }
        }

		private string xs = "";

        public string Xs
		{
			get { return xs; }
			set { xs = value; 
                NotifyPropertyChanged(nameof(xs));
            }
        }

		private string xe = "";

        public string Xe
		{
			get { return xe; }
			set { xe = value; 
                NotifyPropertyChanged(nameof(xe));
            }
        }

		private string ys = "";

        public string Ys
		{
			get { return ys; }
			set { ys = value; 
                NotifyPropertyChanged(nameof(ys));
            }
        }

		private string ye = "";

        public string Ye
		{
			get { return ye; }
			set { ye = value; 
                NotifyPropertyChanged(nameof(ye));
            }
        }
        
        public void Update(Observer.Cmd cmd)
        {
            switch (cmd)
            {
                case Observer.Cmd.UpdateFromJson:
                    this.UpdateFromJson();
                    break;
                case Observer.Cmd.UpdateToJson:
                    UpdateToJson();
                    break;
            }
        }

        public void UpdateFromJson()
        {
            try
            {
                if (Document.Instance.jsonUtil.jsonVisionSetting != null &&
                    Document.Instance.jsonUtil.jsonVisionSetting.HasValues == true)
                {
                    ActionName = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["action"]!["action_name"]!.ToString();
                    W = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["resize"]![0]!.ToString();
                    H = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["resize"]![1]!.ToString();
                    Xs = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["roi"]![0]!.ToString();
                    Xe = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["roi"]![1]!.ToString();
                    Ys = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["roi"]![2]!.ToString();
                    Ye = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["roi"]![3]!.ToString();

                    MinMatcingPoints = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["min_matching_points"]!.ToString();
                }
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
                if (Document.Instance.jsonUtil.jsonVisionSetting != null &&
                    Document.Instance.jsonUtil.jsonVisionSetting.HasValues == true)
                {
                    Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["action"]!["action_name"] = ActionName;
                    Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["resize"]![0] = W;
                    Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["resize"]![1] = H;
                    Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["roi"]![0] = Xs;
                    Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["roi"]![1] = Xe;
                    Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["roi"]![2] = Ys;
                    Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["roi"]![3] = Ye;
                    Document.Instance.jsonUtil.jsonVisionSetting["Vision"]!["vision_node1"]!["preprocessing"]!["min_matching_points"] = MinMatcingPoints;
                }
            }
            catch
            {

            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
