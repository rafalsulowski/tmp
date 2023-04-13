using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GrafikaKomputerowa1
{
    public class Line2D
    {
        public Vector3[] points; //tablica na punkt poczatkowy i koncowy

        public Line2D(Vector3 a, Vector3 b)
        {
            points = new Vector3[2] { a, b };
        }
    }
}
