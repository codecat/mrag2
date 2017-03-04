using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mrag2
{
  /// <summary>
  /// Derive from this class to make it possible to get hit by using a RayTrace.
  /// </summary>
  public abstract class RayBox
  {
    /// <summary>
    /// The RotRect used for raytracing, make sure you update this as the object moves around.
    /// </summary>
    public RotRect BoxForRayTracing = null;
  }

  /// <summary>
  /// Class to perform a RayTrace. Make sure you derive objects you want to be able to hit from RayBox and add them to the static RayBoxes field in this class.
  /// </summary>
  public class RayTrace
  {
    /// <summary>
    /// Add RayBoxes here.
    /// </summary>
    public static List<RayBox> RayBoxes = new List<RayBox>();

    /// <summary>
    /// Whether the ray has hit something.
    /// </summary>
    public bool Hit;
    /// <summary>
    /// The point at which it has hit something.
    /// </summary>
    public Vector2 HitPoint;
    /// <summary>
    /// The length from the starting point to the point where it hit something.
    /// </summary>
    public float HitLength;
    /// <summary>
    /// The RayBox that it has hit. (You could cast this to any object of your choosing if you derived your objects from this)
    /// </summary>
    public RayBox HitBox;
    /// <summary>
    /// The ray as a Line object.
    /// </summary>
    public Line HitLine;

    /// <summary>
    /// Perform a RayTrace from A to B. The object itself contains the results.
    /// </summary>
    /// <param name="origin">The origin point.</param>
    /// <param name="orientation">The location the ray should go to.</param>
    /// <param name="length">The length of the ray.</param>
    public RayTrace(Vector2 origin, Vector2 orientation, float length)
    {
      Line rayLine = new Line(origin, orientation, length);
      HitPoint = rayLine.B;
      float fClosestBox = float.PositiveInfinity;
      foreach (RayBox box in RayBoxes) {
        // TODO: This could be optimized to not test for boxes outside of the ray's line range.

        Vector2? hitPoint = box.BoxForRayTracing.IntersectsAt(rayLine);
        if (!hitPoint.HasValue) {
          continue;
        }

        float fDistance = Vector2.Distance(origin, hitPoint.Value);
        if (fDistance < fClosestBox) {
          fClosestBox = fDistance;

          Hit = true;
          HitPoint = hitPoint.Value;
          HitLength = fDistance;
          HitBox = box;
        }
      }
      HitLine = new Line(origin, HitPoint);
    }
  }
}
