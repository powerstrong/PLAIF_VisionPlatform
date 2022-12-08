using PLAIF_VisionPlatform.ViewModel.HelixView;
using PLAIF_VisionPlatform.ViewModel.Settings;
using System.Windows.Controls;

namespace PLAIF_VisionPlatform.View.Settings_View
{
    /// <summary>
    /// PickPosePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PickPosePage : Page
    {
        public PickPosePage()
        {
            InitializeComponent();

            var vm = new PickposeViewModel();
            this.DataContext = vm;
            var vgm = new ViewportGeometryModel(new HelixViewInterface(this.vp_pickpose));
            this.vp_pickpose.DataContext = vgm;
            vm.Vgm = vgm;
        }
    }
}
