using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Imaging;

namespace Mrag2
{
  /// <summary>
  /// Static root class, use this for initialization and additional cleanup.
  /// </summary>
  public static class Mrag
  {
    internal static Game Game = null;
    internal static GraphicsDeviceManager Graphics = null;
    internal static CustomSpriteBatch SpriteBatch = null;

    /// <summary>
    /// A random number generator you could use.
    /// </summary>
    public static Random Rnd = new Random();

    /// <summary>
    /// Call this to start initialization of all necessary classes.
    /// </summary>
    public static void Initialize(Game game, GraphicsDeviceManager graphics, CustomSpriteBatch spritebatch, GraphicsDevice graphicsDevice)
    {
      // remember for later
      Game = game;
      Graphics = graphics;
      SpriteBatch = spritebatch;

      // load Mrag.ini if present
      if (File.Exists("Mrag.ini")) {
        Config config = new Config("Mrag.ini");
        if (config.HasGroup("MRAG")) {
          game.IsMouseVisible = config.GetBool("MRAG", "MouseVisible");
          graphics.PreferredBackBufferWidth = config.GetInt("MRAG", "ScreenWidth");
          graphics.PreferredBackBufferHeight = config.GetInt("MRAG", "ScreenHeight");
          graphics.IsFullScreen = config.GetBool("MRAG", "FullScreen");
          graphics.SynchronizeWithVerticalRetrace = config.GetBool("MRAG", "Vsync");
          graphics.ApplyChanges();
        }
      }

      // load the default font from resources
      SpriteBatch.LoadDefaultFont();

      // set up lighting target for spritebatch
      SpriteBatch.lightingTarget = new RenderTarget2D(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

      // initialize individual required components
      ContentRegister.Initialize(game.Content);

      // create pixel for spritebatch
      if (SpriteBatch != null) {
        SpriteBatch.Pixel = new Texture2D(game.GraphicsDevice, 1, 1);
        SpriteBatch.Pixel.SetData<Color>(new Color[] { Color.White });
      }

      // initialize GUI
      GUI.GUI.Initialize();
    }

    /// <summary>
    /// Call this in your game's main Update function.
    /// </summary>
    public static void Update()
    {
      // note: think about the order of these update methods for a second before changing it!!
      Input.Update();
      Paths.Update();
    }

    /// <summary>
    /// Call this after you're done drawing everything and before the spritebatch end.
    /// </summary>
    public static void Render()
    {
      Paths.Render();
    }
  }
}
