using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ClipperLib;

namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;


    //参数选择枚举类型
    #region
    //选择路劲生成方法
    enum pathGenerSoluType : int
    {
        convexSolution,
        squareSolution,
    }

    //凸分区方法种类
    enum convexCutType : int
    {
        fullConvex,
        approximateConvex1,
        approximateConvex2,
    }

    //子填充方法选择
    enum subsecInfillType : int
    {
        smallDirec,
        squareGride,
        mixDirecSquare,
    }

    //选择正方形网格类型
    enum choiseType : int
    {
        maxiside,
        firstside,
        minside,
        minpathnum,
        minnonpathlen,
    }

    //正方形格子下填充倾角类型patternA、B
    enum fillOritionType : int
    {
        patternA,
        patternB,
    }

    #endregion

    public partial class Form1 : Form
    {

        private Bitmap mybitmap;         //全局bitmap对象
        private Bitmap mybitmap1;        //导入轮廓bitmap对象
        private float scale = 1000;       //求偏置时取整放大倍数,调整精度
        private float scalef = 2.5f;       //暂时没有用，仅分割后显示倍数
        private GraphicsPath path = new GraphicsPath();  //画布中保存的图形对象bitmap
        private GraphicsPath path1 = new GraphicsPath();  //画布中保存的图形对象bitmap1
        private List<PointF> AABBoriginal = new List<PointF>();  //存储初始多边形的最小包围盒，用来平移最终图象

        //用于保存全局数据
        private PointF[] originalPologon;       //初始数据源
        private Paths decompPologonDatas = new Paths();      //进行分解后的多边形组
        private Paths offsetPologonDatas = new Paths();      //分解后的多边形组进行偏置产生的新多边形组
        private List<float> listRotationAngle = new List<float>();  //存储分解后多边形的最小包围盒与x轴角度
        private Paths OutMinRectList = new Paths();         //所有多边形的最小包围盒
        Paths infillpathSet = new Paths();                  //内部填充路劲集
        Paths orderedInfillpathSet = new Paths();           //顺序优化的内部填充路径集
        private float viewScale=1.0f;                       //存储显示倍数
        private bool changeZoomState = true;                //存储放大或缩小状态

        //给DrawBitmap确定是line还是多边形
        private static bool isDrawPologonState = true;
        private static bool isDrawLineState = false;

         List<lengthRowColNum> moutlengthRowColNum = new List<lengthRowColNum>();                // //输出  边长、行数、列数
         List<List<Paths>> outMeshRectPologons = new List<List<Paths>>();
         List<List<maddtionpologonIndex>> outAddtionRectPologons = new List<List<maddtionpologonIndex>>(); 


        List<Paths> meshOutRec = new List<Paths>();
        List<Paths> outInfillPathsMatrix = new List<Paths>();  //填充完毕的曲线
        Paths finalMergerPaths = new Paths();
        Paths secondInfillPath = new Paths();     //子区域填充时第二层路径集合
        Paths thirdInfillPaths = new Paths();    //求交集时的多余区域
        
        //2016-06-13
        List<imgInfo> imgInfoSaves = new List<imgInfo>();        //格子求交时存储所有的分区路径结果
        imgInfo outimgInfo = new imgInfo();        //格子求交时存储最终的分区路径结果

        pathGenerSoluType mypathGenerSoluType = pathGenerSoluType.convexSolution; //选择路径生成方案，凸分区或正方形网格
        convexCutType myconvexCutType = convexCutType.fullConvex;                 //凸分区方法选择
        subsecInfillType mysubsecInfillType = subsecInfillType.smallDirec;        //子区域填充方案选择
        choiseType mychoiseType = choiseType.firstside;                           //首选则正方形网格的选择项
        fillOritionType myfillOritionType = fillOritionType.patternA;             //正方形网格填充pattern
        private float concaveThreshold = 0.01f;          //凸分区下的凹度阈值
        private float fullDegreeThread = 0.5f;          //复合填充下的饱满度阈值
        private float fillDegreeThread = 5.0f;          //复合填充下的充盈度阈值

        private bool isConvexSolutionState = true;
        
        
        List<List<Vector2>> firstoutpologonsList = new List<List<Vector2>>();


        //2016_04_23
        List<RectInfos> RectPologonInfos = new List<RectInfos>();  //存储子分区的面积，饱满度、充盈度List
        List<List<Vector2>> listOfConvexPolygonPartionBaya = new List<List<Vector2>>();  //  存储分区后的Vector2型数据

        List<Vector2> newpointInList6 = new List<Vector2>();

        


        public Form1()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
            mybitmap = new Bitmap(
            pictureBox1.ClientRectangle.Width,
            pictureBox1.ClientRectangle.Height,
            PixelFormat.Format32bppArgb);
            //nudOffset.Text = "-0.5";       // 偏置量初始化   
            zoom_Times.Text = "3.0";       // 最小网格边长
            TextBox_lineSpace.Text= "0.5";        //偏置大小
            textBox_Rotation.Text = "8.0";      //最大网格边长

            concaveDegree_Te.Text = "0.01";
            fillDegree_Te.Text = "0.5";
            fullDegree_Te.Text = "5.0";

        }

        //多边形数据转存为浮点数组 Path = List<IntPoint>;
        static private PointF[] PolygonToPointFArray(Path pg, float scale)
        {
            PointF[] result = new PointF[pg.Count];
            for (int i = 0; i < pg.Count; ++i)
            {
                result[i].X = (float)pg[i].X / scale;
                result[i].Y = (float)pg[i].Y / scale;
            }
            return result;
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

        static private PointF[] PointFArrayMove(PointF[] pf, PointF moveBasePoint)
        {
            PointF[] result = new PointF[pf.Length];
            for (int i = 0; i < pf.Length; ++i)
            {
                result[i].X = (float)pf[i].X - moveBasePoint.X;
                result[i].Y = (float)pf[i].Y-moveBasePoint.Y;
            }
            return result;
        }

        //需要从浮点数到整数 搜小倍数，为小数0.001
        static private Path ConvexPolygonToPath(List<Vector2> pg, float scale)
        {
            PointF[] repointfs = ConvexPolygonToPointFArray(pg, scale);
            Point startPoint = new Point();
            Path resultpath=new Path();
            foreach (PointF pm in repointfs)
            { 
               startPoint=Point.Round(pm);
               resultpath.Add(new IntPoint(startPoint.X, startPoint.Y));
            }
           return resultpath;
        }


        //2016.3.1 此版可以的相邻两段path之间会链接，可以用在最终路径添加上
        //private void DrawBitmap(Paths subjects 路径集合, Color stroke 线色, Color brush ,float viewScale 放大倍数,bool lineOrPologonstate line和pologon状态 )
        private void DrawBitmap(Paths subjects, Color stroke, Color brush, float viewScale, bool lineOrPologonstate)
        {
            //Cursor.Current = Cursors.WaitCursor;
            ////            3.using语句，定义一个范围，在范围结束时处理对象。 
            ////场景： 
            ////当在某个代码段中使用了类的实例，而希望无论因为什么原因，只要离开了这个代码段就自动调用这个类实例的Dispose。 
            ////要达到这样的目的，用try...catch来捕捉异常也是可以的，但用using也很方便。
            //using (Graphics newgraphic = Graphics.FromImage(mybitmap))
            //using (GraphicsPath path = new GraphicsPath())
            //{
            //newgraphic.SmoothingMode = SmoothingMode.AntiAlias;
            //newgraphic.Clear(Color.White);
            ////draw subjects ...
            Graphics newgraphic = Graphics.FromImage(mybitmap);
            newgraphic.SmoothingMode = SmoothingMode.AntiAlias;
            //           newgraphic.Clear(Color.White); 
            path.FillMode = FillMode.Winding;
            foreach (Path pg in subjects)
            {
                if (pg.Count == 0) { continue; }   //如果pg是空，2016_04_22为了格子画时可能有空的占位
                //PointF[] pts = PolygonToPointFArray(pg, scale/float.Parse(zoom_Times.Text));
                PointF[] pt = PolygonToPointFArray(pg, scale / viewScale);
                //平移到中间
                PointF[] pts = PointFArrayMove(pt,
                    new PointF(AABBoriginal[1].X * viewScale - (pictureBox1.Width / 2+20), AABBoriginal[1].Y * viewScale - (pictureBox1.Height / 2-40)));
                if (lineOrPologonstate) {  path.AddPolygon(pts);  }
                else { path.AddLines(pts); }
                pts = null;
            }
            using (Pen myPen = new Pen(stroke, (float)1.0))
            using (SolidBrush myBrush = new SolidBrush(brush))
            {
                //            newgraphic.FillPath(myBrush, path);
                newgraphic.DrawPath(myPen, path);
                pictureBox1.Image = mybitmap;
                path.Reset();
            }
            //}
        }


        private void DrawBitmapLines(Paths subjects, Color stroke, Color brush, float viewScale)
        {
            Graphics newgraphic = Graphics.FromImage(mybitmap);
            newgraphic.SmoothingMode = SmoothingMode.AntiAlias;
            //           newgraphic.Clear(Color.White); 
            path.FillMode = FillMode.Winding;

            foreach (Path pg in subjects)
            {
                if (pg.Count == 0) { continue; }   //如果pg是空，2016_04_22为了格子画时可能有空的占位
                //PointF[] pts = PolygonToPointFArray(pg, scale/float.Parse(zoom_Times.Text));
                PointF[] pt = PolygonToPointFArray(pg, scale / viewScale);
                //平移到中间
                PointF[] pts = PointFArrayMove(pt,
                    new PointF(AABBoriginal[1].X * viewScale - (pictureBox1.Width / 2 + 20), AABBoriginal[1].Y * viewScale - (pictureBox1.Height / 2 - 40)));
                    
                path.AddLines(pts); 
                pts = null;

                using (Pen myPen = new Pen(stroke, (float)1.0))
                using (SolidBrush myBrush = new SolidBrush(brush))
                {
                    //            newgraphic.FillPath(myBrush, path);
                    newgraphic.DrawPath(myPen, path);
                    pictureBox1.Image = mybitmap;
                    path.Reset();
                }
                //}

                path = new GraphicsPath();
            }
        }


        /// <summary>
        /// 画分区后的图形，调试时采用的，现在没有采用
        /// </summary>
        /// <param name="subjects"></param>
        /// <param name="stroke"></param>
        /// <param name="brush"></param>
        private void DrawBitmapVector(List<List<Vector2>> subjects, Color stroke, Color brush)
        {
            //Cursor.Current = Cursors.WaitCursor;
            ////            3.using语句，定义一个范围，在范围结束时处理对象。 
            ////场景： 
            ////当在某个代码段中使用了类的实例，而希望无论因为什么原因，只要离开了这个代码段就自动调用这个类实例的Dispose。 
            ////要达到这样的目的，用try...catch来捕捉异常也是可以的，但用using也很方便。
            //using (Graphics newgraphic = Graphics.FromImage(mybitmap))
            //using (GraphicsPath path = new GraphicsPath())
            //{
            //newgraphic.SmoothingMode = SmoothingMode.AntiAlias;
            //newgraphic.Clear(Color.White);
            ////draw subjects ...
            Graphics newgraphic = Graphics.FromImage(mybitmap);
            newgraphic.SmoothingMode = SmoothingMode.AntiAlias;
            //           newgraphic.Clear(Color.White); 
            path.FillMode = FillMode.Winding;
            foreach (List<Vector2> pg in subjects)
            {
                //原始数据被放大了
                PointF[] pts = ConvexPolygonToPointFArray(pg, scalef);
                path.AddPolygon(pts);
                pts = null;
            }
            using (Pen myPen = new Pen(stroke, (float)0.5))
            using (SolidBrush myBrush = new SolidBrush(brush))
            {
//              newgraphic.FillPath(myBrush, path);
                newgraphic.DrawPath(myPen, path);
                pictureBox1.Image = mybitmap;
                path.Reset();

            }
            //}
        }

        /// <summary>
        /// 加载原始数据
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public DataSet LoadDataFromExcel(string Path)
        {
            string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + Path + ";Extended Properties='Excel 12.0; HDR=NO; IMEX=1'";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = new DataSet ();
            strExcel = "select * from [sheet1$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            //DataSet dw = new DataSet();
            myCommand.Fill(ds);
            //myCommand.Fill(dw);
            return ds;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text =
            "Tip: Use the mouse-wheel (or +,-,0) to adjust the offset of the solution polygons.";
        }

        /// <summary>
        /// 分区及偏置一圈点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bk_Generate_Click(object sender, EventArgs e)
        {
           
            List<List<Vector2>> outpologonsList = new List<List<Vector2>>();
            List<List<Vector2>> newpointLists = new List<List<Vector2>>(inputPologonVectors);
            AABBoriginal = AABBf1(inputPologonVectors[0]);
           
            firstPartition mfirstPartition = new firstPartition();
            outpologonsList = mfirstPartition.excutePartition(newpointLists);
            firstoutpologonsList = new List<List<Vector2>>(outpologonsList);

            //// List<List<Vector2>> listOfConvexPolygonPartionBaya = new List<List<Vector2>>();
            foreach (List<Vector2> HullpointOutList in outpologonsList)      //第一次分区之后进行第二次分区      
            {
                List<Vector2> vertices = HullpointOutList;
                List<List<Vector2>> listOfConvexPolygonPoints = BayazitDecomposer.ConvexPartition(vertices, concaveThreshold);
                listOfConvexPolygonPartionBaya.AddRange(listOfConvexPolygonPoints);    //插入一段list
            }


            //画分区后的轮廓
            Paths subj = new Paths(listOfConvexPolygonPartionBaya.Count);
            
            //2016_04_23加入填充方法选择
            //List<RectInfos>  RectPologonInfos = new List<RectInfos> (); 

            foreach (List<Vector2> pointsOfIndivualConvexPolygon in listOfConvexPolygonPartionBaya)
            {
                List<Vector2> currentPolygonVertices = pointsOfIndivualConvexPolygon;

                //2016.04.21 计算最小包围盒的倾斜角度
                CalculateMinRect minRect1 = new CalculateMinRect();
                minRect1.MinAreaRec(currentPolygonVertices);
                listRotationAngle.Add (minRect1.OritionAngle);
                RectInfos rectInfo = minRect1.getPologonRectInfos();       //返回多边形饱和度充盈度等
                List<Vector2> OutpointInList = minRect1.GetOBBPoints();   //返回最小盒子多边形
                OutMinRectList.Add(ConvexPolygonToPath(OutpointInList, 1/scale));
                RectPologonInfos.Add(rectInfo);           //2016——04——23存储多边形饱和度充盈度等List
                
                //数据先扩大了Scale倍，保存精度，变成IntPoint
                Path pathConver= ConvexPolygonToPath(currentPolygonVertices, 1/scale);
                subj.Add(pathConver);
            }

            DrawBitmap(subj, Color.Blue,
              Color.FromArgb(0x60, 0, 0, 0xFF), 1.0f, isDrawPologonState);

            DrawBitmap(OutMinRectList, Color.Black,
             Color.FromArgb(0x60, 0, 0, 0xFF), 1.0f, isDrawPologonState);

            decompPologonDatas = subj;   //保存进行分解后的多边形组

             }



        //2016——04——23由于正方形格子的输入是list<vector2> 而改
        private void bk_Infill_Click(object sender, EventArgs e)
        {
            // Path tempPath = new Path(offsetPologonDatas[0]);
            //tempPath = offsetPologonDatas[0];           //注意此时修改temp会改变offsetPologonDatas[0]

            float mlineSpacing = float.Parse(TextBox_lineSpace.Text);
            float maxLimitSide = float.Parse(textBox_Rotation.Text);
            float minLimitSide = float.Parse(zoom_Times.Text);
            int mzoomlineSpacing = (int)(mlineSpacing*scale);
           
            foreach (List<Vector2> pointsOfIndivualConvexPolygon in listOfConvexPolygonPartionBaya)
            {
               switch(mysubsecInfillType)
               {
                   case subsecInfillType.smallDirec:
                       Path tempsecondinfillpath = new Path();
                       Path temppathConver = ConvexPolygonToPath(pointsOfIndivualConvexPolygon, 1 / scale);
                       Path fillpath = InfillPath.generateLineInfill(temppathConver, mzoomlineSpacing, listRotationAngle[listOfConvexPolygonPartionBaya.IndexOf(pointsOfIndivualConvexPolygon)], ref tempsecondinfillpath);
                       infillpathSet.Add(fillpath);
                       infillpathSet.Add(tempsecondinfillpath);
                       break;

                   case subsecInfillType.squareGride:
                       Paths tempPathafterMesh = generMeshPath(pointsOfIndivualConvexPolygon, mlineSpacing, maxLimitSide, minLimitSide);
                       infillpathSet.AddRange(tempPathafterMesh);
                       break;

                   case subsecInfillType.mixDirecSquare:
                        RectInfos temp = RectPologonInfos[listOfConvexPolygonPartionBaya.IndexOf(pointsOfIndivualConvexPolygon)];
                        if ((temp.m_fulldegree > fullDegreeThread) || (temp.m_filldegree < fillDegreeThread))
                        {
                            Path mtempsecondinfillpath = new Path();
                            Path mtemppathConver = ConvexPolygonToPath(pointsOfIndivualConvexPolygon, 1 / scale);
                            Path mfillpath = InfillPath.generateLineInfill(mtemppathConver, mzoomlineSpacing, listRotationAngle[listOfConvexPolygonPartionBaya.IndexOf(pointsOfIndivualConvexPolygon)], ref mtempsecondinfillpath);
                            infillpathSet.Add(mfillpath);
                            infillpathSet.Add(mtempsecondinfillpath);
                        }
                        else 
                        {
                            Paths mtempPathafterMesh = generMeshPath(pointsOfIndivualConvexPolygon, mlineSpacing, maxLimitSide, minLimitSide);
                            infillpathSet.AddRange(mtempPathafterMesh);
                        }
                        break;
               }
               
            }

        }


        private void Merge_bk_Click(object sender, EventArgs e)
        {
           int mzoomlineSpacing = (int)(double.Parse(TextBox_lineSpace.Text) * scale);
           List<Paths> tempPathss = new List<Paths>();
           tempPathss.Add(infillpathSet);
           finalMergerPaths = PathsMerge.PathsMatrixMergeByRandom(tempPathss, 2.5f * mzoomlineSpacing);
        }


        private void Bk_orderPath_Click(object sender, EventArgs e)
        {
            //IntPoint startPoint = new IntPoint(0,0);
            IntPoint startPoint = new IntPoint(AABBoriginal[1].X * scale, AABBoriginal[1].Y * scale);   
            PathOrderOptimizer mOrderPathOptimizer = new PathOrderOptimizer(startPoint, finalMergerPaths);
            orderedInfillpathSet = mOrderPathOptimizer.GetOrderOptimizePaths();
            float nonPathdiscost = mOrderPathOptimizer.pathDiscost / scale;


            Tour tour = mOrderPathOptimizer.mtour;

            Population pop = new Population(50, true, finalMergerPaths, startPoint, tour);

            for (int i = 0; i < 400; i++)
            {
                pop = GA.evolvePopulation(pop);
            }

            Tour tour1 = pop.getFittest();
            //float nonPathdiscost1 = pop.getFittest().getDistance(finalMergerPaths, startPoint) / scale;
            //orderedInfillpathSet = pop.getFittest().orderPath;

            float nonPathdiscost1 = tour1.distance / scale;
            orderedInfillpathSet = new Paths(tour1.orderPath);


            Path startpath = new Path();
            startpath.Add(startPoint);
            orderedInfillpathSet.Insert(0, startpath);    //插入起始点，作为一条独立的路径

            DrawBitmap(orderedInfillpathSet, Color.Red,       //***注意drawBitmap画的是闭合曲线
            Color.FromArgb(0x60, 0, 0, 0xFF), 1.0f, isDrawLineState);
        }
       

        //刷新bitmap绘图
        void ZoomAndRefreshBitmap(bool tchangeZoomState)
        {
            if (mybitmap != null)               //删除bitmap对象
                mybitmap.Dispose();
            mybitmap = new Bitmap(
                pictureBox1.ClientRectangle.Width,
                pictureBox1.ClientRectangle.Height,
                PixelFormat.Format32bppArgb);
            viewScale = (float)(tchangeZoomState ? 1.1 : 0.9) * viewScale;  //更新放大倍数
            pictureBox1.Image = null;

          //  DrawBitmap(OutMinRectList, Color.Red,            //每个多边形的最小包围盒
          //Color.FromArgb(0x60, 0, 0, 0xFF), viewScale, isDrawPologonState);

           // DrawBitmap(decompPologonDatas, Color.Pink,           //画分解后的图形
            //  Color.FromArgb(0x60, 0, 0, 0xFF), viewScale, isDrawPologonState);

            //DrawBitmapVector(firstoutpologonsList, Color.Black,           //画分解后的图形
            //  Color.FromArgb(0x60, 0, 0, 0xFF));

            //DrawBitmap(offsetPologonDatas, Color.Green,         //画偏置后的图形
            //  Color.White, viewScale, isDrawPologonState);

            //DrawBitmapLines(infillpathSet, Color.Blue,             //***注意drawBitmap画的是闭合曲线。画填充线
            //  Color.FromArgb(0x60, 0, 0, 0xFF), viewScale);

            DrawBitmap(orderedInfillpathSet, Color.Red,       //***注意drawBitmap画的是闭合曲线 ,顺序曲线
            Color.FromArgb(0x60, 0, 0, 0xFF), viewScale, isDrawLineState);

           // DrawBitmap(newpointInList6, Color.Pink,       //***注意drawBitmap画的是闭合曲线
           //Color.FromArgb(0x60, 0, 0, 0xFF), viewScale, isDrawLineState);



            //foreach (Paths meshRect in meshOutRec)
            //{
            //    DrawBitmap(meshRect, Color.Pink,            //网格路径 一行行的画 
            //    Color.FromArgb(0x60, 0, 0, 0xFF), viewScale, isDrawPologonState);
            //}

            //foreach (Paths outpath in outInfillPathsMatrix)
            //{
            //    DrawBitmapLines(outpath, Color.Red,            //网格填充路径 一行行的画
            //    Color.FromArgb(0x60, 0, 0, 0xFF), viewScale);
            //}

            DrawBitmapLines(finalMergerPaths, Color.Green,            //合并后的路径
               Color.FromArgb(0x60, 0, 0, 0xFF), viewScale);
        }


        //暂时没有使用0228，设置控件的中间
        void SetCenterScreen(Control control)
        {
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            int targetLocationLeft;
            int targetLocationTop;
            targetLocationLeft = (screenWidth - control.Width) / 2;
            targetLocationTop = (screenHeight - control.Height) / 2;
            if (control.Parent != null)
                control.Location = control.Parent.PointToClient(new Point(targetLocationLeft, targetLocationTop));
            else
                control.Location = new Point(targetLocationLeft, targetLocationTop);
        }
   
        //求一个多边形的最小包围盒
        private List<IntPoint> AABBpath(Path pgpath)     //最小包围盒
        {
            List<Int64> xarr=new List<Int64>();
            List<Int64> yarr=new List<Int64>();
            List<IntPoint> result = new List<IntPoint>();
            foreach(IntPoint po in pgpath)
            {
                xarr.Add(po.X);
                yarr.Add(po.Y);
            }
            result.Add(new IntPoint(xarr.Min(), yarr.Min()));  //逆时针AABB 左下
            result.Add(new IntPoint(xarr.Max(), yarr.Min()));  //右下point
            result.Add(new IntPoint(xarr.Max(), yarr.Max()));  //右上point
            result.Add(new IntPoint(xarr.Min(), yarr.Max()));  //左上point
            return result;
        }

        //获得PointF[] pointfArry数组的最小包围盒
        private List<PointF> AABBf(PointF[] pointfArry)     //最小包围盒
        {
            List<float> xarr = new List<float>();
            List<float> yarr = new List<float>();
            List<PointF> result = new List<PointF>();
            foreach (PointF po in pointfArry)
            {
                xarr.Add(po.X);
                yarr.Add(po.Y);
            }
            result.Add(new PointF(xarr.Min(), yarr.Min()));  //逆时针AABB 左下
            result.Add(new PointF(xarr.Max(), yarr.Min()));  //右下point
            result.Add(new PointF(xarr.Max(), yarr.Max()));  //右上point
            result.Add(new PointF(xarr.Min(), yarr.Max()));  //左上point
            return result;
        }

        //鼠标滚轮滚动事件，放大和缩小，e.Delta>0，向上滚动
        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0 && viewScale < 10.0)
            {
                changeZoomState = true;
                ZoomAndRefreshBitmap(changeZoomState);
            }
            else if (e.Delta < 0 && viewScale > 0.5) 
            {
                changeZoomState = false;
                ZoomAndRefreshBitmap(changeZoomState);
            }
        }

        private void BK_generGcode_Click(object sender, EventArgs e)
        {
            String myFilePath = @"C:\Users\Administrator\Desktop\分区0102\file.txt";
            int machineSpeed=100;
            int mTravelSpeed=200;
            mGcodeExport myGcodeExport = new mGcodeExport(myFilePath);
            myGcodeExport.ConfigMachingParam(machineSpeed, mTravelSpeed, (int)scale);
            myGcodeExport.AddPathsOrPologons(decompPologonDatas,isDrawPologonState);
            myGcodeExport.AddPathsOrPologons(offsetPologonDatas, isDrawPologonState);
            myGcodeExport.AddPathsOrPologons(orderedInfillpathSet, isDrawLineState);
            myGcodeExport.GenerateGcode();
            textBox_GcodeShow.Text = myGcodeExport.outGcodeStr;
        }

        private List<PointF> AABBf1(List<Vector2> pointfArry)     //最小包围盒
        {
            List<float> xarr = new List<float>();
            List<float> yarr = new List<float>();
            List<PointF> result = new List<PointF>();
            foreach (Vector2 po in pointfArry)
            {
                xarr.Add(po.x);
                yarr.Add(po.y);
            }
            result.Add(new PointF(xarr.Min(), yarr.Min()));  //逆时针AABB 左下
            result.Add(new PointF(xarr.Max(), yarr.Min()));  //右下point
            result.Add(new PointF(xarr.Max(), yarr.Max()));  //右上point
            result.Add(new PointF(xarr.Min(), yarr.Max()));  //左上point
            return result;
        }

        private Paths generMeshPath(List<Vector2> inPologon, float m_Spaceing, float m_Lmax, float m_SideLengthMin)
        {
            List<List<Vector2>> newpointLists = new List<List<Vector2>>();
            newpointLists.Add(inPologon);

            //float mlineSpacing = float.Parse(TextBox_lineSpace.Text);
            //float maxLimitSide = float.Parse(textBox_Rotation.Text);
            //float minLimitSide = float.Parse(zoom_Times.Text);

            imgInfoSaves.Clear();        //清空数据
            outimgInfo = new imgInfo();  //清空数据
            PologonSqualMesh meshSample = new PologonSqualMesh(newpointLists, m_Spaceing, m_Lmax, m_SideLengthMin);
            meshSample.CalculatePologonSqualMesh();

            for (int inde = 0; inde < meshSample.outMeshRectPologons.Count; ++inde)
            {
                Paths mtemp_infillpathSet = new Paths();                  //内部填充路劲集
                Paths mtemp_orderedInfillpathSet = new Paths();           //顺序优化的内部填充路径集

                List<Paths> mtemp_outInfillPathsMatrix = new List<Paths>();  //填充完毕的曲线
                Paths mtemp_finalMergerPaths = new Paths();
                Paths mtemp_secondInfillPaths = new Paths();     //子区域填充时第二层路径集合           
                List<Paths> mtemp_meshOutRec = new List<Paths>();
                List<maddtionpologonIndex> mtemp_outAddtionRectPologons = new List<maddtionpologonIndex>();  //存储求交后的多余路径及位置2016-06-03

                mtemp_meshOutRec = meshSample.outMeshRectPologons[inde];
                mtemp_outAddtionRectPologons = meshSample.outAddtionRectPologons[inde];  //到处多余的区域，单独填充2016-06-03
                lengthRowColNum meshlengthRowColNum = meshSample.moutlengthRowColNum[inde];             // //输出  边长、行数、列数  
                float rotationAngle = meshSample.rotationAngle;         //输出倾斜角

                List<Paths> afterOffestmeshRec = new List<Paths>();
                //网格阶段画图
                foreach (Paths meshRect in mtemp_meshOutRec)
                {
                    DrawBitmap(meshRect, Color.Pink,            //网格路径 一行行的画 
                    Color.FromArgb(0x60, 0, 0, 0xFF), viewScale, isDrawPologonState);
                }

                //网格填充  //注意空路径在上一层list中数量是1
                //List<Paths> mtemp_outInfillPathsMatrix = new List<Paths>();
                int mlineSpacingScale = (int)(m_Spaceing * scale);
                float angletemp = rotationAngle;        //路径中占位
                Path nopath = new Path();
                for (int i = 0; i < mtemp_meshOutRec.Count; i++)  //行
                {
                    Paths m_tempPaths = new Paths();
                    for (int j = 0; j < mtemp_meshOutRec[i].Count; j++)  //列
                    {
                        if (mtemp_meshOutRec[i][j].Count > 0)  //路径非空
                        {
                            Path tempsecondinfillpath = new Path();

                            if (myfillOritionType == fillOritionType.patternA)
                            { angletemp = (i + j) % 2 == 0 ? rotationAngle - 180 : rotationAngle - 90; }

                            if (myfillOritionType == fillOritionType.patternB)
                            { angletemp = (i + j) % 2 == 0 ? rotationAngle + 90 : rotationAngle; }
                            Path fillpath = InfillPath.generateLineInfill(mtemp_meshOutRec[i][j], mlineSpacingScale, angletemp, ref tempsecondinfillpath);
                            m_tempPaths.Add(fillpath);
                            if (tempsecondinfillpath.Count > 0) { mtemp_secondInfillPaths.Add(tempsecondinfillpath); }   //填充时的多条路径
                        }
                        else { m_tempPaths.Add(nopath); }   //路径为空需要占位   //行路劲集合
                    }

                    mtemp_outInfillPathsMatrix.Add(m_tempPaths);
                }

                foreach (Paths outpath in mtemp_outInfillPathsMatrix)
                {
                    DrawBitmapLines(outpath, Color.Red,            //网格填充路径 一行行的画
                    Color.FromArgb(0x60, 0, 0, 0xFF), viewScale);
                    mtemp_infillpathSet.AddRange(outpath);                     //存储为未合并路径
                }

                //处理网格求交时多余的区域
                foreach (maddtionpologonIndex tempgindex in mtemp_outAddtionRectPologons)
                {
                    Path tempsecondinfillpath = new Path();
                    if (myfillOritionType == fillOritionType.patternA)
                    { angletemp = (tempgindex.Rownum + tempgindex.Colnumnum) % 2 == 0 ? rotationAngle - 180 : rotationAngle - 90; }
                    if (myfillOritionType == fillOritionType.patternB)
                    { angletemp = (tempgindex.Rownum + tempgindex.Colnumnum) % 2 == 0 ? rotationAngle + 90 : rotationAngle ; }
                    Path fillpath = InfillPath.generateLineInfill(tempgindex.pg, mlineSpacingScale, angletemp, ref tempsecondinfillpath);
                    mtemp_secondInfillPaths.Add(fillpath);
                    if (tempsecondinfillpath.Count > 0) { mtemp_secondInfillPaths.Add(tempsecondinfillpath); }
                }

                mtemp_infillpathSet.AddRange(mtemp_secondInfillPaths);                  //存储为未合并路径
                mtemp_finalMergerPaths = PathsMerge.PathsMatrixMergeByRandom(mtemp_outInfillPathsMatrix, 2.0f * mlineSpacingScale, mtemp_secondInfillPaths);

                IntPoint startPoint = new IntPoint(AABBoriginal[1].X * scale, AABBoriginal[1].Y * scale);
                PathOrderOptimizer mOrderPathOptimizer = new PathOrderOptimizer(startPoint, mtemp_finalMergerPaths);
                mtemp_orderedInfillpathSet = mOrderPathOptimizer.GetOrderOptimizePaths();
                float nonPathdiscost = mOrderPathOptimizer.pathDiscost / scale;

                Tour tour = mOrderPathOptimizer.mtour;

                Population pop = new Population(50, true, mtemp_finalMergerPaths, startPoint, tour);

                for (int i = 0; i < 400; i++)
                {
                    pop = GA.evolvePopulation(pop);
                }

                Tour tour1 = pop.getFittest();
                //float nonPathdiscost1 = pop.getFittest().getDistance(mtemp_finalMergerPaths, startPoint) / scale;
                //mtemp_orderedInfillpathSet = pop.getFittest().orderPath;

                float nonPathdiscost1 = tour1.distance / scale;
                mtemp_orderedInfillpathSet = new Paths(tour1.orderPath);

                Path startpath = new Path();
                startpath.Add(startPoint);
                mtemp_orderedInfillpathSet.Insert(0, startpath);    //插入起始点，作为一条独立的路径

                DrawBitmap(mtemp_orderedInfillpathSet, Color.Gray,       //***注意drawBitmap画的是闭合曲线
                Color.FromArgb(0x60, 0, 0, 0xFF), 1.0f, isDrawLineState);

                imgInfo m_imginfo = new imgInfo();
                m_imginfo.mspacing = TextBox_lineSpace.Text;
                m_imginfo.msidelength = meshlengthRowColNum.Sidelength;
                m_imginfo.mcolnum = meshlengthRowColNum.Colnumnum;
                m_imginfo.mrownum = meshlengthRowColNum.Rownum;
                m_imginfo.mtoolparhNum = mtemp_orderedInfillpathSet.Count - 1;
                m_imginfo.mnonTravellen = nonPathdiscost1;
                m_imginfo.orderPaths = new Paths(mtemp_orderedInfillpathSet);
                m_imginfo.meshOutRec = mtemp_meshOutRec;
                m_imginfo.infillPathset = new Paths(mtemp_infillpathSet);
                m_imginfo.mergerPaths = new Paths(mtemp_finalMergerPaths);
                m_imginfo.originalPaths = new Paths();
                m_imginfo.AABBoriginal = AABBoriginal;
                imgInfoSaves.Add(m_imginfo);         //存储分区路径结果
                //setFrmShowImg(m_imginfo);
                frmShowImg showfrm = new frmShowImg(m_imginfo);
                showfrm.Text = "frmShowImg" + "划分" + inde;
                showfrm.Show();
            }
            calcuateTheChoisedPath();
            imgInfo tempOutInfo = outimgInfo;
            return tempOutInfo.orderPaths;
        }

        private void Mesh_bk_Click(object sender, EventArgs e)
        {
            imgInfoSaves.Clear();
            List<List<Vector2>> newpointLists = new List<List<Vector2>>(inputPologonVectors);
            AABBoriginal = AABBf1(inputPologonVectors[0]);

            float mlineSpacing = float.Parse(TextBox_lineSpace.Text);
            float maxLimitSide = float.Parse(textBox_Rotation.Text);
            float minLimitSide = float.Parse(zoom_Times.Text);

           // List<maddtionpologonIndex> outAddtionRectPologons = new List<maddtionpologonIndex>();  //存储求交后的多余路径及位置2016-06-03
            PologonSqualMesh meshSample = new PologonSqualMesh(newpointLists, mlineSpacing, maxLimitSide, minLimitSide);
            meshSample.CalculatePologonSqualMesh();
            for (int inde = 0; inde < meshSample.outMeshRectPologons.Count; ++inde)
            {
                Paths mtemp_infillpathSet = new Paths();                  //内部填充路劲集
                Paths mtemp_orderedInfillpathSet = new Paths();           //顺序优化的内部填充路径集

                List<Paths> mtemp_outInfillPathsMatrix = new List<Paths>();  //填充完毕的曲线
                Paths mtemp_finalMergerPaths = new Paths();
                Paths mtemp_secondInfillPaths = new Paths();     //子区域填充时第二层路径集合           
                List<Paths> mtemp_meshOutRec = new List<Paths>();
                List<maddtionpologonIndex> mtemp_outAddtionRectPologons = new List<maddtionpologonIndex>();  //存储求交后的多余路径及位置2016-06-03

                mtemp_meshOutRec = meshSample.outMeshRectPologons[inde];
                mtemp_outAddtionRectPologons = meshSample.outAddtionRectPologons[inde];  //到处多余的区域，单独填充2016-06-03
                lengthRowColNum meshlengthRowColNum = meshSample.moutlengthRowColNum[inde];             // //输出  边长、行数、列数  
                float rotationAngle = meshSample.rotationAngle;         //输出倾斜角

                List<Paths> afterOffestmeshRec = new List<Paths>();
                //网格阶段画图
                foreach (Paths meshRect in mtemp_meshOutRec)
                {
                    DrawBitmap(meshRect, Color.Pink,            //网格路径 一行行的画 
                    Color.FromArgb(0x60, 0, 0, 0xFF), viewScale, isDrawPologonState);
                }

                //网格填充  //注意空路径在上一层list中数量是1
                //List<Paths> mtemp_outInfillPathsMatrix = new List<Paths>();
                int mlineSpacingScale = (int)(mlineSpacing * scale);
                float angletemp = rotationAngle;        //路径中占位
                Path nopath = new Path();
                for (int i = 0; i < mtemp_meshOutRec.Count; i++)  //行
                {
                    Paths m_tempPaths = new Paths();
                    for (int j = 0; j < mtemp_meshOutRec[i].Count; j++)  //列
                    {
                        if (mtemp_meshOutRec[i][j].Count > 0)  //路径非空
                        {
                            Path tempsecondinfillpath = new Path();
                            if (myfillOritionType == fillOritionType.patternA)
                            { angletemp = (i + j) % 2 == 0 ? rotationAngle -180 : rotationAngle-90; }
                            if (myfillOritionType == fillOritionType.patternB)
                            { angletemp = (i + j) % 2 == 0 ? rotationAngle + 90 : rotationAngle; }
                            Path fillpath = InfillPath.generateLineInfill(mtemp_meshOutRec[i][j], mlineSpacingScale, angletemp, ref tempsecondinfillpath);
                            m_tempPaths.Add(fillpath);
                            if (tempsecondinfillpath.Count > 0) { mtemp_secondInfillPaths.Add(tempsecondinfillpath); }   //填充时的多条路径
                        }
                        else { m_tempPaths.Add(nopath); }   //路径为空需要占位   //行路劲集合
                    }

                    mtemp_outInfillPathsMatrix.Add(m_tempPaths);
                }

                foreach (Paths outpath in mtemp_outInfillPathsMatrix)
                {
                    DrawBitmapLines(outpath, Color.Red,            //网格填充路径 一行行的画
                    Color.FromArgb(0x60, 0, 0, 0xFF), viewScale);
                    mtemp_infillpathSet.AddRange(outpath);                     //存储为未合并路径
                }

                //处理网格求交时多余的区域
                foreach (maddtionpologonIndex tempgindex in mtemp_outAddtionRectPologons)
                {
                    Path tempsecondinfillpath = new Path();
                    angletemp = (tempgindex.Rownum + tempgindex.Colnumnum) % 2 == 0 ? rotationAngle + 90 : rotationAngle;
                    Path fillpath = InfillPath.generateLineInfill(tempgindex.pg, mlineSpacingScale, angletemp, ref tempsecondinfillpath);
                    mtemp_secondInfillPaths.Add(fillpath);
                    if (tempsecondinfillpath.Count > 0) { mtemp_secondInfillPaths.Add(tempsecondinfillpath); }
                }

                mtemp_infillpathSet.AddRange(mtemp_secondInfillPaths);                  //存储为未合并路径
                mtemp_finalMergerPaths = PathsMerge.PathsMatrixMergeByRandom(mtemp_outInfillPathsMatrix, 2.0f * mlineSpacingScale, mtemp_secondInfillPaths);
                Paths tempdrPaths = new Paths();
                tempdrPaths.Add(mtemp_finalMergerPaths[0]);

                // IntPoint startPoint = new IntPoint(0, 0);               //起始点
                IntPoint startPoint = new IntPoint(AABBoriginal[1].X * scale, AABBoriginal[1].Y * scale);
                PathOrderOptimizer mOrderPathOptimizer = new PathOrderOptimizer(startPoint, mtemp_finalMergerPaths);
                mtemp_orderedInfillpathSet = mOrderPathOptimizer.GetOrderOptimizePaths();
                float nonPathdiscost = mOrderPathOptimizer.pathDiscost / scale;

                Tour tour = mOrderPathOptimizer.mtour;

                Population pop = new Population(50, true, mtemp_finalMergerPaths, startPoint, tour);

                for (int i = 0; i < 400; i++)
                {
                    pop = GA.evolvePopulation(pop);
                }

                Tour tour1 = pop.getFittest();
                //float nonPathdiscost1 = pop.getFittest().getDistance(mtemp_finalMergerPaths, startPoint) / scale;
                //mtemp_orderedInfillpathSet = pop.getFittest().orderPath;

                float nonPathdiscost1 = tour1.distance / scale;
                mtemp_orderedInfillpathSet = new Paths(tour1.orderPath);

                Path startpath = new Path();
                startpath.Add(startPoint);
                mtemp_orderedInfillpathSet.Insert(0, startpath);    //插入起始点，作为一条独立的路径

                DrawBitmap(mtemp_orderedInfillpathSet, Color.Gray,       //***注意drawBitmap画的是闭合曲线
                Color.FromArgb(0x60, 0, 0, 0xFF), 1.0f, isDrawLineState);

                imgInfo m_imginfo = new imgInfo();
                m_imginfo.mspacing = TextBox_lineSpace.Text;
                m_imginfo.msidelength = meshlengthRowColNum.Sidelength;
                m_imginfo.mcolnum = meshlengthRowColNum.Colnumnum;
                m_imginfo.mrownum = meshlengthRowColNum.Rownum;
                m_imginfo.mtoolparhNum = mtemp_orderedInfillpathSet.Count - 1;
                m_imginfo.mnonTravellen = nonPathdiscost1;
                m_imginfo.orderPaths = new Paths(mtemp_orderedInfillpathSet);
                m_imginfo.meshOutRec = mtemp_meshOutRec;
                m_imginfo.infillPathset = new Paths(mtemp_infillpathSet);
                m_imginfo.mergerPaths = new Paths(mtemp_finalMergerPaths);
                m_imginfo.originalPaths = new Paths();
                m_imginfo.AABBoriginal = AABBoriginal;
                imgInfoSaves.Add(m_imginfo);         //存储分区路径结果
                //setFrmShowImg(m_imginfo);
                frmShowImg showfrm = new frmShowImg(m_imginfo);
                showfrm.Text = "frmShowImg" + "划分" + inde;
                showfrm.Show();
            }

            calcuateTheChoisedPath();
            if (outimgInfo != null)      //如果outimgInfo已经更新了
            {
                frmShowImg showfrm1 = new frmShowImg(outimgInfo);
                showfrm1.Text = "frmShowImg" + "选择" + mychoiseType;
                showfrm1.TopLevel = false;                        //顶级控件不能加载到panel
                this.showImgPanel.Controls.Clear();              //清空内部控件
                showfrm1.Dock = DockStyle.Fill;
                this.showImgPanel.Controls.Add(showfrm1);
                showfrm1.Show();

                //显示全局数据刷新
                orderedInfillpathSet = outimgInfo.orderPaths;
                

            }
        }


        /// <summary>
        /// 调用第二个窗口
        /// </summary>
        /// <param name="info"></param>
        private void setFrmShowImg(imgInfo info)
        {
            frmShowImg showfrm = new frmShowImg(info);
            //showfrm.Owner = this;                 //独立的窗口
            showfrm.Show();
        }


        //选择是哪种选择方案
        #region
        /// <summary>
        /// 选择是哪种选择方案的触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectedpath_CheckedChanged(object sender, EventArgs e)
        {
            if (firstside_RBK.Checked) { mychoiseType = choiseType.firstside; }
            if (maxside_RBK.Checked ) { mychoiseType = choiseType.maxiside ; }
            if (minside_RBK.Checked) { mychoiseType = choiseType.minside; }
            if (minpathnum_RBK.Checked) { mychoiseType = choiseType.minpathnum; }
            if (minnonpathlen_RBK.Checked  ) { mychoiseType = choiseType.minnonpathlen; }

            /* //06-16  适应全局方法
            calcuateTheChoisedPath();

            if (outimgInfo!=null)      //如果outimgInfo已经更新了
            {
                frmShowImg showfrm = new frmShowImg(outimgInfo);
                showfrm.Text = "frmShowImg" + "选择" + mychoiseType;
                showfrm.TopLevel = false;                        //顶级控件不能加载到panel
                this.showImgPanel.Controls.Clear();              //清空内部控件
                showfrm.Dock = DockStyle.Fill;
                this.showImgPanel.Controls.Add(showfrm);
                showfrm.Show();
            }
            */
        }

        
        /// <summary>
        ///当有多个路径时，确定选择的路径,更新全局变量outimgInfo
        /// </summary>
        private void calcuateTheChoisedPath()
        {
            if (imgInfoSaves.Count>0)
            {
                int count = imgInfoSaves.Count ;
                if (count == 1)
                {
                    outimgInfo = imgInfoSaves[0];
                }
                else
                {
                    if (mychoiseType == choiseType.firstside)      //返回第一个
                    {
                        outimgInfo = imgInfoSaves[0];
                    }
                    if (mychoiseType == choiseType.minside)       //返回最小边长
                    {
                        float[] msidelen = new float[count];
                        for (int i = 0; i < count; ++i)
                        { msidelen[i] = imgInfoSaves[i].msidelength; }
                        int index = sortReIndex(msidelen, count, true);
                        outimgInfo = imgInfoSaves[index];
                    }
                    if (mychoiseType == choiseType.maxiside)      //返回最大边长
                    {
                        float[] msidelen = new float[count];
                        for (int i = 0; i < count; ++i)
                        { msidelen[i] = imgInfoSaves[i].msidelength; }
                        int index = sortReIndex(msidelen, count, false);
                        outimgInfo = imgInfoSaves[index];
                    }
                    if (mychoiseType == choiseType.minpathnum)      //返回最小数目
                    {
                        float[] msidelen = new float[count];
                        for (int i = 0; i < count; ++i)
                        { msidelen[i] = imgInfoSaves[i].mtoolparhNum; }
                        int index = sortReIndex(msidelen, count, true);
                        outimgInfo = imgInfoSaves[index];
                    }
                    if (mychoiseType == choiseType.minnonpathlen)     //返回最小空行程
                    {
                        float[] msidelen = new float[count];
                        for (int i = 0; i < count; ++i)
                        { msidelen[i] = imgInfoSaves[i].mnonTravellen; }
                        int index = sortReIndex(msidelen, count, true);
                        outimgInfo = imgInfoSaves[index];
                    }
                }
            }
        }


        /// <summary>
        ///寻找数组的最小值和最大值，返回索引坐标
        /// </summary>
        /// <param name="sidelen"></param>
        /// <param name="count">数组数目</param>
        /// <param name="minOrMax">小于时是true,从大到小排列是false</param>
        /// <returns></returns>
        private int sortReIndex(float[] sidelen, int count,bool minOrMax)
        {
           
            int index = 0;
            if (count>0)
            {
                if (minOrMax)
                {
                    float min = sidelen[0];

                    for (int i = 1; i < count; ++i)
                    {
                        if (sidelen[i] < min)
                        {
                            min = sidelen[i];
                            index = i;
                        }
                    }
                }
                else
                {
                    float max = sidelen[0];

                    for (int i = 1; i < count; ++i)
                    {
                        if (sidelen[i] > max)
                        {
                            max = sidelen[i];
                            index = i;
                        }
                    }
                }
            }
            return index;     
        }
        #endregion

        //从外部导入数据
        #region
        /// <summary>
        /// 导入excel路径的单选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        List<String> pologonPathExcelInput = new List<string>();                //记录excel路劲
        List<List<Vector2>> inputPologonVectors = new List<List<Vector2>>();       //记录输入的多边形轮廓

        private void importexcel_BK_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "Excel文件|*.xlsx";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = this.openFileDialog1.FileName;
                // 你的 处理文件路径代码 
                if (outerPologonexce_Te.Text == "") { outerPologonexce_Te.Text = FileName; pologonPathExcelInput.Add(FileName); handleTheInputExcel(); }
                else if (innerPologonexce0_Te.Text == "") { innerPologonexce0_Te.Text = FileName; pologonPathExcelInput.Add(FileName); handleTheInputExcel(); }
                else if (innerPologonexce1_Te.Text == "") { innerPologonexce1_Te.Text = FileName; pologonPathExcelInput.Add(FileName); handleTheInputExcel(); }
                else if (innerPologonexce1_Te.Text == "") { innerPologonexce1_Te.Text = FileName; pologonPathExcelInput.Add(FileName); handleTheInputExcel(); }
                else if (innerPologonexce2_Te.Text == "") { innerPologonexce2_Te.Text = FileName; pologonPathExcelInput.Add(FileName); handleTheInputExcel(); }
                else if (innerPologonexce3_Te.Text == "") { innerPologonexce3_Te.Text = FileName; pologonPathExcelInput.Add(FileName); handleTheInputExcel(); }

                //handleTheInputExcel();      //插入最新数据
                drawOriginalPologon(inputPologonVectors);
            }
        }


        /// <summary>
        /// 清空所有路径输入文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearAllpologonInput_BK_Click(object sender, EventArgs e)
        {
           
            outerPologonexce_Te.Text = "";
            innerPologonexce0_Te.Text = "";
            innerPologonexce1_Te.Text = "";
            innerPologonexce2_Te.Text = "";
            innerPologonexce3_Te.Text = "";

            pologonPathExcelInput.Clear();         //清空输入的路劲
            inputPologonVectors.Clear();           //原始路径
            pologonShow_Te.Clear();


           // handleTheInputExcel();               //更新数据插入最新数据
            drawOriginalPologon(inputPologonVectors);  //画多边形
        }

     
        /// <summary>
        /// 更新excel输入的路径，多边形list，插入最新的数据
        /// </summary>
        private void handleTheInputExcel()
        {
            //inputPologonVectors.Clear();          //先清空
            //foreach (String pologonPathExcel in pologonPathExcelInput)
            if (pologonPathExcelInput.Count>0)
            {
                String pologonPathExcel = pologonPathExcelInput[pologonPathExcelInput.Count - 1];
                List<Vector2> worldColPoints = new List<Vector2>();
                DataSet ds = LoadDataFromExcel(pologonPathExcel);
                DataTable dt = new DataTable();
                dt = ds.Tables[0];
                PointF[] col = new PointF[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    float a1 = Convert.ToSingle(dt.Rows[i][0]);
                    float a2 = Convert.ToSingle(dt.Rows[i][1]);
                    PointF polygonarrytemp = new PointF(a1, a2);
                    col[i] = polygonarrytemp;
                }

                //originalPologon = col; //用于保存原始数据
                //AABBoriginal = AABBf(col);

                //构造Vector2链表对象
                foreach (PointF core in col)
                {
                    Vector2 vcol = new Vector2(core.X, core.Y);
                    worldColPoints.Add(vcol);
                }
                inputPologonVectors.Add(worldColPoints);
            }
        }


        /// <summary>
        /// 画初始输入图形
        /// </summary>
        /// <param name="inputPologonVes"></param>
        private void drawOriginalPologon(List<List<Vector2>> inputPologonVes)
        {
  
                DrawBitmapVectorInput(inputPologonVes, Color.Blue,
                        Color.FromArgb(0x60, 0, 0, 0xFF));       
        }


        private void DrawBitmapVectorInput(List<List<Vector2>> subjects, Color stroke, Color brush)
        {
            if (mybitmap1 != null)               //删除bitmap对象
                mybitmap1.Dispose();
            mybitmap1 = new Bitmap(
                pictureBox1.ClientRectangle.Width,
                pictureBox1.ClientRectangle.Height,
                PixelFormat.Format32bppArgb);
 
            Graphics newgraphic = Graphics.FromImage(mybitmap1);
            newgraphic.SmoothingMode = SmoothingMode.AntiAlias;
            //newgraphic.Clear(Color.White); 
            path1.FillMode = FillMode.Winding;
            foreach (List<Vector2> pg in subjects)
            {
                //原始数据被放大了
                PointF[] pt = ConvexPolygonToPointFArray(pg, scalef);
                PointF[] pts = PointFArrayMove(pt,
                   new PointF( - (pictureBox1.Width / 2 + 20), - (pictureBox1.Height / 2 - 40)));
                path1.AddPolygon(pts);
                pts = null;
            }
            using (Pen myPen = new Pen(stroke, (float)0.5))
            using (SolidBrush myBrush = new SolidBrush(brush))
            {
                //newgraphic.FillPath(myBrush, path);
                newgraphic.DrawPath(myPen, path1);
                pictureBox2.Image = mybitmap1;
                path1.Reset();
            }
            //}
        }


        /// <summary>
        /// 手动输入多边形数据处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handInsertPolo_BK_Click(object sender, EventArgs e)
        {
            frmAddInputPologon frmAddInputPologon1 = new frmAddInputPologon();
            frmAddInputPologon1.Text = "frmShowImg" + "手动插入轮廓";
            if (frmAddInputPologon1.ShowDialog() == DialogResult.OK)
            {
                List<Vector2> temp = frmAddInputPologon1.insertPologonVec;
                bool outerState = frmAddInputPologon1.isOuterPologon;
                String showPoint = frmAddInputPologon1.showPointText;

                if(temp.Count>0)
                {
                    if (outerState)            //若果是外轮廓
                    {
                        clearAllpologonInput_BK_Click(sender, e);
                        inputPologonVectors.Clear();
                       
                        outerPologonexce_Te.Text = "已经手动输入";

                       // handleTheInputExcel();               //更新数据
                        inputPologonVectors.Add(temp);
                       
                        drawOriginalPologon(inputPologonVectors);  //画多边形

                        pologonShow_Te.AppendText("外轮廓"+"\n");
                        pologonShow_Te.AppendText(showPoint);

                    }
                    else
                    {                   
                       // handleTheInputExcel();               //更新数据

                        inputPologonVectors.Add(temp);
                        pologonShow_Te.AppendText("\r\n 内轮廓" + "\n");  //更新显示框
                        pologonShow_Te.AppendText(showPoint);
                        drawOriginalPologon(inputPologonVectors);  //画多边形
                    }
                }    
            }
            frmAddInputPologon1.Dispose();
        }


        #endregion



        //填充方法选择，方案参数设置
        #region

        /// <summary>
        /// 选择整体划分方法的GroupBox中的RadioButton点击事件，
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void solutionChoosed_rbkclick(object sender, EventArgs e)
        {
            if (convexSolution_RBK.Checked)        //选择凸分区方法
            {
                convexSolution_GB.Enabled = true;       //使能凸多边新设置GB
                if (smallDirec_RBK.Checked) { squareSolution_GB.Enabled = false; } else { squareSolution_GB.Enabled = true; }
                     //失能正方形划分设置GB
                //isConvexSolutionState = true;           //标记为凸分区，全局变量
                mypathGenerSoluType = pathGenerSoluType.convexSolution;  //标记为凸分区，全局变量
            }
            if(squareSolution_RBK.Checked)         //选择正方形划分方法
            {
                convexSolution_GB.Enabled = false;  //disabled the convex grid Grpupbox
                squareSolution_GB.Enabled = true;   //enable the squale grid Groupbox
                isConvexSolutionState = false;      //state set as square gride ，全局变量
                mypathGenerSoluType = pathGenerSoluType.squareSolution;     //state set as square gride ，全局变量
            } 
        }

        /// <summary>
        /// 凸分区分区方式选择，完全分区、近似凸分区1、近似凸分区2, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void convexGridType_rbkclick(object sender, EventArgs e)
        {
            if (fullConvex_RBK.Checked) { concaveDegree_Te.Enabled = false; myconvexCutType = convexCutType.fullConvex; }       //禁能凹度阈值输入框
            if (approximateConvex1_RBK.Checked) { concaveDegree_Te.Enabled = true; concaveDegree_Te.Text = "0.01"; myconvexCutType = convexCutType.approximateConvex1; }//使能并清空凹度阈值输入框
            if (approximateConvex2_RBK.Checked) { concaveDegree_Te.Enabled = true; concaveDegree_Te.Text = "0.2"; myconvexCutType = convexCutType.approximateConvex2; }
        }

        private void areaInfillType_rbkclick(object sender, EventArgs e)          //主要是适应动画需要
        {
            if (smallDirec_RBK.Checked) { fillDegree_Te.Enabled = false; fullDegree_Te.Enabled = false; squareSolution_GB.Enabled = false; mysubsecInfillType = subsecInfillType.smallDirec; }
            if (squaregride_RBK.Checked) { fillDegree_Te.Enabled = false; fullDegree_Te.Enabled = false; squareSolution_GB.Enabled = true; mysubsecInfillType = subsecInfillType.squareGride; }    //enable the squale grid Groupbox
            if (mixDirecSqure_RBK.Checked) { fillDegree_Te.Enabled = true; fullDegree_Te.Enabled = true; squareSolution_GB.Enabled = true; mysubsecInfillType = subsecInfillType.mixDirecSquare; };
        }

        private void infillOritionType_rbkClick(object sender, EventArgs e)
        {
            if (patternA_RBK.Checked) { myfillOritionType = fillOritionType.patternA; }  //填充A
            if (patternB_RBK.Checked) { myfillOritionType = fillOritionType.patternB; }  //填充B
        }

        private void updateParam_BK_Click(object sender, EventArgs e)
        {
            if (concaveDegree_Te.Text != "") { concaveThreshold = float.Parse(concaveDegree_Te.Text); }   //更新凹度阈值
            if (fillDegree_Te.Text != "") { fillDegreeThread = float.Parse(fillDegree_Te.Text); }   //更新充盈度阈值
            if (fullDegree_Te.Text != "") { fullDegreeThread = float.Parse(fullDegree_Te.Text); }   //更新饱满度阈值
        }

        #endregion

        private void execute_BK_Click(object sender, EventArgs e)
        {
            if (mypathGenerSoluType == pathGenerSoluType.convexSolution)
            {
                bk_Generate_Click( sender,  e);
                bk_Infill_Click( sender,  e);
                Merge_bk_Click( sender,  e);
                Bk_orderPath_Click( sender,  e);
                BK_generGcode_Click( sender,  e); 
            }

            if (mypathGenerSoluType == pathGenerSoluType.squareSolution)
            {
                Mesh_bk_Click(sender, e);
                BK_generGcode_Click(sender, e); 
            }

        }

     


    }


    /// <summary>
    /// 记录正方形格子路径信息类
    /// </summary>
    public class imgInfo
    {
        public string mspacing;             // 行距
        public float msidelength;            //格子边长
        public int mcolnum;                  //列数
        public int mrownum;                  //行数
        public int mtoolparhNum;             //路径条数
        public float mnonTravellen;          //空行程长度
        public float mtoolPathlen;           //路径长度
        public Paths originalPaths;          //初始多边形
        public List<Paths> meshOutRec;       //格子多边形
        public Paths infillPathset;          //未合并填充曲线
        public Paths mergerPaths;           //合并后的路径
        public Paths orderPaths;            //顺序后最终路径
        public List<PointF> AABBoriginal;   //最小包围盒
    }
}