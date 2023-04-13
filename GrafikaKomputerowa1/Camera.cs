using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace GrafikaKomputerowa1
{
    internal class Camera
    {
        public Matrix4x4 stage;
        public Matrix projectionTo2d; //macierz 3 na 4
        public List<Line3D> Lines { get; set; }
        public float Focal { get; set; }
        private float FocalConstant { get; set; }
        public float RotationStep { get; set; }
        public float MoveStep { get; set; }
        public float FieldOfViewX { get; set; }
        public float FieldOfViewY { get; set; }

        public Camera(double width, double height, List<Line3D> lines)
        {
            Configure((float)width, (float)height);
            Lines = lines;
        }



        private void Configure(float screenWidth, float screenHeight)
        {
            FieldOfViewX = screenWidth / 2;
            FieldOfViewY = screenHeight / 2;
            MoveStep = 10f;
            RotationStep = 1f;
            Focal = 400f;
            FocalConstant = 400f;

            projectionTo2d = new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0);
            
            stage = Matrix4x4.Identity; //zwroc macierz startowa
            // Ustaw polorzenie domyslne - poczatkowe
            stage.M14 = -150f;
            stage.M24 = -100f;
            stage.M34 = 250f;
        }

        public void SetNewLines(List<Line3D> lines)
        {
            Lines = lines;
        }

        private Matrix4x4 BuildRotationXMatrix(float angle)
        {
            Matrix4x4 rotationX = Matrix4x4.Identity;
            float rad = Calculation.CalculateRadians(angle);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            rotationX.M22 = cos;
            rotationX.M23 = -sin;
            rotationX.M32 = sin;
            rotationX.M33 = cos;

            return rotationX;
        }


        private Matrix4x4 BuildRotationYMatrix(float angle)
        {
            Matrix4x4 rotationY = Matrix4x4.Identity;
            float rad = Calculation.CalculateRadians(angle);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            rotationY.M11 = cos;
            rotationY.M13 = sin;
            rotationY.M31 = -sin;
            rotationY.M33 = cos;

            return rotationY;
        }


        private Matrix4x4 BuildRotationZMatrix(float angle)
        {
            Matrix4x4 rotationZ = Matrix4x4.Identity;
            float rad = Calculation.CalculateRadians(angle);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            rotationZ.M11 = cos;
            rotationZ.M12 = -sin;
            rotationZ.M21 = sin;
            rotationZ.M22 = cos;

            return rotationZ;
        }

        public void Rotate(string axis, int direction, int forClear = 1)
        {
            switch (axis)
            {
                case "X":
                    stage = BuildRotationXMatrix(direction * RotationStep * forClear);
                    break;
                case "Y":
                    stage = BuildRotationYMatrix(direction * RotationStep * forClear);
                    break;
                case "Z":
                    stage = BuildRotationZMatrix(direction * RotationStep * forClear);
                    break;
            }
        }

        public void Zoom(float a)
        {
            stage = Matrix4x4.Identity;
            if (Focal + a > 0)
            {
                Focal += a;
            }
        }

        public void ClearZoom()
        {
            Focal = FocalConstant;
        }

        public void Move(int x, int y, int z)
        {
            stage = Matrix4x4.Identity;
            stage.M14 = x * MoveStep;
            stage.M24 = y * MoveStep;
            stage.M34 = z * MoveStep;
        }
    }
}
