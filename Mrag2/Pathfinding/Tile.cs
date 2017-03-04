using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mrag2.Pathfinding
{
  public class Tile
  {
    public Point til_vPosition = new Point(0, 0);
    public int til_iID = 0;
    public bool til_bSolid = false;

    internal bool til_bStart = false;
    internal bool til_bEnd = false;

    public Tile()
    {
    }

    public Tile(Point vPosition, int iNewID)
    {
      til_vPosition = vPosition;
      til_iID = iNewID;
    }
  }
}
