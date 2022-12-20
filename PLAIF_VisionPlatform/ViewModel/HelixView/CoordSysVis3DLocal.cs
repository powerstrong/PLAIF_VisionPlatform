using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var xaxis = new ArrowVisual3D();
            xaxis.BeginEdit();
            xaxis.Point1 = point;
            xaxis.Point2 = new Point3D(point.X+len, point.Y, point.Z);
            xaxis.Diameter = dia;
            xaxis.Fill = new SolidColorBrush(this.XAxisColor);
            xaxis.EndEdit();
            this.Children.Add(xaxis);

            var yaxis = new ArrowVisual3D();
            yaxis.BeginEdit();
            yaxis.Point1 = point;
            yaxis.Point2 = new Point3D(point.X, point.Y+len, point.Z);
            yaxis.Diameter = dia;
            yaxis.Fill = new SolidColorBrush(this.YAxisColor);
            yaxis.EndEdit();
            this.Children.Add(yaxis);

            var zaxis = new ArrowVisual3D();
            zaxis.BeginEdit();
            zaxis.Point1 = point; 
            zaxis.Point2 = new Point3D(point.X, point.Y, point.Z+len);
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
    }
}
