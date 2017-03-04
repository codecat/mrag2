using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mrag2;
using Mrag2.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mrag2Demo
{
	public class DemoGame : Game
	{
		public GraphicsDeviceManager m_graphics;
		public CustomSpriteBatch m_spriteBatch;

		public DemoGame()
		{
			m_graphics = new GraphicsDeviceManager(this);
			m_graphics.PreferredBackBufferWidth = 640;
			m_graphics.PreferredBackBufferHeight = 480;
			m_graphics.SynchronizeWithVerticalRetrace = false;
			m_graphics.PreferMultiSampling = true;
			m_graphics.ApplyChanges();

			IsMouseVisible = true;

			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			m_spriteBatch = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice), GraphicsDevice);

			Mrag.Initialize(this, m_graphics, m_spriteBatch, GraphicsDevice);

			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			Mrag.Update();

			if (Input.KeyPressed(Keys.Escape)) {
				Exit();
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			m_spriteBatch.Begin();
			GraphicsDevice.Clear(Color.Black);

			double fps = Math.Round(1000.0 / gameTime.ElapsedGameTime.TotalMilliseconds);
			m_spriteBatch.DrawString($"FPS: {fps}", Vector2.Zero);

			GUI.Begin();
			if (GUI.Button("Click me!", 50, 50)) {
				Console.WriteLine("Button was clicked :O");
			}
			GUI.End();

			Mrag.Render();
			m_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
