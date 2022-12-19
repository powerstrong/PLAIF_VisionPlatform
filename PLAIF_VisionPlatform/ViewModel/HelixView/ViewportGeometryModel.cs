using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace PLAIF_VisionPlatform.ViewModel.HelixView
{
    public class ViewportGeometryModel
    {
        public enum CirclePlane
        {
            PLANE_XY,
            PLANE_XZ,
            PLANE_YZ
        }

        protected PointsVisual3D m_pointsVisual; //3D point cloud for magnetometer points
        protected Point3DCollection m_points;   

        public PointsVisual3D calpointsVisual
        {
            get
            {
                return m_pointsVisual;
            }
        }

        public Point3DCollection Points
        {
            get
            {
                return m_points;
            }

            set
            {
                m_points = value;
            }
        }

        private HelixViewInterface _hvi;

        public Model3D GeometryModel { get; set; } //this is bound to the viewport
        protected const int POINTSIZE = 3; //display size for magnetometer points

        private Model3DGroup modelGroup;

        /// <summary>
        /// Initializes a new instance of the MagViewerGeometryModel class.
        /// </summary>
        public ViewportGeometryModel(HelixViewInterface hvi)
        {
            m_pointsVisual = new PointsVisual3D { Color = Colors.Red, Size = POINTSIZE };
            _hvi = hvi;

            Points = new Point3DCollection();
            m_pointsVisual.Points = Points;
            hvi.AddValue(m_pointsVisual);
        }

        public void AddVisual3Ds(List<Visual3D> visual3Ds)
        {
            foreach (var v3d in visual3Ds)
                _hvi.AddValue(v3d);
        }

        public void ClearVisual3Ds()
        {
            _hvi.ClearValue();
        }

        public void InitializeVisual3Ds()
        {
            ClearVisual3Ds();
            var v3ds = new List<Visual3D>();

            //var pt = new Point3D(2, 2, 2);
            //var vec = new Vector3D(1, 1, 1);
            //CoordSysVis3DLocal coord = new(pt, vec);
            //coord.ArrowLengths = 0.1;
            //coord.Transform.Transform(new Point3D(1000, 10, 0));
            //v3ds.Add(coord);

            SunLight light = new();
            v3ds.Add(light);

            AddVisual3Ds(v3ds);
        }


        public void ReplaceVisual3Ds(List<Visual3D> visual3Ds)
        {
            ClearVisual3Ds();
            AddVisual3Ds(visual3Ds);
        }
        

        public void DrawRefCircles(HelixViewport3D viewport, double radius = 1, bool bEnable = false)
        {
            //// Create a model group
            if (modelGroup == null)
            {
                modelGroup = new Model3DGroup();// Create an empty model group if necessary
            }
            else //already exists - remove all children so we can start over
            {
                modelGroup.Children.Clear();
            }

            if (bEnable) //if checkbox state is 'checked', create the ref circle objects
            {
                // Create the materials (colors) we will need
                var greenMaterial = MaterialHelper.CreateMaterial(Colors.Green);
                var redMaterial = MaterialHelper.CreateMaterial(Colors.Red);
                var blueMaterial = MaterialHelper.CreateMaterial(Colors.Blue);

                //create reference circles using TubeVisual3D objects
                //probably should do this using transforms, but don't know how :-(
                double thicknessfactor = 0.05; //established empirically

                //ring in XY plane
                TubeVisual3D t_xy = new TubeVisual3D();
                //t_xy.Fill = System.Windows.Media.Brushes.Black;
                Point3DCollection p3dc = GenerateCirclePoints(36, radius, CirclePlane.PLANE_XY);
                t_xy.Diameter = thicknessfactor * radius; //1% factor emperically determined
                t_xy.Material = blueMaterial; //to match viewport coord sys colors
                t_xy.Path = p3dc;
                modelGroup.Children.Add(t_xy.Model);

                //ring in Xz plane
                TubeVisual3D t_xz = new TubeVisual3D();
                t_xz.Material = greenMaterial;
                p3dc = GenerateCirclePoints(36, radius, CirclePlane.PLANE_XZ);
                t_xz.Material = greenMaterial; //to match viewport coord sys colors
                t_xz.Path = p3dc;
                t_xz.Diameter = t_xy.Diameter;
                modelGroup.Children.Add(t_xz.Model);

                ////ring in yz plane
                TubeVisual3D t_yz = new TubeVisual3D();
                p3dc = GenerateCirclePoints(36, radius, CirclePlane.PLANE_YZ);
                t_yz.Diameter = t_xy.Diameter;
                t_yz.Material = redMaterial; //to match viewport coord sys colors
                t_yz.Path = p3dc;
                modelGroup.Children.Add(t_yz.Model);
            }

            // GeometryModel is bound to HelixViewport3D with the line
            //  <ModelVisual3D Content="{Binding GeometryModel}"/> in MainWindow.xaml)
            GeometryModel = modelGroup;
        }

        public Point3DCollection GeneratePoints(int n, double time)
        {
            Point3DCollection pc = new Point3DCollection();
            const double R = 2;
            const double Q = 0.5;
            for (int i = 0; i < n; i++)
            {
                double t = Math.PI * 2 * i / (n - 1);
                double u = t * 24 + time * 5;
                var pt = new Point3D(Math.Cos(t) * (R + Q * Math.Cos(u)), Math.Sin(t) * (R + Q * Math.Cos(u)), Q * Math.Sin(u));
                pc.Add(pt);
            }
            return pc;
        }

        public Point3DCollection GenerateCirclePoints(int numpts = 100, double radius = 1, CirclePlane plane = CirclePlane.PLANE_XY)
        {
            double d_theta = Math.PI * 2 / numpts;
            Point3DCollection p3dc = new Point3DCollection();

            switch (plane)
            {
                case CirclePlane.PLANE_XY:
                    for (int i = 0; i <= numpts; i++)
                    {
                        p3dc.Add(new Point3D(Math.Cos(i * d_theta) * radius, Math.Sin(i * d_theta) * radius, 0));
                        Debug.Print("point " + i + ": " + p3dc[i].ToString());
                    }
                    break;
                case CirclePlane.PLANE_XZ:
                    for (int i = 0; i <= numpts; i++)
                    {
                        p3dc.Add(new Point3D(Math.Cos(i * d_theta) * radius, 0, Math.Sin(i * d_theta) * radius));
                        Debug.Print("point " + i + ": " + p3dc[i].ToString());
                    }
                    break;
                case CirclePlane.PLANE_YZ:
                    for (int i = 0; i <= numpts; i++)
                    {
                        p3dc.Add(new Point3D(0, Math.Cos(i * d_theta) * radius, Math.Sin(i * d_theta) * radius));
                        Debug.Print("point " + i + ": " + p3dc[i].ToString());
                    }
                    break;
                default:
                    break;
            }

            return p3dc;
        }
    }
}
