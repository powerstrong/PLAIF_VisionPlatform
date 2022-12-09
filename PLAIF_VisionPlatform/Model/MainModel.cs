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

    public class CalMatrixRow
    {
        public CalMatrixRow()
        {

        }

        public CalMatrixRow(Single _M0, Single _M1, Single _M2, Single _M3)
        {
            M0 = _M0;
            M1 = _M1;
            M2 = _M2;
            M3 = _M3;
        }
        public Single M0 { get; set; }
        public Single M1 { get; set; }
        public Single M2 { get; set; }
        public Single M3 { get; set; }
    }

    public class Vision_Result
    {
        public Vision_Result()
        {

        }

        public Vision_Result(int _Index, int _ModelID, double _Matching_Score, Vector3 _position, Vector4 _orientation)
        {
            Index               = _Index;
            ModelID             = _ModelID;
            Matching_Score      = _Matching_Score;           
            position            = _position;
            strPosition         = string.Format("[{0} {1} {2}]", position.X, position.Y, position.Z);
            orientation         = _orientation;
        }

        public int Index { get; set; }

        public int ModelID { get; set; }
        public double Matching_Score { get; set; }
        public string strPosition { get; set; }
        public Vector3 position { get; set; }
        public Vector4 orientation { get; set; }
    }
}
