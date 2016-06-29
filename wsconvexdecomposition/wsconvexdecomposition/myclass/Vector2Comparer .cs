using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wsconvexdecomposition
{
    class Vector2Comparer : IComparer<Vector2>
    {
       public int Compare(Vector2 a, Vector2 b)
        {
            if (a.x != b.x) return a.x < b.x?-1:1;
            return a.y < b.y?-1:1;;
        }
    }
}
