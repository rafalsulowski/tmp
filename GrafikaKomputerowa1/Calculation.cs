using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GrafikaKomputerowa1
{
    public static class Calculation
    {

        //funkcja do obliczenia kąta w radianach
        public static float CalculateRadians(float angle)
        {
            return (float)((Math.PI / 180) * angle);
        }

        //funkcja do mnozenia macierzy przez wektor
        public static Vector4 MultiplyMatrix(Vector4 v, Matrix4x4 m)
        {
            //obliczanie wspolczynnikow
            float w = v.X * m.M41 + v.Y * m.M42 + v.Z * m.M43 + v.W * m.M44;
            float x = v.X * m.M11 + v.Y * m.M12 + v.Z * m.M13 + v.W * m.M14;
            float y = v.X * m.M21 + v.Y * m.M22 + v.Z * m.M23 + v.W * m.M24;
            float z = v.X * m.M31 + v.Y * m.M32 + v.Z * m.M33 + v.W * m.M34;

            //obliczanie wspolczynnikow
            if (w != 1 && w != 0)
            {
                w /= w;
                x /= w;
                y /= w;
                z /= w;
            }
            return new Vector4(x, y, z, w);
        }

        //funkcja do mnozenia macierzy 3x4 razy wektor
        public static Vector3 MultiplyMatrix(Vector4 v, Matrix m)
        {
            float x = m[0, 0] * v.X + m[0, 1] * v.Y + m[0, 2] * v.Z + m[0, 3] * v.W;
            float y = m[1, 0] * v.X + m[1, 1] * v.Y + m[1, 2] * v.Z + m[1, 3] * v.W;
            float w = m[2, 0] * v.X + m[2, 1] * v.Y + m[2, 2] * v.Z + m[2, 3] * v.W;

            return new Vector3(x, y, w);
        }
    }
}
