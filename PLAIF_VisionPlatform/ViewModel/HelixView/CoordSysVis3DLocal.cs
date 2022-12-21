using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PLAIF_VisionPlatform.ViewModel.HelixView
{
    internal class CoordSysVis3DLocal : LabeledCoordSysVis3D
    {
        public CoordSysVis3DLocal(Point3D _point, Vector3D _vector)
        { 
            point = _point;
            vector = _vector;
            OnGeometryChanged();
        }

        private Point3D point;

        public Point3D Point
        {
            get { return point; }
            set { point = value; }
        }

        private Vector3D vector;

        public Vector3D Vector
        {
            get { return vector; }
            set { vector = value; }
        }

        protected override void OnGeometryChanged()
        {
            this.Children.Clear();
            double len = this.ArrowLengths;
            double dia = len * 0.1;
            double lentxt = len * 1.2;

            var vec_xyz = GetVectorXYZAxis(vector.X, vector.Y, vector.Z);

            var xaxis = new ArrowVisual3D();
            xaxis.BeginEdit();
            xaxis.Point1 = point;
            xaxis.Point2 = new Point3D(point.X + vec_xyz[0].X*len, point.Y + vec_xyz[0].Y*len, point.Z + vec_xyz[0].Z*len);
            xaxis.Diameter = dia;
            xaxis.Fill = new SolidColorBrush(this.XAxisColor);
            xaxis.EndEdit();
            this.Children.Add(xaxis);

            var yaxis = new ArrowVisual3D();
            yaxis.BeginEdit();
            yaxis.Point1 = point;
            yaxis.Point2 = new Point3D(point.X + vec_xyz[1].X * len, point.Y + vec_xyz[1].Y * len, point.Z + vec_xyz[1].Z * len);
            yaxis.Diameter = dia;
            yaxis.Fill = new SolidColorBrush(this.YAxisColor);
            yaxis.EndEdit();
            this.Children.Add(yaxis);

            var zaxis = new ArrowVisual3D();
            zaxis.BeginEdit();
            zaxis.Point1 = point; 
            zaxis.Point2 = new Point3D(point.X + vec_xyz[2].X * len, point.Y + vec_xyz[2].Y * len, point.Z + vec_xyz[2].Z * len);
            zaxis.Diameter = dia;
            zaxis.Fill = new SolidColorBrush(this.ZAxisColor);
            zaxis.EndEdit();
            this.Children.Add(zaxis);

            this.Children.Add(new CubeVisual3D { Center = point, SideLength = dia, Fill = Brushes.Black });

            var xlabel = new BillboardTextVisual3D();
            xlabel.Text = XAxisLabel.ToString();
            xlabel.Foreground = new SolidColorBrush(XAxisColor);
            xlabel.FontSize = LabelFontSize;
            xlabel.Position = new Point3D(point.X + vec_xyz[0].X * lentxt, point.Y + vec_xyz[0].Y * lentxt, point.Z + vec_xyz[0].Z * lentxt);
            Children.Add(xlabel);

            var ylabel = new BillboardTextVisual3D();
            ylabel.Text = YAxisLabel.ToString();
            ylabel.Foreground = new SolidColorBrush(YAxisColor);
            ylabel.FontSize = LabelFontSize;
            ylabel.Position = new Point3D(point.X + vec_xyz[1].X * lentxt, point.Y + vec_xyz[1].Y * lentxt, point.Z + vec_xyz[1].Z * lentxt);
            Children.Add(ylabel);

            var zlabel = new BillboardTextVisual3D();
            zlabel.Text = ZAxisLabel.ToString();
            zlabel.Foreground = new SolidColorBrush(ZAxisColor);
            zlabel.FontSize = LabelFontSize;
            zlabel.Position = new Point3D(point.X + vec_xyz[2].X * lentxt, point.Y + vec_xyz[2].Y * lentxt, point.Z + vec_xyz[2].Z * lentxt);
            Children.Add(zlabel);
        }

        private List<Vector3> GetVectorXYZAxis(double rx, double ry, double rz)
        {
            var vec_x = new Vector3();
            var vec_y = new Vector3();
            var vec_z = new Vector3();
            try
            {
                float yaw = (float)ry; //y
                float pitch = (float)rx; //X
                float roll = -(float)rz; //z
                                
                //Quaternion 변환
                System.Numerics.Quaternion quaternion = new System.Numerics.Quaternion();
                quaternion = System.Numerics.Quaternion.CreateFromYawPitchRoll(Convert.ToSingle(yaw * (Math.PI / 180f)), Convert.ToSingle(pitch * (Math.PI / 180f)), Convert.ToSingle(roll * (Math.PI / 180f)));
                //Roll Pitch yaw 각도 전환
                System.Numerics.Matrix4x4 matrix = new System.Numerics.Matrix4x4();
                matrix = System.Numerics.Matrix4x4.CreateFromYawPitchRoll(Convert.ToSingle(yaw * (Math.PI / 180f)), Convert.ToSingle(pitch * (Math.PI / 180f)), Convert.ToSingle(roll * (Math.PI / 180f)));
                vec_x = System.Numerics.Vector3.Transform(new Vector3(1, 0, 0), matrix);
                vec_y = System.Numerics.Vector3.Transform(new Vector3(0, 1, 0), matrix);
                vec_z = System.Numerics.Vector3.Transform(new Vector3(0, 0, 1), matrix);

                // quaternion을 yaw pitch roll로 변환
                (yaw, pitch, roll) = QuaternionToYawPitchRoll(quaternion);
            }
            catch
            {
                MessageBox.Show("Set non-null values for the quaternion.");
            }

            return new List<Vector3> { vec_x, vec_y, vec_z };
        }

        static (float, float, float) QuaternionToYawPitchRoll(System.Numerics.Quaternion q)
        {
            // Extract the yaw, pitch, and roll from the quaternion
            float yaw = (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, 1 - 2 * q.Y * q.Y - 2 * q.Z * q.Z);
            float pitch = (float)Math.Asin(2 * q.X * q.Y + 2 * q.Z * q.W);
            float roll = (float)Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, 1 - 2 * q.X * q.X - 2 * q.Z * q.Z);

            return (yaw, pitch, roll);
        }
    }
}
