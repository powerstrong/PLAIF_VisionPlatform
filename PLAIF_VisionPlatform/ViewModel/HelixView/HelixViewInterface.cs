using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PLAIF_VisionPlatform.ViewModel.HelixView
{
    class HelixViewInterface
    {
        HelixViewport3D _hvp;

        public HelixViewInterface(HelixViewport3D hvp)
        {
            _hvp = hvp;
        }

        public void AddValue(Visual3D value)
        {
            _hvp.Children.Add(value);
        }

        public void ClearValue()
        {
            _hvp.Children.Clear();
        }

        public bool ContainsVlue(Visual3D value)
        {
            return _hvp.Children.Contains(value);
        }
    }
}
