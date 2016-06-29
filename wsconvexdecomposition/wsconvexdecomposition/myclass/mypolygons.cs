using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;

namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    class mypolygons
    {
        private Paths polygons=new Paths ();
        public IntPoint minPoint=new IntPoint();
        public IntPoint maxPoint=new IntPoint();

       public mypolygons(Paths subjs) 
        { 
            polygons = subjs;
        }

       public mypolygons(Path subjs)
       {
           polygons.Add(subjs);
       }

       public Paths offset(int distance) 
        {     
            Paths solution2 = new Paths();
            ClipperOffset co = new ClipperOffset();
            co.AddPaths(polygons, JoinType.jtRound, EndType.etClosedPolygon);
            //偏置时，偏置量需要先扩大Scale倍
            //co.MiterLimit = 2.0;
            co.Execute(ref solution2, distance);
            return solution2;
        }

       //public Paths intersection(Paths other_pologon)
       //{
       //    Paths solution3 = new Paths();
       //    Clipper co1 = new Clipper(0);          //不知道怎么用,在Polygons中有使用
       //    co1.AddPaths(polygons, ClipperLib.PolyType.ptSubject, true);
       //    co1.AddPaths(other_pologon,ClipperLib.PolyType.ptClip,true);
       //    co1.Execute(ClipperLib.ClipType.ctIntersection, solution3);
       //    return solution3;
       //}


       public Paths intersection(Paths other_pologon, int rownum, ref List<maddtionpologonIndex> outAddtionPathsIndex)
       {
           Paths solution4 = new Paths();
           Path tempnull = new Path();     //用来占位用   2016-04-22
           for (int j = 0; j < polygons.Count; j++)
           {
               Paths solution3 = new Paths();
               Clipper co1 = new Clipper(0);          //不知道怎么用,在Polygons中有使用

               //之前使用04_22晚间
               co1.AddPath(polygons[j], ClipperLib.PolyType.ptSubject, true);
               co1.AddPaths(other_pologon, ClipperLib.PolyType.ptClip, true);

               //co1.AddPath(temppath, ClipperLib.PolyType.ptClip, true);
               //co1.AddPaths(other_pologon, ClipperLib.PolyType.ptSubject, true);
               co1.Execute(ClipperLib.ClipType.ctIntersection, solution3);

               //为了占位，形成n*n的格子，不相交的地方用空格子占位,由于一次只比较一个格子，所以只有一个分区块，不考虑被分成两部分
               if (solution3.Count < 1) { solution4.Add(tempnull); }
               else
               {
                   solution4.Add(solution3[0]);
                   for (int i = 1; i < solution3.Count; ++i)
                   {
                       outAddtionPathsIndex.Add(new maddtionpologonIndex(solution3[i], rownum, j));
                   }
               }
           }
           return solution4;
       }


        //进行旋转
       public void applyMatrix(PointMatrix matrix)
        {
            for( int i=0; i<polygons.Count; i++)
            {
                for( int j=0; j<polygons[i].Count(); j++)
                {
                    polygons[i][j] = matrix.apply(polygons[i][j]);
                }
            }
        }

       public void calculateAABB()
    {
        minPoint.X = polygons[0][0].X;    //初始化minPoint和maxpoint
        minPoint.Y = polygons[0][0].Y;
        maxPoint.X = polygons[0][0].X;
        maxPoint.Y = polygons[0][0].Y;

        for(int i=0; i<polygons.Count(); i++)
        {
            for (int j = 0; j < polygons[i].Count(); j++)
            {
                if (minPoint.X > polygons[i][j].X) minPoint.X = polygons[i][j].X;
                if (minPoint.Y > polygons[i][j].Y) minPoint.Y = polygons[i][j].Y;
                if (maxPoint.X < polygons[i][j].X) maxPoint.X = polygons[i][j].X;
                if (maxPoint.Y < polygons[i][j].Y) maxPoint.Y = polygons[i][j].Y;
            }
        }
    }
       public Paths getPologons() { return polygons; }
       public IntPoint getMinPoint() { return minPoint; }
       public IntPoint getMaxPoint() { return maxPoint; }

    }


    class maddtionpologonIndex
    {
        public Path pg;        //  边长
        public int Rownum;              //行号
        public int Colnumnum;           //列号

        public maddtionpologonIndex(Path m_pg, int row, int col)
        {
            this.pg = m_pg;
            this.Rownum = row;
            this.Colnumnum = col;

        }
    }

}
