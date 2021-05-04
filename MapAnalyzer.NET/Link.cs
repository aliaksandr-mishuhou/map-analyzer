using System;

namespace MapAnalyzer
{
    public partial class Link
    {
        private readonly Point3D _p1;
        private readonly Point3D _p2;

        public const int StepMeters = 500;
        private const double K = 0.2; // -> excelent elevation = StepMeters * K: 500m * 0.2 -> 100m
        private const int MaxLevel = 5;

        public Link(Point3D p1, Point3D p2)
        {
            _p1 = p1;
            _p2 = p2;
        }

        public int Value => _p2.Elevation - _p1.Elevation;
        public LinkDirection Direction => (Value > 0) ? LinkDirection.Asc : LinkDirection.Desc;

        public int Level
        {
            get
            {
                var v = Math.Abs(Value);
                var levelDiff = StepMeters * K / MaxLevel;
                var level = Convert.ToInt32(Math.Round(v / levelDiff));

                if (level > MaxLevel) 
                {
                    level = MaxLevel;
                }

                return level;
            }
        }
    }
}
