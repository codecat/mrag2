using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mrag2
{
  /// <summary>
  /// Static class containing some special color values.
  /// </summary>
  public static class SpecialColor
  {
    private static Random rnd = new Random();

    private static int rainbowDelta = 0;
    private static Color spectrumColor(int w)
    {
      float r = 0.0f;
      float g = 0.0f;
      float b = 0.0f;

      w = w % 100;

      if (w < 17) {
        r = -(w - 17.0f) / 17.0f;
        b = 1.0f;
      } else if (w < 33) {
        g = (w - 17.0f) / (33.0f - 17.0f);
        b = 1.0f;
      } else if (w < 50) {
        g = 1.0f;
        b = -(w - 50.0f) / (50.0f - 33.0f);
      } else if (w < 67) {
        r = (w - 50.0f) / (67.0f - 50.0f);
        g = 1.0f;
      } else if (w < 83) {
        r = 1.0f;
        g = -(w - 83.0f) / (83.0f - 67.0f);
      } else {
        r = 1.0f;
        b = (w - 83.0f) / (100.0f - 83.0f);
      }

      return new Color(r, g, b);
    }

    private static float RandomFloat()
    {
      return (float)rnd.NextDouble();
    }

    /// <summary>
    /// Returns a color from the visible color spectrum.
    /// </summary>
    public static Color Rainbow { get { return spectrumColor(rainbowDelta++); } }

    /// <summary>
    /// Returns a completely random color.
    /// </summary>
    public static Color Random { get { return new Color(RandomFloat(), RandomFloat(), RandomFloat()); } }

    /// <summary>
    /// Returns a completely random color with a completely random alpha.
    /// </summary>
    public static Color RandomAlpha { get { return new Color(RandomFloat(), RandomFloat(), RandomFloat(), RandomFloat()); } }

    private static int dashieDelta = 0;
    /// <summary>
    /// Returns a color from Rainbow Dash's mane.
    /// </summary>
    public static Color RainbowDash
    {
      get
      {
        int w = (dashieDelta++ / 2) % 100;
        float x = SpecialMath.SmootherStep((float)(w % 17) / 17.0f);

        if (w < 17) {
          return Color.Lerp(RainbowDashRed, RainbowDashOrange, x);
        } else if (w < 34) {
          return Color.Lerp(RainbowDashOrange, RainbowDashYellow, x);
        } else if (w < 51) {
          return Color.Lerp(RainbowDashYellow, RainbowDashGreen, x);
        } else if (w < 68) {
          return Color.Lerp(RainbowDashGreen, RainbowDashDarkBlue, x);
        } else if (w < 85) {
          return Color.Lerp(RainbowDashDarkBlue, RainbowDashPurple, x);
        } else {
          return Color.Lerp(RainbowDashPurple, RainbowDashRed, x);
        }
      }
    }

    public static Color RainbowDashBlue = new Color(0x9E, 0xDB, 0xF9);
    public static Color RainbowDashRed = new Color(0xEE, 0x41, 0x44);
    public static Color RainbowDashOrange = new Color(0xF3, 0x70, 0x33);
    public static Color RainbowDashYellow = new Color(0xFD, 0xF6, 0xAF);
    public static Color RainbowDashGreen = new Color(0x62, 0xBC, 0x4D);
    public static Color RainbowDashDarkBlue = new Color(0x1E, 0x98, 0xD3);
    public static Color RainbowDashPurple = new Color(0x67, 0x2F, 0x89);

    public static Color VinylScratchWhite = new Color(0xFE, 0xFD, 0xE7);
    public static Color VinylScratchLightBlue = new Color(0x18, 0xE7, 0xE7);
    public static Color VinylScratchBlue = new Color(0x33, 0x66, 0xCC);
  }
}
