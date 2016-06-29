using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClipperLib;

namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public partial class frmAddInputPologon : Form
    {

        public List<Vector2> insertPologonVec = new List<Vector2> ();
        public bool isOuterPologon = true;      //确定是内轮廓还是外轮廓
        public String showPointText; //存储点串
        public frmAddInputPologon()
        {
            InitializeComponent();
        }

        private void insertPoint_BK_Click(object sender, EventArgs e)
        {
            String pointStr = pointInput_Te.Text;
            if (pointStr != "")
            {
                //try
                //{
                    String[] pointStrArr = pointStr.Split(',');
                    Vector2 temppointF = new Vector2();
                    temppointF.x = (float.Parse(pointStrArr[0]));
                    temppointF.y = (float.Parse(pointStrArr[1]));
                    insertPologonVec.Add(temppointF);
                    showPoint_Te.AppendText(pointStr+"\r\n");  //插入显示框
                    pointInput_Te.Text = "";          //清空输入框
                //}
                //catch (Exception ee) { return; }
            }
        }

        private void insertPologon_BK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            showPointText = showPoint_Te.Text;
            showPoint_Te.Clear();                        //清空数据框关闭对话框
            if (outerType_RBK.Checked) { isOuterPologon = true; }
            if (innerType_RBK.Checked ) { isOuterPologon = false; }
           
            this.Close();
        }




    }
}
