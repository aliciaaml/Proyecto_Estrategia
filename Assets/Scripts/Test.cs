using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public static GridManager<StringGridObject> stringGrid;  //Solo para escribir las letras en las celdas
    public static Pathfinding pathfinding;
    public static bool IATurn;
    public static int playerTurn;

    //public GameObject m_Character1;
    //public GameObject m_Enemy1;
    public GameObject m_TWall;
    public GameObject m_SWall;
    public GameObject m_Ammo;
    public static List<PathNode> tallWallList;
    private List<PathNode> smallWallList;
    public GameObject IACharacter;

    public static bool saltado = false;

    public EnemyPathfinding enemyPathfinding;

    private void Awake()
    {
        pathfinding = new Pathfinding(12, 12);
        stringGrid = new GridManager<StringGridObject>(12, 12, 10f, Vector3.zero, (GridManager<StringGridObject> g, int x, int y) => new StringGridObject(g, x, y));  // width, heigth, cellSize, OriginPosition

        IATurn = false;
        playerTurn = 1;
        
        tallWallList = new List<PathNode>();
        smallWallList = new List<PathNode>();

        //SPAWN DE MUROS ALEATORIO
        /////////////////////////////////////////////////////

        for (int x = 0; x < pathfinding.GetGrid().GetWidth(); x++)
        {
            for (int y = 3; y < 9; y++)
            {
                var i = UnityEngine.Random.Range(0, 8);

                if (i == 0)
                {
                    pathfinding.GetNode(x, y).SetIsWalkeable(false);
                    pathfinding.GetNode(x, y).SetIsTallWall(true);
                    pathfinding.GetNode(x, y).SetIsFullHiding(false);
                    pathfinding.GetNode(x, y).SetIsHalfHiding(false);
                    stringGrid.GetGridObject(x, y).AddLetter("TW");
                    Vector3 post = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                    Instantiate(m_TWall, post, Quaternion.identity);
                    tallWallList.Add(pathfinding.GetNode(x, y));
                }

                else if (i == 1)
                {
                    pathfinding.GetNode(x, y).SetIsWalkeable(false);
                    pathfinding.GetNode(x, y).SetIsSmallWall(true);
                    pathfinding.GetNode(x, y).SetIsFullHiding(false);
                    pathfinding.GetNode(x, y).SetIsHalfHiding(false);
                    stringGrid.GetGridObject(x, y).AddLetter("SW");
                    Vector3 post = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                    Instantiate(m_SWall, post, Quaternion.identity);
                    smallWallList.Add(pathfinding.GetNode(x, y));
                }
            }
        }

        foreach (PathNode wall in tallWallList)
        {
            List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(wall.x, wall.y));
            foreach (PathNode neighbourNode in neighbours)
            {
                if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
                {
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
                }

                else
                {
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(true);
                    stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("FH");
                }
            }
        }

        foreach (PathNode wall in smallWallList)
        {
            List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(wall.x, wall.y));
            foreach (PathNode neighbourNode in neighbours)
            {
                if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
                {
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
                }

                else
                {
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(true);
                    stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("HH");
                }
            }

            List<PathNode> neighboursAmmo = pathfinding.GetNeighbourListAmmo(pathfinding.GetNode(wall.x, wall.y));

            foreach (PathNode neighbourNode in neighboursAmmo)
            {
                if (neighbourNode.isWalkeable)
                {
                    Vector3 post = pathfinding.GetGrid().GetWorldPosition(neighbourNode.x, neighbourNode.y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsAmmo(true);
                    Instantiate(m_Ammo, post, Quaternion.identity);
                    Debug.Log("ammo");
                }
            }
        }

        ////////////////////////////////////////////////////



        /*pathfinding.GetNode(3, 3).SetIsPlayer(true);
        stringGrid.GetGridObject(3, 3).AddLetter("Player");

        pathfinding.GetNode(2, 2).SetIsIA(true);
        stringGrid.GetGridObject(2, 2).AddLetter("IA");*/


        //Tall walls
        pathfinding.GetNode(0, 6).SetIsWalkeable(false);
        pathfinding.GetNode(0, 6).SetIsTallWall(true);
        pathfinding.GetNode(0, 6).SetIsFullHiding(false);
        pathfinding.GetNode(0, 6).SetIsHalfHiding(false);
        stringGrid.GetGridObject(0, 6).AddLetter("TW");
        Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(0, 6) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
        GameObject tWallInstance = Instantiate(m_TWall, localPost, Quaternion.identity) as GameObject;
        tallWallList.Add(pathfinding.GetNode(0, 6));

        pathfinding.GetNode(8, 8).SetIsWalkeable(false);
        pathfinding.GetNode(8, 8).SetIsTallWall(true);
        pathfinding.GetNode(8, 8).SetIsFullHiding(false);
        pathfinding.GetNode(8, 8).SetIsHalfHiding(false);
        stringGrid.GetGridObject(8, 8).AddLetter("TW");
        Vector3 localPost2 = pathfinding.GetGrid().GetWorldPosition(8, 8) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
        GameObject tWallInstance2 = Instantiate(m_TWall, localPost2, Quaternion.identity) as GameObject;
        tallWallList.Add(pathfinding.GetNode(8, 8));

        foreach (PathNode wall in tallWallList)
        {
            List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(wall.x, wall.y));
            foreach (PathNode neighbourNode in neighbours)
            {
                if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
                {
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
                }

                else
                {
                    pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(true);
                    stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("FH");
                }
            }
        }



        //Vector3 localPost1 = pathfinding.GetGrid().GetWorldPosition(2, 2) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
        //GameObject Character1Instance = Instantiate(m_Character1, localPost1, Quaternion.identity) as GameObject;

        //Vector3 localPost2 = pathfinding.GetGrid().GetWorldPosition(5, 5) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
        //GameObject Enemy1Instance = Instantiate(m_Enemy1, localPost2, Quaternion.identity) as GameObject;



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
        //COMPROBAMOS SI LA IA HA SALTADO YA A ALG�N ENEMIGO

        if (IATurn)
        {
            pathfinding.GetGrid().GetXY(IACharacter.transform.position, out int x, out int y);

            if (pathfinding.GetNode(x, y).isIA)
            {
                saltado = true;
            }
        }


        /*if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        }*/


        //if (Input.GetKey(KeyCode.P))
        //{
        //    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

        //    pathfinding.GetNode(x, y).SetIsPlayer(true);
        //    stringGrid.GetGridObject(x, y).AddLetter("Player");

        //}


        //if (Input.GetKey(KeyCode.I))
        //{
        //    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

        //    pathfinding.GetNode(x, y).SetIsIA(true);
        //    stringGrid.GetGridObject(x, y).AddLetter("IA");

        //}




        //Poner muros manualmente
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            
            pathfinding.GetNode(x, y).SetIsWalkeable(!pathfinding.GetNode(x, y).isWalkeable);

            if (!pathfinding.GetNode(x, y).isWalkeable)
            {
                var i = UnityEngine.Random.Range(0,2);

                if(i == 0)
                {
                
                    pathfinding.GetNode(x, y).SetIsTallWall(true);
                    pathfinding.GetNode(x, y).SetIsFullHiding(false);
                    pathfinding.GetNode(x, y).SetIsHalfHiding(false);
                    stringGrid.GetGridObject(x, y).AddLetter("TW");
                    Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                    GameObject tWallInstance = Instantiate(m_TWall, localPost, Quaternion.identity) as GameObject;
                    tallWallList.Add(pathfinding.GetNode(x, y));


                }

                else
                {
                    pathfinding.GetNode(x, y).SetIsSmallWall(true);
                    pathfinding.GetNode(x, y).SetIsFullHiding(false);
                    pathfinding.GetNode(x, y).SetIsHalfHiding(false);
                    stringGrid.GetGridObject(x, y).AddLetter("SW");
                    Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                    GameObject sWallInstance = Instantiate(m_SWall, localPost, Quaternion.identity) as GameObject;
                    smallWallList.Add(pathfinding.GetNode(x, y));


                }
            }

            else
            {
                if (pathfinding.GetNode(x, y).isTWall)
                {
                    stringGrid.GetGridObject(x, y).AddLetter("");
                    pathfinding.GetNode(x, y).SetIsTallWall(false);
                    tallWallList.Remove(pathfinding.GetNode(x, y));

                    List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(x, y));
                    foreach (PathNode neighbourNode in neighbours)
                    {
                        if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
                        {
                            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
                            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
                        }

                        else
                        {
                            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
                            stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("");
                        }
                    }
                }

                else if (pathfinding.GetNode(x, y).isSWall)
                {
                    stringGrid.GetGridObject(x, y).AddLetter("");
                    pathfinding.GetNode(x, y).SetIsSmallWall(false);
                    smallWallList.Remove(pathfinding.GetNode(x, y));

                    List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(x, y));
                    foreach (PathNode neighbourNode in neighbours)
                    {
                        if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
                        {
                            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
                            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
                        }

                        else
                        {
                            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
                            stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("");
                        }
                    }
                }
            }

            foreach (PathNode wall in tallWallList)
            {
                List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(wall.x, wall.y));
                foreach (PathNode neighbourNode in neighbours)
                {
                    if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
                    {
                        pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
                        pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
                    }

                    else
                    {
                        pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(true);
                        stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("FH");
                    }
                }
            }

            foreach (PathNode wall in smallWallList)
            {
                List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(wall.x, wall.y));
                foreach (PathNode neighbourNode in neighbours)
                {
                    if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
                    {
                        pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
                        pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
                    }

                    else
                    {
                        pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(true);
                        stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("HH");
                    }
                }
            }
        }
    }

    //public Vector3 GetPosition()
    //{
    //    return transform.position;
    //}
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
