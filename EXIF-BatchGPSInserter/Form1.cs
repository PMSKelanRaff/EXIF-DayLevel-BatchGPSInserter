using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;
using ExifLibrary;


namespace EXIF_BatchGPSInserter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void RSPdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SelectFolderBtn_Click(object sender, EventArgs e)
        {
            //select a folder
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select a network folder";
            dialog.UseDescriptionForTitle = true; // Show description in title bar
            dialog.SelectedPath = @"\\PMSPC31\US Data 24-25";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                folderTextBox.Text = dialog.SelectedPath;
            }
        }

        private void SelectRSPBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "RSP files (*.rsp)|*.rsp|All files (*.*)|*.*";
                openFileDialog.Title = "Select an RSP file";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    rspTextBox.Text = openFileDialog.FileName;

                    // Read and process the file
                    var points = new List<GpsPoint>();
                    var lines = File.ReadAllLines(openFileDialog.FileName);
                    var headings = new List<Tuple<double, double>>();

                    // Parse headings from 5420 lines
                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');
                        if (parts.Length > 6 && parts[0].StartsWith("5420"))
                        {
                            if (double.TryParse(parts[1], out double distanceFeet) &&
                                double.TryParse(parts[6], out double heading))
                            {
                                // Place your headings.Add here:
                                headings.Add(Tuple.Create(distanceFeet * 0.3048, heading));
                            }
                        }
                    }

                    // Parse GPS points from 5280 lines
                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');
                        if (parts.Length > 6 && parts[0].StartsWith("5280"))
                        {
                            if (double.TryParse(parts[1], out double distanceFeet) &&
                                double.TryParse(parts[5], out double lat) &&
                                double.TryParse(parts[6], out double lng))
                            {
                                points.Add(new GpsPoint
                                {
                                    DistanceMeters = distanceFeet * 0.3048,
                                    Latitude = lat,
                                    Longitude = lng,
                                });
                            }
                        }
                    }

                    // Interpolate to 5 meter intervals
                    var interpolated = InterpolatePoints(points, 5.0);

                    // Assign closest heading to each interpolated point
                    foreach (var pt in interpolated)
                    {
                        if (headings.Count > 0)
                        {
                            var closest = headings.OrderBy(h => Math.Abs(h.Item1 - pt.DistanceMeters)).First();
                            pt.Heading = closest.Item2;
                        }
                    }

                    // Display in DataGridView
                    RSPdataGridView.DataSource = interpolated;
                }
            }
        }

        private void Startbtn_Click(object sender, EventArgs e)
        {
            var gpsData = RSPdataGridView.DataSource as List<GpsPoint>;
            if (gpsData == null || gpsData.Count == 0)
            {
                MessageBox.Show("No GPS data loaded. Please select and process an RSP file first.");
                return;
            }

            int processed = ProcessImagesAndWriteExif(gpsData);
            if (processed > 0)
                MessageBox.Show("EXIF writing complete.");
            else
                MessageBox.Show("No images were found or processed.");
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

        private int ProcessImagesAndWriteExif(List<GpsPoint> interpolated)
        {
            string rootFolder = folderTextBox.Text;
            if (string.IsNullOrWhiteSpace(rootFolder) || !Directory.Exists(rootFolder))
            {
                MessageBox.Show("Please select a valid folder.");
                return 0;
            }

            // Get all Cam subfolders
            var camFolders = Directory.GetDirectories(rootFolder, "Cam*", SearchOption.TopDirectoryOnly);

            // Get all image files in each Cam folder, sorted by name
            var camImages = camFolders
                .Select(folder => Directory.GetFiles(folder, "*.jpg").OrderBy(f => f).ToList())
                .ToList();

            int maxImages;

            // Count total images to process
            int totalImages = camImages.Sum(list => list.Count);
            progressBar1.Minimum = 0;
            progressBar1.Maximum = totalImages;
            progressBar1.Value = 0;

            // Find the max number of images in any Cam folder
            if (camFolders.Length == 0)
            {
                MessageBox.Show("No Cam folders found in the selected root folder.");
                return 0;
            }
            else
            {
                maxImages = camImages.Max(list => list.Count);
            }

            int processedCount = 0;

            for (int i = 0; i < maxImages; i++)
            {
                // For each index, get the image from each Cam folder (if exists)
                var imagesAtIndex = camImages
                    .Select(list => i < list.Count ? list[i] : null)
                    .Where(f => f != null)
                    .ToList();

                if (imagesAtIndex.Count == 0)
                    continue;

                // Extract distance from filename (assumes format: ... <distance> <index>.jpg)
                string sampleFile = Path.GetFileNameWithoutExtension(imagesAtIndex[0]);
                var parts = sampleFile.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                    continue;

                if (!double.TryParse(parts[parts.Length - 2], out double distanceFeet))
                    continue;

                double distanceMeters = distanceFeet * 0.3048;

                // Find closest GPS point
                var gps = interpolated.OrderBy(p => Math.Abs(p.DistanceMeters - distanceMeters)).FirstOrDefault();
                if (gps == null)
                    continue;

                // Write EXIF metadata to each image at this index
                foreach (var imagePath in imagesAtIndex)
                {
                    WriteGpsToExif(imagePath, gps);
                    processedCount++;
                    progressBar1.Value = Math.Min(processedCount, progressBar1.Maximum);
                    Application.DoEvents(); // Refresh UI
                    //PrintGpsImgDirection(imagePath); // Print GPSImgDirection for debugging
                }
            }
            progressBar1.Value = progressBar1.Maximum;
            return processedCount;
        }

        private void WriteGpsToExif(string imagePath, GpsPoint gps)
        {
            try
            {
                var file = ExifLibrary.ImageFile.FromFile(imagePath);

                // Helper function to find a property by tag
                ExifLibrary.ExifProperty FindProperty(ExifLibrary.ExifTag tag)
                {
                    foreach (var prop in file.Properties)
                    {
                        if (prop.Tag == tag)
                            return prop;
                    }
                    return null;
                }

                // Check if all GPS tags already exist
                var latProp = FindProperty(ExifLibrary.ExifTag.GPSLatitude);
                var latRefProp = FindProperty(ExifLibrary.ExifTag.GPSLatitudeRef);
                var lngProp = FindProperty(ExifLibrary.ExifTag.GPSLongitude);
                var lngRefProp = FindProperty(ExifLibrary.ExifTag.GPSLongitudeRef);
                var imgDirProp = FindProperty(ExifLibrary.ExifTag.GPSImgDirection);
                var imgDirRefProp = FindProperty(ExifLibrary.ExifTag.GPSImgDirectionRef);

                if (latProp != null && latRefProp != null && lngProp != null && lngRefProp != null
                    && imgDirProp != null && imgDirRefProp != null)
                {
                    // All tags are present, skip writing
                    return;
                }

                // Remove existing properties to avoid duplicates
                if (latProp != null) file.Properties.Remove(latProp);
                if (latRefProp != null) file.Properties.Remove(latRefProp);
                if (lngProp != null) file.Properties.Remove(lngProp);
                if (lngRefProp != null) file.Properties.Remove(lngRefProp);
                if (imgDirProp != null) file.Properties.Remove(imgDirProp);
                if (imgDirRefProp != null) file.Properties.Remove(imgDirRefProp);

                // Convert decimal degrees to degrees, minutes, seconds
                double lat = gps.Latitude;
                double lng = gps.Longitude;

                int latDeg = (int)lat;
                double latMinFull = (lat - latDeg) * 60;
                int latMin = (int)latMinFull;
                double latSec = (latMinFull - latMin) * 60;

                int lngDeg = (int)lng;
                double lngMinFull = (lng - lngDeg) * 60;
                int lngMin = (int)lngMinFull;
                double lngSec = (lngMinFull - lngMin) * 60;

                // Add new GPS properties
                file.Properties.Add(ExifLibrary.ExifTag.GPSLatitude, latDeg, latMin, (float)latSec);
                file.Properties.Add(ExifLibrary.ExifTag.GPSLatitudeRef, gps.LatitudeDirection);
                file.Properties.Add(ExifLibrary.ExifTag.GPSLongitude, lngDeg, lngMin, (float)lngSec);
                file.Properties.Add(ExifLibrary.ExifTag.GPSLongitudeRef, gps.LongitudeDirection);

                // Add GPSImgDirection and GPSImgDirectionRef
                float imgDirection = gps.Heading.HasValue ? (float)gps.Heading.Value : 0.0f;
                file.Properties.Add(ExifLibrary.ExifTag.GPSImgDirection, imgDirection);
                file.Properties.Add(ExifLibrary.ExifTag.GPSImgDirectionRef, "T"); // "T" for true north, "M" for magnetic north

                // Save changes (overwrite original)
                file.Save(imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to write EXIF for {imagePath}: {ex.Message}");
            }
        }

        private void PrintGpsImgDirection(string imagePath)
        {
            try
            {
                var file = ExifLibrary.ImageFile.FromFile(imagePath);

                // Find the GPSImgDirection property
                var directionProp = file.Properties
                    .FirstOrDefault(p => p.Tag == ExifLibrary.ExifTag.GPSImgDirection);

                if (directionProp != null)
                {
                    var value = directionProp.Value;
                    double headingValue;

                    // ExifLibrary stores rational as ExifRational or ExifURational
                    if (value is ExifLibrary.ExifURational urational)
                    {
                        headingValue = urational.ToFloat(); 
                    }
                    else if (value is float f)
                    {
                        headingValue = f;
                    }
                    else if (value is double d)
                    {
                        headingValue = d;
                    }
                    else
                    {
                        headingValue = Convert.ToDouble(value);
                    }

                    MessageBox.Show($"GPSImgDirection for {Path.GetFileName(imagePath)}: {headingValue:0.##}°");
                }
                else
                {
                    MessageBox.Show($"GPSImgDirection not found in {Path.GetFileName(imagePath)}.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading EXIF from {imagePath}: {ex.Message}");
            }
        }
    }
}
