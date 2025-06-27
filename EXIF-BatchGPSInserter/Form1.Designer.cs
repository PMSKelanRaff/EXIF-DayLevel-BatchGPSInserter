namespace EXIF_BatchGPSInserter
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
            this.RSPdataGridView = new System.Windows.Forms.DataGridView();
            this.SelectRSPBtn = new System.Windows.Forms.Button();
            this.rspTextBox = new System.Windows.Forms.TextBox();
            this.folderTextBox = new System.Windows.Forms.TextBox();
            this.SelectFolderBtn = new System.Windows.Forms.Button();
            this.Startbtn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.RSPdataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // RSPdataGridView
            // 
            this.RSPdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RSPdataGridView.Location = new System.Drawing.Point(13, 13);
            this.RSPdataGridView.Name = "RSPdataGridView";
            this.RSPdataGridView.Size = new System.Drawing.Size(1039, 418);
            this.RSPdataGridView.TabIndex = 0;
            this.RSPdataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.RSPdataGridView_CellContentClick);
            // 
            // SelectRSPBtn
            // 
            this.SelectRSPBtn.Location = new System.Drawing.Point(508, 475);
            this.SelectRSPBtn.Name = "SelectRSPBtn";
            this.SelectRSPBtn.Size = new System.Drawing.Size(84, 23);
            this.SelectRSPBtn.TabIndex = 1;
            this.SelectRSPBtn.Text = "Select RSP File";
            this.SelectRSPBtn.UseVisualStyleBackColor = true;
            this.SelectRSPBtn.Click += new System.EventHandler(this.SelectRSPBtn_Click);
            // 
            // rspTextBox
            // 
            this.rspTextBox.Location = new System.Drawing.Point(33, 475);
            this.rspTextBox.Name = "rspTextBox";
            this.rspTextBox.Size = new System.Drawing.Size(469, 20);
            this.rspTextBox.TabIndex = 2;
            // 
            // folderTextBox
            // 
            this.folderTextBox.Location = new System.Drawing.Point(33, 449);
            this.folderTextBox.Name = "folderTextBox";
            this.folderTextBox.Size = new System.Drawing.Size(469, 20);
            this.folderTextBox.TabIndex = 3;
            // 
            // SelectFolderBtn
            // 
            this.SelectFolderBtn.Location = new System.Drawing.Point(508, 449);
            this.SelectFolderBtn.Name = "SelectFolderBtn";
            this.SelectFolderBtn.Size = new System.Drawing.Size(84, 23);
            this.SelectFolderBtn.TabIndex = 4;
            this.SelectFolderBtn.Text = "Select Folder";
            this.SelectFolderBtn.UseVisualStyleBackColor = true;
            this.SelectFolderBtn.Click += new System.EventHandler(this.SelectFolderBtn_Click);
            // 
            // Startbtn
            // 
            this.Startbtn.Location = new System.Drawing.Point(977, 521);
            this.Startbtn.Name = "Startbtn";
            this.Startbtn.Size = new System.Drawing.Size(75, 23);
            this.Startbtn.TabIndex = 5;
            this.Startbtn.Text = "Start";
            this.Startbtn.UseVisualStyleBackColor = true;
            this.Startbtn.Click += new System.EventHandler(this.Startbtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(733, 521);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(238, 23);
            this.progressBar1.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 556);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Startbtn);
            this.Controls.Add(this.SelectFolderBtn);
            this.Controls.Add(this.folderTextBox);
            this.Controls.Add(this.rspTextBox);
            this.Controls.Add(this.SelectRSPBtn);
            this.Controls.Add(this.RSPdataGridView);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.RSPdataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView RSPdataGridView;
        private System.Windows.Forms.Button SelectRSPBtn;
        private System.Windows.Forms.TextBox rspTextBox;
        private System.Windows.Forms.TextBox folderTextBox;
        private System.Windows.Forms.Button SelectFolderBtn;
        private System.Windows.Forms.Button Startbtn;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

