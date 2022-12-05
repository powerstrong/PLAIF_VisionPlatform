using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLAIF_VisionPlatform.Interface
{
    public interface IObservable
    {
        void Add(Observer ob);
        void Remove(Observer ob);
        void NotifyFromJson();

        void NotifyToJson();
    }
}
