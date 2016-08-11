namespace demojsonparser
{
    partial class StartView
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
            this.headline = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxChoose = new System.Windows.Forms.TextBox();
            this.choose_demo = new System.Windows.Forms.Button();
            this.button_parseJSON = new System.Windows.Forms.Button();
            this.logdelete = new System.Windows.Forms.Button();
            this.fileDialogChoose = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.errorbox = new System.Windows.Forms.RichTextBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // headline
            // 
            this.headline.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.headline.AutoSize = true;
            this.headline.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headline.Location = new System.Drawing.Point(247, 0);
            this.headline.Name = "headline";
            this.headline.Size = new System.Drawing.Size(844, 80);
            this.headline.TabIndex = 0;
            this.headline.Text = "CS:GO-Demo to JSON Parser";
            this.headline.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.2848F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.7152F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 169F));
            this.tableLayoutPanel1.Controls.Add(this.headline, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.button_parseJSON, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.errorbox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 62.00378F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 37.99622F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 402F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1264, 679);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.textBoxChoose);
            this.flowLayoutPanel1.Controls.Add(this.choose_demo);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(247, 83);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10, 10, 0, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(844, 43);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose a .dem file";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBoxChoose
            // 
            this.textBoxChoose.Location = new System.Drawing.Point(138, 13);
            this.textBoxChoose.Name = "textBoxChoose";
            this.textBoxChoose.Size = new System.Drawing.Size(390, 20);
            this.textBoxChoose.TabIndex = 0;
            // 
            // choose_demo
            // 
            this.choose_demo.Location = new System.Drawing.Point(534, 13);
            this.choose_demo.Name = "choose_demo";
            this.choose_demo.Size = new System.Drawing.Size(129, 26);
            this.choose_demo.TabIndex = 1;
            this.choose_demo.Text = "Choose demo";
            this.choose_demo.UseVisualStyleBackColor = true;
            this.choose_demo.Click += new System.EventHandler(this.choose_demo_Click);
            // 
            // button_parseJSON
            // 
            this.button_parseJSON.Location = new System.Drawing.Point(247, 132);
            this.button_parseJSON.Name = "button_parseJSON";
            this.button_parseJSON.Size = new System.Drawing.Size(844, 51);
            this.button_parseJSON.TabIndex = 2;
            this.button_parseJSON.Text = "Parse .dem to GameState JSON";
            this.button_parseJSON.UseVisualStyleBackColor = true;
            this.button_parseJSON.Click += new System.EventHandler(this.button_parseJSON_Click);
            // 
            // logdelete
            // 
            this.logdelete.Location = new System.Drawing.Point(108, 3);
            this.logdelete.Name = "logdelete";
            this.logdelete.Size = new System.Drawing.Size(162, 22);
            this.logdelete.TabIndex = 3;
            this.logdelete.Text = "Delete logfile";
            this.logdelete.UseVisualStyleBackColor = true;
            // 
            // fileDialogChoose
            // 
            this.fileDialogChoose.FileName = "fileDialogChoose";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Error logging";
            // 
            // errorbox
            // 
            this.errorbox.Location = new System.Drawing.Point(247, 279);
            this.errorbox.Name = "errorbox";
            this.errorbox.Size = new System.Drawing.Size(844, 390);
            this.errorbox.TabIndex = 5;
            this.errorbox.Text = "";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label2);
            this.flowLayoutPanel2.Controls.Add(this.logdelete);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(247, 242);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(844, 31);
            this.flowLayoutPanel2.TabIndex = 6;
            // 
            // StartView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.BurlyWood;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "StartView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CS:GO Demo to JSON";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label headline;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxChoose;
        private System.Windows.Forms.Button choose_demo;
        private System.Windows.Forms.OpenFileDialog fileDialogChoose;
        private System.Windows.Forms.Button button_parseJSON;
        private System.Windows.Forms.Button logdelete;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox errorbox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    }
}

