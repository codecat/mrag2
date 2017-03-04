using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Mrag2
{
	public enum MouseButton : int
	{
		Left = 0,
		Right = 1,
		Middle = 2
	}

	/// <summary>
	/// Basic input methods such as keys and mouse.
	/// </summary>
	public static class Input
	{
		private static bool[] keys = new bool[255];
		private static bool[] mouseButtons = new bool[3];

		/// <summary>
		/// The last keyboard state, if you ever need it.
		/// </summary>
		public static KeyboardState KeyboardState;
		/// <summary>
		/// The last mouse state, if you ever need it.
		/// </summary>
		public static MouseState MouseState;

		/// <summary>
		/// The position of the mouse in the world, as a vector.
		/// </summary>
		public static Vector2 MousePositionWorld
		{
			get
			{
				Matrix mat = Matrix.Invert(Mrag.SpriteBatch.GetTransformation());
				return Vector2.Transform(new Vector2(MouseState.X, MouseState.Y), mat);
			}
		}

		/// <summary>
		/// The position of the mouse on the screen.
		/// </summary>
		public static Point MousePositionScreen
		{
			get { return new Point(MouseState.X, MouseState.Y); }
		}

		internal static void Update()
		{
			KeyboardState = Keyboard.GetState();
			MouseState = Mouse.GetState();

			Keys[] pressedKeys = KeyboardState.GetPressedKeys();
			for (int i = 0; i < keys.Length; i++) {
				if (!pressedKeys.Contains((Keys)i)) {
					keys[i] = false;
				}
			}

			if (MouseState.LeftButton == ButtonState.Released) mouseButtons[0] = false;
			if (MouseState.RightButton == ButtonState.Released) mouseButtons[1] = false;
			if (MouseState.MiddleButton == ButtonState.Released) mouseButtons[2] = false;
		}

		/// <summary>
		/// Check if the key is currently down.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>True if the key is down, false if it's up.</returns>
		public static bool KeyDown(Keys key)
		{
			if (!Mrag.Game.IsActive) return false;
			return KeyboardState.IsKeyDown(key);
		}

		/// <summary>
		/// Check if any of the given keys are currently pressed.
		/// </summary>
		/// <param name="keys">The keys.</param>
		/// <returns>True if any of the keys is currently pressed, otherwise false.</returns>
		public static bool AnyKeyDown(params Keys[] keys)
		{
			for (int i = 0; i < keys.Length; i++) {
				if (KeyDown(keys[i])) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Check if the key has been pressed this frame. (By some people this is referred to as a "toggle")
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>True if the key has just been pressed, otherwise false.</returns>
		public static bool KeyPressed(Keys key)
		{
			if (KeyDown(key)) {
				bool bPressed = !keys[(int)key];
				keys[(int)key] = true;
				return bPressed;
			}
			return false;
		}

		/// <summary>
		/// Check if any of the given keys have been pressed this frame. (By some people this is referred to as a "toggle")
		/// </summary>
		/// <param name="keys">The keys.</param>
		/// <returns>True if any of the keys have just been pressed, otherwise false.</returns>
		public static bool AnyKeyPressed(params Keys[] keys)
		{
			for (int i = 0; i < keys.Length; i++) {
				if (KeyPressed(keys[i])) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Make sure that the key is discarded for other listeners.
		/// </summary>
		public static void DiscardKey(Keys key)
		{
			keys[(int)key] = true;
		}

		/// <summary>
		/// Checks if a certain mouse button is currently pressed.
		/// </summary>
		/// <param name="button">Which mouse button to check.</param>
		/// <returns>True if the button is pressed, false if it's not pressed.</returns>
		public static bool MouseButtonDown(MouseButton button)
		{
			if (!Mrag.Game.IsActive) return false;

			switch (button) {
				case MouseButton.Left: return MouseState.LeftButton == ButtonState.Pressed;
				case MouseButton.Right: return MouseState.RightButton == ButtonState.Pressed;
				case MouseButton.Middle: return MouseState.MiddleButton == ButtonState.Pressed;
			}

			return false;
		}

		/// <summary>
		/// Checks if a certain mouse button is pressed this frame. (By some people this is referred to as a "toggle")
		/// </summary>
		/// <param name="button">Which mouse button to check.</param>
		/// <returns>True if the button is pressed, false if it's not pressed.</returns>
		public static bool MouseButtonPressed(MouseButton button)
		{
			if (!Mrag.Game.IsActive) return false;

			if (MouseButtonDown(button)) {
				bool bPressed = !mouseButtons[(int)button];
				mouseButtons[(int)button] = true;
				return bPressed;
			}
			return false;
		}

		/// <summary>
		/// Makes sure that the button is discarded for other listeners.
		/// </summary>
		public static void DiscardMouseButton(int button)
		{
			mouseButtons[button] = true;
		}
	}
}
