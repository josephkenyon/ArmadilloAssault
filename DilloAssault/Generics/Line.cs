using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DilloAssault.Generics
{
    public class Line
    {
        public Vector2 A { get; set; }
        public Vector2 B { get; set; }

        public Line(int x1, int y1, int x2, int y2)
        {
            A = new Vector2(x1, y1);
            B = new Vector2(x2, y2);
        }

        public Line(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }

        //private Vector2? GetIntersection(Line line)
        //{
        //    var s1 = new Vector2(B.X - A.X, B.Y - A.Y);
        //    var s2 = new Vector2(line.B.X - line.A.X, line.B.Y - line.A.Y);

        //    float s, t;
        //    s = (-s1.Y * (A.X - line.A.X) + s1.X * (A.Y - line.A.Y)) / (-s2.X * s1.Y + s1.X * s2.Y);
        //    t = (s2.X * (A.Y - line.A.Y) - s2.Y * (A.X - line.A.X)) / (-s2.X * s1.Y + s1.X * s2.Y);

        //    if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        //    {
        //        return new Vector2(A.X + (t * s1.X), A.Y + (t * s2.Y));
        //    }

        //    return null;
        //}

        // Function to find the orientation of three Vector2s
        private static int Orientation(Vector2 p, Vector2 q, Vector2 r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
            if (val == 0)
                return 0; // Collinear
            return (val > 0) ? 1 : 2; // Clockwise or counterclockwise
        }

        // Function to check if two segments intersect
        private static bool DoIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
        {
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }

        // Function to check if Vector2 q lies on segment p-r
        private static bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }

        // Function to find the intersection Vector2 of two line segments
        private static Vector2? FindIntersection(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
        {
            // If segments don't intersect, return NaN
            if (!DoIntersect(p1, q1, p2, q2))
                return null;

            // Line equations
            double A1 = q1.Y - p1.Y;
            double B1 = p1.X - q1.X;
            double C1 = A1 * p1.X + B1 * p1.Y;

            double A2 = q2.Y - p2.Y;
            double B2 = p2.X - q2.X;
            double C2 = A2 * p2.X + B2 * p2.Y;

            double determinant = A1 * B2 - A2 * B1;

            // Calculate intersection Vector2
            Vector2 intersection;
            intersection.X = (float)((C1 * B2 - C2 * B1) / determinant);
            intersection.Y = (float)((A1 * C2 - A2 * C1) / determinant);

            return intersection;
        }

        public Vector2? GetIntersection(Line line)
        {
            return FindIntersection(A, B, line.A, line.B);
        }

        public Vector2? GetIntersection(LineQuad lineQuad)
        {
            List<Vector2?> list = [];

            list.Add(lineQuad.Left.GetIntersection(this));
            list.Add(lineQuad.Right.GetIntersection(this));
            list.Add(lineQuad.Top.GetIntersection(this));
            list.Add(lineQuad.Bottom.GetIntersection(this));

            List<Vector2> intersections = list.Where(vector => vector != null).Select(vector => (Vector2)vector).ToList();

            if (intersections.Count > 0)
            {
                return intersections.OrderBy(vector => MathUtils.DistanceBetweenTwoVectors(vector, A)).First();
            }

            return null;
        }
    }
}
