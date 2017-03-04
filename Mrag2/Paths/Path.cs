using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mrag2
{
  /// <summary>
  /// Class used for automated paths, which can be smoothed or a bezier curve.
  /// Example usages:
  /// new Path(MyPosition, MyTarget, 2.0f, (Vector2 vNewPos) => { MyPosition = vNewPos });
  /// new Path(MyPosition, MyTarget, 2.0f, (Vector2 vNewPos) => { MyPosition = vNewPos }) { Bezier = true };
  /// </summary>
  public class Path
  {
    internal bool Ready = false;

    /// <summary>
    /// Which easing mode to use.
    /// </summary>
    public Func<float, float> UseEasing = null;
    /// <summary>
    /// Lerp using PosBezierAnchor.
    /// </summary>
    public bool Bezier = false;

    /// <summary>
    /// The current frame counter.
    /// </summary>
    public int CurrentFrameCount;
    /// <summary>
    /// The total frames to span the movement on.
    /// </summary>
    public int TotalFrameCount;

    /// <summary>
    /// The base position.
    /// </summary>
    public Vector2 Pos1;
    /// <summary>
    /// The position you want to lerp to.
    /// </summary>
    public Vector2 Pos2;
    /// <summary>
    /// The anchor (middle point) to use for bezier curves. (Bezier must be true if you want to use this)
    /// </summary>
    public Vector2 PosBezierAnchor;

    /// <summary>
    /// The update callback function.
    /// </summary>
    public Action<Vector2, Path> OnUpdate;
    /// <summary>
    /// The finished callback function.
    /// </summary>
    public Action<Path> OnFinished;

    /// <summary>
    /// Create a new Path from 2 points, which will automatically be lerped in between using the given frame count and reported in an update callback.
    /// </summary>
    /// <param name="vPos1">The base position.</param>
    /// <param name="vPos2">The position you want to lerp to.</param>
    /// <param name="iTotalFrameCount">The amount of frames to span the movement over.</param>
    /// <param name="onUpdate">The update callback, called every update containing the newly lerped Vector2. This callback can optionally be null.</param>
    /// <param name="onFinished">The finished callback, called when the movement has completed.
    /// This also gives you the Path object itself for reference, but won't be updated anymore as it's removed from the internal Paths collection.
    /// Before this is called, OnUpdate will be called one more time so you can use that to get the last lerped position.</param>
    public Path(Vector2 vPos1, Vector2 vPos2, int iTotalFrameCount, Action<Vector2, Path> onUpdate, Action<Path> onFinished = null)
    {
      Pos1 = vPos1;
      Pos2 = vPos2;

      OnUpdate = onUpdate;
      OnFinished = onFinished;

      TotalFrameCount = iTotalFrameCount;
      UseEasing = Easing.Linear;

      Paths.Add(this);
    }
    /// <summary>
    /// Create a new Path from 2 points, which will automatically be lerped in between using the given TimeSpan and reported in an update callback.
    /// </summary>
    /// <param name="vPos1">The base position.</param>
    /// <param name="vPos2">The position you want to lerp to.</param>
    /// <param name="tsTime">The TimeSpan to span the movement over.</param>
    /// <param name="onUpdate">The update callback, called every update containing the newly lerped Vector2. This callback can optionally be null.</param>
    /// <param name="onFinished">The finished callback, called when the movement has completed.
    /// This also gives you the Path object itself for reference, but won't be updated anymore as it's removed from the internal Paths collection.
    /// Before this is called, OnUpdate will be called one more time so you can use that to get the last lerped position.</param>
    public Path(Vector2 vPos1, Vector2 vPos2, TimeSpan tsTime, Action<Vector2, Path> onUpdate, Action<Path> onFinished = null) :
      this(vPos1, vPos2, (int)System.Math.Ceiling((float)tsTime.TotalMilliseconds / 16.6667f), onUpdate, onFinished) { }
    /// <summary>
    /// Create a new Path from 2 points, which will automatically be lerped in between using the given amount of seconds and reported in an update callback.
    /// </summary>
    /// <param name="vPos1">The base position.</param>
    /// <param name="vPos2">The position you want to lerp to.</param>
    /// <param name="fSeconds">The amount of seconds to span the movement over.</param>
    /// <param name="onUpdate">The update callback, called every update containing the newly lerped Vector2. This callback can optionally be null.</param>
    /// <param name="onFinished">The finished callback, called when the movement has completed.
    /// This also gives you the Path object itself for reference, but won't be updated anymore as it's removed from the internal Paths collection.
    /// Before this is called, OnUpdate will be called one more time so you can use that to get the last lerped position.</param>
    public Path(Vector2 vPos1, Vector2 vPos2, float fSeconds, Action<Vector2, Path> onUpdate, Action<Path> onFinished = null) :
      this(vPos1, vPos2, (int)System.Math.Ceiling(fSeconds / 0.0166667f), onUpdate, onFinished) { }

    private float GetNonSmoothedDelta()
    {
      return (float)CurrentFrameCount / (float)TotalFrameCount;
    }

    /// <summary>
    /// Get the current delta value.
    /// </summary>
    /// <returns>The delta value, in the 0 to 1 range.</returns>
    public float GetCurrentDelta()
    {
      float x = GetNonSmoothedDelta();
      if (UseEasing != null) {
        x = UseEasing(x);
      }
      return x;
    }

    /// <summary>
    /// Get the current position.
    /// </summary>
    /// <returns>Vector2 of the current position.</returns>
    public Vector2 GetCurrentPosition()
    {
      float x = GetCurrentDelta();
      if (Bezier) {
        if (x == 0) {
          return Pos1;
        } else if (x == 1) {
          return Pos2;
        } else {
          Vector2 v1 = Vector2.Lerp(Pos1, PosBezierAnchor, x);
          Vector2 v2 = Vector2.Lerp(PosBezierAnchor, Pos2, x);

          return Vector2.Lerp(v1, v2, x);
        }
      } else {
        return Vector2.Lerp(Pos1, Pos2, x);
      }
    }

    /// <summary>
    /// Modify the time to take for this path to finish. This will also scale the current frame count, so this function is safe to use in mid-movement.
    /// </summary>
    /// <param name="iFrameCount">The amount of frames.</param>
    public void SetTime(int iFrameCount)
    {
      if (iFrameCount == TotalFrameCount) {
        return;
      }

      float fDelta = GetNonSmoothedDelta();
      TotalFrameCount = iFrameCount;
      CurrentFrameCount = (int)System.Math.Round(fDelta * (float)TotalFrameCount);
    }

    /// <summary>
    /// Modify the time to take for this path to finish. This will also scale the current frame count, so this function is safe to use in mid-movement.
    /// </summary>
    /// <param name="tsTime">The amount of time.</param>
    public void SetTime(TimeSpan tsTime)
    {
      SetTime((int)System.Math.Ceiling((float)tsTime.TotalMilliseconds / 16.6667f));
    }

    /// <summary>
    /// Modify the time to take for this path to finish. This will also scale the current frame count, so this function is safe to use in mid-movement.
    /// </summary>
    /// <param name="fSeconds">The amount of seconds.</param>
    public void SetTime(float fSeconds)
    {
      SetTime((int)System.Math.Ceiling(fSeconds / 0.0166667f));
    }

    /// <summary>
    /// Swap Pos1 and Pos2, useful for reverting a movement after is has been finished. Often used in combination with Reset().
    /// </summary>
    public void Swap()
    {
      Vector2 vPosTemp = Pos1;
      Pos1 = Pos2;
      Pos2 = vPosTemp;
    }

    /// <summary>
    /// Reset the path so it will be re-used. This is useful if you simply want to replay the path at OnFinish.
    /// </summary>
    public void Reset()
    {
      if (!Paths.aPaths.Contains(this)) {
        Paths.Add(this);
      }

      Ready = false;
      CurrentFrameCount = 0;
    }

    internal void Update()
    {
      CurrentFrameCount++;

      if (OnUpdate != null) {
        OnUpdate(GetCurrentPosition(), this);
      }

      if (CurrentFrameCount >= TotalFrameCount) {
        Paths.Remove(this);
        if (OnFinished != null) {
          OnFinished(this);
        }
      }
    }

    internal void Render()
    {
      // TODO: Debug draw
    }
  }
}
