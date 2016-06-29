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
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    class PologonSqualMesh
    {
        /// <summary>
        /// 输入，需要构造函数
        /// </summary>
        private const float FLT_MIN = 1.175494351e-38F;
        private float scale = 1000f;           //放大倍数

       public  List<Vector2> mrignalPologon = new List<Vector2>();    //存储初始多边形外环
       public List< List<Vector2>> OmrignalPologons = new List<List<Vector2>>();    //存储初始多边形集合

       public Path moRignalPath = new Path();
       public Paths mOrignalPaths = new Paths();                        
       public float spacing;                                          //扫描行距
       public float Lmax;                                            // 最大允许线长
       public float side_min;                                        //最小允许边长

       public CalculateMinRect minRect = new CalculateMinRect();     //存储最小包围盒
       float rect_h;                                                 //最小包围盒高度
       float rect_w;                                                 //最小包围盒的宽度
       public float rotationAngle;                                          //存储最小包围盒短边y向与Y轴的夹角
       List<Vector2> minRectPoints = new List<Vector2>();            //存储最小包围盒的坐标
       
        
      /// <summary>
      /// 输出数据
      /// </summary>
      /// <param name="m_OrignalPologon"></param>
       float sideLength;                                             //正方形网格的边长
       public List<lengthRowColNum> moutlengthRowColNum = new List<lengthRowColNum>();                // //输出  边长、行数、列数
       public List<List<Paths>> outMeshRectPologons = new List<List<Paths>>();

       public List<List<maddtionpologonIndex>> outAddtionRectPologons = new List<List<maddtionpologonIndex>>();   //2016-06-03附加的多边形，针对一个多边形可能存在多个区域

        //构造函数
       public PologonSqualMesh(List<List<Vector2>> mm_OrignalPologons, float m_spacing, float m_Lmax, float m_side_min)
       {
           OmrignalPologons = mm_OrignalPologons;
           mrignalPologon = mm_OrignalPologons[0];    //外环
           moRignalPath = ConvexPolygonToPath(mm_OrignalPologons[0], 1 / scale);  
           //mOrignalPaths.Add(moRignalPath);
           foreach (List<Vector2> temp in mm_OrignalPologons)
           {
               mOrignalPaths.Add(ConvexPolygonToPath(temp, 1 / scale));    //放大N倍
           }
           
           spacing = m_spacing;
           Lmax = m_Lmax;
           side_min = m_side_min;
       }

        //执行函数
        public void CalculatePologonSqualMesh() 
        {
            List<Vector2> m_OrignalPologon=mrignalPologon ;                   //保存初始化多边形
            
            minRect.MinAreaRec(m_OrignalPologon);
            minRectPoints = minRect.GetOBBPoints();                  //存储最小多边形的坐标
            rotationAngle = minRect.OritionAngle;                   //倾斜角度

            float temp_rect_h = minRect.obb.e[0] * 2.0f;                       //最小包围盒高度/宽度
            float temp_rect_w = minRect.obb.e[1] * 2.0f;
            if (temp_rect_h < temp_rect_w) { rect_h = temp_rect_h; rect_w = temp_rect_w; }    //短边表示H，长边表示W，主要针对旋转后短边是Y轴
            else { rect_h = temp_rect_w; rect_w = temp_rect_h; }

            moutlengthRowColNum = CalculateSideLength(rect_h, rect_w, spacing, Lmax, side_min);  //获得最小包围盒信息

            //数据先扩大了Scale倍，保存精度，变成IntPoint
            Path pathConver = ConvexPolygonToPath(minRectPoints, 1 / scale);  //构造数据
            

            foreach (lengthRowColNum mlengthRowColNum in moutlengthRowColNum)
            {
                List<Paths> tempoutMeshRectPologons = new List<Paths>();
                int mspcing = (int)(mlengthRowColNum.Sidelength * scale);
                //函数原型
                //List<Paths> GetMeshPologons( Path pgPaths, float rotation, int side_length, int rownum, int colnum) 
                tempoutMeshRectPologons = GetMeshPologons( new Path(pathConver), rotationAngle, mspcing, mlengthRowColNum.Rownum, mlengthRowColNum.Colnumnum);
                outMeshRectPologons.Add(tempoutMeshRectPologons);
            }
        }

    
        //求最优边长
        /// <summary>
        /// 求最优边长
        /// </summary>
        /// <param name="m_rect_h">高度</param>
        /// <param name="m_rect_w">宽度</param>
        /// <param name="m_spacing">行距</param>
        /// <param name="m_sidemax">最大允许长度</param>
        /// <param name="m_side_min">最小边长</param>
        /// <returns>m_sidelength 边长</returns>
        private List<lengthRowColNum> CalculateSideLength(float m_rect_h, float m_rect_w, float m_spacing, float m_Lmax, float m_side_min)
        {
            List<lengthRowColNum> m_lengthRowColNums = new List<lengthRowColNum>();
            lengthRowColNum mtemp_lengthRowColNum = new lengthRowColNum();
            //float[] m_lengthRowColNum = new float[3];  //输出  边长、行数、列数
            float m_sidelength = m_Lmax;       //边长
            int m_Rownum=0, m_Columnum=0;         //行数、列数
            float m_Dmax;         //允许的最大边长
            int m_Rownum_Max = 0, m_Columnnum_Max = 0;
            float m_Drerow;          //行余量、即高度余量
            float m_Drecol;          //列余量 、即宽度余量
            bool rowState = false;           //判读行是否可行，若可行则进行列判断
            bool colState = false;           //判读列是否可行，若可行则循环截止

            m_Dmax = (((Int32)((m_Lmax - m_spacing) / 2 / m_spacing)) * 2 + 1) * m_spacing;   //最大边长 初始迭代值
            m_sidelength = m_Dmax;

            //用来判断初始情况
            m_Rownum_Max = (int)(m_rect_h / m_sidelength);         //最大行数
            m_Drerow = m_rect_h - m_sidelength * m_Rownum_Max;     //求取行余量
            m_Columnnum_Max = (int)(m_rect_w / m_sidelength);      //最大列数
            m_Drecol = m_rect_w - m_sidelength * m_Columnnum_Max;         //求取列余量


            if ((m_Drerow <= FLT_MIN) && (m_Drecol <= FLT_MIN))    //如果行和列都刚刚好
            {
                //mtemp_lengthRowColNum.Sidelength = m_Dmax;
                //mtemp_lengthRowColNum.Rownum = m_Rownum_Max;
                //mtemp_lengthRowColNum.Colnumnum = m_Columnnum_Max;
                //m_lengthRowColNums.Add(mtemp_lengthRowColNum);

                //mtemp_lengthRowColNum = new lengthRowColNum();
               // m_sidelength = m_sidelength - 2 * m_spacing; colState = false;
                //return m_lengthRowColNum;
            }
            if (m_Drerow > FLT_MIN) { m_Rownum_Max++; }         //保存最大情况，以防无法优化
            if (m_Drecol > FLT_MIN) { m_Columnnum_Max++; }

            m_sidelength = m_sidelength - 2 * m_spacing; colState = false;

            //若初始情况无法跳出，进行判断
            while (m_sidelength >= m_side_min)        //如果小于最小边长  直接退回原始状况并返回
            {

                int temp_numDrow = (int)(m_rect_h / m_sidelength);         //h=n*d+Dre
                m_Drerow = m_rect_h - m_sidelength * temp_numDrow;         //求取行余量

                //判读余量的情况，进行决策
   
                if ((m_Drerow + m_sidelength) <= m_Dmax+FLT_MIN)      //如果Dre+D<Dmax +FLT_MIN解决了整数倍的情况
                {
                    m_Rownum = temp_numDrow;
                    rowState = true;
                }
                else if (m_Drerow >= m_side_min)           //如果Dre>Dmin
                {
                    m_Rownum = temp_numDrow + 1;
                    rowState = true;
                }
                else { m_sidelength = m_sidelength - 2 * m_spacing; rowState = false; }

                if (rowState == true)      //如果行好使则进行列判断
                {
                    int temp_numDcol = (int)(m_rect_w / m_sidelength);         //h=n*d+Dre
                    m_Drecol = m_rect_w - m_sidelength * temp_numDcol;         //求取列余量
                   // if (m_Drecol < FLT_MIN) { m_Columnum = m_Drecol; }         //刚好整数倍
                    if ((m_Drecol + m_sidelength) <= m_Dmax+FLT_MIN)      //如果Dre+D<Dmax  包含了整数倍的情况
                    {
                        m_Columnum = temp_numDcol;
                        colState = true;
                    }
                    else if (m_Drerow >= m_side_min)           //如果Dre>Dmin
                    {
                        m_Columnum = temp_numDcol + 1;
                        colState = true;
                    }
                    else { m_sidelength = m_sidelength - 2 * m_spacing; colState = false; }
                  }
                if (rowState && colState)
                {
                    mtemp_lengthRowColNum.Sidelength = m_sidelength;    //边长
                    mtemp_lengthRowColNum.Rownum = m_Rownum;        //行数
                    mtemp_lengthRowColNum.Colnumnum = m_Columnum;      //列数
                    m_lengthRowColNums.Add(mtemp_lengthRowColNum);
                    mtemp_lengthRowColNum = new lengthRowColNum();
                    m_sidelength = m_sidelength - 2 * m_spacing;
                    rowState = false;
                    colState = false;
                }
            }
            mtemp_lengthRowColNum.Sidelength = m_Dmax;
            mtemp_lengthRowColNum.Rownum = m_Rownum_Max;
            mtemp_lengthRowColNum.Colnumnum = m_Columnnum_Max;
            m_lengthRowColNums.Add(mtemp_lengthRowColNum);
            return m_lengthRowColNums;
          
        }


        //注意输入和输出类型 ，利用mypoloygons类进行旋转及求交
        /// <summary>
        /// 划分最小格子，并求交集
        /// </summary>
        /// <param name="pgPaths">最小多边形盒子构成的Paths</param>
        /// <param name="rotation"></param>
        /// <param name="side_length"></param>
        /// <param name="rownum"></param>
        /// <param name="colnum"></param>
        /// <returns></returns>
        private List<Paths> GetMeshPologons(Path pgPaths, float rotation, int side_length, int rownum, int colnum) 
        {
            PointMatrix matrix = new PointMatrix(rotation);     //生成旋转矩阵

            PointMatrix matrix1 = new PointMatrix(-rotation);   //反旋转矩阵，恢复数据使用

            mypolygons pgPolygons = new mypolygons(pgPaths);
            pgPolygons.applyMatrix(matrix);                    //对多边形轮廓进行旋转
            pgPolygons.calculateAABB();


            mypolygons mmpgPolygons = new mypolygons(mOrignalPaths);
            pgPolygons.applyMatrix(matrix);   

            Paths convPgpaths = pgPolygons.getPologons();  //获得旋转后返回的pologons值
            IntPoint minPoint = pgPolygons.getMinPoint();  //获得最小包围盒的最小点和最大点
            IntPoint maxPoint = pgPolygons.getMaxPoint();

            IntPoint[,] IntPointDotMatrix = new IntPoint[rownum+1,colnum+1];     //定义点阵存储交点

            List<Paths> meshPath = new List<Paths>();       //网格输出，每一行一个paths

            //存储数据点矩阵
            for (int i = 0; i < rownum + 1; i++)
            {
                for (int j = 0; j < colnum + 1; j++)
                {
                    if (i < rownum)    //不是最后一行
                    { IntPointDotMatrix[i, j].Y = maxPoint.Y - i * side_length; }
                    else { IntPointDotMatrix[i, j].Y = minPoint.Y; }           //最后一行，直接等于ymin

                    if (j < colnum)
                    { IntPointDotMatrix[i, j].X = minPoint.X + j * side_length; }
                    else { IntPointDotMatrix[i, j].X = maxPoint.X; }           //最后一列
                }
            }


            // 存储网格多边形
            List<maddtionpologonIndex> mtempoutAddtionPathsIndex = new List<maddtionpologonIndex>();
            for (int i = 0; i < rownum ; i++)    //行数
            {
                Paths tempPaths = new Paths();    //存储行Paths
                for (int j = 0; j < colnum ; j++) //列数
                {
                    Path pathgezi = new Path();     
                    pathgezi.Add(IntPointDotMatrix[i,j]);          //逆时针存储
                    pathgezi.Add(IntPointDotMatrix[i+1, j]);
                    pathgezi.Add(IntPointDotMatrix[i+1, j+1]);
                    pathgezi.Add(IntPointDotMatrix[i, j+1]);
                    tempPaths.Add(pathgezi);
                }
                mypolygons temppgPolygons = new mypolygons(tempPaths);
                temppgPolygons.applyMatrix(matrix1);              //旋转回去，恢复最开始的方向

                ////不求交集时使用
               // meshPath.Add(temppgPolygons.getPologons());              

                ////求交集时使用,但是如果temppgPolygons完全在原始多边形内就会被合并了。需修改intersection函数2016_04_21
                //注意在填充时为保证位置上的相邻判断，在结果中添加了空格子占位 2016_04_22
                //Paths m_testOut = temppgPolygons.intersection(mOrignalPaths);
                //meshPath.Add(temppgPolygons.intersection(mOrignalPaths));
                List<maddtionpologonIndex> tempoutAddtionPathsIndex = new List<maddtionpologonIndex> ();
                meshPath.Add(temppgPolygons.intersection(mOrignalPaths, i, ref tempoutAddtionPathsIndex));

                mtempoutAddtionPathsIndex.AddRange(tempoutAddtionPathsIndex);  //给符合的多边形
            }
            outAddtionRectPologons.Add(mtempoutAddtionPathsIndex);
           return meshPath;
        }




        //多边形分解后的数据转存为浮点数组 
        static private PointF[] ConvexPolygonToPointFArray(List<Vector2> pg, float scale)
        {
            PointF[] result = new PointF[pg.Count];
            for (int i = 0; i < pg.Count; ++i)
            {
                result[i].X = (float)pg[i].x / scale;
                result[i].Y = (float)pg[i].y / scale;
            }

            return result;
        }

        //需要从浮点数到整数 搜小倍数，为小数0.001
        static private Path ConvexPolygonToPath(List<Vector2> pg, float scale)
        {
            PointF[] repointfs = ConvexPolygonToPointFArray(pg, scale);
            Point startPoint = new Point();
            Path resultpath = new Path();
            foreach (PointF pm in repointfs)
            {
                startPoint = Point.Round(pm);
                resultpath.Add(new IntPoint(startPoint.X, startPoint.Y));
            }
            return resultpath;
        }


        ////求交集
        //public Paths intersection(Paths polygons, Paths other_pologon)
        //{
        //    Paths solution3 = new Paths();
        //    Clipper co1 = new Clipper(0);          //不知道怎么用,在Polygons中有使用
        //    co1.AddPaths(polygons, ClipperLib.PolyType.ptSubject, true);
        //    co1.AddPaths(other_pologon, ClipperLib.PolyType.ptClip, true);
        //    co1.Execute(ClipperLib.ClipType.ctIntersection, solution3);
        //    return solution3;
        //}
       
    }

    /// <summary>
    /// 作为计算边长输出数据结构
    /// </summary>
    class lengthRowColNum
    {
        public float Sidelength;        //  边长
        public int Rownum;              //行数
        public int Colnumnum;           //列数
    }
}
