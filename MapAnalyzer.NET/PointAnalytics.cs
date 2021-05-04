using System.Linq;

namespace MapAnalyzer
{
    public class PointAnalytics
    {
        private readonly Point3D _point;
        private Point3D _w;
        private Point3D _e;
        private Point3D _n;
        private Point3D _s;

        public PointAnalytics(Point3D point)
        {
            _point = point;
        }

        public Point3D W
        {
            get => _w;
            set
            {
                _w = value;
                LinkW = BuildLink(_w);
            }
        }
        public Point3D E
        {
            get => _e;
            set
            {
                _e = value;
                LinkE = BuildLink(_e);
            }
        }
        public Point3D N
        {
            get => _n;
            set
            {
                _n = value;
                LinkN = BuildLink(_n);
            }
        }
        public Point3D S
        {
            get => _s;
            set
            {
                _s = value;
                LinkS = BuildLink(_s);
            }
        }

        private Link BuildLink(Point3D p) => (p != null) ? new Link(_point, p) : null;

        public Link LinkW { get; private set; }
        public Link LinkE { get; private set; }
        public Link LinkN { get; private set; }
        public Link LinkS { get; private set; }

        public int Quality 
        {
            get 
            {
                var links = new[] { LinkW, LinkE, LinkN, LinkS };
                var maxLevel =  links.Where(l => l != null).Max(l => (int)l.Level);
                return maxLevel;
            }
        }

        public override string ToString()
        {
            return $"[{_point.Elevation}/{Quality}]";
        }
    }
}
