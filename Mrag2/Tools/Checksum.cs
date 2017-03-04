using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Mrag2
{
  /// <summary>
  /// Functions to test for file integrity.
  /// </summary>
  public static class Checksum
  {
    /// <summary>
    /// Information about a file.
    /// </summary>
    public class FileInfo
    {
      public string Path;
      public string Hash;
      public string LastWritten;
      public string LastAccessed;
      public string DateCreated;

      public long Size;

      public FileInfo() { }
    }

    /// <summary>
    /// Get the full file information from the given filename, including file hash.
    /// </summary>
    /// <param name="Filename">The filename.</param>
    /// <returns>The info about the file.</returns>
    public static FileInfo GetInfo(string Filename)
    {
      FileInfo f = new FileInfo();

      System.IO.FileInfo Fileinfo = new System.IO.FileInfo(Filename);

      f.Hash = FileMD5(Filename);
      f.DateCreated = Fileinfo.CreationTimeUtc.ToString();
      f.LastAccessed = Fileinfo.LastAccessTimeUtc.ToString();
      f.LastWritten = Fileinfo.LastWriteTimeUtc.ToString();
      f.Size = Fileinfo.Length;

      return f;
    }

    /// <summary>
    /// Calculate the MD5 hash of the given file.
    /// </summary>
    /// <param name="Filename">The filename.</param>
    /// <returns>The MD5 hash in hexadecimal format.</returns>
    public static string FileMD5(String Filename)
    {
      FileStream File = new FileStream(Filename, FileMode.Open);
      MD5 md5 = new MD5CryptoServiceProvider();
      byte[] RetVal = md5.ComputeHash(File);
      File.Close();

      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < RetVal.Length; i++) {
        sb.Append(RetVal[i].ToString("x2"));
      }

      return sb.ToString().ToLower();
    }

    /// <summary>
    /// Calculate the MD5 hash of the given string.
    /// </summary>
    /// <param name="input">The string.</param>
    /// <returns>The MD5 hash in hexadecimal format.</returns>
    public static string StringMD5(string input)
    {
      byte[] hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));

      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < hashBytes.Length; i++) {
        sb.Append(hashBytes[i].ToString("x2"));
      }

      return sb.ToString();
    }
  }
}
