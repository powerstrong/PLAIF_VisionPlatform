using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PLAIF_VisionPlatform.ViewModel;

namespace PLAIF_VisionPlatform.Model
{
    class MainModel
    {
        public MainModel()
        {

        }
    }

    public class Pickpose
    {
        public double[]? values;
        public Pickpose()
        {
            values = new double[6];
        }

        public Pickpose(double x, double y, double z, double rx, double ry, double rz)
        {
            values = new double[6];
            values[0] = x;
            values[1] = y;
            values[2] = z;
            values[3] = rx;
            values[4] = ry;
            values[5] = rz;
        }
    }
}
