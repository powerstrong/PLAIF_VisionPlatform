using PLAIF_VisionPlatform.Model;
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

    class PickPoseDefineViewService : IWindowService
    {
        public void CreateWindow()
        {
            var view = new PickPoseDefineView
            {
                DataContext = new PickPoseDefineViewModel()
            };
            view.Show();
        }
        public void CreateWindow(Pickpose pose)
        {
            var vm = new PickPoseDefineViewModel();
            vm.Pose = pose;
            vm.X = pose.X.ToString();
            vm.Y = pose.Y.ToString();
            vm.Z = pose.Z.ToString();
            vm.Rx = pose.RX.ToString();
            vm.Ry = pose.RY.ToString();
            vm.Rz = pose.RZ.ToString();
            
            var view = new PickPoseDefineView();
            view.DataContext = vm;
            view.Show();
        }
    }
}
