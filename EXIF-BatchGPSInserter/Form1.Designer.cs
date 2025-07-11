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
            this.Startbtn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.camFoldersCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Startbtn
            // 
            this.Startbtn.Location = new System.Drawing.Point(269, 138);
            this.Startbtn.Name = "Startbtn";
            this.Startbtn.Size = new System.Drawing.Size(75, 23);
            this.Startbtn.TabIndex = 5;
            this.Startbtn.Text = "Start";
            this.Startbtn.UseVisualStyleBackColor = true;
            this.Startbtn.Click += new System.EventHandler(this.Startbtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 138);
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
            this.directoryTextBox.Location = new System.Drawing.Point(12, 112);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.Size = new System.Drawing.Size(332, 20);
            this.directoryTextBox.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 169);
            this.Controls.Add(this.directoryTextBox);
            this.Controls.Add(this.camFoldersCheckedListBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Startbtn);
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
    }
}

