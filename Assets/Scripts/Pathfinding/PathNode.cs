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
    public bool isInRange;
    public bool isEnemy;
    public bool isFriend;
    public bool isTWall;
    public bool isSWall;
    public bool isHalfHiding;
    public bool isFullHiding;

    public PathNode cameFromNode;

    public PathNode(GridManager<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        
        isWalkeable = true;
        isInRange = false;
        isEnemy = false;
        isFriend = false;
        isTWall = false;
        isSWall = false;
        isHalfHiding = false;
        isFullHiding = false;
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

    public void SetIsInRange(bool isInRange)
    {
        this.isInRange = isInRange;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetIsFriend(bool isFriend)
    {
        this.isFriend = isFriend;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetIsEnemy(bool isEnemy)
    {
        this.isEnemy = isEnemy;
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

    public void SetIsHalfHiding(bool isHalfHiding)
    {
        this.isHalfHiding = isHalfHiding;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetIsFullHiding(bool isFullHiding)
    {
        this.isFullHiding = isFullHiding;
        grid.TriggerGridObjectChanged(x, y);
    }


    public override string ToString()
    {
        //return x + "," + y;
        return "";
    }
}
