using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridManager<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkeable;
    public PathNode cameFromNode;

    public PathNode(GridManager<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkeable = true;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkeable(bool isWalkeable)
    {
        this.isWalkeable = isWalkeable;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
