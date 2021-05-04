using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MapAnalyzer
{
    class Program
    {
        private const string ElevationUrlTemplate = "https://maps.googleapis.com/maps/api/elevation/json?locations={0}&key=AIzaSyCedNIEpsE75q1V1poKZWT7PQ4uvJeRNxo";
        private static HttpClient _client = new HttpClient();

        private const string P1 = "54.160984, 27.695470";
        private const string P2 = "54.266886, 27.983876";

        private const string MatrixPathTemplate = "matrix_{0}.json";

        static void Main(string[] args)
        {
            var p1 = Point2D.FromString(P1);
            var p2 = Point2D.FromString(P2);

            var dataMatrix = GetMatrixAsync(p1, p2).Result;

            var analyticMatrix = BuildAnalyticMatrix(dataMatrix);

            WriteMatrix(analyticMatrix);
        }

        private static void WriteMatrix<TPoint>(PointMatrix<TPoint> matrix)
        {
            var sb = new StringBuilder();
            for (var i = matrix.Count - 1; i >= 0; i--)
            {
                for (var j = 0; j < matrix[i].Count; j++)
                {
                    if (j > 0) 
                    {
                        sb.Append(" - ");
                    }

                    var point = matrix[i][j];
                    sb.Append(point);
                }

                sb.AppendLine();
            }

            var s = sb.ToString();
            Console.WriteLine(s);
            File.WriteAllText("out.txt", s);
        }

        private static PointMatrix<PointAnalytics> BuildAnalyticMatrix(PointMatrix<Point3D> matrix)
        {
            var analytics = new PointMatrix<PointAnalytics>();

            // copy
            for (var i = 0; i < matrix.Count; i++)
            {
                var list = new List<PointAnalytics>();
                for (var j = 0; j < matrix[i].Count; j++)
                {
                    list.Add(new PointAnalytics(matrix[i][j]));
                }

                analytics.Add(list);
            }

            // links
            // y: S -> N
            for (var i = 0; i < matrix.Count; i++)
            {
                // x: W -> E
                for (var j = 0; j < matrix[i].Count; j++)
                {
                    var point = analytics[i][j];
                    point.W = matrix.TryToGet(i, j - 1);
                    point.E = matrix.TryToGet(i, j + 1);
                    point.S = matrix.TryToGet(i - 1, j);
                    point.N = matrix.TryToGet(i + 1, j);
                }
            }

            return analytics;
        }

        private static async Task<PointMatrix<Point3D>> GetMatrixAsync(Point2D p1, Point2D p2)
        {
            var areaId = $"{Link.StepMeters}_{p1}_{p2}".Replace(",", "_").Replace(".", "_");
            var matrixPath = string.Format(MatrixPathTemplate, areaId);

            PointMatrix<Point3D> matrix;
            if (File.Exists(matrixPath))
            {
                var json = await File.ReadAllTextAsync(matrixPath);
                matrix = JsonConvert.DeserializeObject<PointMatrix<Point3D>>(json);
            }
            else
            {
                matrix = BuildMatrixAsync(p1, p2).Result;
                var json = JsonConvert.SerializeObject(matrix);
                await File.WriteAllTextAsync(matrixPath, json);
            }

            return matrix;
        }

        private static async Task<PointMatrix<Point3D>> BuildMatrixAsync(Point2D p1, Point2D p2)
        {
            // validate
            if (p1.Lat > p2.Lat || p1.Long > p2.Long) 
            {
                throw new ArgumentException("p1 > p2");
            }

            // build matrix
            var xMaxMeters = Util.GetDistanceMeters(p1.Lat, p1.Long, p1.Lat, p2.Long);
            var yMaxMeters = Util.GetDistanceMeters(p1.Lat, p1.Long, p2.Lat, p1.Long);

            var stepMeters = Link.StepMeters;

            var stepLong = Math.Round((p2.Long - p1.Long) / xMaxMeters * stepMeters, 6);
            var stepLat = Math.Round((p2.Lat - p1.Lat) / yMaxMeters * stepMeters, 6);

            var xCount = Convert.ToInt32(Math.Floor(xMaxMeters / stepMeters));
            var yCount = Convert.ToInt32(Math.Floor(yMaxMeters / stepMeters));

            Console.WriteLine($"Area: w = {xMaxMeters}m, h = {yMaxMeters}m, step = {stepMeters}m, stepLong = {stepLong}, stepLat= {stepLat}, matrix = {xCount}x{yCount}");

            // matrix

            var matrix = new PointMatrix<Point3D>();
            // y: S -> N
            for (var y = p1.Lat; y < p2.Lat; y += stepLat)
            {
                // x: W -> E
                var list = new List<Point3D>();
                for (var x = p1.Long; x < p2.Long; x += stepLong)
                {
                    var point = new Point3D
                    {
                        Long = Math.Round(x, 6),
                        Lat = Math.Round(y, 6)
                    };

                    var elevation = await GetElevationAsync(point);
                    point.Elevation = elevation;

                    list.Add(point);
                }

                matrix.Add(list);
            }

            return matrix;
        }


        private static async Task<int> GetElevationAsync(Point2D point) 
        {
#if TEST
            return 0;
#else
            var url = string.Format(ElevationUrlTemplate, point);
            var json = await _client.GetStringAsync(url);
            var response = JsonConvert.DeserializeObject<ElevationResponse>(json);
            return Convert.ToInt32(response.Results[0].Elevation);
#endif
        }

        public class ElevationResponse
        {
            [JsonProperty("results")]
            public List<Result> Results { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        public class Location
        {
            [JsonProperty("lat")]
            public double Lat { get; set; }

            [JsonProperty("lng")]
            public double Lng { get; set; }
        }

        public class Result
        {
            [JsonProperty("elevation")]
            public double Elevation { get; set; }

            [JsonProperty("location")]
            public Location Location { get; set; }

            [JsonProperty("resolution")]
            public double Resolution { get; set; }
        }
    }
}
