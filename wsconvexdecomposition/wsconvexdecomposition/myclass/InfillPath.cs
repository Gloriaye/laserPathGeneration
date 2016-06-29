using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;


namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public static class InfillPath
    {
        /// <summary>
        /// 填充内部区域的
        /// </summary>
        /// <param name="pgPaths"> 输入的多边形</param>
        /// <param name="lineSpacing">线宽</param>
        /// <param name="rotation">旋转角度</param>
        /// Outinfillpath 修改为输出一条路径，之前是输出一条路径集合 //便于画图 2016_04_22
        /// 
        public static Path generateLineInfill(Path pgPath, int lineSpacing, float rotation, ref Path secondinfillpath)    //可以是path，也可以是paths,lineSpacing有倍数
        {
            Path Outinfillpath = new Path();                  //内部填充路劲输出

            ///先偏置一圈2016_04_22
            Paths orpgPaths = new Paths();
            orpgPaths.Add(pgPath);
            Paths solution3 = new Paths();                    //产生偏置
            ClipperOffset co = new ClipperOffset();
            co.AddPaths(orpgPaths, JoinType.jtRound, EndType.etClosedPolygon);
            //偏置时，偏置量需要先扩大Scale倍
            co.Execute(ref solution3, -lineSpacing * 0.4);
            //        DrawBitmap(solution2, Color.Green,
            //          Color.White, 1.0f, isDrawPologonState);


            PointMatrix matrix = new PointMatrix(rotation);     //生成旋转矩阵
            mypolygons pgPolygons = new mypolygons(new Path());
            if (solution3.Count > 0)   //如果求交后无点
            {
                pgPolygons = new mypolygons(solution3[0]);
            }
            else { return new Path(); }
            pgPolygons.applyMatrix(matrix);                    //对多边形轮廓进行旋转
            pgPolygons.calculateAABB();                     //计算最小包围盒
            Paths convPgpaths = pgPolygons.getPologons();  //获得旋转后返回的pologons值
            IntPoint minPoint = pgPolygons.getMinPoint();  //获得最小包围盒的最小点和最大点
            IntPoint maxPoint = pgPolygons.getMaxPoint();

            ////2016_04_22设计的
            //if (minPoint.X >= 0) { minPoint.X = ((minPoint.X / lineSpacing) - 1) * lineSpacing; }
            //else
            //{
            //    minPoint.X = ((minPoint.X / lineSpacing) + 1) * lineSpacing;
            //}  //原始的

            //minPoint.X = ((minPoint.X / lineSpacing) - 1) * lineSpacing;
            // minPoint.X = ((minPoint.X / lineSpacing) - 1) * lineSpacing;

            //  minPoint.X = ((minPoint.X / lineSpacing)) * lineSpacing;
            int lineCount = (int)(maxPoint.X - minPoint.X + (lineSpacing - 1)) / lineSpacing;  //获得线的条数

            int yuliang = (int)(maxPoint.X - minPoint.X) - (lineCount-1) * lineSpacing;

            
            //2016-0604调整填充的线的大小
            int lowdoor = (int)(4 * lineSpacing / 10);
            int highdoor = (int)(8 * lineSpacing / 10);
            int deta = (int)(1 * lineSpacing / 10);

            if (yuliang > lowdoor && yuliang < highdoor)
            {
                lineSpacing = lineSpacing + (yuliang - deta) / lineCount;
            }
            if (yuliang >= highdoor)
            {

                lineSpacing = (int)(maxPoint.X - minPoint.X - deta) / lineCount;
                lineCount = lineCount + 1;
            }


            List<List<Int64>> cutList = new List<List<Int64>>();
            for (int n = 0; n < lineCount; n++)            //初始化cutlist
                cutList.Add(new List<Int64>());
            for (int polyNr = 0; polyNr < convPgpaths.Count(); polyNr++)
            {
                IntPoint p1 = convPgpaths[polyNr][convPgpaths[polyNr].Count() - 1];   //获得轮廓的最后一个点
                for (int i = 0; i < convPgpaths[polyNr].Count(); i++)
                {
                    IntPoint p0 = convPgpaths[polyNr][i];
                    int idx0 = (int)(p0.X - minPoint.X) / lineSpacing;        //获得p0p1线段的交点x坐标范围
                    int idx1 = (int)(p1.X - minPoint.X) / lineSpacing;
                    Int64 xMin = p0.X, xMax = p1.X;
                    if (p0.X > p1.X) { xMin = p1.X; xMax = p0.X; }
                    if (idx0 > idx1) { int tmp = idx0; idx0 = idx1; idx1 = tmp; }
                    for (int idx = idx0; idx <= idx1; idx++)
                    {
                        //int x = (int)((idx * lineSpacing) + minPoint.X + lineSpacing / 2);
                        int x = (int)((idx * lineSpacing) + minPoint.X + 0.04*deta);  //已经先偏置处理了，第一条线有点偏移即可
                        if (x < xMin) continue;
                        if (x >= xMax) continue;
                        int y = (int)(p0.Y + (p1.Y - p0.Y) * (x - p0.X) / (p1.X - p0.X));
                        cutList[idx].Add(y);
                        //插入到对应的位置
                    }
                    p1 = p0;
                }
            }

            //进行点的排列，对于凸分区可以直接排列；对于Paths需要更改，凸多边形只有两个交点
            int index = 0;   //等同于idx
            Boolean linkDir = true;
            Path infillpath = new Path(); //线段的输出
            Boolean secondeExise = false;

            IntPoint tempPointFir = new IntPoint ();
            IntPoint tempPointSec = new IntPoint ();

            //for (Int64 x = minPoint.X + lineSpacing / 2; x < maxPoint.X; x += lineSpacing)  //原来的，2016_04_22
            for (Int64 x = minPoint.X + 1; x < maxPoint.X; x += lineSpacing)  //还原x
            {
                // qsort(cutList[index].data(), cutList[index].size(), sizeof(int64_t), compare_int64_t); //排序算法暂时不需要使用
                cutList[index].Sort();    // 注意从小到大排序
                int dotnum = cutList[index].Count();
                for (int i = 0; i + 1 < 2; i += 2)
                {
                    if (dotnum < 4)
                    {
                        if (!secondeExise)     //second不存在
                        {
                            if (linkDir)
                            {

                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));  //正向链接
                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1])));

                                tempPointFir = matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1]));
                            }
                            else
                            {
                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1])));//反向链接
                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));

                                tempPointFir = matrix.unapply(new IntPoint(x, cutList[index][i]));

                            }
                        }

                        else     //second存在
                        {
                            IntPoint point_i = matrix.unapply(new IntPoint(x, cutList[index][i]));    //点i
                            IntPoint point_i1 = matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1]));    //点i+1

                            if (linkDir)
                            {
                                float dis0 = Dis(point_i, tempPointFir);
                                float dis1 = Dis(point_i, tempPointSec);

                                if (dis0 < dis1)
                                {
                                    infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));  //正向链接
                                    infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1])));

                                    tempPointFir = matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1]));
                                }
                                else
                                {
                                    secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));  //正向链接
                                    secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1])));

                                    tempPointSec = matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1]));
                                }

                            }
                            else
                            {
                                float dis0 = Dis(point_i1, tempPointFir);
                                float dis1 = Dis(point_i1, tempPointSec);
                                if (dis0 < dis1)
                                {
                                    infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1])));  //fan向链接
                                    infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));

                                    tempPointFir = matrix.unapply(new IntPoint(x, cutList[index][i]));
                                }
                                else
                                {
                                    secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][dotnum - 1])));  //fan向链接
                                    secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));

                                    tempPointSec = matrix.unapply(new IntPoint(x, cutList[index][i]));
                                }

                            }
                        }


                    }

                    else
                    {
                        IntPoint point_i = matrix.unapply(new IntPoint(x, cutList[index][i]));    //点i
                        IntPoint point_i1 = matrix.unapply(new IntPoint(x, cutList[index][i + 1]));    //点i+1
                        IntPoint point_i2 = matrix.unapply(new IntPoint(x, cutList[index][i + 2]));    //点i+2
                        IntPoint point_i3 = matrix.unapply(new IntPoint(x, cutList[index][i + 3]));    //点i+3

                        secondeExise = true;

                        if (linkDir)
                        {
                            float dis0 = Dis(point_i, tempPointFir);
                            float dis01 = Dis(point_i2, tempPointFir);

                            if (dis0 < dis01)
                            {
                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));  //正向链接
                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 1])));

                                tempPointFir = matrix.unapply(new IntPoint(x, cutList[index][i + 1]));

                                secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 2])));  //正向链接
                                secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 3])));

                                tempPointSec = matrix.unapply(new IntPoint(x, cutList[index][i + 3]));
                               
                            }
                            else
                            {
                                secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));  //正向链接
                                secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 1])));

                                tempPointSec = matrix.unapply(new IntPoint(x, cutList[index][i + 1]));

                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 2])));  //正向链接
                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 3])));

                                tempPointFir = matrix.unapply(new IntPoint(x, cutList[index][i + 3]));
                            }

                        }

                        else
                        {
                            float dis0 = Dis(point_i1, tempPointFir);
                            float dis01 = Dis(point_i3, tempPointFir);

                            if (dis0 < dis01)
                            {
                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 1])));  //fan向链接
                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));

                                tempPointFir = matrix.unapply(new IntPoint(x, cutList[index][i]));

                                secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 3])));  //fan向链接
                                secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 2])));

                                tempPointSec = matrix.unapply(new IntPoint(x, cutList[index][i + 2]));
                            }
                            else
                            {
                                secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 1])));  //fan向链接
                                secondinfillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i])));

                                tempPointSec = matrix.unapply(new IntPoint(x, cutList[index][i]));

                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 3])));  //fan向链接
                                infillpath.Add(matrix.unapply(new IntPoint(x, cutList[index][i + 2])));

                                tempPointFir = matrix.unapply(new IntPoint(x, cutList[index][i + 2]));
                            }
                        }
                        continue;
                    }       
                    
                }
                index += 1;
                linkDir = !linkDir;
            }
            Outinfillpath = infillpath;
            return Outinfillpath;
        }


        public static float Dis(IntPoint v1, IntPoint v2)
        {
            return (float)Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
        }
    }

}
