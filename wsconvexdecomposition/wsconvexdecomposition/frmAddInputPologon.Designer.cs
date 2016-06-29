namespace wsconvexdecomposition
{
    partial class frmAddInputPologon
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.showPoint_Te = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pointInput_Te = new System.Windows.Forms.TextBox();
            this.insertPoint_BK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.insertPologon_BK = new System.Windows.Forms.Button();
            this.innerType_RBK = new System.Windows.Forms.RadioButton();
            this.outerType_RBK = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.showPoint_Te);
            this.groupBox1.Location = new System.Drawing.Point(21, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(124, 245);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "轮廓点";
            // 
            // showPoint_Te
            // 
            this.showPoint_Te.Location = new System.Drawing.Point(6, 20);
            this.showPoint_Te.Multiline = true;
            this.showPoint_Te.Name = "showPoint_Te";
            this.showPoint_Te.Size = new System.Drawing.Size(112, 209);
            this.showPoint_Te.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "坐标(x,y)";
            // 
            // pointInput_Te
            // 
            this.pointInput_Te.Location = new System.Drawing.Point(79, 24);
            this.pointInput_Te.Name = "pointInput_Te";
            this.pointInput_Te.Size = new System.Drawing.Size(96, 21);
            this.pointInput_Te.TabIndex = 2;
            // 
            // insertPoint_BK
            // 
            this.insertPoint_BK.Location = new System.Drawing.Point(48, 61);
            this.insertPoint_BK.Name = "insertPoint_BK";
            this.insertPoint_BK.Size = new System.Drawing.Size(75, 23);
            this.insertPoint_BK.TabIndex = 3;
            this.insertPoint_BK.Text = "插入点";
            this.insertPoint_BK.UseVisualStyleBackColor = true;
            this.insertPoint_BK.Click += new System.EventHandler(this.insertPoint_BK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pointInput_Te);
            this.groupBox2.Controls.Add(this.insertPoint_BK);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(151, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 100);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.insertPologon_BK);
            this.groupBox3.Controls.Add(this.innerType_RBK);
            this.groupBox3.Controls.Add(this.outerType_RBK);
            this.groupBox3.Location = new System.Drawing.Point(152, 139);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(191, 117);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "轮廓类型";
            // 
            // insertPologon_BK
            // 
            this.insertPologon_BK.Location = new System.Drawing.Point(27, 79);
            this.insertPologon_BK.Name = "insertPologon_BK";
            this.insertPologon_BK.Size = new System.Drawing.Size(75, 23);
            this.insertPologon_BK.TabIndex = 1;
            this.insertPologon_BK.Text = "确定";
            this.insertPologon_BK.UseVisualStyleBackColor = true;
            this.insertPologon_BK.Click += new System.EventHandler(this.insertPologon_BK_Click);
            // 
            // innerType_RBK
            // 
            this.innerType_RBK.AutoSize = true;
            this.innerType_RBK.Location = new System.Drawing.Point(27, 43);
            this.innerType_RBK.Name = "innerType_RBK";
            this.innerType_RBK.Size = new System.Drawing.Size(59, 16);
            this.innerType_RBK.TabIndex = 0;
            this.innerType_RBK.Text = "内轮廓";
            this.innerType_RBK.UseVisualStyleBackColor = true;
            // 
            // outerType_RBK
            // 
            this.outerType_RBK.AutoSize = true;
            this.outerType_RBK.Checked = true;
            this.outerType_RBK.Location = new System.Drawing.Point(27, 21);
            this.outerType_RBK.Name = "outerType_RBK";
            this.outerType_RBK.Size = new System.Drawing.Size(59, 16);
            this.outerType_RBK.TabIndex = 0;
            this.outerType_RBK.TabStop = true;
            this.outerType_RBK.Text = "外轮廓";
            this.outerType_RBK.UseVisualStyleBackColor = true;
            // 
            // frmAddInputPologon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 274);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmAddInputPologon";
            this.Text = "frmAddInputPologon";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox showPoint_Te;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pointInput_Te;
        private System.Windows.Forms.Button insertPoint_BK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button insertPologon_BK;
        private System.Windows.Forms.RadioButton innerType_RBK;
        private System.Windows.Forms.RadioButton outerType_RBK;
    }
}