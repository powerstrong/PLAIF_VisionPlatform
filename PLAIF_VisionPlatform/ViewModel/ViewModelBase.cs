using PLAIF_VisionPlatform.Interface;
using PLAIF_VisionPlatform.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.ViewModel
{
    public class ViewModelBase : Observer
    {
        public ViewModelBase()
        {
            Document.Instance.updater.Add(this);
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
