using System;
using PDFExtractor.Primitives;

namespace PDFExtractor
{
    public class Matrix
    {
        double[,] array;


        public double A => array[0, 0];
        public double B => array[0, 1];
        public double C => array[0, 2];
        public double D => array[1, 0];
        public double E => array[1, 1];
        public double F => array[1, 2];
        public double G => array[2, 0];
        public double H => array[2, 1];
        public double I => array[2, 2];

        public Matrix(double[,] arr)
        {
            if (arr.Length != 9)
                throw new Exception("Array to small");
            array = arr;
        }

        public Matrix(double a1, double a2, double a3, double a4, double a5, double a6, double a7, double a8, double a9)
        {
            array = new double[,] {
                {a1,a2,a3},
                {a4,a5,a6},
                {a7,a8,a9},
            };
        }

        public Matrix()
        {
            array = new double[,] {
                {0,0,0},
                {0,0,0},
                {0,0,0},
            };
        }

        public static Matrix GetIdenity() => new Matrix(1,0,0,0,1,0,0,0,1);
        
        public static Matrix GetPointMatrix(double x, double y) => new Matrix(1, 0, 0, 0, 1, 0, x, y, 1);

        public static Matrix operator *(Matrix l, Matrix r)
        {
            return new Matrix(
                ((l.A * r.A) + (l.B * r.D) + (l.C * r.G)), ((l.A * r.B) + (l.B * r.E) + (l.C * r.H)), ((l.A * r.C) + (l.B * r.F) + (l.C * r.I)),
                ((l.D * r.A) + (l.E * r.D) + (l.F * r.G)), ((l.D * r.B) + (l.E * r.E) + (l.F * r.H)), ((l.D * r.C) + (l.E * r.F) + (l.F * r.I)),
                ((l.G * r.A) + (l.H * r.D) + (l.I * r.G)), ((l.G * r.B) + (l.H * r.E) + (l.I * r.H)), ((l.G * r.C) + (l.H * r.F) + (l.I * r.I)));
        }

        public PointD ToPointD() => new PointD(G,H);
        
        public void CopyTo(Matrix toMatrix) 
        {
            toMatrix.array[0,0] = A;
            toMatrix.array[0,1] = B;
            toMatrix.array[0,2] = C;
            toMatrix.array[1,0] = D;
            toMatrix.array[1,1] = E;
            toMatrix.array[1,2] = F;
            toMatrix.array[2,0] = G;
            toMatrix.array[2,1] = H;
            toMatrix.array[2,2] = I;
        }
    }


}
