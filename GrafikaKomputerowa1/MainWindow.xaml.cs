using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GrafikaKomputerowa1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private Camera VirtualCamera { get; set; } //obiekt kamery


        public MainWindow()
        {
            InitializeComponent();
            VirtualCamera = new Camera(cameraSpace.Width, cameraSpace.Height, new List<Line3D>());
        }

        private void Button_Load(object sender, RoutedEventArgs e)
        {
            //okienko do wyboru pliku
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                //odczyt pliku
                string line;
                List<Line3D> lines = new List<Line3D>();
                using(StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        //pominiecie komentarzy
                        if (line.StartsWith("#") || line.Length < 5)
                            continue;

                        line = line.Trim().Replace("[", "").Replace("]", "");
                        string[] points = line.Split(",");
                        float[] pointsAsNumber = new float[6];

                        for(int i = 0; i < points.Length; i++)
                        {
                            pointsAsNumber[i] = float.Parse(points[i]);
                        }

                        lines.Add(new Line3D(new Vector4(pointsAsNumber[0], pointsAsNumber[1], pointsAsNumber[2], 1),
                            new Vector4(pointsAsNumber[3], pointsAsNumber[4], pointsAsNumber[5], 1)));
                    }
                }


                //zaladowanie linii do kamery
                VirtualCamera.SetNewLines(lines);

                //narysowanie widkou
                Draw();

            }

        }


        private void KeyPushed(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.U:
                    VirtualCamera.Rotate("Z", -1);
                    if(int.Parse(axisZ.Content.ToString()) % 90 == 0)
                        Thread.Sleep(400);
                    axisZ.Content = int.Parse(axisZ.Content.ToString()) - VirtualCamera.RotationStep;
                    Draw();
                    break;
                case Key.O:
                    VirtualCamera.Rotate("Z", 1);
                    if (int.Parse(axisZ.Content.ToString()) % 90 == 0)
                        Thread.Sleep(400);
                    axisZ.Content = int.Parse(axisZ.Content.ToString()) + VirtualCamera.RotationStep;
                    Draw();
                    break;
                case Key.I:
                    VirtualCamera.Rotate("X", 1);
                    if (int.Parse(axisX.Content.ToString()) % 90 == 0)
                        Thread.Sleep(400);
                    axisX.Content = int.Parse(axisX.Content.ToString()) + VirtualCamera.RotationStep;
                    Draw();
                    break;
                case Key.K:
                    VirtualCamera.Rotate("X", -1); 
                    if (int.Parse(axisX.Content.ToString()) % 90 == 0)
                        Thread.Sleep(400);
                    axisX.Content = int.Parse(axisX.Content.ToString()) - VirtualCamera.RotationStep;
                    Draw();
                    break;
                case Key.J:
                    VirtualCamera.Rotate("Y", 1);
                    if (int.Parse(axisY.Content.ToString()) % 90 == 0)
                        Thread.Sleep(400);
                    axisY.Content = int.Parse(axisY.Content.ToString()) + VirtualCamera.RotationStep;
                    Draw();
                    break;
                case Key.L:
                    VirtualCamera.Rotate("Y", -1);
                    if (int.Parse(axisY.Content.ToString()) % 90 == 0)
                        Thread.Sleep(400);
                    axisY.Content = int.Parse(axisY.Content.ToString()) - VirtualCamera.RotationStep;
                    Draw();
                    break;
                case Key.W:
                    VirtualCamera.Move(0, 0, -1);
                    Draw();
                    break;
                case Key.S:
                    VirtualCamera.Move(0, 0, 1);
                    Draw();
                    break;
                case Key.A:
                    VirtualCamera.Move(1, 0, 0);
                    Draw();
                    break;
                case Key.D:
                    VirtualCamera.Move(-1, 0, 0);
                    Draw();
                    break;
                case Key.Q:
                    VirtualCamera.Move(0, -1, 0);
                    Draw();
                    break;
                case Key.E:
                    VirtualCamera.Move(0, 1, 0);
                    Draw();
                    break;
                case Key.M:
                    VirtualCamera.Zoom(5);
                    Zoom.Content = (((VirtualCamera.Focal - 400) / 400) * 100).ToString();
                    Draw();
                    break;
                case Key.N:
                    VirtualCamera.Zoom(-5);
                    Zoom.Content = (((VirtualCamera.Focal - 400) / 400) * 100).ToString();
                    Draw();
                    break;
            }
        }

        private void Draw()
        {
            cameraSpace.Children.Clear();

            foreach (Line3D line in VirtualCamera.Lines)
            {
                line.points[0] = Calculation.MultiplyMatrix(line.points[0], VirtualCamera.stage);
                line.points[1] = Calculation.MultiplyMatrix(line.points[1], VirtualCamera.stage);

                DrawLineOnCanvas(new Line2D(CastPointFrom3dTo2d(line.points[0]), CastPointFrom3dTo2d(line.points[1])));
            }

        }
        private void DrawLineOnCanvas(Line2D line2d)
        {
            Line line = new Line
            {
                Visibility = Visibility.Visible,
                Stroke = Brushes.White,

                X1 = line2d.points[0].X + VirtualCamera.FieldOfViewX,
                X2 = line2d.points[1].X + VirtualCamera.FieldOfViewX,
                Y1 = cameraSpace.Height - (line2d.points[0].Y + VirtualCamera.FieldOfViewY),
                Y2 = cameraSpace.Height - (line2d.points[1].Y + VirtualCamera.FieldOfViewY)
            };

            cameraSpace.Children.Add(line);
        }

        private Vector3 CastPointFrom3dTo2d(Vector4 point)
        {
            if (point.Z <= 0)
                point.Z = (float)0.1;

            VirtualCamera.projectionTo2d[0, 0] = VirtualCamera.Focal / point.Z;
            VirtualCamera.projectionTo2d[1, 1] = VirtualCamera.Focal / point.Z;
            VirtualCamera.projectionTo2d[2, 2] = VirtualCamera.Focal / point.Z;
            return Calculation.MultiplyMatrix(point, VirtualCamera.projectionTo2d);

        }

        private void Button_ClearX(object sender, RoutedEventArgs e)
        {
            
            int toRotate = -int.Parse(axisX.Content.ToString());
            VirtualCamera.Rotate("X", 1, toRotate);
            axisX.Content = "0";
            Draw();
        }

        private void Button_ClearY(object sender, RoutedEventArgs e)
        {
            int toRotate = -int.Parse(axisY.Content.ToString());
            VirtualCamera.Rotate("Y", 1, toRotate);
            axisY.Content = "0";
            Draw();
        }

        private void Button_ClearZ(object sender, RoutedEventArgs e)
        {
            int toRotate = -int.Parse(axisZ.Content.ToString());
            VirtualCamera.Rotate("Z", 1, toRotate);
            axisZ.Content = "0";
            Draw();
        }

        private void Button_ClearZoom(object sender, RoutedEventArgs e)
        {
            VirtualCamera.ClearZoom();
            Zoom.Content = "0";
            Draw();
        }
    }
}
