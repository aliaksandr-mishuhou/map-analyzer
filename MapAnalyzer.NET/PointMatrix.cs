using System.Collections.Generic;

namespace MapAnalyzer
{
    public class PointMatrix<TPoint> : List<List<TPoint>>
    {
        public TPoint TryToGet(int i, int j) 
        {
            if (i >= 0 && i < Count && j >= 0 && j < this[i].Count) 
            {
                return this[i][j];
            }

            return default;
        }
    }
}
