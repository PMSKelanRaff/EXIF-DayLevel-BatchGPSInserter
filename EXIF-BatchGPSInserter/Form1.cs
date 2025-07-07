using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;
using ExifLibrary;
using System.Xml.Linq;


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

        private void RSPdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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

                // 🔽 Get selected camera folders from checkedListBoxCams
                var selectedCams = camFoldersCheckedListBox.CheckedItems
                    .Cast<string>()
                    .ToList();

                if (selectedCams.Count == 0)
                {
                    MessageBox.Show("Please select at least one camera folder (Cam1–Cam4).", "No Cameras Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 🔽 Start processing with progress bar
                progressBar1.Value = 0;
                progressBar1.Visible = true;

                int totalImages = ProcessAllSubFolders(parentDir, selectedCams, progressBar1);

                MessageBox.Show($"EXIF batch processing completed.\n\n{totalImages} images processed.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private List<GpsPoint> InterpolatePoints(List<GpsPoint> points, double intervalMeters)
        {
            var result = new List<GpsPoint>();
            if (points.Count == 0)
                return result;

            double current = 0.0; // Start from 0
            double max = points.Last().DistanceMeters;
            int i = 0;

            while (current <= max)
            {
                // Find the segment containing 'current'
                while (i < points.Count - 1 && points[i + 1].DistanceMeters < current)
                    i++;

                if (i == points.Count - 1)
                    break;

                var p1 = points[i];
                var p2 = points[i + 1];

                double t = (current - p1.DistanceMeters) / (p2.DistanceMeters - p1.DistanceMeters);

                double lat = p1.Latitude + t * (p2.Latitude - p1.Latitude);
                double lng = p1.Longitude + t * (p2.Longitude - p1.Longitude);

                // Determine direction and remove sign
                string latDir = lat >= 0 ? "N" : "S";
                string lngDir = lng >= 0 ? "E" : "W";

                result.Add(new GpsPoint
                {
                    DistanceMeters = current,
                    Latitude = Math.Abs(lat),
                    LatitudeDirection = latDir,
                    Longitude = Math.Abs(lng),
                    LongitudeDirection = lngDir
                });

                current += intervalMeters;
            }

            return result;
        }


        private void AddGpsDataToSegmentXmls(string xmlDirectory, List<GpsPoint> gpsPoints)
        {
            // Get all XML files in the directory, sorted by their numeric suffix
            var xmlFiles = Directory.GetFiles(xmlDirectory, "LcmsResult_*.xml")
                .OrderBy(f =>
                {
                    var name = Path.GetFileNameWithoutExtension(f);
                    var parts = name.Split('_');
                    int num = 0;
                    if (parts.Length > 1)
                        int.TryParse(parts[1], out num);
                    return num;
                })
                .ToList();

            // Interpolate at 10m intervals (if not already)
            var interpolated10m = InterpolatePoints(gpsPoints, 10.0);

            //assign heading
            foreach (var pt in interpolated10m)
            {
                if (headings.Count > 0)
                {
                    var closest = headings.OrderBy(h => Math.Abs(h.Item1 - pt.DistanceMeters)).First();
                    pt.Heading = closest.Item2;
                }
            }


            // For each XML file, add GPS data
            for (int i = 0; i < xmlFiles.Count && i < interpolated10m.Count; i++)
            {
                var xmlPath = xmlFiles[i];
                var gps = interpolated10m[i];

                var doc = XDocument.Load(xmlPath);
                var root = doc.Element("LcmsAnalyserResults");
                if (root == null)
                    continue;

                var section = root.Element("RoadSectionInfo");
                if (section == null)
                    continue;

                // Add or update GPS elements
                section.SetElementValue("Latitude", gps.Latitude);
                section.SetElementValue("LatitudeDirection", gps.LatitudeDirection);
                section.SetElementValue("Longitude", gps.Longitude);
                section.SetElementValue("LongitudeDirection", gps.LongitudeDirection);
                section.SetElementValue("Heading", gps.Heading.HasValue ? gps.Heading.Value : 0.0);

                doc.Save(xmlPath);
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

        public static class ExifBatchProcessor
        {
            public static int Process(string[] camFolders, string rspFile, ProgressBar progressBar)
            {
                int total = 0;
                foreach (string camFolder in camFolders)
                {
                    var jpgFiles = Directory.GetFiles(camFolder, "*.JPG");
                    int count = jpgFiles.Length;
                    int processed = 0;

                    foreach (var file in jpgFiles)
                    {
                        // TODO: Your logic to interpolate data and write EXIF here
                        // Example:
                        // var gps = GetInterpolatedData(file);
                        // ExifWriter.Write(file, gps);

                        processed++;
                        if (progressBar != null && progressBar.Maximum > 0)
                        {
                            progressBar.Value = Math.Min(progressBar.Maximum, progressBar.Value + 1);
                        }
                    }

                    total += processed;
                }

                return total;
            }
        }

    }
}

