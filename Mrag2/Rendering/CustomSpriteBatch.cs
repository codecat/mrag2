using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using Mrag2.Properties;
using Mrag2.Rendering;

namespace Mrag2
{
	public enum TextRenderAlignmentH { Left, Middle, Right }
	public enum TextRenderAlignmentV { Top, Middle, Bottom }

	/// <summary>
	/// Information about the currently active camera on the CustomSpriteBatch.
	/// </summary>
	public class SpriteBatchCamera
	{
		/// <summary>
		/// Scale of everything.
		/// </summary>
		public float Scale = 1.0f;
		/// <summary>
		/// Rotation of everything.
		/// </summary>
		public float Rotation = 0.0f;
		/// <summary>
		/// Whether to make the camera offset smooth moving.
		/// </summary>
		public bool SmoothCameraOffset = true;
		/// <summary>
		/// The modifier for the camera smooth movement. Higher values make it go faster, while lower values make it go slower.
		/// </summary>
		public float SmoothCameraModifier = 0.04f;
		/// <summary>
		/// The current camera offset. Don't read from this, because it doesn't contain camera smoothed values.
		/// </summary>
		public Vector2 Offset = Vector2.Zero;
		/// <summary>
		/// The actual current camera offset, which also contains the camera smoothed values.
		/// </summary>
		public Vector2 OffsetActual = Vector2.Zero;
		/// <summary>
		/// Allow camera rotating.
		/// </summary>
		public bool AllowRotating = true;
		/// <summary>
		/// Allow camera scaling.
		/// </summary>
		public bool AllowScaling = true;
		/// <summary>
		/// Allow camera offset.
		/// </summary>
		public bool AllowOffset = true;
		/// <summary>
		/// Allow global transforming.
		/// </summary>
		public bool AllowTransforming = true;
		/// <summary>
		/// The amount of shaking.
		/// </summary>
		public float ShakeAmount = 2.0f;
		/// <summary>
		/// The total time that the camera is shaking for.
		/// </summary>
		public int ShakingTime = 0;
		/// <summary>
		/// Time that the screen still have to shake for.
		/// </summary>
		public TimeSpan Shaking = TimeSpan.Zero;

		public void Shake(int ms, float amount)
		{
			ShakingTime = ms;
			Shaking = TimeSpan.FromMilliseconds(ms);
			ShakeAmount = amount;
		}

		public void Update(GameTime gameTime)
		{
			if (SmoothCameraOffset) {
				OffsetActual += (Offset - OffsetActual) * SmoothCameraModifier;
			} else {
				OffsetActual = Offset;
			}

			if (Shaking.TotalMilliseconds > 0) {
				Shaking -= gameTime.ElapsedGameTime;
			}
		}
	}

	/// <summary>
	/// Extended SpriteBatch that allows for camera movements/zooming (using scale) and some other useful features.
	/// </summary>
	public class CustomSpriteBatch
	{
		internal Texture2D Pixel;

		private SpriteBatch spriteBatch;
		private GraphicsDevice graphicsDevice;
		/// <summary>
		/// The original XNA SpriteBatch.
		/// </summary>
		public SpriteBatch Original { get { return spriteBatch; } }

		/// <summary>
		/// Currently active camera information.
		/// </summary>
		public SpriteBatchCamera Camera = new SpriteBatchCamera();
		/// <summary>
		/// The default shading color. Usually Color.White.
		/// </summary>
		public Color Color = Color.White;
		/// <summary>
		/// The default font to use for string rendering.
		/// </summary>
		public BMFont Font;
		/// <summary>
		/// Use lighting system.
		/// </summary>
		/// <seealso cref="LightSources"/>
		public bool AllowLighting = false;
		/// <summary>
		/// Ambient color to use for lighting. You could classify this as a "Shadow color", too.
		/// </summary>
		/// <seealso cref="LightSources"/>
		/// <seealso cref="AllowLighting"/>
		public Color AmbientColor = new Color(50, 50, 50);
		/// <summary>
		/// Disables all lights and shows everything in full bright.
		/// </summary>
		public bool FullBright = false;
		/// <summary>
		/// List with light sources.
		/// </summary>
		/// <seealso cref="AllowLighting"/>
		public List<LightSource> LightSources = new List<LightSource>();
		/// <summary>
		/// Current depth to use for rendering textures.
		/// </summary>
		public float CurrentDepth = 0;

		internal RenderTarget2D lightingTarget;
		private bool beganWithLighting = false;

		/// <summary>
		/// Construct the custom SpriteBatch from a regular XNA SpriteBatch.
		/// </summary>
		/// <param name="spriteBatch">The original XNA SpriteBatch.</param>
		/// <param name="graphicsDevice">The graphics device.</param>
		public CustomSpriteBatch(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
		{
			this.spriteBatch = spriteBatch;
			this.graphicsDevice = graphicsDevice;
		}

		internal void LoadDefaultFont()
		{
			using (var ms = new MemoryStream(Resources.DefaultFont)) {
				using (var page = new System.Drawing.Bitmap(Resources.DefaultFont_0)) {
					Font = new BMFont(ms, page);
				}
			}
			/*
			string strFilename = Mrag.Game.Content.RootDirectory + "/MragTempFont.xnb";
			File.WriteAllBytes(strFilename, Resources.MragFont);
			Font = Mrag.Game.Content.Load<SpriteFont>("MragTempFont");
			File.Delete(strFilename);
			*/
		}

		private void SetRenderTarget(bool lighting)
		{
			if (!lighting) {
				graphicsDevice.SetRenderTarget(null);
				BlendState bs = new BlendState();
				bs.AlphaSourceBlend = Blend.SourceAlpha;
				bs.ColorSourceBlend = Blend.SourceAlpha;
				bs.AlphaDestinationBlend = Blend.InverseSourceAlpha;
				bs.ColorDestinationBlend = Blend.InverseSourceAlpha;
				graphicsDevice.BlendState = bs;
			} else {
				graphicsDevice.SetRenderTarget(lightingTarget);
			}
		}

		/// <summary>
		/// Begin the SpriteBatch.
		/// </summary>
		/// <param name="blendState">Optionally, give a BlendState to use. Leave it null to use AlphaBlend.</param>
		public void Begin(BlendState blendState = null)
		{
			if (Pixel == null) {
				throw new Exception("You forgot to call Mrag.Initialize!");
			}

			if (blendState == null) {
				blendState = BlendState.AlphaBlend;
			}

			spriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointClamp, null, null);

			if (AllowLighting) {
				SetRenderTarget(true);

				graphicsDevice.Clear(AmbientColor);
				foreach (LightSource ls in LightSources) {
					ls.Render(this);
				}

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointClamp, null, null);

				SetRenderTarget(false);
			}

			beganWithLighting = AllowLighting;

			CurrentDepth = 0;
		}

		/// <summary>
		/// End the SpriteBatch.
		/// </summary>
		public void End()
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

			if (beganWithLighting && AllowLighting && !FullBright) {
				BlendState bs = new BlendState();
				bs.AlphaSourceBlend = Blend.DestinationAlpha;
				bs.ColorSourceBlend = Blend.DestinationColor;
				bs.AlphaDestinationBlend = Blend.Zero;
				bs.ColorDestinationBlend = Blend.Zero;
				graphicsDevice.BlendState = bs;
				spriteBatch.Draw(lightingTarget, new Rectangle(0, 0, lightingTarget.Width, lightingTarget.Height), Color.White);
			}

			spriteBatch.End();
		}

		/// <summary>
		/// Get the transformation matrix for things to draw.
		/// </summary>
		/// <returns></returns>
		public Matrix GetTransformation()
		{
			Matrix mat = Matrix.Identity;
			if (Camera.AllowTransforming) {
				if (Camera.AllowRotating) {
					mat *= Matrix.CreateRotationZ(Camera.Rotation);
				}
				if (Camera.AllowScaling) {
					mat *= Matrix.CreateScale(Camera.Scale);
				}
				if (Camera.AllowOffset) {
					mat *= Matrix.CreateTranslation(new Vector3(Camera.OffsetActual, 0));
				}
				if (Camera.Shaking.TotalMilliseconds > 0) {
					float tm = (float)Camera.Shaking.TotalMilliseconds;
					float factor = tm / Camera.ShakingTime;
					float offsetX = (float)Math.Sin(tm * 2.0f * factor) * Camera.ShakeAmount * factor;
					float offsetY = (float)Math.Cos(tm * 2.0f * factor) * Camera.ShakeAmount * factor * 0.5f;
					mat *= Matrix.CreateTranslation(new Vector3(offsetX, offsetY, 0));
				}
			}
			return mat;
		}

		/// <summary>
		/// Draw a texture.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="destinationRectangle">The XNA Rectangle to draw to on the screen in world coordinates.</param>
		public void Draw(Texture2D texture, Rectangle destinationRectangle) { this.Draw(texture, destinationRectangle, this.Color, false, false); }
		/// <summary>
		/// Draw a texture.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="destinationRectangle">The RotRect to draw to on the screen in world coordinates.</param>
		public void Draw(Texture2D texture, RotRect destinationRectangle) { this.Draw(texture, destinationRectangle, this.Color, false, false, false); }
		/// <summary>
		/// Draw a texture with a shading color.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="destinationRectangle">The XNA Rectangle to draw to on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color) { this.Draw(texture, destinationRectangle, color, false, false); }
		/// <summary>
		/// Draw a texture with a shading color.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="destinationRectangle">The RotRect to draw to on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		public void Draw(Texture2D texture, RotRect destinationRectangle, Color color) { this.Draw(texture, destinationRectangle, color, false, false, false); }
		/// <summary>
		/// Draw a texture with a shading color that can be flipped horizontally or vertically.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="destinationRectangle">The XNA Rectangle to draw to on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		/// <param name="flipH">Whether to flip the texture horizontally.</param>
		/// <param name="flipV">Whether to flip the texture veritcally.</param>
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color, bool flipH, bool flipV) { Draw(texture, new RotRect(destinationRectangle), color, flipH, flipV, false); }
		/// <summary>
		/// Draw a texture with a shading color that can be flipped horizontally or vertically.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="destinationRectangle">The RotRect to draw to on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		/// <param name="flipH">Whether to flip the texture horizontally.</param>
		/// <param name="flipV">Whether to flip the texture veritcally.</param>
		public void Draw(Texture2D texture, RotRect destinationRectangle, Color color, bool flipH, bool flipV) { Draw(texture, destinationRectangle, color, flipH, flipV, false); }

		/// <summary>
		/// Draw a texture with a shading color that can be flipped horizontally or vertically, as well as wrapped.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="destinationRectangle">The RotRect to draw to on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		/// <param name="flipH">Whether to flip the texture horizontally.</param>
		/// <param name="flipV">Whether to flip the texture vertically.</param>
		/// <param name="wrapping">Whether to use texture wrapping.</param>
		public void Draw(Texture2D texture, RotRect destinationRectangle, Color color, bool flipH, bool flipV, bool wrapping) { Draw(texture, null, destinationRectangle, color, flipH, flipV, wrapping); }

		/// <summary>
		/// Draw a texture with a shading color that can be flipped horizontally or vertically, as well as wrapped.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="sourceRectangle">The source rectangle.</param>
		/// <param name="destinationRectangle">The RotRect to draw to on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		/// <param name="flipH">Whether to flip the texture horizontally.</param>
		/// <param name="flipV">Whether to flip the texture vertically.</param>
		/// <param name="wrapping">Whether to use texture wrapping.</param>
		public void Draw(Texture2D texture, Rectangle? sourceRectangle, RotRect destinationRectangle, Color color, bool flipH, bool flipV, bool wrapping)
		{
			if (texture == null) {
				return;
			}

			RotRect drawRect = destinationRectangle;

			if (Camera.AllowScaling && Camera.AllowTransforming) {
				drawRect.width *= Camera.Scale;
				drawRect.height *= Camera.Scale;
			}

			SpriteEffects se = SpriteEffects.None;
			if (flipH) { se |= SpriteEffects.FlipHorizontally; }
			if (flipV) { se |= SpriteEffects.FlipVertically; }

			Rectangle rectSrc = Rectangle.Empty;
			if (sourceRectangle.HasValue) {
				rectSrc = sourceRectangle.Value;
			} else {
				if (wrapping) {
					rectSrc = new Rectangle(0, 0, (int)drawRect.width, (int)drawRect.height);
				} else {
					rectSrc = new Rectangle(0, 0, texture.Width, texture.Height);
				}
			}

			Vector2 vOrigin = Vector2.Zero;
			if (destinationRectangle.anchorInCenter) {
				vOrigin = new Vector2(rectSrc.Width / 2.0f, rectSrc.Height / 2.0f);
			}

			Vector2 vTransformedPos = Vector2.Transform(new Vector2(drawRect.x, drawRect.y), GetTransformation());
			drawRect.x = vTransformedPos.X;
			drawRect.y = vTransformedPos.Y;

			float fRot = 0.0f;
			if (Camera.AllowRotating && Camera.AllowTransforming) {
				fRot = Camera.Rotation;
			}

			if (wrapping) {
				this.spriteBatch.Draw(texture, drawRect.ToRectangle(),
					rectSrc, color, drawRect.rotation + fRot, vOrigin, se, CurrentDepth);
			} else {
				this.spriteBatch.Draw(texture, drawRect.ToRectangle(),
					rectSrc, color, drawRect.rotation + fRot, vOrigin, se, CurrentDepth);
			}
		}

		/// <summary>
		/// Draw a texture.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="position">The position to draw on the screen in world coordinates.</param>
		public void Draw(Texture2D texture, Vector2 position)
		{
			this.Draw(texture, position, this.Color);
		}

		/// <summary>
		/// Draw a texture with a shading color.
		/// </summary>
		/// <param name="texture">The texture.</param>
		/// <param name="position">The position to draw on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		public void Draw(Texture2D texture, Vector2 position, Color color)
		{
			Rectangle drawRect = new Rectangle();

			drawRect.X = (int)position.X;
			drawRect.Y = (int)position.Y;

			if (texture == null) {
				return;
			}

			drawRect.Width = texture.Width;
			drawRect.Height = texture.Height;

			this.Draw(texture, drawRect, color);
		}

		/// <summary>
		/// Draw a string.
		/// </summary>
		/// <param name="text">The string.</param>
		/// <param name="position">The position to draw on the screen in world coordinates.</param>
		public void DrawString(string text, Vector2 position)
		{
			Font.Render(this, text, position, Color);
		}

		/// <summary>
		/// Draw a string with a shading color.
		/// </summary>
		/// <param name="text">The string.</param>
		/// <param name="position">The position to draw on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		public void DrawString(string text, Vector2 position, Color color)
		{
			Font.Render(this, text, position, color);
		}

		/// <summary>
		/// Draw a string with a shading color and some alignment.
		/// </summary>
		/// <param name="text">The string.</param>
		/// <param name="position">The position to draw on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		/// <param name="alignH">Horizontal text alignment.</param>
		/// <param name="alignV">Vertical text alignment.</param>
		public void DrawString(string text, Vector2 position, Color color, TextRenderAlignmentH alignH, TextRenderAlignmentV alignV)
		{
			Font.Render(this, text, position, color, alignH, alignV);
		}

		/// <summary>
		/// Draw a string with a shading color and a different font.
		/// </summary>
		/// <param name="spriteFont">The font.</param>
		/// <param name="text">The string.</param>
		/// <param name="position">The position to draw on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
		{
			this.DrawString(spriteFont, text, position, color, 0, TextRenderAlignmentH.Left, TextRenderAlignmentV.Top);
		}

		/// <summary>
		/// Draw a string with a shading color, a different font and some alignment.
		/// </summary>
		/// <param name="spriteFont">The font.</param>
		/// <param name="text">The string.</param>
		/// <param name="position">The position to draw on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		/// <param name="alignH">Horizontal text alignment.</param>
		/// <param name="alignV">Vertical text alignment.</param>
		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, TextRenderAlignmentH alignH, TextRenderAlignmentV alignV)
		{
			this.DrawString(spriteFont, text, position, color, 0, TextRenderAlignmentH.Left, TextRenderAlignmentV.Top);
		}

		/// <summary>
		/// Draw a string with a shading color, a different font, a rotation and some alignment.
		/// </summary>
		/// <param name="spriteFont">The font.</param>
		/// <param name="text">The string.</param>
		/// <param name="position">The position to draw on the screen in world coordinates.</param>
		/// <param name="color">Shading color to use.</param>
		/// <param name="fRot">Rotation of the text.</param>
		/// <param name="alignH">Horizontal text alignment.</param>
		/// <param name="alignV">Vertical text alignment.</param>
		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float fRot, TextRenderAlignmentH alignH, TextRenderAlignmentV alignV)
		{
			Vector2 drawPos = Vector2.Transform(new Vector2((int)position.X, (int)position.Y), GetTransformation());

			if (Camera.AllowRotating && Camera.AllowTransforming) {
				fRot += Camera.Rotation;
			}

			float fScale = 1.0f;
			if (Camera.AllowScaling && Camera.AllowTransforming) {
				fScale = Camera.Scale;
			}

			Vector2 vTextSize = spriteFont.MeasureString(text);

			if (alignH == TextRenderAlignmentH.Middle) {
				drawPos.X -= vTextSize.X / 2f;
			} else if (alignH == TextRenderAlignmentH.Right) {
				drawPos.X -= vTextSize.X;
			}

			if (alignV == TextRenderAlignmentV.Middle) {
				drawPos.Y -= vTextSize.Y / 2f;
			} else if (alignV == TextRenderAlignmentV.Bottom) {
				drawPos.Y -= vTextSize.Y;
			}

			this.spriteBatch.DrawString(spriteFont, text, drawPos, color, fRot, Vector2.Zero, fScale, SpriteEffects.None, 0);
		}

		/// <summary>
		/// Draw a box on the screen.
		/// </summary>
		/// <param name="rect">The RotRect of the box.</param>
		/// <param name="col">The color.</param>
		public void DrawBox(RotRect rect, Color col)
		{
			// we need to draw this using wrapping, otherwise the rotation origin messes up.
			// this is because rect.Width / 2 could be 100 / 2 = 50, but Pixel is always 1x1.
			// this causes XNA to take origin 50x50 using texture 1x1 by fucking up the rotation.
			// if we turn on wrapping, the source rectangle is the drawrect width/height, which solves this problem.
			Draw(Pixel, rect, col, false, false, true);
		}

		/// <summary>
		/// Draw an outlined box on the screen.
		/// </summary>
		/// <param name="rect">The RotRect of the box.</param>
		/// <param name="bordersize">The border size.</param>
		/// <param name="col">The color.</param>
		public void DrawOutlinedBox(RotRect rect, float bordersize, Color col)
		{
			if (bordersize == 0) {
				return;
			}

			bordersize *= Camera.Scale;

			Vector2[] avec = rect.GetRotatedPosArray(); // [topLeft, topRight, botRight, botLeft]
			DrawLine(avec[0], avec[1], bordersize, col); // top left to top right
			DrawLine(avec[1], avec[2], bordersize, col); // top right to bottom right
			DrawLine(avec[0], avec[3], bordersize, col); // top left to bottom left
			DrawLine(avec[2], avec[3], bordersize, col); // bottom left to bottom right
		}

		/// <summary>
		/// Draw a line on the screen.
		/// </summary>
		/// <param name="v1">Starting point.</param>
		/// <param name="v2">Ending point.</param>
		public void DrawLine(Vector2 v1, Vector2 v2)
		{
			DrawLine(v1, v2, 1, Color);
		}

		/// <summary>
		/// Draw a line on the screen.
		/// </summary>
		/// <param name="v1">Starting point.</param>
		/// <param name="v2">Ending point.</param>
		/// <param name="color">The color.</param>
		public void DrawLine(Vector2 v1, Vector2 v2, Color color)
		{
			DrawLine(v1, v2, 1, color);
		}

		/// <summary>
		/// Draw a line on the screen.
		/// </summary>
		/// <param name="v1">Starting point.</param>
		/// <param name="v2">Ending point.</param>
		/// <param name="width">The width.</param>
		/// <param name="color">The color.</param>
		public void DrawLine(Vector2 v1, Vector2 v2, float width, Color color)
		{
			if (width == 0) {
				return;
			}

			float x0, x1, y0, y1;
			x0 = v1.X; y0 = v1.Y;
			x1 = v2.X; y1 = v2.Y;

			width *= Camera.Scale;

			float ang = (float)-System.Math.Atan2(x0 - x1, y0 - y1) - (float)(System.Math.PI / 2);
			float dist = (float)System.Math.Sqrt(((x0 - x1) * (x0 - x1)) + ((y0 - y1) * (y0 - y1)));

			Draw(Pixel, new RotRect(x0, y0, dist, width, ang), color);
		}
	}
}
