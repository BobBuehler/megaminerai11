using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pizza
{
    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;

        public override bool Equals(object obj)
        {
            //if (!(obj is Point))
            //{
            //    return false;
            //}
            Point o = (Point)obj;
            return X == o.X && Y == o.Y;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public override string ToString()
        {
            return String.Format("({0},{1})", X, Y);
        }
    }
}
