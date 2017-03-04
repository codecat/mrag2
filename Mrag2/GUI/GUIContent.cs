using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Mrag2.Properties;

namespace Mrag2.GUI
{
  public enum GUIContentBoxType
  {
    Box,
    Button
  }

  public static class GUIContent
  {
    // *** Box ***
    public static Texture2D Box_TopLeft;
    public static Texture2D Box_Top;
    public static Texture2D Box_TopRight;
    public static Texture2D Box_Left;
    public static Texture2D Box_Mid;
    public static Texture2D Box_Right;
    public static Texture2D Box_BottomLeft;
    public static Texture2D Box_Bottom;
    public static Texture2D Box_BottomRight;

    // *** Button ***
    public static Texture2D Button_TopLeft;
    public static Texture2D Button_Top;
    public static Texture2D Button_TopRight;
    public static Texture2D Button_Left;
    public static Texture2D Button_Mid;
    public static Texture2D Button_Right;
    public static Texture2D Button_BottomLeft;
    public static Texture2D Button_Bottom;
    public static Texture2D Button_BottomRight;
    public static Color Button_ColorMiddleBlend;

    public static void LoadDefaultSkin()
    {
      // *** Box ***
      Box_TopLeft = ContentRegister.BitmapToTexture(Resources.Box_TopLeft, "Resource_Box_TopLeft");
      Box_Top = ContentRegister.BitmapToTexture(Resources.Box_Top, "Resource_Box_Top");
      Box_TopRight = ContentRegister.BitmapToTexture(Resources.Box_TopRight, "Resource_Box_TopRight");
      Box_Left = ContentRegister.BitmapToTexture(Resources.Box_Left, "Resource_Box_Left");
      Box_Mid = ContentRegister.BitmapToTexture(Resources.Box_Mid, "Resource_Box_Mid");
      Box_Right = ContentRegister.BitmapToTexture(Resources.Box_Right, "Resource_Box_Right");
      Box_BottomLeft = ContentRegister.BitmapToTexture(Resources.Box_BottomLeft, "Resource_Box_BottomLeft");
      Box_Bottom = ContentRegister.BitmapToTexture(Resources.Box_Bottom, "Resource_Box_Bottom");
      Box_BottomRight = ContentRegister.BitmapToTexture(Resources.Box_BottomRight, "Resource_Box_BottomRight");

      // *** Button ***
      Button_TopLeft = ContentRegister.BitmapToTexture(Resources.Button_TopLeft, "Resource_Button_TopLeft");
      Button_Top = ContentRegister.BitmapToTexture(Resources.Button_Top, "Resource_Button_Top");
      Button_TopRight = ContentRegister.BitmapToTexture(Resources.Button_TopRight, "Resource_Button_TopRight");
      Button_Left = ContentRegister.BitmapToTexture(Resources.Button_Left, "Resource_Button_Left");
      Button_Mid = Mrag.SpriteBatch.Pixel; //ContentRegister.BitmapToTexture(Resource.Button_Mid, "Resource_Button_Mid");
      Button_Right = ContentRegister.BitmapToTexture(Resources.Button_Right, "Resource_Button_Right");
      Button_BottomLeft = ContentRegister.BitmapToTexture(Resources.Button_BottomLeft, "Resource_Button_BottomLeft");
      Button_Bottom = ContentRegister.BitmapToTexture(Resources.Button_Bottom, "Resource_Button_Bottom");
      Button_BottomRight = ContentRegister.BitmapToTexture(Resources.Button_BottomRight, "Resource_Button_BottomRight");
      Button_ColorMiddleBlend = new Color(26, 29, 34);
    }

    /// <summary>
    /// Draw a box from GUI content
    /// </summary>
    public static void DrawBox(Rectangle rect, GUIContentBoxType type)
    {
      switch (type) {
        case GUIContentBoxType.Box: DrawBox(rect, type, Color.White); break;
        case GUIContentBoxType.Button: DrawBox(rect, type, Button_ColorMiddleBlend); break;
      }
    }

    /// <summary>
    /// Draw a box from GUI content
    /// </summary>
    public static void DrawBox(Rectangle rect, GUIContentBoxType type, Color color)
    {
      switch (type) {
        case GUIContentBoxType.Box:
          DrawBox(rect,
            Box_TopLeft, Box_Top, Box_TopRight,
            Box_Left, Box_Mid, Box_Right,
            Box_BottomLeft, Box_Bottom, Box_BottomRight,
            color);
          break;

        case GUIContentBoxType.Button:
          DrawBox(rect,
            Button_TopLeft, Button_Top, Button_TopRight,
            Button_Left, Button_Mid, Button_Right,
            Button_BottomLeft, Button_Bottom, Button_BottomRight,
            color);
          break;
      }
    }

    /// <summary>
    /// Draw a box from GUI content
    /// </summary>
    public static void DrawBox(Rectangle rect,
      Texture2D tl, Texture2D t, Texture2D tr,
      Texture2D l, Texture2D m, Texture2D r,
      Texture2D bl, Texture2D b, Texture2D br,
      Color colMidBlend)
    {
      // content
      int x = rect.X;
      int y = rect.Y;
      int w = rect.Width;
      int h = rect.Height;

      // top left
      int tl_x = x - tl.Width;
      int tl_y = y - tl.Height;
      int tl_w = tl.Width;
      int tl_h = tl.Height;

      // top
      int t_x = x;
      int t_y = y - t.Height;
      int t_w = w;
      int t_h = t.Height;

      // top right
      int tr_x = x + w;
      int tr_y = y - tr.Height;
      int tr_w = tr.Width;
      int tr_h = tr.Height;

      // left
      int l_x = x - l.Width;
      int l_y = y;
      int l_w = l.Width;
      int l_h = h;

      // right
      int r_x = x + w;
      int r_y = y;
      int r_w = r.Width;
      int r_h = h;

      // bottom left
      int bl_x = x - bl.Width;
      int bl_y = y + h;
      int bl_w = bl.Width;
      int bl_h = bl.Height;

      // bottom
      int b_x = x;
      int b_y = y + h;
      int b_w = w;
      int b_h = b.Height;

      // bottom right
      int br_x = x + w;
      int br_y = y + h;
      int br_w = br.Width;
      int br_h = br.Height;

      // top row
      Mrag.SpriteBatch.Draw(tl, new Rectangle(tl_x, tl_y, tl_w, tl_h));
      Mrag.SpriteBatch.Draw(t, new RotRect(t_x, t_y, t_w, t_h), Color.White, false, false, true);
      Mrag.SpriteBatch.Draw(tr, new Rectangle(tr_x, tr_y, tr_w, tr_h));

      // middle row
      Mrag.SpriteBatch.Draw(l, new RotRect(l_x, l_y, l_w, l_h), Color.White, false, false, true);
      Mrag.SpriteBatch.Draw(r, new RotRect(r_x, r_y, r_w, r_h), Color.White, false, false, true);

      // bottom row
      Mrag.SpriteBatch.Draw(bl, new Rectangle(bl_x, bl_y, bl_w, bl_h));
      Mrag.SpriteBatch.Draw(b, new RotRect(b_x, b_y, b_w, b_h), Color.White, false, false, true);
      Mrag.SpriteBatch.Draw(br, new Rectangle(br_x, br_y, br_w, br_h));

      // content
      Mrag.SpriteBatch.Draw(m, new RotRect(x, y, w, h), colMidBlend, false, false, true);
    }
  }
}
