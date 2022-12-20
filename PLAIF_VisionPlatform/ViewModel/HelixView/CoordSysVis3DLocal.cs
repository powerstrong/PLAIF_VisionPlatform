using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PLAIF_VisionPlatform.ViewModel.HelixView
{
    internal class CoordSysVis3DLocal : LabeledCoordSysVis3D
    {
        public CoordSysVis3DLocal(Point3D _point, Vector4 _vector)
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

        private Vector4 vector;

        public Vector4 Vector
        {
            get { return vector; }
            set { vector = value; }
        }

        protected override void OnGeometryChanged()
        {
            this.Children.Clear();
            double len = this.ArrowLengths;
            double dia = len * 0.1;

            var pt_x_end = MovePtByQuaternions(point, new Vector4(vector.X, 0, 0, vector.W), len);
            var pt_y_end = MovePtByQuaternions(point, new Vector4(0, vector.Y, 0, vector.W), len);
            var pt_z_end = MovePtByQuaternions(point, new Vector4(0, 0, vector.Z, vector.W), len);

            var xaxis = new ArrowVisual3D();
            xaxis.BeginEdit();
            xaxis.Point1 = point;
            xaxis.Point2 = pt_x_end;
            xaxis.Diameter = dia;
            xaxis.Fill = new SolidColorBrush(this.XAxisColor);
            xaxis.EndEdit();
            this.Children.Add(xaxis);

            var yaxis = new ArrowVisual3D();
            yaxis.BeginEdit();
            yaxis.Point1 = point;
            yaxis.Point2 = pt_y_end;
            yaxis.Diameter = dia;
            yaxis.Fill = new SolidColorBrush(this.YAxisColor);
            yaxis.EndEdit();
            this.Children.Add(yaxis);

            var zaxis = new ArrowVisual3D();
            zaxis.BeginEdit();
            zaxis.Point1 = point;
            zaxis.Point2 = pt_z_end;
            zaxis.Diameter = dia;
            zaxis.Fill = new SolidColorBrush(this.ZAxisColor);
            zaxis.EndEdit();
            this.Children.Add(zaxis);

            this.Children.Add(new CubeVisual3D { Center = point, SideLength = dia, Fill = Brushes.Black });

            var xlabel = new BillboardTextVisual3D();
            xlabel.Text = XAxisLabel.ToString();
            xlabel.Foreground = new SolidColorBrush(XAxisColor);
            xlabel.FontSize = LabelFontSize;
            xlabel.Position = new Point3D(point.X + ArrowLengths * 1.2, point.Y, point.Z);
            Children.Add(xlabel);

            var ylabel = new BillboardTextVisual3D();
            ylabel.Text = YAxisLabel.ToString();
            ylabel.Foreground = new SolidColorBrush(YAxisColor);
            ylabel.FontSize = LabelFontSize;
            ylabel.Position = new Point3D(point.X, point.Y + ArrowLengths * 1.2, point.Z);
            Children.Add(ylabel);

            var zlabel = new BillboardTextVisual3D();
            zlabel.Text = ZAxisLabel.ToString();
            zlabel.Foreground = new SolidColorBrush(ZAxisColor);
            zlabel.FontSize = LabelFontSize;
            zlabel.Position = new Point3D(point.X, point.Y, point.Z + ArrowLengths * 1.2);
            Children.Add(zlabel);
        }

        private Point3D MovePtByQuaternions(Point3D from, Vector4 quat, double dist)
        {
            double[,] R = new double[3, 3]
            {
                { 1 - 2*quat.Y*quat.Y - 2*quat.Z*quat.Z,   2*quat.X*quat.Y - 2*quat.W*quat.Z,   2*quat.X*quat.Z + 2*quat.W*quat.Y },
                {   2*quat.X*quat.Y + 2*quat.W*quat.Z, 1 - 2*quat.X*quat.X - 2*quat.Z*quat.Z,   2*quat.Y*quat.Z - 2*quat.W*quat.X },
                {   2*quat.X*quat.Z - 2*quat.W*quat.Y,   2*quat.Y*quat.Z + 2*quat.W*quat.X, 1 - 2*quat.X*quat.X - 2*quat.Y*quat.Y }
            };

            double x1_new = from.X * R[0, 0] + from.Y * R[0, 1] + from.Z * R[0, 2];
            double y1_new = from.X * R[1, 0] + from.Y * R[1, 1] + from.Z * R[1, 2];
            double z1_new = from.X * R[2, 0] + from.Y * R[2, 1] + from.Z * R[2, 2];

            return new Point3D(x1_new, y1_new, z1_new);
        }

        private void aa()
        {

        }
    }
}
