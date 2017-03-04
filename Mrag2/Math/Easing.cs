using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mrag2
{
  public static class Easing
  {
    public static Func<float, float> InOutRandom()
    {
      int iFunction = Mrag.Rnd.Next(0, 10);
      switch (iFunction) {
        default: return Linear;
        case 0: return InOutQuad;
        case 1: return InOutCubic;
        case 2: return InOutQuart;
        case 3: return InOutQuint;
        case 4: return InOutSine;
        case 5: return InOutExpo;
        case 6: return InOutCircle;
        case 7: return InOutElastic;
        case 8: return InOutBack;
        case 9: return InOutBounce;
      }
    }

    public static float Linear(float x)
    {
      return x;
    }

    public static float InQuad(float x)
    {
      return x * x;
    }

    public static float OutQuad(float x)
    {
      return -x * (x - 2);
    }

    public static float InOutQuad(float x)
    {
      if ((x /= 0.5f) < 1) return 0.5f * x * x;
      return -0.5f * ((--x) * (x - 2) - 1);
    }

    public static float InCubic(float x)
    {
      return x * x * x;
    }

    public static float OutCubic(float x)
    {
      return (x -= 1) * x * x + 1;
    }

    public static float InOutCubic(float x)
    {
      if ((x /= 0.5f) < 1) return 0.5f * x * x * x;
      return 0.5f * ((x -= 2) * x * x + 2);
    }

    public static float InQuart(float x)
    {
      return x * x * x * x;
    }

    public static float OutQuart(float x)
    {
      return -((x -= 1) * x * x * x - 1);
    }

    public static float InOutQuart(float x)
    {
      if ((x /= 0.5f) < 1) return 0.5f * x * x * x * x;
      return -0.5f * ((x -= 2) * x * x * x - 2);
    }

    public static float InQuint(float x)
    {
      return x * x * x * x * x;
    }

    public static float OutQuint(float x)
    {
      return (x -= 1) * x * x * x * x + 1;
    }

    public static float InOutQuint(float x)
    {
      if ((x /= 0.5f) < 1) return 0.5f * x * x * x * x * x;
      return 0.5f * ((x -= 2) * x * x * x * x + 2);
    }

    public static float InSine(float x)
    {
      return -(float)System.Math.Cos(x * (System.Math.PI / 2)) + 1;
    }

    public static float OutSine(float x)
    {
      return (float)System.Math.Sin(x * (System.Math.PI / 2));
    }

    public static float InOutSine(float x)
    {
      return -0.5f * ((float)System.Math.Cos(System.Math.PI * x) - 1);
    }

    public static float InExpo(float x)
    {
      return (x == 0) ? 0 : (float)System.Math.Pow(2, 10 * (x - 1));
    }

    public static float OutExpo(float x)
    {
      return (x == 1) ? 1 : -(float)System.Math.Pow(2, -10 * x) + 1;
    }

    public static float InOutExpo(float x)
    {
      if (x == 0) return 0;
      if (x == 1) return 1;
      if ((x /= 0.5f) < 1) return 0.5f * (float)System.Math.Pow(2, 10 * (x - 1));
      return 0.5f * (-(float)System.Math.Pow(2, -10 * --x) + 2);
    }

    public static float InCircle(float x)
    {
      return -((float)System.Math.Sqrt(1 - x * x) - 1);
    }

    public static float OutCircle(float x)
    {
      return (float)System.Math.Sqrt(1 - (x -= 1) * x);
    }

    public static float InOutCircle(float x)
    {
      if ((x /= 0.5f) < 1) return -0.5f * ((float)System.Math.Sqrt(1 - x * x) - 1);
      return 0.5f * ((float)System.Math.Sqrt(1 - (x -= 2) * x) + 1);
    }

    public static float InElastic(float x)
    {
      float s = 0.075f;
      if (x == 0) return 0;
      if (x == 1) return 1;
      return -((float)System.Math.Pow(2, 10 * (x -= 1)) * (float)System.Math.Sin((x - s) * (2 * System.Math.PI) / 0.3f));
    }

    public static float OutElastic(float x)
    {
      float s = 0.075f;
      if (x == 0) return 0;
      if (x == 1) return 1;
      return (float)System.Math.Pow(2, -10 * x) * (float)System.Math.Sin((x - s) * (2 * System.Math.PI) / 0.3f) + 1;
    }

    public static float InOutElastic(float x)
    {
      float s = 0.1125f;
      if (x == 0) return 0;
      if ((x /= 0.5f) == 2) return 1;
      if (x < 1) return -0.5f * ((float)System.Math.Pow(2, 10 * (x -= 1)) * (float)System.Math.Sin((x - s) * (2 * System.Math.PI) / 0.45f));
      return (float)System.Math.Pow(2, -10 * (x -= 1)) * (float)System.Math.Sin((x - s) * (2 * System.Math.PI) / 0.45f) * 0.5f + 1;
    }

    public static float InBack(float x)
    {
      float s = 1.70158f;
      return x * x * ((s + 1) * x - s);
    }

    public static float OutBack(float x)
    {
      float s = 1.70158f;
      return (x -= 1) * x * ((s + 1) * x + s) + 1;
    }

    public static float InOutBack(float x)
    {
      float s = 1.70158f;
      if ((x /= 0.5f) < 1) return 0.5f * (x * x * (((s *= 1.525f) + 1) * x - s));
      return 0.5f * ((x -= 2) * x * (((s *= 1.525f) + 1) * x + s) + 2);
    }

    public static float InBounce(float x)
    {
      return 1 - OutBounce(1 - x);
    }

    public static float OutBounce(float x)
    {
      if (x < (1 / 2.75)) {
        return 7.5625f * x * x;
      } else if (x < (2 / 2.75)) {
        return 7.5625f * (x -= (1.5f / 2.75f)) * x + 0.75f;
      } else if (x < (2.5 / 2.75)) {
        return 7.5625f * (x -= (2.25f / 2.75f)) * x + 0.9375f;
      } else {
        return 7.5625f * (x -= (2.625f / 2.75f)) * x + 0.984375f;
      }
    }

    public static float InOutBounce(float x)
    {
      if (x < 0.5f) return InBounce(x * 2) * 0.5f;
      return OutBounce(x * 2 - 1) * 0.5f + 0.5f;
    }
  }
}
