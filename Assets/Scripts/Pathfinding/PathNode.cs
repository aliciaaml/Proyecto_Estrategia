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
    public bool isPlayer;
    public bool isIA;
    public bool isTWall;
    public bool isSWall;
    public bool isHalfHiding;
    public bool isFullHiding;
    public bool isAmmo;

    public PathNode cameFromNode;

    public PathNode(GridManager<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        
        isWalkeable = true;
        isInRange = false;
        isPlayer = false;
        isIA = false;
        isTWall = false;
        isSWall = false;
        isHalfHiding = false;
        isFullHiding = false;
        isAmmo = false;
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

    public void SetIsPlayer(bool isPlayer)
    {
        this.isPlayer = isPlayer;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetIsIA(bool isIA)
    {
        this.isIA = isIA;
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

    public void SetIsAmmo(bool isAmmo)
    {
        this.isAmmo = isAmmo;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        //return x + "," + y;
        return "";
    }
}
