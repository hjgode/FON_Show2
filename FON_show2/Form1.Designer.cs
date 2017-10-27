namespace FON_show2
{
    partial class Form1
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.txtHex = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.hScrollBar2 = new System.Windows.Forms.HScrollBar();
            this.bt_splitROM = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.AcceptsTab = true;
            this.textBox1.Location = new System.Drawing.Point(370, 32);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(245, 232);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Courier New", 6F);
            this.textBox2.Location = new System.Drawing.Point(642, 32);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(244, 361);
            this.textBox2.TabIndex = 1;
            this.textBox2.WordWrap = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(147, 78);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(160, 232);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(147, 50);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(160, 25);
            this.hScrollBar1.TabIndex = 4;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(147, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 21);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "label2";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 32);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 25);
            this.button1.TabIndex = 7;
            this.button1.Text = "Select file";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(11, 78);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(130, 160);
            this.listBox1.TabIndex = 8;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            // 
            // txtHex
            // 
            this.txtHex.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHex.Location = new System.Drawing.Point(11, 399);
            this.txtHex.Multiline = true;
            this.txtHex.Name = "txtHex";
            this.txtHex.ReadOnly = true;
            this.txtHex.Size = new System.Drawing.Size(875, 112);
            this.txtHex.TabIndex = 9;
            this.txtHex.WordWrap = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(11, 281);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(130, 102);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            // 
            // hScrollBar2
            // 
            this.hScrollBar2.Location = new System.Drawing.Point(10, 256);
            this.hScrollBar2.Maximum = 20;
            this.hScrollBar2.Minimum = 1;
            this.hScrollBar2.Name = "hScrollBar2";
            this.hScrollBar2.Size = new System.Drawing.Size(130, 22);
            this.hScrollBar2.TabIndex = 11;
            this.hScrollBar2.Value = 2;
            this.hScrollBar2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar2_Scroll);
            // 
            // bt_splitROM
            // 
            this.bt_splitROM.Location = new System.Drawing.Point(513, 333);
            this.bt_splitROM.Name = "bt_splitROM";
            this.bt_splitROM.Size = new System.Drawing.Size(102, 32);
            this.bt_splitROM.TabIndex = 12;
            this.bt_splitROM.Text = "split ROM";
            this.bt_splitROM.UseVisualStyleBackColor = true;
            this.bt_splitROM.Click += new System.EventHandler(this.bt_splitROM_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 523);
            this.Controls.Add(this.bt_splitROM);
            this.Controls.Add(this.hScrollBar2);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.txtHex);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox txtHex;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.HScrollBar hScrollBar2;
        private System.Windows.Forms.Button bt_splitROM;
    }
}

