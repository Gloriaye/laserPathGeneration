namespace wsconvexdecomposition
{
    partial class frmShowImg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShowImg));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.meshGrid_RBK = new System.Windows.Forms.CheckBox();
            this.gridInfill_RBK = new System.Windows.Forms.CheckBox();
            this.pologon_RBK = new System.Windows.Forms.CheckBox();
            this.mergePath_RBK = new System.Windows.Forms.CheckBox();
            this.finalOrderPath_RBK = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.toolpathNum_Te = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.nonTravellen_Te = new System.Windows.Forms.TextBox();
            this.toolpathlen_Te = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sidelen_Te = new System.Windows.Forms.TextBox();
            this.rowInfo_Te = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.columnInfo_Te = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.minRect_Te = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.maxSidelen_Te = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.minSidelen_Te = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.spaceing_Te = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(199, 448);
            this.panel1.TabIndex = 6;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.meshGrid_RBK);
            this.groupBox4.Controls.Add(this.gridInfill_RBK);
            this.groupBox4.Controls.Add(this.pologon_RBK);
            this.groupBox4.Controls.Add(this.mergePath_RBK);
            this.groupBox4.Controls.Add(this.finalOrderPath_RBK);
            this.groupBox4.Location = new System.Drawing.Point(12, 364);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(181, 80);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "曲线显示";
            // 
            // meshGrid_RBK
            // 
            this.meshGrid_RBK.AutoSize = true;
            this.meshGrid_RBK.Location = new System.Drawing.Point(11, 60);
            this.meshGrid_RBK.Name = "meshGrid_RBK";
            this.meshGrid_RBK.Size = new System.Drawing.Size(72, 16);
            this.meshGrid_RBK.TabIndex = 1;
            this.meshGrid_RBK.Text = "格子曲线";
            this.meshGrid_RBK.UseVisualStyleBackColor = true;
            this.meshGrid_RBK.CheckedChanged += new System.EventHandler(this.showSeleted_Click);
            // 
            // gridInfill_RBK
            // 
            this.gridInfill_RBK.AutoSize = true;
            this.gridInfill_RBK.Location = new System.Drawing.Point(11, 40);
            this.gridInfill_RBK.Name = "gridInfill_RBK";
            this.gridInfill_RBK.Size = new System.Drawing.Size(96, 16);
            this.gridInfill_RBK.TabIndex = 1;
            this.gridInfill_RBK.Text = "格子填充曲线";
            this.gridInfill_RBK.UseVisualStyleBackColor = true;
            this.gridInfill_RBK.CheckedChanged += new System.EventHandler(this.showSeleted_Click);
            // 
            // pologon_RBK
            // 
            this.pologon_RBK.AutoSize = true;
            this.pologon_RBK.Location = new System.Drawing.Point(99, 60);
            this.pologon_RBK.Name = "pologon_RBK";
            this.pologon_RBK.Size = new System.Drawing.Size(72, 16);
            this.pologon_RBK.TabIndex = 1;
            this.pologon_RBK.Text = "原始曲线";
            this.pologon_RBK.UseVisualStyleBackColor = true;
            this.pologon_RBK.CheckedChanged += new System.EventHandler(this.showSeleted_Click);
            // 
            // mergePath_RBK
            // 
            this.mergePath_RBK.AutoSize = true;
            this.mergePath_RBK.Location = new System.Drawing.Point(99, 20);
            this.mergePath_RBK.Name = "mergePath_RBK";
            this.mergePath_RBK.Size = new System.Drawing.Size(72, 16);
            this.mergePath_RBK.TabIndex = 1;
            this.mergePath_RBK.Text = "合并曲线";
            this.mergePath_RBK.UseVisualStyleBackColor = true;
            this.mergePath_RBK.CheckedChanged += new System.EventHandler(this.showSeleted_Click);
            // 
            // finalOrderPath_RBK
            // 
            this.finalOrderPath_RBK.AutoSize = true;
            this.finalOrderPath_RBK.Checked = true;
            this.finalOrderPath_RBK.CheckState = System.Windows.Forms.CheckState.Checked;
            this.finalOrderPath_RBK.Location = new System.Drawing.Point(11, 20);
            this.finalOrderPath_RBK.Name = "finalOrderPath_RBK";
            this.finalOrderPath_RBK.Size = new System.Drawing.Size(72, 16);
            this.finalOrderPath_RBK.TabIndex = 1;
            this.finalOrderPath_RBK.Text = "最终曲线";
            this.finalOrderPath_RBK.UseVisualStyleBackColor = true;
            this.finalOrderPath_RBK.CheckedChanged += new System.EventHandler(this.showSeleted_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.toolpathNum_Te);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.nonTravellen_Te);
            this.groupBox3.Controls.Add(this.toolpathlen_Te);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 258);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(181, 96);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "输出结果";
            // 
            // toolpathNum_Te
            // 
            this.toolpathNum_Te.Enabled = false;
            this.toolpathNum_Te.Location = new System.Drawing.Point(79, 18);
            this.toolpathNum_Te.Name = "toolpathNum_Te";
            this.toolpathNum_Te.Size = new System.Drawing.Size(92, 21);
            this.toolpathNum_Te.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "路径数目";
            // 
            // nonTravellen_Te
            // 
            this.nonTravellen_Te.Enabled = false;
            this.nonTravellen_Te.Location = new System.Drawing.Point(79, 43);
            this.nonTravellen_Te.Name = "nonTravellen_Te";
            this.nonTravellen_Te.Size = new System.Drawing.Size(92, 21);
            this.nonTravellen_Te.TabIndex = 1;
            // 
            // toolpathlen_Te
            // 
            this.toolpathlen_Te.Enabled = false;
            this.toolpathlen_Te.Location = new System.Drawing.Point(79, 70);
            this.toolpathlen_Te.Name = "toolpathlen_Te";
            this.toolpathlen_Te.Size = new System.Drawing.Size(92, 21);
            this.toolpathlen_Te.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "总路径长度";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "空行程";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.sidelen_Te);
            this.groupBox2.Controls.Add(this.rowInfo_Te);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.columnInfo_Te);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 146);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(181, 103);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "格子参数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "行数（row）";
            // 
            // sidelen_Te
            // 
            this.sidelen_Te.Enabled = false;
            this.sidelen_Te.Location = new System.Drawing.Point(94, 20);
            this.sidelen_Te.Name = "sidelen_Te";
            this.sidelen_Te.Size = new System.Drawing.Size(77, 21);
            this.sidelen_Te.TabIndex = 1;
            // 
            // rowInfo_Te
            // 
            this.rowInfo_Te.Enabled = false;
            this.rowInfo_Te.Location = new System.Drawing.Point(94, 47);
            this.rowInfo_Te.Name = "rowInfo_Te";
            this.rowInfo_Te.Size = new System.Drawing.Size(77, 21);
            this.rowInfo_Te.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "格子边长";
            // 
            // columnInfo_Te
            // 
            this.columnInfo_Te.Enabled = false;
            this.columnInfo_Te.Location = new System.Drawing.Point(94, 74);
            this.columnInfo_Te.Name = "columnInfo_Te";
            this.columnInfo_Te.Size = new System.Drawing.Size(77, 21);
            this.columnInfo_Te.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "列数（column）";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.minRect_Te);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.maxSidelen_Te);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.minSidelen_Te);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.spaceing_Te);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 124);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设置项";
            // 
            // minRect_Te
            // 
            this.minRect_Te.Enabled = false;
            this.minRect_Te.Location = new System.Drawing.Point(80, 17);
            this.minRect_Te.Name = "minRect_Te";
            this.minRect_Te.Size = new System.Drawing.Size(91, 21);
            this.minRect_Te.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "最小包围盒";
            // 
            // maxSidelen_Te
            // 
            this.maxSidelen_Te.Enabled = false;
            this.maxSidelen_Te.Location = new System.Drawing.Point(80, 43);
            this.maxSidelen_Te.Name = "maxSidelen_Te";
            this.maxSidelen_Te.Size = new System.Drawing.Size(91, 21);
            this.maxSidelen_Te.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 47);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "最大边长";
            // 
            // minSidelen_Te
            // 
            this.minSidelen_Te.Enabled = false;
            this.minSidelen_Te.Location = new System.Drawing.Point(80, 69);
            this.minSidelen_Te.Name = "minSidelen_Te";
            this.minSidelen_Te.Size = new System.Drawing.Size(91, 21);
            this.minSidelen_Te.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 73);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "最小边长";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 99);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 0;
            this.label10.Text = "扫描间距";
            // 
            // spaceing_Te
            // 
            this.spaceing_Te.Enabled = false;
            this.spaceing_Te.Location = new System.Drawing.Point(80, 95);
            this.spaceing_Te.Name = "spaceing_Te";
            this.spaceing_Te.Size = new System.Drawing.Size(91, 21);
            this.spaceing_Te.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Location = new System.Drawing.Point(199, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(513, 448);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(199, 426);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(513, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // frmShowImg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 448);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmShowImg";
            this.Text = "frmShowImg";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            this.panel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox sidelen_Te;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox toolpathNum_Te;
        private System.Windows.Forms.TextBox nonTravellen_Te;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox rowInfo_Te;
        private System.Windows.Forms.TextBox columnInfo_Te;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox minRect_Te;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox maxSidelen_Te;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox minSidelen_Te;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox spaceing_Te;
        private System.Windows.Forms.TextBox toolpathlen_Te;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox pologon_RBK;
        private System.Windows.Forms.CheckBox mergePath_RBK;
        private System.Windows.Forms.CheckBox finalOrderPath_RBK;
        private System.Windows.Forms.CheckBox meshGrid_RBK;
        private System.Windows.Forms.CheckBox gridInfill_RBK;
    }
}