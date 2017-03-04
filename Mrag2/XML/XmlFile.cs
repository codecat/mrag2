using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Dynamic;
using System.Diagnostics;

using Mrag2.Extensions;

namespace Mrag2.XML
{
  public class XmlFile
  {
    public XmlTag Decleration { get; set; }
    public XmlTag Root { get; set; }

    public XmlFile()
    {
      Decleration = new XmlTag(null);
      Root = new XmlTag(null);
    }

    public XmlFile(string strFilename)
    {
      Load(strFilename);
    }

    public XmlFile(Stream stream)
    {
      Load(stream);
    }

    public void Load(string strFilename)
    {
      Decleration = new XmlTag(null);
      Root = new XmlTag(null);

      if (!File.Exists(strFilename)) {
        return;
      }

      using (var reader = new StreamReader(File.OpenRead(strFilename))) {
        while (!reader.EndOfStream) {
          char c = reader.ReadChar();

          if (c == '<') {
            if (reader.PeekChar() == '?') {
              Decleration.Parse(reader);
            } else {
              XmlTag newTag = new XmlTag(Root);
              newTag.Parse(reader);
              Root.Children.Add(newTag);
            }
          }
        }
      }
    }

    public void Load(Stream stream)
    {
      Decleration = new XmlTag(null);
      Root = new XmlTag(null);

      using (var reader = new StreamReader(stream)) {
        while (!reader.EndOfStream) {
          char c = reader.ReadChar();

          if (c == '<') {
            if (reader.PeekChar() == '?') {
              Decleration.Parse(reader);
            } else {
              XmlTag newTag = new XmlTag(Root);
              newTag.Parse(reader);
              Root.Children.Add(newTag);
            }
          }
        }
      }
    }

    public void Save(string strFilename)
    {
      //TODO
    }

    private void PrintToConsole(XmlTag root, int depth)
    {
      string strStart = "";
      for (int i = 0; i < depth; i++) {
        strStart += "-- ";
      }

      foreach (XmlTag tag in root.Children) {
        if (tag.IsComment) {
          Console.WriteLine(strStart + "(comment)");
        } else {
          Console.WriteLine(strStart + tag.Name);
        }

        if (tag.Children.Count > 0) {
          PrintToConsole(tag, depth + 1);
        }
      }
    }

    public void PrintToConsole()
    {
      if (Root.Children.Count > 0) {
        PrintToConsole(Root, 0);
      }
    }

    public void IterateAllTags(Action<XmlTag, int> callback)
    {
      IterateAllTags(Root, callback, 0);
    }

    internal void IterateAllTags(XmlTag root, Action<XmlTag, int> callback, int depth)
    {
      foreach (XmlTag tag in root.Children) {
        callback(tag, depth);

        if (tag.Children.Count > 0) {
          IterateAllTags(tag, callback, depth + 1);
        }
      }
    }

    public XmlTag this[string strQuery]
    {
      get { return Root[strQuery]; }
    }

    public static dynamic operator ~(XmlFile file)
    {
      return new XmlFileDynamic(file);
    }
  }

  public class XmlFileDynamic : DynamicObject
  {
    XmlFile file;

    public XmlFileDynamic(XmlFile f)
    {
      file = f;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      result = new XmlTagDynamic(file.Root.FindTagByName(binder.Name));
      return true;
    }
  }
}
