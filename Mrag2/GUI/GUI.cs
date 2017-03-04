using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Mrag2.GUI
{
	public static class GUI
	{
		internal static bool WasTransforming = false;

		public static void Initialize()
		{
			GUIContent.LoadDefaultSkin();
		}

		public static void Begin()
		{
			WasTransforming = Mrag.SpriteBatch.Camera.AllowTransforming;
			Mrag.SpriteBatch.Camera.AllowTransforming = false;
		}

		public static void End()
		{
			Mrag.SpriteBatch.Camera.AllowTransforming = WasTransforming;
		}

		public static bool Button(string text, int x, int y)
		{
			var textSize = Mrag.SpriteBatch.Font.Measure(text);
			var rectButton = new Rectangle(x, y, (int)textSize.X + 16, (int)textSize.Y + 8);

			var hovering = rectButton.Contains(Input.MousePositionScreen);
			var down = Input.MouseButtonDown(MouseButton.Left);
			var pressed = hovering && Input.MouseButtonPressed(MouseButton.Left);

			Color colBlend = GUIContent.Button_ColorMiddleBlend;
			if (down) {
				colBlend = Color.Lerp(colBlend, Color.Black, 0.4f);
			} else if (hovering) {
				colBlend = Color.Lerp(colBlend, Color.White, 0.1f);
			}

			GUIContent.DrawBox(rectButton, GUIContentBoxType.Button, colBlend);
			Mrag.SpriteBatch.DrawString(text, new Vector2(x + 8, y + 4));

			return pressed;
		}
	}
}
