using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClipperLib;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public partial class frmShowImg : Form
    {
        private imgInfo m_imgInfo;        //参数信息
        private Bitmap mybitmap;         //全局bitmap对象
        private float scale = 1000;       //求偏置时取整放大倍数,调整精度
        private float viewScale = 3.0f;
        private GraphicsPath path = new GraphicsPath();  //画布中保存的图形对象

        //初始显示的曲线
        private bool showOrderPathState = true;
        private bool showMergePathState = false;
        private bool showOrignalPathState = false;
        private bool showMeshGrideState = false;
        private bool showInfillPathstate = false;

        //给DrawBitmap确定是line还是多边形
        private static bool isDrawPologonState = true;
        private static bool isDrawLineState = false;

        //鼠标拖动事件确定初始距离
        Point start;
        Point endPoint;
        Point locat;
        Point detaPoint;
        Point offsetvalue = new Point(0, 0);
        bool movestate = false;

        public frmShowImg()
        {
            InitializeComponent();
        }

        public frmShowImg(imgInfo info)
        {
            this.m_imgInfo = info;
            InitializeComponent();
            mybitmap = new Bitmap(
            pictureBox1.ClientRectangle.Width,
            pictureBox1.ClientRectangle.Height,
            PixelFormat.Format32bppArgb);

            this.MouseWheel += new MouseEventHandler(frmShowImg_MouseWheel);
            setShowPara();
            ZoomAndRefreshBitmap();
        }

        
        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text =
            "Tip: Use the mouse-wheel (or +,-,0) to adjust the viewtimes of the polygons.";
        }

        private void showSeleted_Click(object sender, EventArgs e)
        {
             showOrderPathState = finalOrderPath_RBK.Checked;        //画最终曲线
             showMergePathState = mergePath_RBK.Checked;             //画合并后的曲线
             showOrignalPathState = pologon_RBK.Checked;             //画原始多边形
             showInfillPathstate = gridInfill_RBK.Checked;            //画未合并填充曲线
             showMeshGrideState = meshGrid_RBK.Checked;               //画格子
             ZoomAndRefreshBitmap();
        }


        //鼠标滚轮滚动事件，放大和缩小，e.Delta>0，向上滚动
        private void frmShowImg_MouseWheel(object sender, MouseEventArgs e)
        {
            bool changeZoomState = false;       //确定是放大还是缩小状态
            if (e.Delta > 0 && viewScale < 15.0)
            {
                changeZoomState = true;
            }
            else if (e.Delta < 0 && viewScale > 0.3)
            {
                changeZoomState = false;            
            }
            viewScale = (float)(changeZoomState ? 1.1 : 0.9) * viewScale;  //更新放大倍数
            ZoomAndRefreshBitmap();
        }

        //显示事件调用画布刷新事件
        void ZoomAndRefreshBitmap()
        {
            if (mybitmap != null)               //删除bitmap对象
                mybitmap.Dispose();
            mybitmap = new Bitmap(
                pictureBox1.ClientRectangle.Width,
                pictureBox1.ClientRectangle.Height,
                PixelFormat.Format32bppArgb);

            pictureBox1.Image = null;

            if (showOrderPathState)
            {
                DrawBitmap(m_imgInfo.orderPaths, Color.Red,            //最终路径
                     Color.FromArgb(0x60, 0, 0, 0xFF), viewScale, isDrawLineState);
            }
            if (showMergePathState)
            {
                DrawBitmapLines(m_imgInfo.mergerPaths, Color.Green,            //合并路径
                     Color.FromArgb(0x60, 0, 0, 0xFF), viewScale);
            }

            //if (showOrignalPathState)
            //{
            //    DrawBitmapLines(m_imgInfo.orderPaths, Color.Green,            //原始路径
            //         Color.FromArgb(0x60, 0, 0, 0xFF), viewScale);
            //}

            if (showInfillPathstate)
            {
                DrawBitmapLines(m_imgInfo.infillPathset, Color.Blue,            //画未合并路劲
                     Color.FromArgb(0x60, 0, 0, 0xFF), viewScale);
            }

            if (showMeshGrideState)
            {
                foreach (Paths meshRect in m_imgInfo.meshOutRec)
                {
                    DrawBitmap(meshRect, Color.Pink,                          //网格框路径 一行行的画 
                    Color.FromArgb(0x60, 0, 0, 0xFF), viewScale, isDrawPologonState);
                }
            }
        }




        //设置参数显示信息
        private void setShowPara()
        {
            spaceing_Te.Text = m_imgInfo.mspacing;
            sidelen_Te.Text = m_imgInfo.msidelength.ToString();
            rowInfo_Te.Text = m_imgInfo.mrownum.ToString();
            columnInfo_Te.Text = m_imgInfo.mcolnum.ToString();
            toolpathNum_Te.Text = m_imgInfo.mtoolparhNum.ToString();
            nonTravellen_Te.Text = m_imgInfo.mnonTravellen.ToString();
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


        static private PointF[] PointFArrayMove(PointF[] pf, PointF moveBasePoint)
        {
            PointF[] result = new PointF[pf.Length];
            for (int i = 0; i < pf.Length; ++i)
            {
                result[i].X = (float)pf[i].X - moveBasePoint.X;
                result[i].Y = (float)pf[i].Y - moveBasePoint.Y;
            }
            return result;
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
            if (subjects ==null) { return; }
            foreach (Path pg in subjects)
            {
                if (pg.Count == 0) { continue; }   //如果pg是空，2016_04_22为了格子画时可能有空的占位
                //PointF[] pts = PolygonToPointFArray(pg, scale/float.Parse(zoom_Times.Text));
                PointF[] pt = PolygonToPointFArray(pg, scale / viewScale);
                //平移到中间
                PointF[] pts = PointFArrayMove(pt,
                    new PointF(m_imgInfo.AABBoriginal[1].X * viewScale - (pictureBox1.Width / 2 + 20) - offsetvalue.X, m_imgInfo.AABBoriginal[1].Y * viewScale - (pictureBox1.Height / 2-40)-offsetvalue.Y));
                if (lineOrPologonstate) { path.AddPolygon(pts); }
                else { path.AddLines(pts); }
                pts = null;
            }
            using (Pen myPen = new Pen(stroke, (float)1.0))
            using (SolidBrush myBrush = new SolidBrush(brush))
            {
                //  newgraphic.FillPath(myBrush, path);
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
                    new PointF(m_imgInfo.AABBoriginal[1].X * viewScale - (pictureBox1.Width / 2 + 20)- offsetvalue.X, m_imgInfo.AABBoriginal[1].Y * viewScale - (pictureBox1.Height / 2 - 40)-offsetvalue.Y));

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

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
             start = Control.MousePosition;
             locat = pictureBox1.Location;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                endPoint = Control.MousePosition;
                movestate = true;
               // pictureBox1.Location = new Point(locat.X + temp.X - start.Y, locat.X + temp.Y - start.Y);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
           // Point endpoint = Control.MousePosition;
            if (movestate)
            {
                detaPoint = new Point(endPoint.X - start.X, endPoint.Y - start.Y);
                offsetvalue.X = offsetvalue.X + detaPoint.X;
                offsetvalue.Y = offsetvalue.Y + detaPoint.Y;
                ZoomAndRefreshBitmap();
                endPoint = new Point();
                start = new Point();
                movestate = false;
            }
        }    
    }
}
