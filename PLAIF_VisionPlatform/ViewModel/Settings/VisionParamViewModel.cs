using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.ViewModel.Settings
{
    internal class VisionParamViewModel : ViewModelBase, INotifyPropertyChanged
    {
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

		public event PropertyChangedEventHandler? PropertyChanged;
    }
}
