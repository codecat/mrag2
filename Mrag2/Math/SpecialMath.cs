using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mrag2
{
  /// <summary>
  /// Static class with some useful Math functions.
  /// </summary>
  public static class SpecialMath
  {
    /// <summary>
    /// Quick smoothstep easing function.
    /// </summary>
    /// <param name="x">Float between 0 and 1.</param>
    /// <returns>Float between 0 and 1.</returns>
    public static float SmoothStep(float x)
    {
      return x * x * (3 - 2 * x);
    }

    /// <summary>
    /// Quick smoothstep easing function, which is supposed to be "more smooth".
    /// </summary>
    /// <param name="x">Float between 0 and 1.</param>
    /// <returns>Float between 0 and 1.</returns>
    public static float SmootherStep(float x)
    {
      return x * x * x * (x * (x * 6 - 15) + 10);
    }

    /// <summary>
    /// Get the intersection point between 2 lines.
    /// </summary>
    public static Vector2 IntersectionPoint(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
      float x1, x2, x3, x4, y1, y2, y3, y4;
      x1 = a1.X; y1 = a1.Y;
      x2 = a2.X; y2 = a2.Y;
      x3 = b1.X; y3 = b1.Y;
      x4 = b2.X; y4 = b2.Y;

      // from https://en.wikipedia.org/wiki/Line-line_intersection
      return new Vector2(
        ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)),
        ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)));
    }

    /// <summary>
    /// Check if 2 line segments intersect with each other.
    /// </summary>
    public static bool LineSegmentsIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
      Vector2 intersectionPoint = IntersectionPoint(a1, a2, b1, b2);

      if (intersectionPoint.X > System.Math.Min(a1.X, a2.X) &&
          intersectionPoint.X < System.Math.Max(a1.X, a2.X) &&
          intersectionPoint.Y > System.Math.Min(a1.Y, a2.Y) &&
          intersectionPoint.Y < System.Math.Max(a1.Y, a2.Y) &&

          intersectionPoint.X > System.Math.Min(b1.X, b2.X) &&
          intersectionPoint.X < System.Math.Max(b1.X, b2.X) &&
          intersectionPoint.Y > System.Math.Min(b1.Y, b2.Y) &&
          intersectionPoint.Y < System.Math.Max(b1.Y, b2.Y)
        ) {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Check if and where 2 line segments intersect with each other.
    /// </summary>
    public static Vector2? LineSegmentsIntersectAt(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
      Vector2 intersectionPoint = IntersectionPoint(a1, a2, b1, b2);

      if (intersectionPoint.X > System.Math.Min(a1.X, a2.X) &&
          intersectionPoint.X < System.Math.Max(a1.X, a2.X) &&
          intersectionPoint.Y > System.Math.Min(a1.Y, a2.Y) &&
          intersectionPoint.Y < System.Math.Max(a1.Y, a2.Y) &&

          intersectionPoint.X > System.Math.Min(b1.X, b2.X) &&
          intersectionPoint.X < System.Math.Max(b1.X, b2.X) &&
          intersectionPoint.Y > System.Math.Min(b1.Y, b2.Y) &&
          intersectionPoint.Y < System.Math.Max(b1.Y, b2.Y)
        ) {
        return intersectionPoint;
      }

      return null;
    }

    /// <summary>
    /// Get a normalized orientation vector from the given angle in degrees.
    /// </summary>
    /// <param name="angle">Angle in degrees.</param>
    /// <returns>Orientation vector.</returns>
    public static Vector2 NormalizedOrientationFromDegrees(float angle)
    {
      return new Vector2((float)System.Math.Sin(MathHelper.ToRadians(angle)), (float)System.Math.Cos(MathHelper.ToRadians(angle)));
    }
  }
}
