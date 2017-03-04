using System;
using System.Reflection;

namespace Mrag2.Hacks
{
  /// <summary>
  /// Class that extends the object class with methods that allow you to interact with private members of said objects.
  /// </summary>
  public static class Hacks
  {
    // Helpers
    private static FieldInfo hackFieldInfo(this object obj, string name)
    {
      return obj.GetType().GetField(name, (BindingFlags)65535);
    }

    private static PropertyInfo hackPropertyInfo(this object obj, string name)
    {
      return obj.GetType().GetProperty(name, (BindingFlags)65535);
    }

    private static MethodInfo hackMethodInfo(this object obj, string name)
    {
      return obj.GetType().GetMethod(name, (BindingFlags)65535);
    }

    // Fields
    public static void HackSetField(this object obj, string name, object value)
    {
      FieldInfo fi = obj.hackFieldInfo(name);
      if (fi != null) fi.SetValue(obj, value);
    }

    public static object HackGetField(this object obj, string name)
    {
      FieldInfo fi = obj.hackFieldInfo(name);
      if (fi != null) return fi.GetValue(obj);
      return null;
    }

    // Properties
    public static void HackSetProperty(this object obj, string name, object value)
    {
      PropertyInfo pi = obj.hackPropertyInfo(name);
      if (pi != null) pi.SetValue(obj, value, null);
    }

    public static void HackSetProperty(this object obj, string name, object value, object index)
    {
      PropertyInfo pi = obj.hackPropertyInfo(name);
      if (pi != null) pi.SetValue(obj, value, new object[] { index });
    }

    public static void HackSetProperty(this object obj, string name, object value, object[] index)
    {
      PropertyInfo pi = obj.hackPropertyInfo(name);
      if (pi != null) pi.SetValue(obj, value, index);
    }

    public static object HackGetProperty(this object obj, string name)
    {
      PropertyInfo pi = obj.hackPropertyInfo(name);
      if (pi != null) return pi.GetValue(obj, null);
      return null;
    }

    public static object HackGetProperty(this object obj, string name, object index)
    {
      PropertyInfo pi = obj.hackPropertyInfo(name);
      if (pi != null) return pi.GetValue(obj, new object[] { index });
      return null;
    }

    public static object HackGetProperty(this object obj, string name, object[] index)
    {
      PropertyInfo pi = obj.hackPropertyInfo(name);
      if (pi != null) return pi.GetValue(obj, index);
      return null;
    }

    // Methods
    public static object HackCallMethod(this object obj, string name, params object[] args)
    {
      MethodInfo mi = obj.hackMethodInfo(name);
      if (mi != null) return mi.Invoke(obj, args);
      return null;
    }
  }
}
