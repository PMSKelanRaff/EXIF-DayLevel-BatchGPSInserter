using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        private async void Startbtn_Click(object sender, EventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Select the parent folder containing multiple RSP/image folders",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string parentDir = dialog.SelectedPath;

                // Get selected camera folders
                var selectedCams = camFoldersCheckedListBox.CheckedItems.Cast<string>().ToList();

                if (selectedCams.Count == 0)
                {
                    MessageBox.Show("Please select at least one camera folder (Cam1–Cam4).", "No Cameras Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                progressBar1.Value = 0;
                progressBar1.Visible = true;

                // Run EXIF processing in background thread
                int totalImages = await Task.Run(() =>
                {
                    int total = 0;
                    var subDirs = Directory.GetDirectories(parentDir);

                    foreach (var subDir in subDirs)
                    {
                        string folderName = Path.GetFileName(subDir);
                        string rspPath = Path.Combine(parentDir, folderName + ".RSP");

                        if (!File.Exists(rspPath))
                            continue;

                        // Get camera subfolders like Cam1–Cam4
                        var camPaths = selectedCams
                            .Select(cam => Path.Combine(subDir, cam))
                            .Where(Directory.Exists)
                            .ToArray();

                        if (camPaths.Length == 0)
                            continue;

                        total += ExifBatchProcessor.Process(camPaths, rspPath, progressBar1);
                    }

                    return total;
                });

                MessageBox.Show($"EXIF batch processing completed.\n\n{totalImages} images processed.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static int ProcessAllRoutesInFolder(string parentFolder, ProgressBar progressBar)
        {
            int totalWritten = 0;

            foreach (var routeFolder in Directory.GetDirectories(parentFolder))
            {
                // Find the .RSP file
                var rspFile = Directory.GetFiles(routeFolder, "*.RSP").FirstOrDefault();
                if (rspFile == null) continue;

                // Find Cam1–Cam4 subfolders
                var camFolders = Directory.GetDirectories(routeFolder)
                                          .Where(dir => Path.GetFileName(dir).StartsWith("Cam"))
                                          .ToArray();

                if (camFolders.Length == 0) continue;

                totalWritten += ExifBatchProcessor.Process(camFolders, rspFile, progressBar);
            }

            return totalWritten;
        }
    }
}


