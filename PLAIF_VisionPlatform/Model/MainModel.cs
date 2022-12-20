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
        public double QX { get; set; }
        public double QY { get; set; }
        public double QZ { get; set; }
        public double QW { get; set; }
        public Pickpose()
        {
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
            QX = 0.0;
            QY = 0.0;
            QZ = 0.0;
            QW = 0.0;
        }

        public Pickpose(double x, double y, double z, double qx, double qy, double qz, double qw)
        {
            X = x;
            Y = y;
            Z = z;
            QX = qx;
            QY = qy;
            QZ = qz;
            QW = qw;
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
            strPosition = "[0 0 0]";
        }

        public Vision_Result(int _Index, int _ModelID, string _Matching_Score_Cond, string _Matching_Score_Result, Vector3 _position, Vector4 _orientation)
        {
            Index                   = _Index;
            ModelID                 = _ModelID;
            Matching_Score_Result   = _Matching_Score_Result;
            Matching_Score_Cond     = _Matching_Score_Cond;
            strMatching_Score       = string.Format("[{0} >= {1}]", _Matching_Score_Result, _Matching_Score_Cond);
            position                = _position;
            strPosition             = string.Format("[{0} {1} {2}]", position.X, position.Y, position.Z);
            orientation             = _orientation;
        }

        public int Index { get; set; }

        public int ModelID { get; set; }
        public string Matching_Score_Result { get; set; }

        public string Matching_Score_Cond { get; set; }

        public string strMatching_Score { get; set; }
        public string strPosition { get; set; }
        public Vector3 position { get; set; }
        public Vector4 orientation { get; set; }
    }
}
