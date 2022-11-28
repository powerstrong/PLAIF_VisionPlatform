using PLAIF_VisionPlatform.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PLAIF_VisionPlatform
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selected = sidebar.SelectedItem as NavButton;

            navframe.Navigate(selected.Navlink);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            i += 1;
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //확대 되는 마우스 위치 포인트를 체크 할 것
            //축소는 제한 비율은??
            if(e.Delta > 0)
            {
                myScaleTransform.ScaleX *= 1.1;
                myScaleTransform.ScaleY *= 1.1;

                if (myScaleTransform.ScaleX > 2)
                {
                    myScaleTransform.ScaleX = 2;
                    myScaleTransform.ScaleY = 2;
                }
            }
            else
            {
                myScaleTransform.ScaleX /= 1.1;
                myScaleTransform.ScaleY /= 1.1;

                if (myScaleTransform.ScaleX < 1)
                {
                    myScaleTransform.ScaleX = 1;
                    myScaleTransform.ScaleY = 1;
                }
            }

        }
    }
}
