public class GpsPoint
{
    public double DistanceMeters { get; set; }
    public double Latitude { get; set; }
    public string LatitudeDirection { get; set; }
    public double Longitude { get; set; }
    public string LongitudeDirection { get; set; }
    public double? Heading { get; set; } // Nullable, since not all points may have a heading
}
