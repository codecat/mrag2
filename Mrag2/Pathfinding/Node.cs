using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mrag2.Pathfinding
{
  public class Node
  {
    public Node nod_pParent = null;
    public int nod_iCellIndex = 0;

    public int nod_iF = 0;
    public int nod_iG = 0;
    public int nod_iH = 0;
  }
}
