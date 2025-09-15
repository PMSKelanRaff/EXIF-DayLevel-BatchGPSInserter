using System;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Startbtn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.camFoldersCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.checkBox_override_original = new System.Windows.Forms.CheckBox();
            this.rspDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.browseRspBtn = new System.Windows.Forms.Button();
            this.parentLbl = new System.Windows.Forms.Label();
            this.RSPLbl = new System.Windows.Forms.Label();
            this.browseParentBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Startbtn
            // 
            this.Startbtn.Location = new System.Drawing.Point(269, 201);
            this.Startbtn.Name = "Startbtn";
            this.Startbtn.Size = new System.Drawing.Size(75, 23);
            this.Startbtn.TabIndex = 5;
            this.Startbtn.Text = "Start";
            this.Startbtn.UseVisualStyleBackColor = true;
            this.Startbtn.Click += new System.EventHandler(this.Startbtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 201);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(238, 23);
            this.progressBar1.TabIndex = 6;
            // 
            // camFoldersCheckedListBox
            // 
            this.camFoldersCheckedListBox.FormattingEnabled = true;
            this.camFoldersCheckedListBox.Location = new System.Drawing.Point(12, 12);
            this.camFoldersCheckedListBox.Name = "camFoldersCheckedListBox";
            this.camFoldersCheckedListBox.Size = new System.Drawing.Size(218, 79);
            this.camFoldersCheckedListBox.TabIndex = 8;
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Location = new System.Drawing.Point(39, 146);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.Size = new System.Drawing.Size(305, 20);
            this.directoryTextBox.TabIndex = 9;
            // 
            // checkBox_override_original
            // 
            this.checkBox_override_original.AutoSize = true;
            this.checkBox_override_original.Location = new System.Drawing.Point(241, 89);
            this.checkBox_override_original.Name = "checkBox_override_original";
            this.checkBox_override_original.Size = new System.Drawing.Size(104, 17);
            this.checkBox_override_original.TabIndex = 10;
            this.checkBox_override_original.Text = "Override Original";
            this.checkBox_override_original.UseVisualStyleBackColor = true;
            // 
            // rspDirectoryTextBox
            // 
            this.rspDirectoryTextBox.Location = new System.Drawing.Point(39, 120);
            this.rspDirectoryTextBox.Name = "rspDirectoryTextBox";
            this.rspDirectoryTextBox.Size = new System.Drawing.Size(305, 20);
            this.rspDirectoryTextBox.TabIndex = 11;
            // 
            // browseRspBtn
            // 
            this.browseRspBtn.Location = new System.Drawing.Point(270, 172);
            this.browseRspBtn.Name = "browseRspBtn";
            this.browseRspBtn.Size = new System.Drawing.Size(75, 23);
            this.browseRspBtn.TabIndex = 12;
            this.browseRspBtn.Text = "Select RSP";
            this.browseRspBtn.UseVisualStyleBackColor = true;
            this.browseRspBtn.Click += new System.EventHandler(this.browseRspBtn_Click);
            // 
            // parentLbl
            // 
            this.parentLbl.AutoSize = true;
            this.parentLbl.Location = new System.Drawing.Point(-2, 123);
            this.parentLbl.Name = "parentLbl";
            this.parentLbl.Size = new System.Drawing.Size(39, 13);
            this.parentLbl.TabIndex = 13;
            this.parentLbl.Text = "Folder:";
            // 
            // RSPLbl
            // 
            this.RSPLbl.AutoSize = true;
            this.RSPLbl.Location = new System.Drawing.Point(-2, 149);
            this.RSPLbl.Name = "RSPLbl";
            this.RSPLbl.Size = new System.Drawing.Size(32, 13);
            this.RSPLbl.TabIndex = 14;
            this.RSPLbl.Text = "RSP:";
            // 
            // browseParentBtn
            // 
            this.browseParentBtn.Location = new System.Drawing.Point(185, 172);
            this.browseParentBtn.Name = "browseParentBtn";
            this.browseParentBtn.Size = new System.Drawing.Size(79, 23);
            this.browseParentBtn.TabIndex = 15;
            this.browseParentBtn.Text = "Select Folder";
            this.browseParentBtn.UseVisualStyleBackColor = true;
            this.browseParentBtn.Click += new System.EventHandler(this.browseParentBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 236);
            this.Controls.Add(this.browseParentBtn);
            this.Controls.Add(this.RSPLbl);
            this.Controls.Add(this.parentLbl);
            this.Controls.Add(this.browseRspBtn);
            this.Controls.Add(this.rspDirectoryTextBox);
            this.Controls.Add(this.checkBox_override_original);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.camFoldersCheckedListBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Startbtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "EXIF-DayLevel-BatchProcessor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Startbtn;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckedListBox camFoldersCheckedListBox;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.CheckBox checkBox_override_original;
        private System.Windows.Forms.TextBox rspDirectoryTextBox;
        private System.Windows.Forms.Button browseRspBtn;
        private System.Windows.Forms.Label parentLbl;
        private System.Windows.Forms.Label RSPLbl;
        private System.Windows.Forms.Button browseParentBtn;
    }
}

