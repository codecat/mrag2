using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Mrag2.Properties;

namespace Mrag2
{
  public class LightSource
  {
    public Vector2 Position = Vector2.Zero;
		public Vector2 Offset = Vector2.Zero;
    public float Falloff = 100.0f;
    public Color Color = Color.White;

    public bool Dynamic = true;
    public bool DynamicDebug = true;

		public int Variation = 0;

    internal void Render(CustomSpriteBatch sb)
    {
			System.Drawing.Bitmap bmp = Resources.Light;
			if (Variation == 1) {
				bmp = Resources.Light2;
			}

			sb.Draw(ContentRegister.BitmapToTexture(bmp, "Resource_Light" + Variation),
        new RotRect(Position + Offset, new Vector2(Falloff)) { anchorInCenter = true },
        Color);

      if (Dynamic) {
        if (DynamicDebug) {

        }
      }
    }
  }
}
