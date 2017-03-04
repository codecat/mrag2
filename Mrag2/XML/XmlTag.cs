using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Dynamic;
using Mrag2.Extensions;

namespace Mrag2.XML
{
  public class XmlTag
  {
    public string Name { get; set; }
    public string Value { get; set; }
    public bool IsComment { get; set; }

    public XmlTag Parent { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
    public List<XmlTag> Children { get; set; }

    public XmlTag(XmlTag parent)
    {
      Name = "";
      Value = "";
      IsComment = false;

      Parent = parent;
      Attributes = new Dictionary<string, string>();
      Children = new List<XmlTag>();
    }

    internal void Parse(StreamReader fs)
    {
      bool bReadAttributes = true;
      bool bOpenTag = false;

      while (true) {
        if (fs.PeekChar() == '?') {
					// xml decleration is a small exception
					fs.Expect("?xml ");
          bReadAttributes = true;
          break;
        } else if (fs.PeekChar() == '!') {
          // xml comment
          fs.Expect("!--");
          IsComment = true;
          var strValue = new StringBuilder();
          string strEnding = "-->";
          int i = 0;
          while (!fs.EndOfStream) {
            char c = fs.ReadChar();
            if (c == strEnding[i]) {
              i++;
            } else {
              i = 0;
              strValue.Append(c);
            }
            if (i == strEnding.Length) {
              break;
            }
          }
          Value = strValue.ToString();
          return;
        } else {
          // xml tag
          string strName = "";
          // read the name of the tag, and get the character we ended with
          char c = fs.ReadUntil(out strName, '\r', '\n', '\t', ' ', '>', '/');
          Name = strName;
          if (c == '/') {
            // if it's a slash, it's a closed tag
            bReadAttributes = false;
            bOpenTag = false;
          } else if (c == '>') {
            // if it's a end-of-tag character, it's an open tag 
            bReadAttributes = false;
            bOpenTag = true;
          }
          break;
        }
      }

      // read attributes
      if (bReadAttributes) {
        var attrs = XmlHelpers.ParseAttributes(fs);
        Attributes = attrs.attributes;
        bOpenTag = attrs.bOpenTag;
      }

      // if open tag
      if (bOpenTag) {
        // start reading the value
        var strValue = new StringBuilder();

        while (!fs.EndOfStream) {
          char c = fs.ReadChar();
          // check for tag
          if (c == '<') {
            if (fs.PeekChar() == '/') {
							// end of this tag
							fs.Expect("/" + Name + ">");
              break;
            } else {
              // new tag nested in this tag
              XmlTag newTag = new XmlTag(this);
              newTag.Parse(fs);
              Children.Add(newTag);
            }
          } else {
            // add to value
            strValue.Append(c);
          }
        }

        // set value property
        Value = strValue.ToString();
        strValue.Clear();
      }
    }

    public XmlTag FindTagByName(string strName)
    {
      foreach (XmlTag tag in Children) {
        if (tag.Name == strName) {
          return tag;
        }
      }

      foreach (XmlTag tag in Children) {
        if (tag.Children.Count == 0) {
          continue;
        }

        var ret = tag.FindTagByName(strName);
        if (ret != null) {
          return ret;
        }
      }

      return null;
    }

    public XmlTag[] FindTagsByName(string strName)
    {
      var arr = new List<XmlTag>();

      foreach (XmlTag tag in Children) {
        if (tag.Name == strName) {
          arr.Add(tag);
        }
      }

      foreach (XmlTag tag in Children) {
        if (tag.Children.Count == 0) {
          continue;
        }

        arr.AddRange(tag.FindTagsByName(strName));
      }

      return arr.ToArray();
    }

    public XmlTag FindTagByNameAndAttribute(string strName, string strAttrib, string strValue)
    {
      XmlTag[] tags = FindTagsByName(strName);

      foreach (XmlTag tag in tags) {
        if (tag.Attributes.ContainsKey(strAttrib) && tag.Attributes[strAttrib] == strValue) {
          return tag;
        }
      }

      return null;
    }

    public XmlTag FindTagByAttribute(string strAttrib, string strValue)
    {
      foreach (XmlTag tag in Children) {
        if (tag.Attributes.ContainsKey(strAttrib) && tag.Attributes[strAttrib] == strValue) {
          return tag;
        }
      }

      foreach (XmlTag tag in Children) {
        if (tag.Children.Count == 0) {
          continue;
        }

        var ret = tag.FindTagByAttribute(strAttrib, strValue);
        if (ret != null) {
          return ret;
        }
      }

      return null;
    }

    public XmlTag[] FindTagsByAttribute(string strAttrib, string strValue)
    {
      var arr = new List<XmlTag>();

      foreach (XmlTag tag in Children) {
        if (tag.Attributes.ContainsKey(strAttrib) && tag.Attributes[strAttrib] == strValue) {
          arr.Add(tag);
        }
      }

      foreach (XmlTag tag in Children) {
        if (tag.Children.Count == 0) {
          continue;
        }

        arr.AddRange(tag.FindTagsByAttribute(strAttrib, strValue));
      }

      return arr.ToArray();
    }

    public XmlTag this[string strQuery]
    {
      get
      {
        // "test[a=b]"
        string strAttribRegex = @"^([^\[]+)(\[([^=]+)=([^\]]*)\])?$";
        Match match = Regex.Match(strQuery, strAttribRegex);
        Debug.Assert(match.Success);

        string strName = match.Groups[1].Value;
        string strAttrib = match.Groups[3].Value;
        string strValue = match.Groups[4].Value;

        if (strAttrib != "") {
          return FindTagByNameAndAttribute(strName, strAttrib, strValue);
        } else {
          return FindTagByName(strQuery);
        }
      }
    }
  }

  public class XmlTagDynamic : DynamicObject
  {
    XmlTag tag;

    public XmlTagDynamic(XmlTag t)
    {
      tag = t;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      if (binder.Name == "Value") {
        result = tag.Value;
      } else {
        result = new XmlTagDynamic(tag.FindTagByName(binder.Name));
      }
      return true;
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
      if (indexes.Length == 2) {
        if (indexes[0] is string && indexes[1] is string) {
          result = new XmlTagDynamic(tag.Parent.FindTagByNameAndAttribute(tag.Name, (string)indexes[0], (string)indexes[1]));
        } else {
          Debug.Assert(false);
          result = null;
          return false;
        }
      } else if (indexes.Length == 1) {
        if (indexes[0] is int) {
          result = new XmlTagDynamic(tag.Parent.Children[(int)indexes[0]]);
        } else {
          Debug.Assert(false);
          result = null;
          return false;
        }
      } else {
        Debug.Assert(false);
        result = null;
        return false;
      }
      return true;
    }

    public override bool TryConvert(ConvertBinder binder, out object result)
    {
      if (binder.Type == typeof(XmlTag)) {
        result = tag;
        return true;
      } else if (binder.Type == typeof(string)) {
        result = tag.Value;
        return true;
      }

      result = null;
      return false;
    }
  }
}
