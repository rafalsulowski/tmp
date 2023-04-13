using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GrafikaKomputerowa1
{
    public class Line3D
    {
        public Vector4[] points; //tablica na punkt poczatkowy i koncowy

        public Line3D(Vector4 a, Vector4 b)
        {
            points = new Vector4[2] { a, b };
        }
    }
}
