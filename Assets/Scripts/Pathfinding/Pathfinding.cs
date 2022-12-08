using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public static Pathfinding Instance {get; private set;}
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private GridManager<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public Pathfinding(int width, int height)
    {
        Instance = this;
        grid = new GridManager<PathNode>(width, height, 10f, Vector3.zero, (GridManager<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    public GridManager<PathNode> GetGrid()
    {
        return grid;
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition){

        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }

        else
        {
            List<Vector3> vectorPath = new List<Vector3>();

            foreach(PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x,pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    public List<Vector3> FindSecondPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {

        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindSecondPath(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }

        else
        {
            List<Vector3> vectorPath = new List<Vector3>();

            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeigth(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                //LLega al ultimo nodo
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                if (!neighbourNode.isWalkeable || !neighbourNode.isInRange)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    public List<PathNode> FindSecondPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeigth(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                //LLega al ultimo nodo
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;

                if (!neighbourNode.isWalkeable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    public List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            //Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            //Left Up
            if (currentNode.y + 1 < grid.GetHeigth()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        if (currentNode.x + 1 < grid.GetWidth())
        {
            //Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            //Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            //Right Up
            if (currentNode.y + 1 < grid.GetHeigth()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        //Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));

        //Up
        if (currentNode.y + 1 < grid.GetHeigth()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public List<PathNode> GetNeighbourListAmmo(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        int i = Random.Range(0, 8);

        if (currentNode.x - 1 >= 0)
        {
            //Left
            if (i == 0)
                neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            //Left Down
            if (i == 1)
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            //Left Up
            if (i == 2)
                if (currentNode.y + 1 < grid.GetHeigth()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        if (currentNode.x + 1 < grid.GetWidth())
        {
            //Right
            if (i == 3)
                neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            //Right Down
            if (i == 4)
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            //Right Up
            if (i == 5)
                if (currentNode.y + 1 < grid.GetHeigth()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        //Down
        if (i == 6)
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));

        //Up
        if (i == 7)
            if (currentNode.y + 1 < grid.GetHeigth()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public List<PathNode> GetRangeList(PathNode currentNode)
    {
        List<PathNode> rangeList = new List<PathNode>();

        //LEFT
        if (currentNode.x - 1 >= 0)
        {
            rangeList.Add(GetNode(currentNode.x - 1, currentNode.y));
            if (currentNode.y - 1 >= 0) rangeList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        if (currentNode.x - 2 >= 0)
        {
            rangeList.Add(GetNode(currentNode.x - 2, currentNode.y));
            if (currentNode.y - 1 >= 0) rangeList.Add(GetNode(currentNode.x - 2, currentNode.y - 1));
            if (currentNode.y - 2 >= 0) rangeList.Add(GetNode(currentNode.x - 2, currentNode.y - 2));
            if (currentNode.y + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x - 2, currentNode.y + 1));
            if (currentNode.y + 2 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x - 2, currentNode.y + 2));
        }

        if (currentNode.x - 3 >= 0)
        {
            rangeList.Add(GetNode(currentNode.x - 3, currentNode.y));
            if (currentNode.y - 1 >= 0) rangeList.Add(GetNode(currentNode.x - 3, currentNode.y - 1));
            if (currentNode.y - 2 >= 0) rangeList.Add(GetNode(currentNode.x - 3, currentNode.y - 2));
            if (currentNode.y - 3 >= 0) rangeList.Add(GetNode(currentNode.x - 3, currentNode.y - 3));
            if (currentNode.y + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x - 3, currentNode.y + 1));
            if (currentNode.y + 2 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x - 3, currentNode.y + 2));
            if (currentNode.y + 3 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x - 3, currentNode.y + 3));
        }

        //RIGHT
        if (currentNode.x + 1 < grid.GetWidth())
        {
            rangeList.Add(GetNode(currentNode.x + 1, currentNode.y));
            if (currentNode.y - 1 >= 0) rangeList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        if (currentNode.x + 2 < grid.GetWidth())
        {
            rangeList.Add(GetNode(currentNode.x + 2, currentNode.y));
            if (currentNode.y - 1 >= 0) rangeList.Add(GetNode(currentNode.x + 2, currentNode.y - 1));
            if (currentNode.y - 2 >= 0) rangeList.Add(GetNode(currentNode.x + 2, currentNode.y - 2));
            if (currentNode.y + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 2, currentNode.y + 1));
            if (currentNode.y + 2 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 2, currentNode.y + 2));
        }

        if (currentNode.x + 3 < grid.GetWidth())
        {
            rangeList.Add(GetNode(currentNode.x + 3, currentNode.y));
            if (currentNode.y - 1 >= 0) rangeList.Add(GetNode(currentNode.x + 3, currentNode.y - 1));
            if (currentNode.y - 2 >= 0) rangeList.Add(GetNode(currentNode.x + 3, currentNode.y - 2));
            if (currentNode.y - 3 >= 0) rangeList.Add(GetNode(currentNode.x + 3, currentNode.y - 3));
            if (currentNode.y + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 3, currentNode.y + 1));
            if (currentNode.y + 2 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 3, currentNode.y + 2));
            if (currentNode.y + 3 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 3, currentNode.y + 3));
        }

        //DOWN
        if (currentNode.y - 1 >= 0) rangeList.Add(GetNode(currentNode.x, currentNode.y - 1));
        if (currentNode.y - 2 >= 0)
        {
            rangeList.Add(GetNode(currentNode.x, currentNode.y - 2));
            if (currentNode.x - 1 >= 0) rangeList.Add(GetNode(currentNode.x - 1, currentNode.y - 2));
            if (currentNode.x + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 1, currentNode.y - 2));
        }

        if (currentNode.y - 3 >= 0)
        {
            rangeList.Add(GetNode(currentNode.x, currentNode.y - 3));
            if (currentNode.x - 1 >= 0) rangeList.Add(GetNode(currentNode.x - 1, currentNode.y - 3));
            if (currentNode.x - 2 >= 0) rangeList.Add(GetNode(currentNode.x - 2, currentNode.y - 3));
            if (currentNode.x + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 1, currentNode.y - 3));
            if (currentNode.x + 2 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 2, currentNode.y - 3));
        }

        //UP
        if (currentNode.y + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x, currentNode.y + 1));
        if (currentNode.y + 2 < grid.GetHeigth())
        {
            rangeList.Add(GetNode(currentNode.x, currentNode.y + 2));
            if (currentNode.x - 1 >= 0) rangeList.Add(GetNode(currentNode.x - 1, currentNode.y + 2));
            if (currentNode.x + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 1, currentNode.y + 2));
        }

        if (currentNode.y + 3 < grid.GetHeigth())
        {
            rangeList.Add(GetNode(currentNode.x, currentNode.y + 3));
            if (currentNode.x - 1 >= 0) rangeList.Add(GetNode(currentNode.x - 1, currentNode.y + 3));
            if (currentNode.x - 2 >= 0) rangeList.Add(GetNode(currentNode.x - 2, currentNode.y + 3));
            if (currentNode.x + 1 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 1, currentNode.y + 3));
            if (currentNode.x + 2 < grid.GetHeigth()) rangeList.Add(GetNode(currentNode.x + 2, currentNode.y + 3));
        }

        return rangeList;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    } 

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;

        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();

        return path;
    }

    public int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++){
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
    }

}
