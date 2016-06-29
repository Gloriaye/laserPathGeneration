using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wsconvexdecomposition
{
    class firstPartition
    {

        private  Vector2 At(int i, List<Vector2> vertices)
        {
            int s = vertices.Count;
            return vertices[i < 0 ? s - (-i % s) : i % s];
        }

        private  List<Vector2> Copy(int i, int j, List<Vector2> vertices)
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

        public  List<List<Vector2>> excutePartition(List<List<Vector2>> pologons) 
        {
            if (pologons.Count == 1) { return pologons; }

            List<Vector2> xminList = new List<Vector2>();  //左下角点链表
            List<Vector2> pointOrderOutList = new List<Vector2>();   //左下角点排序后
            List<int> indexAfterOrderList = new List<int>();         //左下角点排序后索引
             List<List<Vector2>> pologonsByOrder = new List<List<Vector2>>();//依据左下角索引形成的新多边形链表
            List<int> setofNearestIndexList = new List<int>();       //最近点索引的链表


            List<List<Vector2>> outPologons = new List<List<Vector2>>();//输出多边形集合

            
            foreach (List<Vector2> mpologon in pologons)
            {
               xminList.Add(AABBxmin(mpologon));   //获得最小包围盒左下角链表
            }
            Vector2Comparer cmpPoint = new Vector2Comparer();
            Sort(xminList, out pointOrderOutList, out indexAfterOrderList, cmpPoint);  //排序返回数据排序，和相应的索引
            foreach (int inx in indexAfterOrderList)
            { pologonsByOrder.Add(pologons[inx]); }             //获得排序后的多边形链表

            
            List<int> templist = new List<int>();
            int noindexfist=-1;     //防止只过一个多边形一点
            int noindexSecond=-1;
            for (int i=0;i<pologonsByOrder.Count-1;i++)
            {
                templist = calculateNearestPoint(pologonsByOrder[i], pologonsByOrder[i + 1], noindexfist, noindexSecond);
                setofNearestIndexList.AddRange(templist);
                noindexfist = templist[1];   //每次计算都排除与上一个多边形连接的点
            }
            //加上最后个多边形和第一个多边形的最近点，形成环状
            templist = calculateNearestPoint(pologonsByOrder[pologonsByOrder.Count - 1], pologonsByOrder[0], noindexfist, setofNearestIndexList[0]);
            //setofNearestIndexList.AddRange(templist);//注意setofNearestIndexList的数目
            setofNearestIndexList.Insert(0,templist[1]);//将最外层多边形点插入首
            setofNearestIndexList.Add(templist[0]);  //将最后多边形点插入尾

            List<Vector2> useforcope = new List<Vector2>();
            List<Vector2> tempPologonList = new List<Vector2>();
            for (int i = 0; i < pologonsByOrder.Count; i++) 
            {
                
                useforcope = Copy(setofNearestIndexList[2 * i], setofNearestIndexList[2 * i + 1], pologonsByOrder[i]);
                tempPologonList.AddRange(useforcope);
            }
            outPologons.Add(tempPologonList);
            //List<int> setofNearestIndexListReverse = new List<int>();
            //List<List<Vector2>> pologonsByOrderReverse = new List<List<Vector2>>();

            setofNearestIndexList.Reverse();   //索引翻转，求取第二条多边形
            pologonsByOrder.Reverse();
            tempPologonList = new List<Vector2>();
            for (int i = 0; i < pologonsByOrder.Count; i++)
            {
                useforcope = Copy(setofNearestIndexList[2 * i], setofNearestIndexList[2 * i + 1], pologonsByOrder[i]);
                tempPologonList.AddRange(useforcope);
            }
            outPologons.Add(tempPologonList);
            return outPologons;
        }




        //计算两个多边形的最近点索引(第一个，第二个),noindexfirst排除前一个多边形的某一点，防止只过一个多边形一点
        private  List<int> calculateNearestPoint(List<Vector2> firstVertices, List<Vector2> secondVertices,int noindexfist,int noindexSecond)
        {
            List<int> retIndex=new List<int>();
            float nearestDis = 0xFFFFFFFFFFFFFFFFL;
            float tempDis;
            int firstIndex=new int ();
            int secondIndex=new int ();
            foreach (Vector2 firstVecPoints in firstVertices)
            {
                if (firstVertices.IndexOf(firstVecPoints) == noindexfist) continue;   
                foreach (Vector2 secondVecPoints in secondVertices)
                {
                    if (secondVertices.IndexOf(secondVecPoints) == noindexSecond) continue;   
                    tempDis = Distance(firstVecPoints,secondVecPoints);
                    if (tempDis < nearestDis)
                    {
                        nearestDis = tempDis;
                        secondIndex = secondVertices.IndexOf(secondVecPoints);
                        firstIndex = firstVertices.IndexOf(firstVecPoints);
                    }
                }
            }
            retIndex.Add(firstIndex);
            retIndex.Add(secondIndex);
            return retIndex;
        }

        //距离
        private  float Distance(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Sqrt(Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2));
        }

        //计算最小包围盒
        private List<Vector2> AABBf(List<Vector2> pointfArry)     //最小包围盒
        {
            List<float> xarr = new List<float>();
            List<float> yarr = new List<float>();
            List<Vector2> result = new List<Vector2>();
            foreach (Vector2 po in pointfArry)
            {
                xarr.Add(po.x);
                yarr.Add(po.y);
            }
            result.Add(new Vector2(xarr.Min(), yarr.Min()));  //逆时针AABB 左下
            result.Add(new Vector2(xarr.Max(), yarr.Min()));  //右下point
            result.Add(new Vector2(xarr.Max(), yarr.Max()));  //右上point
            result.Add(new Vector2(xarr.Min(), yarr.Max()));  //左上point
            return result;
        }

        private  Vector2 AABBxmin(List<Vector2> pointfArry)     //返回包围盒xmin,yarr.Min()
        {
            List<float> xarr = new List<float>();
            List<float> yarr = new List<float>();
            foreach (Vector2 po in pointfArry)
            {
                xarr.Add(po.x);
                yarr.Add(po.y);
            }
            return new Vector2(xarr.Min(), yarr.Min());
        }


        void  Sort<T>(
               List<T> input,
               out List<T> output,
               out List<int> permutation,
               IComparer<T> comparer
                   )
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (input.Count == 0)
            {
                // give back empty lists
                output = new List<T>();
                permutation = new List<int>();
                return;
            }
            if (comparer == null) { throw new ArgumentNullException("comparer"); }
            int[] items = Enumerable.Range(0, input.Count).ToArray();
            T[] keys = input.ToArray();
            Array.Sort(keys, items, comparer);
            output = keys.ToList();
            permutation = items.ToList();
        }

    }
}
