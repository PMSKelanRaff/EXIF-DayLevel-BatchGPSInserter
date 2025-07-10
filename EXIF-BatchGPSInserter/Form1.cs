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

            if (dialog.ShowDialog() != DialogResult.OK) return;

            string parentDir = dialog.SelectedPath;

            // Get selected camera folders
            var selectedCams = camFoldersCheckedListBox.CheckedItems.Cast<string>().ToList();

            if (selectedCams.Count == 0)
            {
                MessageBox.Show("Please select at least one camera folder (Cam1–Cam4).", "No Cameras Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            progressBar1.Visible = true;
            progressBar1.Value = 0;

            var subDirs = Directory.GetDirectories(parentDir);

            // Step 1: Count total JPG files across all selected Cam subfolders
            int totalFiles = 0;
            foreach (var subDir in subDirs)
            {
                string folderName = Path.GetFileName(subDir);
                string rspPath = Path.Combine(parentDir, folderName + ".RSP");

                if (!File.Exists(rspPath))
                    continue;

                var camPaths = selectedCams
                    .Select(cam => Path.Combine(subDir, cam))
                    .Where(Directory.Exists)
                    .ToArray();

                foreach (var camPath in camPaths)
                {
                    totalFiles += Directory.GetFiles(camPath, "*.JPG").Length;
                }
            }

            // Step 2: Count total LcmsResult_ImageInt_*.JPG files in all subfolders
            int totalLcmsFiles = 0;
            foreach (var subDir in subDirs)
            {
                var lcmsFiles = Directory.GetFiles(subDir, "LcmsResult_ImageInt_*.JPG", SearchOption.AllDirectories);
                totalLcmsFiles += lcmsFiles.Length;
            }

            progressBar1.Maximum = totalFiles + totalLcmsFiles;
            progressBar1.Value = 0;

            // Step 3: Process RSP + Cam folders
            int totalImages = await Task.Run(() =>
            {
                int total = 0;

                foreach (var subDir in subDirs)
                {
                    string folderName = Path.GetFileName(subDir);
                    string rspPath = Path.Combine(parentDir, folderName + ".RSP");

                    if (!File.Exists(rspPath))
                        continue;

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

            // Step 4: Process LcmsResult_ImageInt_*.JPG images using .HDC files
            int totalLcmsProcessed = await Task.Run(() =>
            {
                int total = 0;

                foreach (var subDir in subDirs)
                {
                    string folderName = Path.GetFileName(subDir);
                    string rspPath = Path.Combine(parentDir, folderName + ".RSP");

                    if (!File.Exists(rspPath))
                        continue;

                    total += ExifBatchProcessor.ProcessLcmsImagesFromHdcFile(subDir, rspPath, progressBar1);
                }

                return total;
            });

            MessageBox.Show(
                $"EXIF batch processing completed.\n\n" +
                $"{totalImages} images processed from Cam folders.\n" +
                $"{totalLcmsProcessed} LcmsResult images processed from .HDC files.",
                "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);

            progressBar1.Value = 0;
            progressBar1.Visible = false;
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


