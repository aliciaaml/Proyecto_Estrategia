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
    public bool isEnemy;
    public bool isTeam;
    public bool isTWall;
    public bool isSWall;

    //Plantear fullhiding y half hidding;
    public bool isHiding;
    public PathNode cameFromNode;

    public PathNode(GridManager<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkeable = true;
        isEnemy = false;
        isTeam = false;
        isTWall = false;
        isSWall = false;
        isHiding = false;
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


    public void SetIsTallWall(bool isTWall)
    {
        this.isTWall = isTWall;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetIsSmallWall(bool isSWall)
    {
        this.isSWall = isSWall;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetIsHiding(bool isHiding)
    {
        this.isHiding = isHiding;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        //return x + "," + y;
        return "";
    }
}
