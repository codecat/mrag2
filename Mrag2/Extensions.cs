using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mrag2;
using Microsoft.Xna.Framework;
using System.IO;

namespace Mrag2.Extensions
{
  public static class Extensions
  {
    /// <summary>
    /// Prase a string to Vector2 (format: "x,y" or "x y")
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <returns>String as Vector2</returns>
    public static Vector2 ParseVector2(this string value)
    {
      //value x,y
      string[] parse = value.Split(',', ' ');
      return new Vector2(float.Parse(parse[0]), float.Parse(parse[1]));
    }

    /// <summary>
    /// Parse a string to Rectangle (format: "x,y,h,w" or "x y w h")
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <returns>String as Rectangle</returns>
    public static Rectangle ParseRectangle(this string value)
    {
      string[] parse = value.Split(',', ' ');
      return new Rectangle(int.Parse(parse[0]), int.Parse(parse[1]), int.Parse(parse[2]), int.Parse(parse[3]));
    }
  }

  public static class FileStreamExtensions
  {
    public static char ReadChar(this StreamReader fs)
    {
      return (char)fs.Read();
    }

    public static char PeekChar(this StreamReader fs)
    {
      return (char)fs.Peek();
    }

    public static void Seek(this StreamReader fs, int dir)
    {
      fs.BaseStream.Seek(dir, SeekOrigin.Current);
    }

    public static string ReadString(this StreamReader fs, int len)
    {
      char[] arr = new char[len];
      fs.Read(arr, 0, len);
      return new string(arr);
    }

    public static string ReadUntil(this StreamReader fs, params char[] c)
    {
      string ret = "";
      fs.ReadUntil(out ret, c);
      return ret;
    }

    public static char ReadUntil(this StreamReader fs, out string strOut, params char[] c)
    {
      var ret = new StringBuilder();
      char ccc = '\0';
      while (!fs.EndOfStream) {
        char cc = fs.ReadChar();
        if (c.Contains(cc)) {
          ccc = cc;
          break;
        }
        ret.Append(cc);
      }
      strOut = ret.ToString();
      return ccc;
    }

    public static bool Expect(this StreamReader fs, string str)
    {
      return fs.ReadString(str.Length) == str;
    }

    public static bool Expect(this StreamReader fs, char c)
    {
      return fs.ReadChar() == c;
    }
  }
}
