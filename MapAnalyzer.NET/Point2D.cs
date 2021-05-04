using System;

namespace MapAnalyzer
{
    public class Point2D
    {
        public double Lat { get; set; }
        public double Long { get; set; }

        public static Point2D FromString(string s) 
        {
            var parts = s.Split(", ");
            return new Point2D 
            {
                Lat = Convert.ToDouble(parts[0]),
                Long = Convert.ToDouble(parts[1])
            };
        }

        public override string ToString()
        {
            return $"{Lat}, {Long}";
        }
    }
}
