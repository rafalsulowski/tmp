using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrafikaKomputerowa1
{
    public class Matrix
    {
        private float[,] data;

        //konstruktor domyslny
        public Matrix()
        {
            data = new float[3, 4];
        }

        //utowrzenie macierzy na podstawie danych
        public Matrix(float a11, float a12, float a13, float a14, float a21, float a22, float a23, float a24, float a31, float a32, float a33, float a34)
        {
            data = new float[3, 4] { { a11, a12, a13, a14 }, { a21, a22, a23, a24 }, { a31, a32, a33, a34 } };
        }

        //przeciazenie operatora [] w celu latwego dostepu do indeksow danych
        public float this[int row, int col]
        {
            get { return data[row, col]; }
            set { data[row, col] = value; }
        }

        //public override string ToString()
        //{
        //    StringBuilder s = new StringBuilder();
        //    s.Append("{ ");
        //    for (int row = 0; row < data.GetLength(0); row++)
        //    {
        //        s.Append("{ ");
        //        for (int col = 0; col < data.GetLength(1); col++)
        //        {
        //            s.Append($"M{row + 1}{col + 1}:{data[row, col]} ");
        //        }
        //        s.Append("}");
        //    }
        //    s.Append(" }");
        //    return s.ToString();
        //}
    }
}
