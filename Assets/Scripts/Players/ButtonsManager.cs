using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
    public Button shootButton;
    public Button moveButton;
    public Button wallButton;
    public Button passButton;
    public GameObject m_Bullet;
    public GameObject m_TWall;
    public Text totalBulletsText;
    public Text totalWallsText;
    //public Transform targetPosition;

    public static bool enabledShoot;
    public static bool enabledMove;
    public static bool enabledWall;

    Pathfinding pathfinding;
    public CharacterPathfinding characterPathfinding;
    public CharacterPathfinding character2Pathfinding;

    private void Start()
    {
        enabledShoot = false;
        enabledMove = false;
        enabledWall = false;

        pathfinding = Pathfinding.Instance;
    }

    private void Update()
    {
        totalBulletsText.text = CharacterPathfinding.totalBullets.ToString();
        totalWallsText.text = CharacterPathfinding.totalWalls.ToString();

        if (Test.IATurn)
        {
            shootButton.interactable = false;
            moveButton.interactable = false;
            wallButton.interactable = false;
        }

        /*if (CharacterPathfinding.totalBullets == 0)
            shootButton.interactable = false;

        if (CharacterPathfinding.totalWalls == 0)
            wallButton.interactable = false;*/

        if (Test.playerTurn == 1)
        {
            if (enabledShoot)
            {
                wallButton.interactable = false;

                if (Input.GetMouseButtonDown(0) && CharacterPathfinding.totalBullets > 0)  //Space de momento, pero bloquear movimiento y usar raton
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                    if (characterPathfinding.range.Contains(pathfinding.GetNode(x, y))){
                        GameObject bulletInstance = Instantiate(m_Bullet, characterPathfinding.transform.position, Quaternion.identity) as GameObject;
                        Vector3 shootDir = (mouseWorldPosition - bulletInstance.transform.position).normalized;
                        bulletInstance.GetComponent<Bullet>().SetUp(shootDir, pathfinding.GetNode(x, y)); 
                        enabledShoot = false;
                        CharacterPathfinding.totalBullets--;

                        if (!moveButton.interactable && !wallButton.interactable)
                        {
                            Test.playerTurn = 2;
                            shootButton.interactable = true;
                            moveButton.interactable = true;
                            wallButton.interactable = true;
                        }

                        //Para no desbloquear el boton despues del disparo, comentar:
                        //shootButton.interactable = true;
                    }
                }  
            }

            if (enabledMove)
            {
                wallButton.interactable = false;

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    characterPathfinding.SetTargetPosition(mouseWorldPosition);

                    if (!shootButton.interactable && !wallButton.interactable)
                    {
                        Test.playerTurn = 2;
                        shootButton.interactable = true;
                        moveButton.interactable = true;
                        wallButton.interactable = true;
                    }

                    //Para no desbloquear el boton despues del disparo, comentar:
                    //moveButton.interactable = true;
                }
            }

            if (enabledWall)
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                if (Input.GetMouseButtonDown(0))
                {
                    if (pathfinding.GetNode(x, y).isWalkeable && CharacterPathfinding.totalWalls > 0)
                    {
                        CharacterPathfinding.totalWalls--;
                        Debug.Log("Muro puesto");

                        pathfinding.GetNode(x, y).SetIsTallWall(true);
                        pathfinding.GetNode(x, y).SetIsWalkeable(false);
                        Test.stringGrid.GetGridObject(x, y).AddLetter("TW");
                        Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                        GameObject tWallInstance = Instantiate(m_TWall, localPost, Quaternion.identity) as GameObject;
                        Test.tallWallList.Add(pathfinding.GetNode(x, y));

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
                                pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(true);
                                Test.stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("FH");
                            }

                        }

                        Test.playerTurn = 2;
                        shootButton.interactable = true;
                        moveButton.interactable = true;
                        wallButton.interactable = true;
                    }
                }
            }
        }

        else if (Test.playerTurn == 2)
        {
            if (enabledShoot)
            {
                wallButton.interactable = false;

                if (Input.GetMouseButtonDown(0) && CharacterPathfinding.totalBullets > 0)  //Space de momento, pero bloquear movimiento y usar raton
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                    if (character2Pathfinding.range.Contains(pathfinding.GetNode(x, y)))
                    {
                        GameObject bulletInstance = Instantiate(m_Bullet, character2Pathfinding.transform.position, Quaternion.identity) as GameObject;
                        Vector3 shootDir = (mouseWorldPosition - bulletInstance.transform.position).normalized;
                        bulletInstance.GetComponent<Bullet>().SetUp(shootDir, pathfinding.GetNode(x, y));
                        enabledShoot = false;
                        CharacterPathfinding.totalBullets--;

                        if (!moveButton.interactable && !wallButton.interactable)
                        {
                            Test.playerTurn = 1;
                            shootButton.interactable = true;
                            moveButton.interactable = true;
                            wallButton.interactable = true;
                        }

                        //Para no desbloquear el boton despues del disparo, comentar:
                        //shootButton.interactable = true;
                    }
                }
            }

            if (enabledMove)
            {
                wallButton.interactable = false;

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    character2Pathfinding.SetTargetPosition(mouseWorldPosition);

                    if (!shootButton.interactable && !wallButton.interactable)
                    {
                        Test.playerTurn = 1;
                        shootButton.interactable = true;
                        moveButton.interactable = true;
                        wallButton.interactable = true;
                    }

                    //Para no desbloquear el boton despues del disparo, comentar:
                    //moveButton.interactable = true;
                }
            }

            if (enabledWall)
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                if (Input.GetMouseButtonDown(0))
                {
                    if (pathfinding.GetNode(x, y).isWalkeable && CharacterPathfinding.totalWalls > 0)
                    {
                        CharacterPathfinding.totalWalls--;
                        Debug.Log("Muro puesto");

                        pathfinding.GetNode(x, y).SetIsTallWall(true);
                        pathfinding.GetNode(x, y).SetIsWalkeable(false);
                        Test.stringGrid.GetGridObject(x, y).AddLetter("TW");
                        Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                        GameObject tWallInstance = Instantiate(m_TWall, localPost, Quaternion.identity) as GameObject;
                        Test.tallWallList.Add(pathfinding.GetNode(x, y));

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
                                pathfinding.GetNode(neighbourNode.x, neighbourNode.y).SetIsFullHiding(true);
                                Test.stringGrid.GetGridObject(neighbourNode.x, neighbourNode.y).AddLetter("FH");
                            }
                            
                        }

                        Test.playerTurn = 1;
                        shootButton.interactable = true;
                        moveButton.interactable = true;
                        wallButton.interactable = true;
                    }
                }
            }
        }
    }

    public void Fire()
    {
        enabledMove = false;
        enabledWall = false;

        enabledShoot = true;
        shootButton.interactable = false;
    }

    public void Move()
    {
        enabledShoot = false;
        enabledWall = false;

        enabledMove = true;
        moveButton.interactable = false;
    }

    public void CreateWall()
    {
        enabledShoot = false;
        enabledMove = false;

        enabledWall = true;
        wallButton.interactable = false;
    }

    public void PassTurn()
    {
        /*moveButton.interactable = true;
        moveButton.interactable = true;
        moveButton.interactable = true;

        if (Test.playerTurn == 1)
            Test.playerTurn = 2;
            

        if (Test.playerTurn == 2)
            Test.playerTurn = 1;*/
    }
}
