using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Mrag2.Rendering
{
	/// <summary>
	/// Loads and renders fonts generated with BMFont: http://www.angelcode.com/products/bmfont/
	/// </summary>
	public class BMFont
	{
		private BMFontModels.FontFile m_font;
		private List<Texture2D> m_pages = new List<Texture2D>();

		/// <summary>
		/// Additional extra kerning to use for each character.
		/// </summary>
		public int ExtraKerning { get; set; }

		/// <summary>
		/// Load a BMFont from a path to an XML-encoded .fnt file.
		/// </summary>
		/// <param name="filename">Path to the XML file.</param>
		public BMFont(string filename)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(BMFontModels.FontFile));
			TextReader textReader = new StreamReader(filename);
			m_font = (BMFontModels.FontFile)deserializer.Deserialize(textReader);
			textReader.Close();

			foreach (var page in m_font.Pages) {
				Texture2D texture = ContentRegister.Texture(System.IO.Path.GetDirectoryName(filename) + "/" + page.File);
				Debug.Assert(texture != null);
				AddPage(texture);
			}
		}

		/// <summary>
		/// Load a BMFont from a stream, and a single page. This page will be used as the first page. More pages can be added manually.
		/// </summary>
		/// <param name="streamXml">The stream for the XML.</param>
		/// <param name="texturePage">The singular (or just the first) texture page.</param>
		public BMFont(Stream streamXml, System.Drawing.Bitmap texturePage)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(BMFontModels.FontFile));
			TextReader textReader = new StreamReader(streamXml);
			m_font = (BMFontModels.FontFile)deserializer.Deserialize(textReader);
			textReader.Close();

			AddPage(ContentRegister.BitmapToTexture(texturePage, "_BMFont_ctor2_" + m_font.Pages[0].File));
		}

		/// <summary>
		/// Add a new texture page to the font.
		/// </summary>
		/// <param name="texture">The texture.</param>
		public void AddPage(Texture2D texture)
		{
			m_pages.Add(texture);
		}

		/// <summary>
		/// Measure the size of a string based on the font.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>The size in pixels.</returns>
		public Vector2 Measure(string str)
		{
			var ret = new Vector2();

			foreach (char c in str) {
				var fc = m_font.Chars.Find(f => f.ID == c);
				ret.Y = Math.Max(ret.Y, fc.Height);
				ret.X += fc.XAdvance + ExtraKerning;
			}

			return ret;
		}

		/// <summary>
		/// Render some text directly to the spritebatch.
		/// </summary>
		/// <param name="sb">The spritebatch to draw with.</param>
		/// <param name="str">The string.</param>
		/// <param name="pos">The top-left position to draw the text at.</param>
		/// <param name="color">The color to draw the text with.</param>
		public void Render(CustomSpriteBatch sb, string str, Vector2 pos, Color color)
		{
			//TODO: Word wrapping

			int curX = 0;
			int curY = 0;

			foreach (char c in str) {
				var fc = m_font.Chars.Find(f => f.ID == c);
				var texture = m_pages[fc.Page];

				var rectSource = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
				var rectDest = new RotRect(pos.X + curX + fc.XOffset, pos.Y + curY + fc.YOffset, fc.Width, fc.Height);

				sb.Draw(texture, rectSource, rectDest, color, false, false, false);

				curX += fc.XAdvance + ExtraKerning;
			}
		}

		/// <summary>
		/// Render some text directly to the spritebatch with the given alignment.
		/// </summary>
		/// <param name="sb">The spritebatch to draw with.</param>
		/// <param name="str">The string.</param>
		/// <param name="pos">The top-left position to draw the text at.</param>
		/// <param name="color">The color to draw the text with.</param>
		/// <param name="alignH">Horizontal alignment.</param>
		/// <param name="alignV">Vertical alignment.</param>
		public void Render(CustomSpriteBatch sb, string str, Vector2 pos, Color color, TextRenderAlignmentH alignH = TextRenderAlignmentH.Left, TextRenderAlignmentV alignV = TextRenderAlignmentV.Top)
		{
			var size = Measure(str);

			if (alignH == TextRenderAlignmentH.Middle) {
				pos.X += size.X / 2.0f;
			} else if (alignH == TextRenderAlignmentH.Right) {
				pos.X += size.X;
			}

			if (alignV == TextRenderAlignmentV.Middle) {
				pos.Y += size.Y / 2.0f;
			} else if (alignV == TextRenderAlignmentV.Bottom) {
				pos.Y += size.Y;
			}

			Render(sb, str, pos, color);
		}
	}
}

namespace Mrag2.Rendering.BMFontModels
{
	[Serializable]
	[XmlRoot("font")]
	public class FontFile
	{
		[XmlElement("info")]
		public FontInfo Info { get; set; }

		[XmlElement("common")]
		public FontCommon Common { get; set; }

		[XmlArray("pages")]
		[XmlArrayItem("page")]
		public List<FontPage> Pages { get; set; }

		[XmlArray("chars")]
		[XmlArrayItem("char")]
		public List<FontChar> Chars { get; set; }

		[XmlArray("kernings")]
		[XmlArrayItem("kerning")]
		public List<FontKerning> Kernings { get; set; }
	}

	[Serializable]
	public class FontInfo
	{
		[XmlAttribute("face")]
		public String Face { get; set; }

		[XmlAttribute("size")]
		public Int32 Size { get; set; }

		[XmlAttribute("bold")]
		public Int32 Bold { get; set; }

		[XmlAttribute("italic")]
		public Int32 Italic { get; set; }

		[XmlAttribute("charset")]
		public String CharSet { get; set; }

		[XmlAttribute("unicode")]
		public Int32 Unicode { get; set; }

		[XmlAttribute("stretchH")]
		public Int32 StretchHeight { get; set; }

		[XmlAttribute("smooth")]
		public Int32 Smooth { get; set; }

		[XmlAttribute("aa")]
		public Int32 SuperSampling { get; set; }

		private Rectangle _Padding;
		[XmlAttribute("padding")]
		public String Padding
		{
			get
			{
				return _Padding.X + "," + _Padding.Y + "," + _Padding.Width + "," + _Padding.Height;
			}
			set
			{
				String[] padding = value.Split(',');
				_Padding = new Rectangle(Convert.ToInt32(padding[0]), Convert.ToInt32(padding[1]), Convert.ToInt32(padding[2]), Convert.ToInt32(padding[3]));
			}
		}

		private Point _Spacing;
		[XmlAttribute("spacing")]
		public String Spacing
		{
			get
			{
				return _Spacing.X + "," + _Spacing.Y;
			}
			set
			{
				String[] spacing = value.Split(',');
				_Spacing = new Point(Convert.ToInt32(spacing[0]), Convert.ToInt32(spacing[1]));
			}
		}

		[XmlAttribute("outline")]
		public Int32 OutLine { get; set; }
	}

	[Serializable]
	public class FontCommon
	{
		[XmlAttribute("lineHeight")]
		public Int32 LineHeight { get; set; }

		[XmlAttribute("base")]
		public Int32 Base { get; set; }

		[XmlAttribute("scaleW")]
		public Int32 ScaleW { get; set; }

		[XmlAttribute("scaleH")]
		public Int32 ScaleH { get; set; }

		[XmlAttribute("pages")]
		public Int32 Pages { get; set; }

		[XmlAttribute("packed")]
		public Int32 Packed { get; set; }

		[XmlAttribute("alphaChnl")]
		public Int32 AlphaChannel { get; set; }

		[XmlAttribute("redChnl")]
		public Int32 RedChannel { get; set; }

		[XmlAttribute("greenChnl")]
		public Int32 GreenChannel { get; set; }

		[XmlAttribute("blueChnl")]
		public Int32 BlueChannel { get; set; }
	}

	[Serializable]
	public class FontPage
	{
		[XmlAttribute("id")]
		public Int32 ID { get; set; }

		[XmlAttribute("file")]
		public String File { get; set; }
	}

	[Serializable]
	public class FontChar
	{
		[XmlAttribute("id")]
		public Int32 ID { get; set; }

		[XmlAttribute("x")]
		public Int32 X { get; set; }

		[XmlAttribute("y")]
		public Int32 Y { get; set; }

		[XmlAttribute("width")]
		public Int32 Width { get; set; }

		[XmlAttribute("height")]
		public Int32 Height { get; set; }

		[XmlAttribute("xoffset")]
		public Int32 XOffset { get; set; }

		[XmlAttribute("yoffset")]
		public Int32 YOffset { get; set; }

		[XmlAttribute("xadvance")]
		public Int32 XAdvance { get; set; }

		[XmlAttribute("page")]
		public Int32 Page { get; set; }

		[XmlAttribute("chnl")]
		public Int32 Channel { get; set; }
	}

	[Serializable]
	public class FontKerning
	{
		[XmlAttribute("first")]
		public Int32 First { get; set; }

		[XmlAttribute("second")]
		public Int32 Second { get; set; }

		[XmlAttribute("amount")]
		public Int32 Amount { get; set; }
	}
}
