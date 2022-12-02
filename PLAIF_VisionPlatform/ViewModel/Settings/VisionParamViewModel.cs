using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class VisionParamViewModel : INotifyPropertyChanged, Observer
    {
		public VisionParamViewModel()
		{
            Document.Instance.updater.Add(this);
            this.Update();
        }

        private string? actionName;

		public string ActionName
        {
			get { return actionName; }
			set { actionName = value; }
		}

		private string w;

		public string W
		{
			get { return w; }
			set { w = value; }
		}

		private string h;

		public string H
		{
			get { return h; }
			set { h = value; }
		}

		private string xs;

		public string Xs
		{
			get { return xs; }
			set { xs = value; }
		}

		private string xe;

		public string Xe
		{
			get { return xe; }
			set { xe = value; }
		}

		private string ys;

		public string Ys
		{
			get { return ys; }
			set { ys = value; }
		}

		private string ye;

		public string Ye
		{
			get { return ye; }
			set { ye = value; }
		}

        public new void Update()
		{
            ActionName = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]["vision_node1"]["action"]["action_name"].ToString();
            W = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]["vision_node1"]["preprocessing"]["resize"][0].ToString();
            H = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]["vision_node1"]["preprocessing"]["resize"][1].ToString();
            Xs = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]["vision_node1"]["preprocessing"]["roi"][0].ToString();
            Xe = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]["vision_node1"]["preprocessing"]["roi"][1].ToString();
            Ys = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]["vision_node1"]["preprocessing"]["roi"][2].ToString();
            Ye = Document.Instance.jsonUtil.jsonVisionSetting["Vision"]["vision_node1"]["preprocessing"]["roi"][3].ToString();
        }

		public event PropertyChangedEventHandler? PropertyChanged;
    }
}
