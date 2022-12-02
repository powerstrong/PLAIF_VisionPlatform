using PLAIF_VisionPlatform.View.Settings_View;
using PLAIF_VisionPlatform.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.Interface
{
    interface IWindowService
    {
        void CreateWindow();
    }
    class ConnectionSshViewService : IWindowService
    {
        public void CreateWindow()
        {
            ConnectionSshView view = new ConnectionSshView
            {
                DataContext = new ConnectionSshViewModel()
            };
            view.Show();
        }
    }
}
