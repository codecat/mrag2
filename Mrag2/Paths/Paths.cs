using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mrag2
{
  internal static class Paths
  {
    private static int pathIterator;
    internal static List<Path> aPaths = new List<Path>();

    public static void Add(Path path)
    {
      aPaths.Add(path);
    }

    public static void Remove(Path path)
    {
      aPaths.Remove(path);
      pathIterator--;
    }

    public static void Update()
    {
      foreach (Path path in aPaths) {
        path.Ready = true;
      }

      for (pathIterator = 0; pathIterator < aPaths.Count; pathIterator++) {
        if (!aPaths[pathIterator].Ready) {
          continue;
        }
        aPaths[pathIterator].Update();
      }
    }

    public static void Render()
    {
      for (pathIterator = 0; pathIterator < aPaths.Count; pathIterator++) {
        aPaths[pathIterator].Render();
      }
    }
  }
}
