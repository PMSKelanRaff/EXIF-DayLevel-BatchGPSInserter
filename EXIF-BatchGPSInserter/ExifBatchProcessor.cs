using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ExifLibrary;

namespace EXIF_BatchGPSInserter
{
    public static class ExifBatchProcessor
    {
        public static int Process(string[] camFolders, string rspFile, ProgressBar progressBar)
        {
            var gpsPoints = ParseRspFile(rspFile);
            if (gpsPoints.Count == 0) return 0;

            var interpolated = InterpolateWithoutHeading(gpsPoints, 10.0);
            int total = 0;

            foreach (string camFolder in camFolders)
            {
                var jpgFiles = Directory.GetFiles(camFolder, "*.JPG")
                                        .OrderBy(f => ExtractDistanceFromFilename(f))
                                        .ToList();

                foreach (var imagePath in jpgFiles)
                {
                    double imgDist = ExtractDistanceFromFilename(imagePath);

                    // Match to closest interpolated GPS point
                    var gps = interpolated.OrderBy(pt => Math.Abs(pt.DistanceMeters - imgDist)).First();

                    try
                    {
                        var file = ExifLibrary.ImageFile.FromFile(imagePath);

                        // Skip if all GPS tags already exist
                        if (HasGpsTags(file)) continue;

                        // Remove existing GPS tags to avoid duplicates
                        RemoveGpsTags(file);

                        // Convert to absolute values for EXIF (numbers must be positive)
                        double absLat = Math.Abs(gps.Latitude);
                        double absLng = Math.Abs(gps.Longitude);
                        float? absHeading = gps.Heading.HasValue && gps.Heading.Value >= 0
                            ? (float?)gps.Heading.Value
                            : null;

                        // Convert decimal degrees to degrees, minutes, seconds (using abs values)
                        int latDeg, latMin;
                        float latSec;
                        ConvertToDMS(absLat, out latDeg, out latMin, out latSec);

                        int lngDeg, lngMin;
                        float lngSec;
                        ConvertToDMS(absLng, out lngDeg, out lngMin, out lngSec);

                        // Add GPS Latitude and Longitude tags with DMS values and direction strings
                        file.Properties.Add(ExifLibrary.ExifTag.GPSLatitude, latDeg, latMin, latSec);
                        file.Properties.Add(ExifLibrary.ExifTag.GPSLatitudeRef, gps.LatitudeDirection);
                        file.Properties.Add(ExifLibrary.ExifTag.GPSLongitude, lngDeg, lngMin, lngSec);
                        file.Properties.Add(ExifLibrary.ExifTag.GPSLongitudeRef, gps.LongitudeDirection);

                        // Add GPS Image Direction and Reference if heading is available
                        if (absHeading.HasValue)
                        {
                            file.Properties.Add(ExifLibrary.ExifTag.GPSImgDirection, absHeading.Value);
                            file.Properties.Add(ExifLibrary.ExifTag.GPSImgDirectionRef, "T"); // True north
                        }

                        // Save changes to the image file
                        file.Save(imagePath);
                        total++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to write EXIF for {imagePath}: {ex.Message}");
                    }

                    if (progressBar != null && progressBar.Maximum > 0)
                    {
                        int newValue = 0;
                        progressBar.Invoke((MethodInvoker)(() =>
                        {
                            newValue = progressBar.Value + 1;
                            if (newValue <= progressBar.Maximum && newValue >= progressBar.Minimum)
                            {
                                progressBar.Value = newValue;
                            }
                        }));
                    }
                }
            }

            return total;
        }

        private static bool HasGpsTags(ImageFile file)
        {
            return file.Properties.Any(p => p.Tag == ExifTag.GPSLatitude) &&
                   file.Properties.Any(p => p.Tag == ExifTag.GPSLatitudeRef) &&
                   file.Properties.Any(p => p.Tag == ExifTag.GPSLongitude) &&
                   file.Properties.Any(p => p.Tag == ExifTag.GPSLongitudeRef) &&
                   file.Properties.Any(p => p.Tag == ExifTag.GPSImgDirection) &&
                   file.Properties.Any(p => p.Tag == ExifTag.GPSImgDirectionRef);
        }

        private static void RemoveGpsTags(ImageFile file)
        {
            var tagsToRemove = new[]
            {
                    ExifTag.GPSLatitude,
                    ExifTag.GPSLatitudeRef,
                    ExifTag.GPSLongitude,
                    ExifTag.GPSLongitudeRef,
                    ExifTag.GPSImgDirection,
                    ExifTag.GPSImgDirectionRef
        };

            foreach (var tag in tagsToRemove)
            {
                var prop = file.Properties.FirstOrDefault(p => p.Tag == tag);
                if (prop != null)
                    file.Properties.Remove(prop);
            }
        }

        private static void ConvertToDMS(double decimalDegrees, out int degrees, out int minutes, out float seconds)
        {
            degrees = (int)decimalDegrees;
            double minutesFull = (decimalDegrees - degrees) * 60;
            minutes = (int)minutesFull;
            seconds = (float)((minutesFull - minutes) * 60);
        }

        private static List<GpsPoint> ParseRspFile(string rspFile)
        {
            var points = new List<GpsPoint>();
            var headings = new List<Tuple<double, double>>();
            var lines = File.ReadAllLines(rspFile);

            // Parse headings from 5420 lines
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length > 6 && parts[0].StartsWith("5420"))
                {
                    if (double.TryParse(parts[1], out double distanceFeet) &&
                        double.TryParse(parts[6], out double heading))
                    {
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
                            LatitudeDirection = lat >= 0 ? "N" : "S",
                            Longitude = lng,
                            LongitudeDirection = lng >= 0 ? "E" : "W"
                        });
                    }
                }
            }

            // Assign closest heading to each point
            foreach (var pt in points)
            {
                if (headings.Count > 0)
                {
                    var closest = headings.OrderBy(h => Math.Abs(h.Item1 - pt.DistanceMeters)).First();
                    pt.Heading = closest.Item2;
                }
            }

            return points;
        }

        private static List<GpsPoint> InterpolateWithoutHeading(List<GpsPoint> points, double intervalMeters)
        {
            var result = new List<GpsPoint>();
            if (points.Count == 0) return result;

            double current = 0.0;
            double max = points.Last().DistanceMeters;
            int i = 0;

            while (current <= max)
            {
                while (i < points.Count - 1 && points[i + 1].DistanceMeters < current)
                    i++;

                if (i == points.Count - 1) break;

                var p1 = points[i];
                var p2 = points[i + 1];
                double t = (current - p1.DistanceMeters) / (p2.DistanceMeters - p1.DistanceMeters);

                double lat = p1.Latitude + t * (p2.Latitude - p1.Latitude);
                double lng = p1.Longitude + t * (p2.Longitude - p1.Longitude);

                result.Add(new GpsPoint
                {
                    DistanceMeters = current,
                    Latitude = lat,
                    LatitudeDirection = lat >= 0 ? "N" : "S",
                    Longitude = lng,
                    LongitudeDirection = lng >= 0 ? "E" : "W"
                    // Heading assigned later
                });

                current += intervalMeters;
            }

            return result;
        }

        private static double ExtractDistanceFromFilename(string filename)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Collect all parts that can be parsed as double
            var numericParts = parts
                .Select(p => double.TryParse(p, out double val) ? (double?)val : null)
                .Where(d => d.HasValue)
                .Select(d => d.Value)
                .ToList();

            if (numericParts.Count >= 2)
            {
                // The second last number is the distance (last number is camera number)
                return numericParts[numericParts.Count - 2];
            }

            // Fallback to zero if not enough numbers found
            return 0;
        }
    }
}
