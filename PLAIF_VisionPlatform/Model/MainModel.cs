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
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double RX { get; set; }
        public double RY { get; set; }
        public double RZ { get; set; }
        public Pickpose()
        {
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
            RX = 0.0;
            RY = 0.0;
            RZ = 0.0;
        }

        public Pickpose(double x, double y, double z, double rx, double ry, double rz)
        {
            X = x;
            Y = y;
            Z = z;
            RX = rx;
            RY = ry;
            RZ = rz;
        }
    }
}
