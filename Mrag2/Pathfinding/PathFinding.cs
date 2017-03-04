using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

// Ported from the following code base: https://bitbucket.org/kc3w/a-algorithm

namespace Mrag2.Pathfinding
{
  public class PathFinding
  {
    public List<Tile> pth_saTiles = new List<Tile>();
    public bool pth_bDiagonal = false;
    public Point pth_vMapSize = new Point(0, 0);

    private List<Node> pth_saOpenList = new List<Node>();
    private List<Node> pth_saClosedList = new List<Node>();
    private Node pth_pnodStart = null;
    private Node pth_pnodEnd = null;
    private Node pth_pnodCurrent = null;
    private int pth_iAddCounter = 0;

    public PathFinding()
    {
    }

    public PathFinding(int width, int height)
    {
      pth_vMapSize = new Point(width, height);
    }

    public void AddTile(Point vPos, bool bSolid)
    {
      Tile tile = new Tile();
      tile.til_vPosition = vPos;
      tile.til_bSolid = bSolid;
      tile.til_iID = pth_iAddCounter++;
      pth_saTiles.Add(tile);
    }

    public bool FindPath(ref List<Point> ret, Point vStart, Point vEnd, int iMaxTiles = 0)
    {
      pth_saOpenList.Clear();
      pth_saClosedList.Clear();

      if (pth_saTiles.Count == 0) {
        // nothing to do
        Debug.Assert(false);
        return false;
      }

      if (vStart == vEnd) {
        // nothing to do
        Debug.Assert(false);
        return false;
      }

      for (int i = 0; i < pth_saTiles.Count; i++) {
        if (pth_saTiles[i].til_vPosition == vStart) {
          pth_pnodStart = new Node();
          pth_pnodStart.nod_iCellIndex = pth_saTiles[i].til_iID;
          pth_saOpenList.Add(pth_pnodStart);
          pth_saTiles[i].til_bStart = true;
        } else if (pth_saTiles[i].til_vPosition == vEnd) {
          pth_pnodEnd = new Node();
          pth_pnodEnd.nod_iCellIndex = pth_saTiles[i].til_iID;
          pth_saTiles[i].til_bEnd = true;
        }
      }

      Debug.Assert(pth_pnodStart != null);
      Debug.Assert(pth_pnodEnd != null);

      bool bFound = false;

      while (pth_saOpenList.Count > 0) {
        pth_pnodCurrent = FindSmallestF();
        if (pth_pnodCurrent == null) {
          Debug.Assert(false);
          break;
        }

        if (pth_pnodCurrent.nod_iCellIndex == pth_pnodEnd.nod_iCellIndex) {
          bFound = true;
          break;
        }

        pth_saOpenList.Remove(pth_pnodCurrent);
        pth_saClosedList.Add(pth_pnodCurrent);

        if (pth_bDiagonal) {
          AddAdjacentCellToOpenList(pth_pnodCurrent, 1, -1, 14);
          AddAdjacentCellToOpenList(pth_pnodCurrent, -1, -1, 14);
          AddAdjacentCellToOpenList(pth_pnodCurrent, -1, 1, 14);
          AddAdjacentCellToOpenList(pth_pnodCurrent, 1, 1, 14);
        }
        AddAdjacentCellToOpenList(pth_pnodCurrent, 0, -1, 14);
        AddAdjacentCellToOpenList(pth_pnodCurrent, -1, 0, 14);
        AddAdjacentCellToOpenList(pth_pnodCurrent, 1, 0, 14);
        AddAdjacentCellToOpenList(pth_pnodCurrent, 0, 1, 14);
      }

      while (pth_pnodCurrent != null) {
        if (pth_pnodStart.nod_iCellIndex == pth_pnodCurrent.nod_iCellIndex) {
          break;
        }
        ret.Insert(0, pth_saTiles[pth_pnodStart.nod_iCellIndex].til_vPosition);
        pth_pnodCurrent = pth_pnodCurrent.nod_pParent;
      }

      return bFound;
    }

    private Node FindSmallestF()
    {
      int smallestF = 999999; // eh.
      Node ret = null;

      foreach (Node node in pth_saOpenList) {
        if (node.nod_iF < smallestF) {
          ret = node;
          smallestF = node.nod_iF;
        }
      }

      return ret;
    }

    private void AddAdjacentCellToOpenList(Node parentNode, int columnOffset, int rowOffset, int gCost)
    {
      Debug.Assert(parentNode != null);

      int adjacentCellIndex = GetAdjacentCellIndex(parentNode.nod_iCellIndex, columnOffset, rowOffset);

      // ignore unwalkable nodes (or nodes outside the grid)
      if (adjacentCellIndex == -1) {
        return;
      }

      // ignore nodes on the closed list
      if (pth_saClosedList.FindIndex((Node node) => node.nod_iCellIndex == adjacentCellIndex) != -1) {
        return;
      }

      int iAdjacentNodeIndex = pth_saOpenList.FindIndex((Node node) => node.nod_iCellIndex == adjacentCellIndex);
      if (iAdjacentNodeIndex != -1) {
        Node adjacentNode = pth_saOpenList[iAdjacentNodeIndex];
        if (parentNode.nod_iG + gCost < adjacentNode.nod_iG) {
          adjacentNode.nod_pParent = parentNode;
          adjacentNode.nod_iG = parentNode.nod_iG + gCost;
          adjacentNode.nod_iF = adjacentNode.nod_iG + adjacentNode.nod_iH;
        }
        return;
      }

      Node newNode = new Node();
      newNode.nod_iCellIndex = adjacentCellIndex;
      newNode.nod_pParent = parentNode;
      newNode.nod_iG = gCost;
      newNode.nod_iH = GetDistance(adjacentCellIndex, pth_pnodEnd.nod_iCellIndex);
      newNode.nod_iF = newNode.nod_iG + newNode.nod_iH;
      pth_saOpenList.Add(newNode);
    }

    private int GetAdjacentCellIndex(int cellIndex, int columnOffset, int rowOffset)
    {
      int x = cellIndex % pth_vMapSize.X;
      int y = cellIndex / pth_vMapSize.X;

      if ((x + columnOffset < 0 || x + columnOffset > pth_vMapSize.X - 1) ||
         (y + rowOffset < 0 || y + rowOffset > pth_vMapSize.Y - 1)) {
        return -1;
      }

      if (pth_saTiles[((y + rowOffset) * pth_vMapSize.X) + (x + columnOffset)].til_bSolid) {
        return -1;
      }

      return cellIndex + columnOffset + (rowOffset * pth_vMapSize.X);
    }

    private int GetDistance(int startTileID, int endTileID)
    {
      Tile tileStart = pth_saTiles[startTileID];
      int startX = tileStart.til_vPosition.X / pth_vMapSize.X;
      int startY = tileStart.til_vPosition.Y / pth_vMapSize.Y;

      Tile tileEnd = pth_saTiles[endTileID];
      int endX = tileEnd.til_vPosition.X / pth_vMapSize.X;
      int endY = tileEnd.til_vPosition.Y / pth_vMapSize.Y;

      return System.Math.Abs(startX - endX) + System.Math.Abs(startY - endY);
    }
  }
}
