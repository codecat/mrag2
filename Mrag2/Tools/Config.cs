using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mrag2
{
  /// <summary>
  /// Simple parsing of ini file-formats.
  /// </summary>
  public class Config
  {
    private Dictionary<string, Dictionary<string, string>> dicValues = new Dictionary<string, Dictionary<string, string>>();

    /// <summary>
    /// Empty Config.
    /// </summary>
    public Config() { }
    /// <summary>
    /// Load a Config from a filename.
    /// </summary>
    /// <param name="strFilename"></param>
    public Config(string strFilename)
    {
      if (!File.Exists(strFilename)) {
        return;
      }

      LoadFrom(strFilename);
    }

    /// <summary>
    /// Save all config values to a file.
    /// </summary>
    /// <param name="strFilename">The filename.</param>
    public void SaveTo(string strFilename)
    {
      if (File.Exists(strFilename)) {
        File.Move(strFilename, strFilename + ".bak");
      }

      using (FileStream fs = File.OpenWrite(strFilename)) {
        SaveTo(fs);
      }

      if (File.Exists(strFilename + ".bak")) {
        File.Delete(strFilename + ".bak");
      }
    }

    /// <summary>
    /// Load config values from a file.
    /// </summary>
    /// <param name="strFilename">The filename.</param>
    public void LoadFrom(string strFilename)
    {
      using (FileStream fs = File.OpenRead(strFilename)) {
        LoadFrom(fs);
      }
    }

    /// <summary>
    /// Save all config values to a stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void SaveTo(Stream stream)
    {
      StreamWriter writer = new StreamWriter(stream);

      foreach (string strGroup in dicValues.Keys) {
        writer.WriteLine("[" + strGroup + "]");
        foreach (string strKey in dicValues[strGroup].Keys) {
          writer.WriteLine(strKey + " = " + dicValues[strGroup][strKey]);
        }
        writer.WriteLine();
      }
    }

    /// <summary>
    /// Load config values from a stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void LoadFrom(Stream stream)
    {
      StreamReader reader = new StreamReader(stream);

      string strCurrentGroup = "";

      // create global group
      if (!dicValues.ContainsKey("")) {
        dicValues[""] = new Dictionary<string, string>();
      }

      while (!reader.EndOfStream) {
        string strLine = reader.ReadLine().TrimStart(' ');
        if (strLine.Length == 0 || strLine[0] == '#' || strLine[0] == ';') {
          continue;
        }

        if (strLine[0] == '[') {
          strCurrentGroup = strLine.Trim().Substring(1);
          strCurrentGroup = strCurrentGroup.Substring(0, strCurrentGroup.Length - 1);

          if (!dicValues.ContainsKey(strCurrentGroup)) {
            dicValues[strCurrentGroup] = new Dictionary<string, string>();
          }
        } else {
          string[] astrParse = strLine.Split('=');

          string strKey = astrParse[0].Trim();
          string strValue = astrParse[1];

          // only trim the first space (intended behaviour)
          if (strValue[0] == ' ') {
            strValue = strValue.Substring(1);
          }

          dicValues[strCurrentGroup][strKey] = strValue;
        }
      }
    }

    /// <summary>
    /// Tests if the given group exists.
    /// </summary>
    /// <param name="strGroup">The group name to test.</param>
    /// <returns>True if it exists, false if it doesn't.</returns>
    public bool HasGroup(string strGroup)
    {
      return dicValues.ContainsKey(strGroup);
    }

    /// <summary>
    /// Tests if the given group and key combination exists.
    /// </summary>
    /// <param name="strGroup">The group name to test.</param>
    /// <param name="strKey">The key name to test.</param>
    /// <returns>True if it exists, false if it doesn't.</returns>
    public bool Exists(string strGroup, string strKey)
    {
      return HasGroup(strGroup) && dicValues[strGroup].ContainsKey(strKey);
    }

    /// <summary>
    /// Get a string config value.
    /// </summary>
    /// <param name="strGroup">The group name.</param>
    /// <param name="strKey">The key name.</param>
    /// <returns>A string value of the requested config value.</returns>
    public string GetString(string strGroup, string strKey)
    {
      if (!Exists(strGroup, strKey)) {
        return "";
      }
      return dicValues[strGroup][strKey];
    }

    /// <summary>
    /// Get an integer config value.
    /// </summary>
    /// <param name="strGroup">The group name.</param>
    /// <param name="strKey">The key name.</param>
    /// <returns>An integer value of the requested config value.</returns>
    public int GetInt(string strGroup, string strKey)
    {
      if (!Exists(strGroup, strKey)) {
        return 0;
      }
      int ret = 0;
      int.TryParse(dicValues[strGroup][strKey], out ret);
      return ret;
    }

    /// <summary>
    /// Get a float config value.
    /// </summary>
    /// <param name="strGroup">The group name.</param>
    /// <param name="strKey">The key name.</param>
    /// <returns>A float value of the requested config value.</returns>
    public float GetFloat(string strGroup, string strKey)
    {
      if (!Exists(strGroup, strKey)) {
        return 0.0f;
      }
      float ret = 0;
      float.TryParse(dicValues[strGroup][strKey], out ret);
      return ret;
    }

    /// <summary>
    /// Get a double config value.
    /// </summary>
    /// <param name="strGroup">The group name.</param>
    /// <param name="strKey">The key name.</param>
    /// <returns>A double value of the requested config value.</returns>
    public double GetDouble(string strGroup, string strKey)
    {
      if (!Exists(strGroup, strKey)) {
        return 0.0d;
      }
      double ret = 0;
      double.TryParse(dicValues[strGroup][strKey], out ret);
      return ret;
    }

    /// <summary>
    /// Get a boolean config value.
    /// </summary>
    /// <param name="strGroup">The group name.</param>
    /// <param name="strKey">The key name.</param>
    /// <returns>A boolean value of the requested config value.</returns>
    public bool GetBool(string strGroup, string strKey)
    {
      if (!Exists(strGroup, strKey)) {
        return false;
      }
      return dicValues[strGroup][strKey].ToLower() == "true";
    }

    /// <summary>
    /// Get the group dictionary.
    /// </summary>
    /// <param name="strGroup">The group name.</param>
    /// <returns>The dictionary of the group.</returns>
    public Dictionary<string, string> this[string strGroup]
    {
      get { return dicValues[strGroup]; }
    }
  }
}
