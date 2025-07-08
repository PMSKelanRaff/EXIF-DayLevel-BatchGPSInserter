using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;


namespace EXIF_BatchGPSInserter
{
    public partial class Form1 : Form
    {
        private List<Tuple<double, double>> headings = new List<Tuple<double, double>>();

        public Form1()
        {
            InitializeComponent();
            camFoldersCheckedListBox.Items.Add("Cam1", true);
            camFoldersCheckedListBox.Items.Add("Cam2", true);
            camFoldersCheckedListBox.Items.Add("Cam3", true);
            camFoldersCheckedListBox.Items.Add("Cam4", true);
        }

        private void Startbtn_Click(object sender, EventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Select the parent folder containing multiple RSP/image folders",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string parentDir = dialog.SelectedPath;

                // Get selected camera folders from checkedListBoxCams
                var selectedCams = camFoldersCheckedListBox.CheckedItems
                    .Cast<string>()
                    .ToList();

                if (selectedCams.Count == 0)
                {
                    MessageBox.Show("Please select at least one camera folder (Cam1–Cam4).", "No Cameras Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Start processing with progress bar
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                int totalImages = ProcessAllSubFolders(parentDir, selectedCams, progressBar1);

                MessageBox.Show($"EXIF batch processing completed.\n\n{totalImages} images processed.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private int ProcessAllSubFolders(string parentDir, List<string> selectedCams, ProgressBar progressBar)
        {
            int totalProcessed = 0;

            var subFolders = Directory.GetDirectories(parentDir);

            foreach (var subFolder in subFolders)
            {
                var rspFile = Directory.GetFiles(parentDir, Path.GetFileName(subFolder) + ".RSP").FirstOrDefault();

                if (!string.IsNullOrEmpty(rspFile))
                {
                    var camFolders = selectedCams
                        .Select(cam => Path.Combine(subFolder, cam))
                        .Where(Directory.Exists)
                        .ToArray();

                    int processed = ExifBatchProcessor.Process(camFolders, rspFile, progressBar);
                    totalProcessed += processed;
                }
            }

            return totalProcessed;
        }
    }
}


