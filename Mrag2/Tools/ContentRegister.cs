using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework;
using Mrag2.Rendering;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Mrag2
{
  /// <summary>
  /// Static class that can be used for easy cached content.
  /// </summary>
  public class ContentRegister
  {
    public static ContentManager RootContent = null;

    private static Dictionary<string, AnimationSheet> dicAnimationSheets = new Dictionary<string, AnimationSheet>();
    private static Dictionary<string, AnimationSheetCollection> dicAnimationSheetCollections = new Dictionary<string, AnimationSheetCollection>();
		private static Dictionary<string, Texture2D> dicTextures = new Dictionary<string, Texture2D>();
    private static Dictionary<string, SpriteFont> dicFonts = new Dictionary<string, SpriteFont>();
    private static Dictionary<string, BMFont> dicBMFonts = new Dictionary<string, BMFont>();
		private static Dictionary<string, SoundEffect> dicSounds = new Dictionary<string, SoundEffect>();
		private static Dictionary<string, Song> dicSongs = new Dictionary<string, Song>();

		/// <summary>
		/// Default texture to use if a texture can't be found.
		/// </summary>
		public static Texture2D DefaultTexture = null;

    /// <summary>
    /// Default font to use if a font can't be found.
    /// </summary>
    public static SpriteFont DefaultFont = null;

    /// <summary>
    /// Initialize the ContentRegister.
    /// Note: You probably want to call Mrag.Initialize(this) instead in your Game initialization code instead of directly calling this!
    /// </summary>
    /// <param name="content">The XNA ContentManager.</param>
    public static void Initialize(ContentManager content)
    {
      RootContent = content;
    }

    /// <summary>
    /// Perform precaching for all textures found in the game's content directory.
    /// </summary>
    public static void PreCache()
    {
      string[] astrFiles = Directory.GetFiles(RootContent.RootDirectory, "*.*", SearchOption.AllDirectories);
      foreach (string strFile in astrFiles) {
        string strSanitizedFilename = SanitizeInput(strFile);
        Console.Write(strSanitizedFilename);
      }
    }

    private static string SanitizeInput(string strFilename)
    {
      // backslashes to forward slashes
      string ret = strFilename.Replace('\\', '/');

      // accidental "Content/<actual filename>"
      if (ret.StartsWith(RootContent.RootDirectory + "/")) {
        ret = ret.Substring(RootContent.RootDirectory.Length + 1);
      }

      // accidental "<actual filename>.xnb"
      if (ret.EndsWith(".xnb")) {
        ret = ret.Substring(0, ret.Length - 4);
      }

      return ret;
    }

    private static string DicName(string strFilename)
    {
#if WINDOWS
      // on NTFS filenames aren't case sensitive
      return strFilename.ToLower();
#else
      // not Windows, so filenames are probably case sensitive
      return strFilename;
#endif
    }

    /// <summary>
    /// Get an animation sheet from cache or from disk.
    /// Note: You will need seperate AnimationSheetState objects to render these, you can't use the internal states.
    /// </summary>
    /// <param name="strFilename">The content filename to the png.</param>
    /// <returns>The animation sheet.</returns>
    public static AnimationSheet AnimationSheet(string strFilename)
    {
      strFilename = SanitizeInput(strFilename);
      string strDicName = DicName(strFilename);

      if (dicAnimationSheets.ContainsKey(strDicName)) {
        return dicAnimationSheets[strDicName];
      } else {
        AnimationSheet sheet = new AnimationSheet(Mrag.SpriteBatch, "Content/" + strFilename);
        dicAnimationSheets[strDicName] = sheet;
        return sheet;
      }
    }

		/// <summary>
		/// Get an animation sheet collection from cache or from disk.
		/// Note: You will need seperate AnimationSheetState objects to render these, you can't use the internal states.
		/// </summary>
		/// <param name="strFilename">The content filename to the png.</param>
		/// <returns>The animation sheet collection.</returns>
		public static AnimationSheetCollection AnimationSheetCollection(string strFilename)
		{
			strFilename = SanitizeInput(strFilename);
			string strDicName = DicName(strFilename);

			if (dicAnimationSheetCollections.ContainsKey(strDicName)) {
				return dicAnimationSheetCollections[strDicName];
			} else {
				AnimationSheetCollection sheet = new AnimationSheetCollection(Mrag.SpriteBatch, "Content/" + strFilename);
				dicAnimationSheetCollections[strDicName] = sheet;
				return sheet;
			}
		}

    /// <summary>
    /// Get a texture from cache or from disk.
    /// </summary>
    /// <param name="strFilename">The content filename.</param>
    /// <returns>The texture.</returns>
    public static Texture2D Texture(string strFilename)
    {
      strFilename = SanitizeInput(strFilename);
      string strDicName = DicName(strFilename);

      if (dicTextures.ContainsKey(strDicName)) {
        return dicTextures[strDicName];
      } else {
        Texture2D tex = null;
        if (strFilename.EndsWith(".png") || strFilename.EndsWith(".jpg")) {
          // NOTE: loading such files is a LOT slower than loading compiled xnb files!
          string strPath = RootContent.RootDirectory + "/" + strFilename;
          if (!File.Exists(strPath)) {
            return DefaultTexture;
          }
          using (FileStream fs = File.OpenRead(strPath)) {
            tex = Texture2D.FromStream(Mrag.Game.GraphicsDevice, fs);
          }
        } else {
          if (!File.Exists(RootContent.RootDirectory + "/" + strFilename + ".xnb")) {
            return DefaultTexture;
          }
          tex = RootContent.Load<Texture2D>(strFilename);
        }
        dicTextures[strDicName] = tex;
        return tex;
      }
    }

    /// <summary>
    /// Get a font from cache or from disk.
    /// </summary>
    /// <param name="strFilename">The content filename.</param>
    /// <returns>The font.</returns>
    public static SpriteFont Font(string strFilename)
    {
      strFilename = SanitizeInput(strFilename);
      string strDicName = DicName(strFilename);

      if (dicFonts.ContainsKey(strDicName)) {
        return dicFonts[strDicName];
      } else {
        SpriteFont font = null;
        if (!File.Exists(RootContent.RootDirectory + "/" + strFilename + ".xnb")) {
          return DefaultFont;
        }
        font = RootContent.Load<SpriteFont>(strFilename);
        dicFonts[strDicName] = font;
        return font;
      }
    }

		/// <summary>
		/// Get a BMFont from cache or from disk.
		/// </summary>
		/// <param name="strFilename">The content filename.</param>
		/// <returns>The BMFont.</returns>
		public static BMFont BMFont(string strFilename)
		{
			strFilename = SanitizeInput(strFilename);
			string strDicName = DicName(strFilename);

			if (dicBMFonts.ContainsKey(strDicName)) {
				return dicBMFonts[strDicName];
			} else {
				BMFont font = new BMFont(RootContent.RootDirectory + "/" + strFilename);
				dicBMFonts[strDicName] = font;
				return font;
			}
		}

		public static SoundEffect Sound(string strFilename)
		{
			strFilename = SanitizeInput(strFilename);
			string strDicName = DicName(strFilename);

			if (dicBMFonts.ContainsKey(strDicName)) {
				return dicSounds[strDicName];
			} else {
				SoundEffect sound = null;
				using (FileStream fs = File.OpenRead(RootContent.RootDirectory + "/" + strFilename)) {
					sound = SoundEffect.FromStream(fs);
				}
				dicSounds[strDicName] = sound;
				return sound;
			}
		}

		public static Song Music(string strFilename)
		{
			strFilename = SanitizeInput(strFilename);
			string strDicName = DicName(strFilename);

			if (dicBMFonts.ContainsKey(strDicName)) {
				return dicSongs[strDicName];
			} else {
				Song song = Song.FromUri(strDicName, new Uri(RootContent.RootDirectory + "/" + strFilename, UriKind.Relative));
				dicSongs[strDicName] = song;
				return song;
			}
		}

    /// <summary>
    /// Generate a vertical gradient texture, which will also be cached.
    /// </summary>
    /// <param name="col1">The top color.</param>
    /// <param name="col2">The bottom color.</param>
    /// <param name="width">The width of the texture. (You should make this 1 and simply stretch it out horizontally to save your precious memory)</param>
    /// <param name="height">The height of the texture.</param>
    /// <returns>The generated texture.</returns>
    public static Texture2D GradientVertical(Color col1, Color col2, int width, int height)
    {
      // generate a fake filename, merely for caching
      string strFilename = "GradientV{" + col1.R + "," + col1.G + "," + col1.B + "," + col2.R + "," + col2.G + "," + col2.B + "," + width + "," + height + "}";

      if (dicTextures.ContainsKey(strFilename)) {
        return dicTextures[strFilename];
      } else {
        Texture2D tex = new Texture2D(Mrag.Game.GraphicsDevice, width, height);
        Color[] aColors = new Color[width * height];
        for (int y = 0; y < height; y++) {
          float fDelta = (float)y / (float)height;
          Color col = Color.Lerp(col1, col2, fDelta);
          for (int x = 0; x < width; x++) {
            aColors[y * width + x] = col;
          }
        }
        tex.SetData<Color>(aColors);
        dicTextures[strFilename] = tex;
        return tex;
      }
    }

    /// <summary>
    /// Convert a System.Drawing.Bitmap object to a Texture2D.
    /// </summary>
    /// <param name="bmp">The bitmap.</param>
    /// <param name="identifier">An identifier used for caching. (TODO: Remove this and generate a CRC value from the Bitmap's MemoryStream instead)</param>
    /// <returns>The texture.</returns>
    public static Texture2D BitmapToTexture(System.Drawing.Bitmap bmp, string identifier)
    {
      if (dicTextures.ContainsKey(identifier)) {
        return dicTextures[identifier];
      } else {
        Texture2D ret = null;
        using (MemoryStream ms = new MemoryStream()) {
          bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
          ret = Texture2D.FromStream(Mrag.Graphics.GraphicsDevice, ms);
        }
        dicTextures[identifier] = ret;
        return ret;
      }
    }
  }
}
