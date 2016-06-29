using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ClipperLib;

namespace wsconvexdecomposition
{ 

    //浮点数矩阵，用作旋转填充的计算。输入和返回值为intpoint
    class PointMatrix
    {
        public float[] matrix=new float[4];

    public PointMatrix()
    {
        matrix[0] = 1;
        matrix[1] = 0;
        matrix[2] = 0;
        matrix[3] = 1;
    }
    
    public PointMatrix(double rotation)
    {
        rotation = rotation / 180 * Math.PI;
        matrix[0] = (float)Math.Cos(rotation);
        matrix[1] = -(float)Math.Sin(rotation);
        matrix[2] = -matrix[1];
        matrix[3] = matrix[0];
    }
    
    public PointMatrix( Point p)
    {
        matrix[0] = p.X;
        matrix[1] = p.Y;
        float f = (float)Math.Sqrt((matrix[0] * matrix[0]) + (matrix[1] * matrix[1]));
        matrix[0] /= f;
        matrix[1] /= f;
        matrix[2] = -matrix[1];
        matrix[3] = matrix[0];
    }

   public  IntPoint apply(IntPoint p)
    {      
        return new IntPoint(p.X * matrix[0] + p.Y * matrix[1], p.X * matrix[2] + p.Y * matrix[3]);
    }

   public IntPoint unapply(IntPoint p) 
    {
        return new IntPoint(p.X * matrix[0] + p.Y * matrix[2], p.X * matrix[1] + p.Y * matrix[3]);
    }
    }
}
