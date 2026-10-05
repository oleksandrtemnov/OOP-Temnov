using System;

namespace Lab4Task
{
    public class Vector3D
    {
        private double _x;
        private double _y;
        private double _z;

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        public static Vector3D Zero
        {
            get { return new Vector3D(0, 0, 0); }
        }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double this[int index]
        {
            get
            {
                if (index == 0) return X;
                if (index == 1) return Y;
                if (index == 2) return Z;
                throw new IndexOutOfRangeException("Індекс повинен бути 0, 1 або 2.");
            }
            set
            {
                if (index == 0) X = value;
                else if (index == 1) Y = value;
                else if (index == 2) Z = value;
                else throw new IndexOutOfRangeException("Індекс повинен бути 0, 1 або 2.");
            }
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static double operator *(Vector3D v1, Vector3D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static bool operator ==(Vector3D v1, Vector3D v2)
        {
            if (ReferenceEquals(v1, v2)) return true;
            if (v1 is null || v2 is null) return false;
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return !(v1 == v2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3D other)
            {
                return Math.Abs(X - other.X) < 1e-9 &&
                       Math.Abs(Y - other.Y) < 1e-9 &&
                       Math.Abs(Z - other.Z) < 1e-9;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override string ToString()
        {
            return "Vector3D(" + X + ", " + Y + ", " + Z + ")";
        }
    }

    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Vector3D v1 = new Vector3D(1.5, 2.0, 3.0);
            Vector3D v2 = new Vector3D(4.0, -1.0, 0.5);
            Console.WriteLine("v1 = " + v1);
            Console.WriteLine("v2 = " + v2);

            Vector3D zero = Vector3D.Zero;
            Console.WriteLine("Vector3D.Zero = " + zero);

            v1.X = 2.5;
            Console.WriteLine("Після зміни v1.X = 2.5: " + v1);

            Console.WriteLine("v1[0] = " + v1[0] + ", v1[1] = " + v1[1] + ", v1[2] = " + v1[2]);
            v1[2] = 5.0;
            Console.WriteLine("Після v1[2] = 5.0: " + v1);

            Vector3D sum = v1 + v2;
            Console.WriteLine("v1 + v2 = " + sum);

            double dotProduct = v1 * v2;
            Console.WriteLine("Скалярний добуток (v1 * v2) = " + dotProduct);

            Vector3D v3 = new Vector3D(2.5, 2.0, 5.0);
            Console.WriteLine("v1 == v3: " + (v1 == v3));
            Console.WriteLine("v1 != v2: " + (v1 != v2));
            Console.WriteLine("v1.Equals(v3): " + v1.Equals(v3));
        }
    }
}