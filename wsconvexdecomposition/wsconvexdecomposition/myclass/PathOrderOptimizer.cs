using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;

namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
 
    /// <summary>
    /// 
    /// </summary>
    class PathOrderOptimizer
    {
        public IntPoint startPoint;      //优化路劲的起始点
        public Paths linePaths;          //优化之前的路径集合起始
        public Paths orderedLinePaths=new Paths ();   //优化后的路径集合
        public List<int> polyStart=new List<int>();      // 路劲的起始点索引
        public List<int> polyOrder = new List<int>();      //路径的顺序索引
        public List<bool> converseState=new List<bool>();  //路劲是否需要翻转
        public float pathDiscost=new float();        //空行程

        public Tour mtour;               //存储路径顺序解

        public PathOrderOptimizer(IntPoint startPoint,Paths linePaths)
        {
            this.startPoint = startPoint;
            this.linePaths=new Paths(linePaths);
            this.pathDiscost = 0.0f;
        }

        public void AddPolygon(Path polygon)   //添加路劲
        {
            this.linePaths.Add(polygon);
        }

         public void AddPolygons(Paths polygons)      //添加路劲集合
        {
            this.linePaths.AddRange(polygons);
        }

         private float vSize2f(IntPoint v1, IntPoint v2)  //测量两点间的距离
         {
             return (float)Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));   //绝对距离
             //return Math.Max(Math.Abs(v1.X - v2.X), Math.Abs(v1.Y - v2.Y));                    //时间距离
         }

         public Paths GetOrderOptimizePaths()    //只看起点和终点
         {
             List<bool> picked = new List<bool>();           //显示是否已经确定顺序了
             IntPoint p0 = new IntPoint(startPoint);


             mtour = new Tour(linePaths.Count());    //初始化tour

             for (int n = 0; n < linePaths.Count(); n++)     //初始化Picked为false,初始化polyStart为0
             {
                 picked.Add(false);
                 polyStart.Add(new int());
                 converseState.Add(false);
             }
                pathDiscost = 0;

             for(int n=0; n<linePaths.Count(); n++)      //开始两点排序
            {
                int best = -1;
                //float bestDist = 0xFFFFFFFFFFFFFFFFL;
                float bestDist = 456789f;
                for (int i = 0; i < linePaths.Count(); i++)
                {
                    if (picked[i] || linePaths[i].Count() < 1)
                        continue;
                    //if (linePaths[i].Count() == 2)
                    else
                    {
                        float dist = vSize2f(linePaths[i][0], p0);   //第一个点
                        if (dist < bestDist)
                        {
                            best = i;
                            bestDist = dist;
                            polyStart[i] = 0;
                            converseState[i] = false;
                        }
                        dist = vSize2f(linePaths[i][linePaths[i].Count() - 1], p0);    //最后一个点
                        if (dist < bestDist)
                        {
                            best = i;
                            bestDist = dist;
                            polyStart[i] = linePaths[i].Count() - 1;
                            converseState[i] = true;               //翻转
                        }
                    }
                 }
                if (best > -1)           //表示数据已经更新了
                {    
                    pathDiscost = pathDiscost + bestDist;
                    picked[best] = true;              //表示已经计算过了
                    polyOrder.Add(best);

                    if (converseState[best])                        //可以使用list.Reverse()
                    {
                        linePaths[best].Reverse();
                      orderedLinePaths.Add(linePaths[best]);   //翻转
                    }
                    else
                    { orderedLinePaths.Add(linePaths[best]); }
                    p0 = linePaths[best][linePaths[best].Count() - 1];

                    mtour.setCity(n, new City(best));
                }
            }
             return orderedLinePaths;
          }


         //public Paths GetOrderOptimizePaths()     //获得优化的顺序
         //{
         //    OrderOptimize();
         //    for( int n=0; n<polyOrder.Count(); n++)
         //       {
         //           int nr = polyOrder[n];
         //           if (converseState[nr])                        //可以使用list.Reverse()
         //           {
         //               linePaths[nr].Reverse();
         //              orderedLinePaths.Add(linePaths[nr]);   //翻转
         //           }
         //           else
         //           { orderedLinePaths.Add(linePaths[nr]); }                 
         //       }
         //    return orderedLinePaths;
         //}


        ////翻转path
        // private Path conversePath(Path pg) 
        // {
        //     Path tempPath = new Path();
        //     for (int i = pg.Count() - 1; i >= 0; i--)
        //     {
        //         tempPath.Add(pg[i]);
        //     }
        //     return tempPath;
        // }



    }
}
