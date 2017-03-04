using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mrag2
{
  public class Line
  {
    public Vector2 A;
    public Vector2 B;

    public Line(Vector2 a, Vector2 b)
    {
      A = a;
      B = b;
    }

    public Line(Vector2 a, Vector2 orientation, float length)
    {
#if DEBUG
      System.Diagnostics.Debug.Assert(orientation.Length() <= 1.0f);
#endif

      A = a;
      B = a + orientation * length;
    }

    public override string ToString()
    {
      return "[" + A + " to " + B + "]";
    }

    public Vector2 IntersectionPoint(Line line)
    {
      return SpecialMath.IntersectionPoint(A, B, line.A, line.B);
    }

    public bool Intersects(Line line)
    {
      return SpecialMath.LineSegmentsIntersect(A, B, line.A, line.B);
    }

    public Vector2[] GetPosArray()
    {
      return new Vector2[] { A, B };
    }
  }
}
