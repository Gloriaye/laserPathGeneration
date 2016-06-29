using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ClipperLib;



namespace wsconvexdecomposition
{

    class CalculateMinRect
    {
        private const float FLT_MIN = 1.175494351e-38F;
        private const float FLT_MAX = 3.402823466e+38F;

        public List<Vector2> mOrignalPologon = new List<Vector2> (); 
        public OBB obb = new OBB();
        List<Vector2> pts = new List<Vector2>(4); //四个坐标点
        public float minArea = FLT_MAX;       //最小包围盒子的面积
        public float OritionAngle;           //短边与y轴的倾斜角

        //2016_04_22晚间 添加求面积
        //public float mPologonArea;           //输入多边形的面积

        public void MinAreaRec(List<Vector2> path)
        {
            mOrignalPologon = path;    //存储原始数据
            int ptsNum = path.Count;

            for (int i = 0, j = ptsNum - 1; i < ptsNum; j = i, i++)
            { //遍历边
                Vector2 u0 = path[i] - path[j];//构造边
                u0 = u0 / Length(u0);
                Vector2 u1 = new Vector2(0 - u0.y, u0.x);//与u0垂直
                float min0 = 0.0f, max0 = 0.0f, min1 = 0.0f, max1 = 0.0f;

                for (int k = 0; k < ptsNum; k++)
                {//遍历点
                    Vector2 d = path[k] - path[j];
                    //投影在u0
                    float dot = Dot(d, u0);
                    if (dot < min0) min0 = dot;
                    if (dot > max0) max0 = dot;
                    //投影在u1
                    dot = Dot(d, u1);
                    if (dot < min1) min1 = dot;
                    if (dot > max1) max1 = dot;
                }

                float area = (max0 - min0) * (max1 - min1);
                if (area < minArea)
                {
                    minArea = area;
                    obb.c = path[j] + (u0 * (max0 + min0) + u1 * (max1 + min1)) * 0.5f;

                    obb.u[0] = u0;
                    obb.u[1] = u1;

                    obb.e[0] = (max0 - min0) * 0.5f;
                    obb.e[1] = (max1 - min1) * 0.5f;
                }
            }

            calculateOriAngle();      //计算最小倾斜角
        }

        public List<Vector2> GetOBBPoints()
        {//获取OBB四个顶点坐标
            List<Vector2> pts = new List<Vector2>(4);

            pts.Add(obb.c + (obb.u[0] * obb.e[0] + obb.u[1] * obb.e[1]));
            pts.Add(obb.c + (obb.u[0] * obb.e[0] - obb.u[1] * obb.e[1]));
            pts.Add(obb.c - (obb.u[0] * obb.e[0] + obb.u[1] * obb.e[1]));
            pts.Add(obb.c + (obb.u[1] * obb.e[1] - obb.u[0] * obb.e[0]));

            return pts;
        }


        private void calculateOriAngle()         //计算倾斜角度 短边与y轴的倾斜角
        {
            Vector2 axis_x = new Vector2(0, 1);  //Y轴单位向量
            Vector2 vec = new Vector2();

            if (obb.e[0] < obb.e[1])
            {
              // OritionAngle = (float)(Math.Acos(obb.u[0] * axis_x) * (180 / Math.PI));
                if (obb.u[0].y < 0)
                {
                    vec.y = -obb.u[0].y;
                    vec.x = -obb.u[0].x;
                }
                else { vec = obb.u[0]; }
                OritionAngle = (float)(Math.Asin(vec.det(axis_x)) * (180 / Math.PI));
            }
            else
            {
               // OritionAngle = (float)(Math.Acos(obb.u[1] * axis_x) * (180 / Math.PI));
                if (obb.u[1].y < 0)
                {
                    vec.y = -obb.u[1].y;
                    vec.x = -obb.u[1].x;
                }
                else { vec = obb.u[1]; }
                OritionAngle = (float)(Math.Asin(vec.det(axis_x)) * (180 / Math.PI));
               // int i = 0;
            }

        }


        public RectInfos getPologonRectInfos()
        {
            RectInfos mInfo = new RectInfos();
            float m_area = new float();
            float m_circleLength = new float();
            int point_num = mOrignalPologon.Count;
            if (point_num < 3) return mInfo;           //如果不是多边形
            for (int i = 0; i < mOrignalPologon.Count; ++i)
            {
                m_area += mOrignalPologon[i].x * mOrignalPologon[(i + 1) % point_num].y - mOrignalPologon[i].y * mOrignalPologon[(i + 1) % point_num].x;

                m_circleLength += Length(mOrignalPologon[(i + 1) % point_num] - mOrignalPologon[i]);
            }
            m_area = (float)Math.Abs(m_area * 0.5);
            mInfo.m_area = m_area;    //面积
            mInfo.m_circleLength = m_circleLength;   //周长
            mInfo.m_minArea = minArea;   //最小多边新面积
            mInfo.m_fulldegree = m_circleLength / (float)Math.Sqrt(m_area);   //饱满度周长/面积开方
            mInfo.m_filldegree = m_area / minArea;                //充盈度
            return mInfo;
        }

        


        public float getminArea()
        {
            return minArea;
        }

        public float Dot(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public float Length(Vector2 p)
        {
            return (float)Math.Sqrt((float)Dot(p, p));
        }
    }

    class OBB
    {
        public Vector2[] u = new Vector2[2]; //x, y轴
        public Vector2 c;	//中心点
        public float[] e = new float[2];	//半长，半宽
    };

    //存储多边形的信息
    class RectInfos
    {
        public float m_area;          //面积
        public float m_minArea;         //最小包围盒面积
        public float m_circleLength;  //周长
        public float m_fulldegree;   //饱满度
        public float m_filldegree;   //充盈度
    }

}
