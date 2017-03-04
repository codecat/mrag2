using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mrag2
{
  /// <summary>
  /// Rotatable rectangle.
  /// </summary>
  public class RotRect
  {
    public float x, y;
    public float width, height;
    /// <summary>
    /// Rotation in radians.
    /// </summary>
    public float rotation;

    /// <summary>
    /// Whether or not to anchor the RotRect in the center. When rendering a texture with a RotRect, it's rendered with x and y set as the anchor, and it will rotate around the center.
    /// </summary>
    public bool anchorInCenter = false;

    /// <summary>
    /// New empty RotRect without any values set.
    /// </summary>
    public RotRect() { }
    /// <summary>
    /// RotRect from XNA Rectangle, simply sets x, y, width and height and leaves rotation at 0.
    /// </summary>
    /// <param name="rect"></param>
    public RotRect(Rectangle rect)
    {
      this.x = (float)rect.X;
      this.y = (float)rect.Y;
      this.width = (float)rect.Width;
      this.height = (float)rect.Height;
    }
    /// <summary>
    /// RotRect from x, y, width and height.
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <param name="w">Width</param>
    /// <param name="h">Height</param>
    public RotRect(float x, float y, float w, float h)
    {
      this.x = x;
      this.y = y;
      this.width = w;
      this.height = h;
    }
    /// <summary>
    /// RotRect from position and size.
    /// </summary>
    /// <param name="v">Position</param>
    /// <param name="s">Size</param>
    public RotRect(Vector2 v, Vector2 s)
    {
      this.x = v.X;
      this.y = v.Y;
      this.width = s.X;
      this.height = s.Y;
    }
    /// <summary>
    /// RotRect from x, y, width, height and rotation.
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <param name="w">Width</param>
    /// <param name="h">Height</param>
    /// <param name="rot">Rotation in radians</param>
    public RotRect(float x, float y, float w, float h, float rot)
    {
      this.x = x;
      this.y = y;
      this.width = w;
      this.height = h;
      this.rotation = rot;
    }

    /// <summary>
    /// Get the bottom Y coordinate.
    /// </summary>
    public float Bottom { get { return this.y + this.height / (anchorInCenter ? 2 : 1); } }
    /// <summary>
    /// Get the right X coordinate.
    /// </summary>
    public float Right { get { return this.x + this.width / (anchorInCenter ? 2 : 1); } }

    public override string ToString()
    {
      return "{" + x + ", " + y + ", " + width + ", " + height + "}";
    }

    /// <summary>
    /// Translate the RotRect with the given vector.
    /// </summary>
    /// <param name="v">The vector to translate this RotRect with.</param>
    public RotRect Translate(Vector2 v)
    {
      x += v.X;
      y += v.Y;
			return this;
    }

    /// <summary>
    /// Rotate the RotRect with the given rotation.
    /// </summary>
    /// <param name="f">The rotation to rotate this RotRect with, in radians.</param>
    public RotRect Rotate(float f)
    {
      rotation += f;
			return this;
    }

    /// <summary>
    /// Get a vector of the x and y.
    /// </summary>
    public Vector2 GetPos()
    {
      return new Vector2(x, y);
    }

    /// <summary>
    /// Get a vector of the width and height.
    /// </summary>
    public Vector2 GetSize()
    {
      return new Vector2(width, height);
    }

    /// <summary>
    /// Get an array of the vectors in the rotated rectangle, being [topLeft, topRight, botRight, botLeft].
    /// </summary>
    /// <returns>Array of vectors.</returns>
    public Vector2[] GetRotatedPosArray()
    {
      Matrix rotMatrix = Matrix.CreateRotationZ(rotation);

      Vector2 topLeft;
      Vector2 topRight;
      Vector2 botRight;
      Vector2 botLeft;

      if (anchorInCenter) {
        // center anchored
        topLeft = GetPos() + Vector2.Transform(new Vector2(-width / 2, -height / 2), rotMatrix);
        topRight = GetPos() + Vector2.Transform(new Vector2(width / 2, -height / 2), rotMatrix);
        botRight = GetPos() + Vector2.Transform(new Vector2(width / 2, height / 2), rotMatrix);
        botLeft = GetPos() + Vector2.Transform(new Vector2(-width / 2, height / 2), rotMatrix);
      } else {
        // topleft anchored
        topLeft = GetPos();
        topRight = GetPos() + Vector2.Transform(new Vector2(width, 0), rotMatrix);
        botRight = GetPos() + Vector2.Transform(new Vector2(width, height), rotMatrix);
        botLeft = GetPos() + Vector2.Transform(new Vector2(0, height), rotMatrix);
      }

      return new Vector2[] { topLeft, topRight, botRight, botLeft };
    }

    public static RotRect operator *(RotRect left, float multiplier)
    {
      left.height *= multiplier;
      left.width *= multiplier;

      return left;
    }

    /// <summary>
    /// Test if the RotRect contains the given point.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point is in the RotRect, false if it's not.</returns>
    public bool Contains(Vector2 point)
    {
      return (PointInPolygon(point, GetRotatedPosArray()));
    }

    /// <summary>
    /// Test if the RotRect contains the given point.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns>True if the point is in the RotRect, false if it's not.</returns>
    public bool Contains(Point point)
    {
      return Contains(new Vector2(point.X, point.Y));
    }

    /// <summary>
    /// Test if the RotRect intersects with another RotRect.
    /// </summary>
    /// <param name="other">The other RotRect to test.</param>
    /// <returns>True if they intersect, false if they don't.</returns>
    public bool Intersects(RotRect other)
    {
      Vector2[] points = GetRotatedPosArray();
      Vector2[] otherPoints = other.GetRotatedPosArray();

      if (PointInPolygon(points[0], otherPoints) ||
          PointInPolygon(points[1], otherPoints) ||
          PointInPolygon(points[2], otherPoints) ||
          PointInPolygon(points[3], otherPoints) ||

          PointInPolygon(otherPoints[0], points) ||
          PointInPolygon(otherPoints[1], points) ||
          PointInPolygon(otherPoints[2], points) ||
          PointInPolygon(otherPoints[3], points)
      ) {
        return true;
      }

      if (SpecialMath.LineSegmentsIntersect(points[0], points[1], otherPoints[0], otherPoints[1]) ||
          SpecialMath.LineSegmentsIntersect(points[1], points[2], otherPoints[0], otherPoints[1]) ||
          SpecialMath.LineSegmentsIntersect(points[2], points[3], otherPoints[0], otherPoints[1]) ||
          SpecialMath.LineSegmentsIntersect(points[3], points[0], otherPoints[0], otherPoints[1]) ||
          SpecialMath.LineSegmentsIntersect(points[0], points[1], otherPoints[1], otherPoints[2]) ||
          SpecialMath.LineSegmentsIntersect(points[1], points[2], otherPoints[1], otherPoints[2]) ||
          SpecialMath.LineSegmentsIntersect(points[2], points[3], otherPoints[1], otherPoints[2]) ||
          SpecialMath.LineSegmentsIntersect(points[3], points[0], otherPoints[1], otherPoints[2]) ||
          SpecialMath.LineSegmentsIntersect(points[0], points[1], otherPoints[2], otherPoints[3]) ||
          SpecialMath.LineSegmentsIntersect(points[1], points[2], otherPoints[2], otherPoints[3]) ||
          SpecialMath.LineSegmentsIntersect(points[2], points[3], otherPoints[2], otherPoints[3]) ||
          SpecialMath.LineSegmentsIntersect(points[3], points[0], otherPoints[2], otherPoints[3]) ||
          SpecialMath.LineSegmentsIntersect(points[0], points[1], otherPoints[3], otherPoints[0]) ||
          SpecialMath.LineSegmentsIntersect(points[1], points[2], otherPoints[3], otherPoints[0]) ||
          SpecialMath.LineSegmentsIntersect(points[2], points[3], otherPoints[3], otherPoints[0]) ||
          SpecialMath.LineSegmentsIntersect(points[3], points[0], otherPoints[3], otherPoints[0])
      ) {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Test if and where the RotRect intersects with a line.
    /// </summary>
    /// <param name="line">The line to test.</param>
    /// <returns>The intersection point closest to the line's A point if it was hit, or null if it didn't hit.</returns>
    public Vector2? IntersectsAt(Line line)
    {
      Vector2[] points = GetRotatedPosArray();
      Vector2[] otherPoints = line.GetPosArray();

      Vector2 a = line.A;
      float closest = float.PositiveInfinity;
      float dist;

      Vector2? ret = null;
      Vector2? test = null;

      test = SpecialMath.LineSegmentsIntersectAt(otherPoints[0], otherPoints[1], points[0], points[1]);
      if (test.HasValue) {
        dist = Vector2.Distance(a, test.Value);
        if (dist < closest) {
          closest = dist;
          ret = test;
        }
      }

      test = SpecialMath.LineSegmentsIntersectAt(otherPoints[0], otherPoints[1], points[1], points[2]);
      if (test.HasValue) {
        dist = Vector2.Distance(a, test.Value);
        if (dist < closest) {
          closest = dist;
          ret = test;
        }
      }

      test = SpecialMath.LineSegmentsIntersectAt(otherPoints[0], otherPoints[1], points[2], points[3]);
      if (test.HasValue) {
        dist = Vector2.Distance(a, test.Value);
        if (dist < closest) {
          closest = dist;
          ret = test;
        }
      }

      test = SpecialMath.LineSegmentsIntersectAt(otherPoints[0], otherPoints[1], points[3], points[0]);
      if (test.HasValue) {
        dist = Vector2.Distance(a, test.Value);
        if (dist < closest) {
          closest = dist;
          ret = test;
        }
      }

      return ret;
    }

    private static bool PointInPolygon(Vector2 p, Vector2[] poly)
    {
      if (poly.Length < 3) {
        return false;
      }

      Vector2 p1, p2;
      bool inside = false;

      Vector2 oldPoint = new Vector2(poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

      for (int i = 0; i < poly.Length; i++) {
        Vector2 newPoint = new Vector2(poly[i].X, poly[i].Y);

        if (newPoint.X > oldPoint.X) {
          p1 = oldPoint;
          p2 = newPoint;
        } else {
          p1 = newPoint;
          p2 = oldPoint;
        }

        if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
            && ((long)p.Y - (long)p1.Y) * (long)(p2.X - p1.X)
              < ((long)p2.Y - (long)p1.Y) * (long)(p.X - p1.X)) {
          inside = !inside;
        }
        oldPoint = newPoint;
      }
      return inside;
    }

    /// <summary>
    /// Get the XNA Rectangle. This simply takes the x, y, width and height and totally ignores the rotation.
    /// Don't use this for rendering, or you'll lose the rotation.
    /// </summary>
    /// <returns>The XNA Rectangle.</returns>
    public Rectangle ToRectangle()
    {
      return new Rectangle((int)x, (int)y, (int)width, (int)height);
    }
  }
}
