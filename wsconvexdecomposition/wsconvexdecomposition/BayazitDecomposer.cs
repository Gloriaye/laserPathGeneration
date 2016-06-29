using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;



namespace wsconvexdecomposition
{
    public static class BayazitDecomposer
    {
        //   wswrite  MathfEpsilon==Mathf.Epsilon
        public const float MathfEpsilon = 0.01f;
        public static float concaveThread  = 0.0f;

        //返回在i位置的vector2
        private static Vector2 At(int i, List<Vector2> vertices)
        {
            int s = vertices.Count;
            return vertices[i < 0 ? s - (-i % s) : i % s];
        }

        private static List<Vector2> Copy(int i, int j, List<Vector2> vertices)
        {
            List<Vector2> p = new List<Vector2>();
            while (j < i) j += vertices.Count;
            //p.reserve(j - i + 1);
            for (; i <= j; ++i)
            {
                p.Add(At(i, vertices));
            }
            return p;
        }


        /// <summary>
        /// Decompose the polygon into several smaller non-concave polygon.
        /// If the polygon is already convex, it will return the original polygon, unless it is over Settings.MaxPolygonVertices.
        /// Precondition: Counter Clockwise polygon
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static List<List<Vector2>> ConvexPartition(List<Vector2> vertices, float thread)
        {
            //We force it to CCW as it is a precondition in this algorithm.
            ForceCounterClockWise(vertices);

            concaveThread = thread;

            List<List<Vector2>> list = new List<List<Vector2>>();
            float d, lowerDist, upperDist;
            Vector2 p;
            Vector2 lowerInt = new Vector2();
            Vector2 upperInt = new Vector2(); // intersection points
            int lowerIndex = 0, upperIndex = 0;
            List<Vector2> lowerPoly, upperPoly;

            for (int i = 0; i < vertices.Count; ++i)
            {
                if (mReflex(i, vertices))
                {
                    lowerDist = upperDist = float.MaxValue; // std::numeric_limits<qreal>::max();
                    for (int j = 0; j < vertices.Count; ++j)          //求两交点的索引
                    {
                        // if line intersects with an edge
                        if (Left(At(i - 1, vertices), At(i, vertices), At(j, vertices)) &&
                            RightOn(At(i - 1, vertices), At(i, vertices), At(j - 1, vertices)))
                        {
                            // find the point of intersection
                            p = LineIntersect(At(i - 1, vertices), At(i, vertices), At(j, vertices),
                                                          At(j - 1, vertices));
                            if (Right(At(i + 1, vertices), At(i, vertices), p))
                            {
                                // make sure it's inside the poly
                                d = SquareDist(At(i, vertices), p);
                                if (d < lowerDist)
                                {
                                    // keep only the closest intersection
                                    lowerDist = d;
                                    lowerInt = p;
                                    lowerIndex = j;
                                }
                            }
                        }

                        if (Left(At(i + 1, vertices), At(i, vertices), At(j + 1, vertices)) &&
                            RightOn(At(i + 1, vertices), At(i, vertices), At(j, vertices)))
                        {
                            p = LineIntersect(At(i + 1, vertices), At(i, vertices), At(j, vertices),
                                                          At(j + 1, vertices));
                            if (Left(At(i - 1, vertices), At(i, vertices), p))
                            {
                                d = SquareDist(At(i, vertices), p);
                                if (d < upperDist)
                                {
                                    upperDist = d;
                                    upperIndex = j;
                                    upperInt = p;
                                }
                            }
                        }
                    }

                    // if there are no vertices to connect to, choose a point in the middle
                    if (lowerIndex == (upperIndex + 1) % vertices.Count)       //两交点间没有其它的点
                    {
                        Vector2 sp = ((lowerInt + upperInt) / 2);         //取中间点

                        lowerPoly = Copy(i, upperIndex, vertices);      //分割成两个多边形  
                        lowerPoly.Add(sp);
                        upperPoly = Copy(lowerIndex, i, vertices);
                        upperPoly.Add(sp);
                    }
                    else
                    {
                        double highestScore = 0, bestIndex = lowerIndex;
                        while (upperIndex < lowerIndex) upperIndex += vertices.Count;
                        for (int j = lowerIndex; j <= upperIndex; ++j)
                        {
                            if (CanSee(i, j, vertices))
                            {
                                double score = 1 / (SquareDist(At(i, vertices), At(j, vertices)) + 1);
                                if (Reflex(j, vertices))
                                {
                                    if (RightOn(At(j - 1, vertices), At(j, vertices), At(i, vertices)) &&
                                        LeftOn(At(j + 1, vertices), At(j, vertices), At(i, vertices)))
                                    {
                                        score += 3;
                                    }
                                    else
                                    {
                                        score += 2;
                                    }
                                }
                                else
                                {
                                    score += 1;
                                }
                                if (score > highestScore)
                                {
                                    bestIndex = j;
                                    highestScore = score;
                                }
                            }
                        }
                        lowerPoly = Copy(i, (int)bestIndex, vertices);
                        upperPoly = Copy((int)bestIndex, i, vertices);
                    }
                    list.AddRange(ConvexPartition(lowerPoly, thread));
                    list.AddRange(ConvexPartition(upperPoly, thread));
                    return list;
                }
            }

            // polygon is already convex
            list.Add(vertices);

            //The polygons are not guaranteed to be without collinear points. We remove
            //them to be sure.
            for (int i = 0; i < list.Count; i++)
            {
                //list[i] = SimplifyTools.CollinearSimplify(list[i], 0);
            }

            //Remove empty vertice collections
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Count == 0)
                    list.RemoveAt(i);
            }

            return list;
        }

        private static bool CanSee(int i, int j, List<Vector2> vertices)
        {
            if (Reflex(i, vertices))
            {
                if (LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices)) &&
                    RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices))) return false;
            }
            else
            {
                if (RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices)) ||
                    LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices))) return false;
            }
            if (Reflex(j, vertices))
            {
                if (LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices)) &&
                    RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices))) return false;
            }
            else
            {
                if (RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices)) ||
                    LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices))) return false;
            }
            for (int k = 0; k < vertices.Count; ++k)
            {
                if ((k + 1) % vertices.Count == i || k == i || (k + 1) % vertices.Count == j || k == j)
                {
                    continue; // ignore incident edges
                }
                Vector2 intersectionPoint;
                if (LineIntersect2(At(i, vertices), At(j, vertices), At(k, vertices), At(k + 1, vertices), out intersectionPoint))
                {
                    return false;
                }
            }
            return true;
        }

        // precondition: ccw
        private static bool Reflex(int i, List<Vector2> vertices)
        {
            return Right(i, vertices);
        }

        private static bool mReflex(int i, List<Vector2> vertices)
        {
            return mRight(i, vertices);
        }

        private static bool Right(int i, List<Vector2> vertices)
        {
            return Right(At(i - 1, vertices), At(i, vertices), At(i + 1, vertices));
        }

        private static bool mRight(int i, List<Vector2> vertices)
        {
            return mRight(At(i - 1, vertices), At(i, vertices), At(i + 1, vertices));
        }

        private static bool Left(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c) > 0;
        }

        private static bool LeftOn(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c) >= 0;
        }

        private static bool Right(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c) < 0;
        }

        private static bool mRight(Vector2 a, Vector2 b, Vector2 c)
        {
            return mArea(ref a, ref b, ref c) < -concaveThread;
        }


        private static float mArea(ref Vector2 a, ref Vector2 b, ref Vector2 c)
        {
            return (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / SquareDist(a, c) / (float)(Math.Log((double)(SquareDist(a, c) + 1)) + 1);
        }

        private static bool RightOn(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c) <= 0;
        }

        private static float SquareDist(Vector2 a, Vector2 b)
        {
            float dx = b.x - a.x;
            float dy = b.y - a.y;
            return dx * dx + dy * dy;
        }

        //forces counter clock wise order.
        private static void ForceCounterClockWise(List<Vector2> vertices)
        {
            if (!IsCounterClockWise(vertices))
            {
                vertices.Reverse();
            }
        }

        private static bool IsCounterClockWise(List<Vector2> vertices)
        {
            //We just return true for lines
            if (vertices.Count < 3)
                return true;

            return (GetSignedArea(vertices) > 0.0f);
        }

        //gets the signed area.
        private static float GetSignedArea(List<Vector2> vertices)
        {
            int i;
            float area = 0;

            for (i = 0; i < vertices.Count; i++)
            {
                int j = (i + 1) % vertices.Count;
                area += vertices[i].x * vertices[j].y;
                area -= vertices[i].y * vertices[j].x;
            }
            area /= 2.0f;
            return area;
        }

        //From Mark Bayazit's convex decomposition algorithm
        private static Vector2 LineIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            Vector2 i = new Vector2(0, 0);
            float a1 = p2.y - p1.y;
            float b1 = p1.x - p2.x;
            float c1 = a1 * p1.x + b1 * p1.y;
            float a2 = q2.y - q1.y;
            float b2 = q1.x - q2.x;
            float c2 = a2 * q1.x + b2 * q1.y;
            float det = a1 * b2 - a2 * b1;

            if (!FloatEquals(det, 0))
            {
                // lines are not parallel
                i.x = (b2 * c1 - b1 * c2) / det;
                i.y = (a1 * c2 - a2 * c1) / det;
            }
            return i;
        }

        //from Eric Jordan's convex decomposition library, it checks if the lines a0->a1 and b0->b1 cross.
        //if they do, intersectionPoint will be filled with the point of crossing. Grazing lines should not return true.
        private static bool LineIntersect2(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1, out Vector2 intersectionPoint)
        {
            //intersectionPoint = Vector2.zero;
            intersectionPoint = new Vector2(0, 0);

            if (a0 == b0 || a0 == b1 || a1 == b0 || a1 == b1)
                return false;

            float x1 = a0.x;
            float y1 = a0.y;
            float x2 = a1.x;
            float y2 = a1.y;
            float x3 = b0.x;
            float y3 = b0.y;
            float x4 = b1.x;
            float y4 = b1.y;

            //AABB early exit
            if (Math.Max(x1, x2) < Math.Min(x3, x4) || Math.Max(x3, x4) < Math.Min(x1, x2))
                return false;

            if (Math.Max(y1, y2) < Math.Min(y3, y4) || Math.Max(y3, y4) < Math.Min(y1, y2))
                return false;

            float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3));
            float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3));
            float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            //if (Math.Abs(denom) < Mathf.Epsilon)
            if (Math.Abs(denom) < MathfEpsilon)
            {
                //Lines are too close to parallel to call
                return false;
            }
            ua /= denom;
            ub /= denom;

            if ((0 < ua) && (ua < 1) && (0 < ub) && (ub < 1))
            {
                intersectionPoint.x = (x1 + ua * (x2 - x1));
                intersectionPoint.y = (y1 + ua * (y2 - y1));
                return true;
            }

            return false;
        }

        private static bool FloatEquals(float value1, float value2)
        {
            return Math.Abs(value1 - value2) <= MathfEpsilon;
        }

        // Returns a positive number if c is to the left of the line going from a to b. Positive number if point is left,
        //negative if point is right,and 0 if points are collinear.</returns>
        private static float Area(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c);
        }

        //returns a positive number if c is to the left of the line going from a to b. Positive number if point is left, negative if point is right, and 0 if points are collinear.</returns>
        private static float Area(ref Vector2 a, ref Vector2 b, ref Vector2 c)
        {
            return a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
        }

        //removes all collinear points on the polygon.
        private static List<Vector2> CollinearSimplify(List<Vector2> vertices, float collinearityTolerance)
        {
            //We can't simplify polygons under 3 vertices
            if (vertices.Count < 3)
                return vertices;

            List<Vector2> simplified = new List<Vector2>();

            for (int i = 0; i < vertices.Count; i++)
            {
                int prevId = PreviousIndex(vertices, i);
                int nextId = NextIndex(vertices, i);

                Vector2 prev = vertices[prevId];
                Vector2 current = vertices[i];
                Vector2 next = vertices[nextId];

                //If they collinear, continue
                if (Collinear(ref prev, ref current, ref next, collinearityTolerance))
                    continue;

                simplified.Add(current);
            }

            return simplified;
        }

        //gets the previous index.
        private static int PreviousIndex(List<Vector2> vertices, int index)
        {
            if (index == 0)
            {
                return vertices.Count - 1;
            }
            return index - 1;
        }

        //nexts the index.
        private static int NextIndex(List<Vector2> vertices, int index)
        {
            if (index == vertices.Count - 1)
            {
                return 0;
            }
            return index + 1;
        }

        private static bool Collinear(ref Vector2 a, ref Vector2 b, ref Vector2 c, float tolerance)
        {
            return FloatInRange(Area(ref a, ref b, ref c), -tolerance, tolerance);
        }

        //checks if a floating point Value is within a specified range of values (inclusive).
        private static bool FloatInRange(float value, float min, float max)
        {
            return (value >= min && value <= max);
        }
    }
}

public struct Vector2
{
    public float x;
    public float y;

    public Vector2(float x1, float y1)
    {
        this.x = x1;
        this.y = y1;
    }

    public static Vector2 operator +(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x + v2.x, v1.y + v2.y);
    }

    public static Vector2 operator -(Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x - v2.x, v1.y - v2.y);
    }

    public static Vector2 operator *(Vector2 v1, float m)
    {
        return new Vector2(v1.x * m, v1.y * m);
    }

    public static float operator *(Vector2 v1, Vector2 v2)
    {
        return v1.x * v2.x + v1.y * v2.y;
    }

    public static Vector2 operator /(Vector2 v1, float m)
    {
        return new Vector2(v1.x / m, v1.y / m);
    }

    //   ws rewrite
    public static bool operator ==(Vector2 v1, Vector2 v2)
    {
        if (System.Object.ReferenceEquals(v1, v2))
        {
            return true;
        }

        if (((object)v1 == null) || ((object)v2 == null))
        {
            return false;
        }

        return v1.x == v2.x && v1.y == v2.y;

    }

    public static bool operator !=(Vector2 v1, Vector2 v2)
    {
        return !(v1 == v2);
    }

    public static float Distance(Vector2 v1, Vector2 v2)
    {
        return (float)Math.Sqrt(Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2));
    }

    public float det(Vector2 v1)
    {
        return x * v1.y - v1.x * y;
    }

    public float Length()
    {
        return (float)Math.Sqrt(x * x + y * y);
    }
}

