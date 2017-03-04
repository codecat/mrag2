using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mrag2
{
  /// <summary>
  /// Functions to test for library presence.
  /// </summary>
  public static class LibraryChecks
  {
    private class _xna { public _xna() { new Microsoft.Xna.Framework.Color(); } }

    /// <summary>
    /// Test if XNA is installed on this machine.
    /// </summary>
    /// <returns>True if XNA is installed, false if it's not.</returns>
    public static bool XNA()
    {
      try {
        new _xna();
        return true;
      } catch {
        return false;
      }
    }
  }
}
