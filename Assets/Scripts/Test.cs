using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private GridManager<int> intGrid;
    private GridManager<StringGridObject> stringGrid;
    private Pathfinding pathfinding;

    public GameObject m_TWall;
    public GameObject m_SWall;
    private List<PathNode> wallList;

    [SerializeField] private CharacterPathfinding characterPathfinding;

    private void Start()
    {
        pathfinding = new Pathfinding(12, 12);
        stringGrid = new GridManager<StringGridObject>(12, 12, 10f, Vector3.zero, (GridManager<StringGridObject> g, int x, int y) => new StringGridObject(g, x, y));  // width, heigth, cellSize, OriginPosition
        wallList = new List<PathNode>();

        /*Tall Walls
        for (int i = 0; i < 6; i++)
            intGrid.SetGridObject(UnityEngine.Random.Range(3, 9), UnityEngine.Random.Range(0, 12), 5);

        //Short Walls
        for (int i = 0; i < 6; i++)
            intGrid.SetGridObject(UnityEngine.Random.Range(3, 9), UnityEngine.Random.Range(0, 12), 7);

        //Players
        intGrid.SetGridObject(UnityEngine.Random.Range(9, 12), UnityEngine.Random.Range(0, 4), 2);
        intGrid.SetGridObject(UnityEngine.Random.Range(9, 12), UnityEngine.Random.Range(5, 7), 2);
        intGrid.SetGridObject(UnityEngine.Random.Range(9, 12), UnityEngine.Random.Range(8, 11), 2);

        //Enemies
        intGrid.SetGridObject(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 4), 3);
        intGrid.SetGridObject(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(5, 7), 3);
        intGrid.SetGridObject(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(8, 11), 3);*/
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);

            if (path != null)
            {
                
                for (int i = 0; i < path.Count - 1; i++)
                {
                    //Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f, Color.green,2);
                }
            }
            characterPathfinding.SetTargetPosition(mouseWorldPosition);
        }



        if (Input.GetMouseButtonDown(1))
        {
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkeable(!pathfinding.GetNode(x, y).isWalkeable);

            if (!pathfinding.GetNode(x, y).isWalkeable)
            {
                var i = UnityEngine.Random.Range(0,2);
                if(i == 0)
                {
                
                    pathfinding.GetNode(x, y).SetIsTallWall(true);
                    pathfinding.GetNode(x, y).SetIsHiding(false);
                    stringGrid.GetGridObject(mouseWorldPosition).AddLetter("TW");
                    Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                    GameObject tWallInstance = Instantiate(m_TWall, localPost, Quaternion.identity) as GameObject;
                    wallList.Add(pathfinding.GetNode(x, y));


                }
                else
                {
                    pathfinding.GetNode(x, y).SetIsSmallWall(true);
                    pathfinding.GetNode(x, y).SetIsHiding(false);
                    stringGrid.GetGridObject(mouseWorldPosition).AddLetter("SW");
                    Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                    GameObject sWallInstance = Instantiate(m_SWall, localPost, Quaternion.identity) as GameObject;
                    wallList.Add(pathfinding.GetNode(x, y));


                }
            }

            else
            {
                stringGrid.GetGridObject(mouseWorldPosition).AddLetter("");
                pathfinding.GetNode(x, y).SetIsTallWall(false);
                pathfinding.GetNode(x, y).SetIsSmallWall(false);
                pathfinding.GetNode(x, y).SetIsHiding(false);
                wallList.Remove(pathfinding.GetNode(x, y));

                List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(x, y));
                foreach (PathNode neighbourNode in neighbours)
                {
                    if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
                    {
                        pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHiding(false);
                        Debug.Log("Vecino Muro");
                    }

                    else
                    {
                        pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHiding(false);
                        stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("");
                    }
                }
            }
            


            foreach (PathNode wall in wallList)
            {
                List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(wall.x, wall.y));
                foreach (PathNode neighbourNode in neighbours)
                {
                    if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
                    {
                        pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHiding(false);
                        Debug.Log("Vecino Muro");
                    }

                    else
                    {
                        if (pathfinding.GetNode(wall.x, wall.y).isWalkeable)
                        {
                            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHiding(false);
                            stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("");
                        }
                        else
                        {
                            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHiding(true);
                            stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("H");
                        }
                    }
                }
            }
        }
    }
}

public class StringGridObject
{
    private GridManager<StringGridObject> grid;
    private int x;
    private int y;

    private string letters;

    public StringGridObject(GridManager<StringGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        letters = "";
    }

    public void AddLetter(string letter)
    {
        letters = letter;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return letters;
    }
}
