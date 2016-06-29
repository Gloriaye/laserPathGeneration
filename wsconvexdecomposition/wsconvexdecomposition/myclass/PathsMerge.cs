using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;


///创建于2016_04_22
namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>; 
    public static class PathsMerge
    {

        //合并路径矩阵
        public static Paths PathsMatrixMergeByRandom(List<Paths> pgss, float m_dis_threshold, Paths addPgs)
        {
            Paths finaloutPathsList = new Paths();     //输出合并后的路径
            Paths mtempPaths = new Paths();          //用作内部零食
            Paths AfterorderPaths = new Paths();     //顺序合并后路径集合
            List<Paths> pgssAfter = new List<Paths>();
            if ((pgss.Count % 2) == 1)
            {
                pgssAfter = new List<Paths> (PathsRowToCol(pgss));
            }
            else
            {
                pgssAfter = new List<Paths> (pgss);
            }
            foreach (Paths pgs in pgssAfter)
            {
                mtempPaths = PathsMergeByOrder(pgs, m_dis_threshold);
                AfterorderPaths.AddRange(mtempPaths);
            }

            AfterorderPaths.AddRange(new Paths (addPgs));     //加入附加的路径
            finaloutPathsList = PathsMergeByRandom(AfterorderPaths, m_dis_threshold);   //最终合并结果
            return finaloutPathsList;
        }

        public static Paths PathsMatrixMergeByRandom(List<Paths> pgss, float m_dis_threshold)
        {
            Paths finaloutPathsList = new Paths();     //输出合并后的路径
            Paths mtempPaths = new Paths();          //用作内部零食
            Paths AfterorderPaths = new Paths();     //顺序合并后路径集合
            List<Paths> pgssAfter = new List<Paths>();
            if ((pgss.Count % 2) == 1)
            {
                pgssAfter = PathsRowToCol(pgss);
            }
            else
            {
                pgssAfter = pgss;
            }
            foreach (Paths pgs in pgssAfter)
            {
                mtempPaths = PathsMergeByOrder(pgs, m_dis_threshold);
                AfterorderPaths.AddRange(mtempPaths);
            }

            //AfterorderPaths.AddRange(addPgs);     //加入附加的路径
            finaloutPathsList = PathsMergeByRandom(AfterorderPaths, m_dis_threshold);   //最终合并结果
            return finaloutPathsList;
        }


        //行列切换
        public static List<Paths> PathsRowToCol(List<Paths> pgss)
        {
            List<Paths> listPathAfterTrans = new List<Paths>();
            int k = 0;

            foreach (Paths pt in pgss)
            {
                k = pt.Count >k? pt.Count:k;
            }

            for (int j = 0; j < k; j++)
            {
                listPathAfterTrans.Add(new Paths());
            }

            foreach (Paths pgs in pgss)
            {
                for (int i = 0; i < pgs.Count; i++)
                {
                    listPathAfterTrans[i].Add(new Path(pgs[i]));
                }
            }

            return listPathAfterTrans;
        }
        
        
        //随机进行合并
        public static Paths PathsMergeByRandom(Paths pgs, float m_dis_threshold)
        {
            Paths outPathsListRandom = new Paths();     //输出合并后的路径
            Paths tempPaths = new Paths();
            tempPaths=PathsMergeByOrder(pgs, m_dis_threshold);      //先从前往后顺序进行一次合并,同时可以祛除空路劲

            Paths mtempMergePath = new Paths();       //存储每次单独合并的输出结果
            bool unistate = false;  //合并状态

            int num = tempPaths.Count();
            List<bool> nonState = new List<bool>();     //表示各条路劲合并状态，
            for (int k=0;k<num;k++){nonState.Add(false);}   //初始化路劲合并状态,类似非空

            //循环合并
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num ;j++ ) 
                {
                    if ((i != j) && (!nonState[i]) && (!nonState[j]))
                    {
                        mtempMergePath = PathMergeAlong(tempPaths[i], tempPaths[j], m_dis_threshold, out unistate);
                        if (unistate)
                        {
                            tempPaths[i] = mtempMergePath[0];
                            nonState[j] = true;              //合并后把j状态清空，相当于清除
                        }
                    }
                }
            }

            //提取结果
            for (int s = 0; s < num ; s++)
            {
                if (!nonState[s])
                {
                    outPathsListRandom.Add(tempPaths[s]);
                }
            }

            return outPathsListRandom;

        }


        //行进行先后合并
        public static Paths PathsMergeByOrder(Paths pgs,float m_dis_threshold)
        {
            Paths outPathsList = new Paths();  //输出合并结果
            Paths tempMergePath = new Paths(); //存储每次hebing输出结果
            Path eachPath = new Path();        //存储每一条独立路径

            bool unistate = false;  //合并状态

            for (int i = 0; i < pgs.Count ; i++)
            {
                if (pgs[i].Count == 0)         //融合是碰到pg（i）为空
                {
                    if (eachPath.Count > 0)      //如果eachPath内有数据,此条合并结束，重置eachPath
                    { outPathsList.Add(eachPath); eachPath = new Path(); }
                }
                else if (eachPath.Count == 0)        //eachPath为空，需要为eachPath赋值一条数据,并退出此次循环
                {
                    eachPath = pgs[i];
                    continue;
                }
                else                                    //均非空,则考虑合并
                {
                    tempMergePath = PathMergeAlong(eachPath, pgs[i], m_dis_threshold, out unistate);
                    if (unistate)
                    {
                        //eachPath.AddRange(tempMergePath[0]);                     //如果可以合并肯定就一条路径
                        eachPath = tempMergePath[0];
                    }
                    else                                              //如果不能合并，切断融合，开始下一条融合
                    { outPathsList.Add(eachPath); 
                      eachPath = new Path();  
                      eachPath.AddRange(pgs[i]);                   //插入下一条进行初始化
                    }   
                }    
            }
            if (eachPath.Count > 0)                //跳出循环后如果eachPath非空则插入
            { outPathsList.Add(eachPath); }

            return outPathsList;
        }
       
        
        //合并两条路径 最好在合并之前先判断一下是不是空路径
        public static Paths PathMergeAlong(Path m_PathA, Path m_PathB ,float m_dis_threshold, out bool unistate)
        {
            unistate = false;   //合并状态
            Paths outPath = new Paths();   //输出合并后路劲
            IntPoint A_startPoint=new IntPoint();
            IntPoint B_startPoint = new IntPoint();
            IntPoint A_endPoint = new IntPoint ();
            IntPoint B_endPoint = new IntPoint ();

            if (m_PathA.Count > 0) { A_startPoint = m_PathA[0]; A_endPoint = m_PathA[m_PathA.Count-1]; }
            if (m_PathB.Count > 0) { B_startPoint = m_PathB[0]; B_endPoint = m_PathB[m_PathB.Count - 1];}

            if (Dis(A_endPoint, B_startPoint) <= m_dis_threshold)   //情况1
            {
                m_PathA.AddRange(m_PathB);
                outPath.Add(m_PathA);
                unistate = true; 
            }
            else if (Dis(A_startPoint, B_endPoint) <= m_dis_threshold)   //情况2
            {
                m_PathB.AddRange(m_PathA);
                outPath.Add(m_PathB);
                unistate = true; 
            }
            else if (Dis(A_endPoint, B_endPoint) <= m_dis_threshold)        //情况3
            {
                m_PathB.Reverse();
                m_PathA.AddRange(m_PathB);
                outPath.Add(m_PathA);
                unistate = true; 
            }
            else if (Dis(A_startPoint, B_startPoint) <= m_dis_threshold)        //情况4
            {
                m_PathA.Reverse();
                m_PathA.AddRange(m_PathB);
                outPath.Add(m_PathA);
                unistate = true; 
            }
            else { outPath.Add(m_PathA); outPath.Add(m_PathB); unistate = false; }      //不能合并的情况




            return outPath; 
        }

        public static float Dis(IntPoint v1, IntPoint v2)
        {
            return (float)Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
        }
        

    }
}
