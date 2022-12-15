using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.Interface
{
    public interface Observer
    {
        enum Cmd
        {
            UpdateFromJson,
            UpdateToJson,
            UpdateView,
            RedrawPickPoseView,
            RedrawPcdView,
            Redraw3dView,
        }

        void Update(Cmd cmd);
    }
}
