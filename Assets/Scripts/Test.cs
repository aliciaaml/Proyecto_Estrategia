using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public static GridManager<StringGridObject> stringGrid;  //Solo para escribir las letras en las celdas
    public static Pathfinding pathfinding;
    public static bool isIATurn;
    public static int IATurn;
    public static int playerTurn;
    public static bool returnTurn;

    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;

    public static GameObject enemy1;
    public static GameObject enemy2;
    public static GameObject enemy3;

    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;

    public static GameObject player1;
    public static GameObject player2;
    public static GameObject player3;

    public GameObject m_TWall;
    public GameObject m_SWall;
    public GameObject m_Ammo;
    public bool randomMap = false;
    public static List<PathNode> tallWallList;
    private List<PathNode> smallWallList;
    public GameObject IACharacter;

    public static bool saltado = false;
    public static bool hide = false;
    public static int ammoReload = 0;

    public EnemyPathfinding enemyPathfinding;

    private void Awake()
    {
        pathfinding = new Pathfinding(12, 12);
        stringGrid = new GridManager<StringGridObject>(12, 12, 10f, Vector3.zero, (GridManager<StringGridObject> g, int x, int y) => new StringGridObject(g, x, y));  // width, heigth, cellSize, OriginPosition

        isIATurn = false;
        IATurn = 1;
        playerTurn = 1;

        enemy1 = Enemy1;
        enemy2 = Enemy2;
        enemy3 = Enemy3;

        player1 = Player1;
        player2 = Player2;
        player3 = Player3;

        tallWallList = new List<PathNode>();
        smallWallList = new List<PathNode>();

        if (randomMap)
        {
            //SPAWN DE MUROS ALEATORIO
            /////////////////////////////////////////////////////

            for (int x = 0; x < pathfinding.GetGrid().GetWidth(); x++)
            {
                for (int y = 3; y < 9; y++)
                {
                    var i = UnityEngine.Random.Range(0, 14);

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
            }

            if (ammoReload % 2 == 0)
            {
                foreach (PathNode wall in smallWallList)
                {
                    List<PathNode> neighboursAmmo = pathfinding.GetNeighbourListAmmo(pathfinding.GetNode(wall.x, wall.y));

                    foreach (PathNode neighbourNode in neighboursAmmo)
                    {
                        if (neighbourNode.isWalkeable && !neighbourNode.isAmmo)
                        {
                            Vector3 post = pathfinding.GetGrid().GetWorldPosition(neighbourNode.x, neighbourNode.y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsAmmo(true);
                            Instantiate(m_Ammo, post, Quaternion.identity);
                        }
                    }
                }
            }

            //////////////////////////////////////////////////
        }

        else //MAPA NO ALEATORIO
        {
            //Tall Walls
            pathfinding.GetNode(0, 5).SetIsWalkeable(false);
            pathfinding.GetNode(0, 5).SetIsTallWall(true);
            pathfinding.GetNode(0, 5).SetIsFullHiding(false);
            pathfinding.GetNode(0, 5).SetIsHalfHiding(false);
            stringGrid.GetGridObject(0, 5).AddLetter("TW");
            Vector3 post1 = pathfinding.GetGrid().GetWorldPosition(0, 5) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_TWall, post1, Quaternion.identity);
            tallWallList.Add(pathfinding.GetNode(0, 5));

            pathfinding.GetNode(2, 8).SetIsWalkeable(false);
            pathfinding.GetNode(2, 8).SetIsTallWall(true);
            pathfinding.GetNode(2, 8).SetIsFullHiding(false);
            pathfinding.GetNode(2, 8).SetIsHalfHiding(false);
            stringGrid.GetGridObject(2, 8).AddLetter("TW");
            Vector3 post2 = pathfinding.GetGrid().GetWorldPosition(2, 8) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_TWall, post2, Quaternion.identity);
            tallWallList.Add(pathfinding.GetNode(2, 8));

            pathfinding.GetNode(10, 8).SetIsWalkeable(false);
            pathfinding.GetNode(10, 8).SetIsTallWall(true);
            pathfinding.GetNode(10, 8).SetIsFullHiding(false);
            pathfinding.GetNode(10, 8).SetIsHalfHiding(false);
            stringGrid.GetGridObject(10, 8).AddLetter("TW");
            Vector3 post4 = pathfinding.GetGrid().GetWorldPosition(10, 8) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_TWall, post4, Quaternion.identity);
            tallWallList.Add(pathfinding.GetNode(10, 8));

            pathfinding.GetNode(5, 4).SetIsWalkeable(false);
            pathfinding.GetNode(5, 4).SetIsTallWall(true);
            pathfinding.GetNode(5, 4).SetIsFullHiding(false);
            pathfinding.GetNode(5, 4).SetIsHalfHiding(false);
            stringGrid.GetGridObject(5, 4).AddLetter("TW");
            Vector3 post5 = pathfinding.GetGrid().GetWorldPosition(5, 4) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_TWall, post5, Quaternion.identity);
            tallWallList.Add(pathfinding.GetNode(5, 4));

            pathfinding.GetNode(4, 3).SetIsWalkeable(false);
            pathfinding.GetNode(4, 3).SetIsTallWall(true);
            pathfinding.GetNode(4, 3).SetIsFullHiding(false);
            pathfinding.GetNode(4, 3).SetIsHalfHiding(false);
            stringGrid.GetGridObject(4, 3).AddLetter("TW");
            Vector3 post6 = pathfinding.GetGrid().GetWorldPosition(4, 3) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_TWall, post6, Quaternion.identity);
            tallWallList.Add(pathfinding.GetNode(4, 3));

            pathfinding.GetNode(11, 6).SetIsWalkeable(false);
            pathfinding.GetNode(11, 6).SetIsTallWall(true);
            pathfinding.GetNode(11, 6).SetIsFullHiding(false);
            pathfinding.GetNode(11, 6).SetIsHalfHiding(false);
            stringGrid.GetGridObject(11, 6).AddLetter("TW");
            Vector3 post7 = pathfinding.GetGrid().GetWorldPosition(11, 6) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_TWall, post7, Quaternion.identity);
            tallWallList.Add(pathfinding.GetNode(11, 6));

            //Short Walls

            pathfinding.GetNode(0, 3).SetIsWalkeable(false);
            pathfinding.GetNode(0, 3).SetIsSmallWall(true);
            pathfinding.GetNode(0, 3).SetIsFullHiding(false);
            pathfinding.GetNode(0, 3).SetIsHalfHiding(false);
            stringGrid.GetGridObject(0, 3).AddLetter("SW");
            Vector3 post8 = pathfinding.GetGrid().GetWorldPosition(0, 3) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_SWall, post8, Quaternion.identity);
            smallWallList.Add(pathfinding.GetNode(0, 3));

            pathfinding.GetNode(6, 3).SetIsWalkeable(false);
            pathfinding.GetNode(6, 3).SetIsSmallWall(true);
            pathfinding.GetNode(6, 3).SetIsFullHiding(false);
            pathfinding.GetNode(6, 3).SetIsHalfHiding(false);
            stringGrid.GetGridObject(6, 3).AddLetter("SW");
            Vector3 post9 = pathfinding.GetGrid().GetWorldPosition(6, 3) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_SWall, post9, Quaternion.identity);
            smallWallList.Add(pathfinding.GetNode(6, 3));

            pathfinding.GetNode(3, 4).SetIsWalkeable(false);
            pathfinding.GetNode(3, 4).SetIsSmallWall(true);
            pathfinding.GetNode(3, 4).SetIsFullHiding(false);
            pathfinding.GetNode(3, 4).SetIsHalfHiding(false);
            stringGrid.GetGridObject(3, 4).AddLetter("SW");
            Vector3 post10 = pathfinding.GetGrid().GetWorldPosition(3, 4) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_SWall, post10, Quaternion.identity);
            smallWallList.Add(pathfinding.GetNode(3, 4));

            pathfinding.GetNode(7, 4).SetIsWalkeable(false);
            pathfinding.GetNode(7, 4).SetIsSmallWall(true);
            pathfinding.GetNode(7, 4).SetIsFullHiding(false);
            pathfinding.GetNode(3, 4).SetIsHalfHiding(false);
            stringGrid.GetGridObject(7, 4).AddLetter("SW");
            Vector3 post11 = pathfinding.GetGrid().GetWorldPosition(7, 4) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_SWall, post11, Quaternion.identity);
            smallWallList.Add(pathfinding.GetNode(7, 4));

            pathfinding.GetNode(6, 8).SetIsWalkeable(false);
            pathfinding.GetNode(6, 8).SetIsSmallWall(true);
            pathfinding.GetNode(6, 8).SetIsFullHiding(false);
            pathfinding.GetNode(6, 8).SetIsHalfHiding(false);
            stringGrid.GetGridObject(6, 8).AddLetter("SW");
            Vector3 post12 = pathfinding.GetGrid().GetWorldPosition(6, 8) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            Instantiate(m_SWall, post12, Quaternion.identity);
            smallWallList.Add(pathfinding.GetNode(6, 8));

            //Ammo
            Vector3 post_A1 = pathfinding.GetGrid().GetWorldPosition(0, 4) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            pathfinding.GetNode(0, 4).SetIsAmmo(true);
            Instantiate(m_Ammo, post_A1, Quaternion.identity);

            Vector3 post_A2 = pathfinding.GetGrid().GetWorldPosition(3, 5) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            pathfinding.GetNode(3, 5).SetIsAmmo(true);
            Instantiate(m_Ammo, post_A2, Quaternion.identity);

            Vector3 post_A3 = pathfinding.GetGrid().GetWorldPosition(7, 7) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            pathfinding.GetNode(7, 7).SetIsAmmo(true);
            Instantiate(m_Ammo, post_A3, Quaternion.identity);

            Vector3 post_A4 = pathfinding.GetGrid().GetWorldPosition(6, 4) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            pathfinding.GetNode(6, 4).SetIsAmmo(true);
            Instantiate(m_Ammo, post_A4, Quaternion.identity);

            Vector3 post_A5 = pathfinding.GetGrid().GetWorldPosition(8, 3) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
            pathfinding.GetNode(8, 3).SetIsAmmo(true);
            Instantiate(m_Ammo, post_A5, Quaternion.identity);

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

        /*pathfinding.GetNode(3, 3).SetIsPlayer(true);
        stringGrid.GetGridObject(3, 3).AddLetter("Player");

        pathfinding.GetNode(2, 2).SetIsIA(true);
        stringGrid.GetGridObject(2, 2).AddLetter("IA");*/


        //Pruebas:
        //pathfinding.GetNode(6, 6).SetIsWalkeable(false);
        //pathfinding.GetNode(6, 6).SetIsTallWall(true);
        //pathfinding.GetNode(6, 6).SetIsFullHiding(false);
        //pathfinding.GetNode(6, 6).SetIsHalfHiding(false);
        //stringGrid.GetGridObject(6, 6).AddLetter("TW");
        //Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(6, 6) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
        //GameObject tWallInstance = Instantiate(m_TWall, localPost, Quaternion.identity) as GameObject;
        //tallWallList.Add(pathfinding.GetNode(6, 6));

        //pathfinding.GetNode(9, 6).SetIsWalkeable(false);
        //pathfinding.GetNode(9, 6).SetIsSmallWall(true);
        //pathfinding.GetNode(9, 6).SetIsFullHiding(false);
        //pathfinding.GetNode(9, 6).SetIsHalfHiding(false);
        //stringGrid.GetGridObject(9, 6).AddLetter("SW");
        //Vector3 localPost2 = pathfinding.GetGrid().GetWorldPosition(9, 6) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
        //GameObject tWallInstance2 = Instantiate(m_SWall, localPost2, Quaternion.identity) as GameObject;
        //smallWallList.Add(pathfinding.GetNode(9, 6));

        //Vector3 post = pathfinding.GetGrid().GetWorldPosition(0, 3) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
        //pathfinding.GetNode(0, 3).SetIsAmmo(true);
        //Instantiate(m_Ammo, post, Quaternion.identity);

        //foreach (PathNode wall in tallWallList)
        //{
        //    List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(wall.x, wall.y));
        //    foreach (PathNode neighbourNode in neighbours)
        //    {
        //        if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
        //        {
        //            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
        //            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
        //        }

        //        else
        //        {
        //            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(true);
        //            stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("FH");
        //        }
        //    }
        //}

        //foreach (PathNode wall in smallWallList)
        //{
        //    List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(wall.x, wall.y));
        //    foreach (PathNode neighbourNode in neighbours)
        //    {
        //        if (pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isTWall || pathfinding.GetNode(neighbourNode.x, neighbourNode.y).isSWall)
        //        {
        //            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(false);
        //            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(false);
        //        }

        //        else
        //        {
        //            pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsHalfHiding(true);
        //            stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("HH");
        //        }
        //    }
        //}


        //Vector3 localPost1 = pathfinding.GetGrid().GetWorldPosition(2, 2) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
        //GameObject Character1Instance = Instantiate(m_Character1, localPost1, Quaternion.identity) as GameObject;

        //Vector3 localPost2 = pathfinding.GetGrid().GetWorldPosition(5, 5) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
        //GameObject Enemy1Instance = Instantiate(m_Enemy1, localPost2, Quaternion.identity) as GameObject;


    }


    private void Update()
    {
  
        //COMPROBAMOS SI LA IA HA SALTADO YA A ALG�N ENEMIGO

        if (isIATurn)
        {
            //Debug.Log("Turno de la IA");
           
            pathfinding.GetGrid().GetXY(IACharacter.transform.position, out int x, out int y);

            if (pathfinding.GetNode(x, y).isIA)
            {
                saltado = true;
            }
        }

        else
        {
           // Debug.Log("Turno del player " + playerTurn);
        }


        


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
