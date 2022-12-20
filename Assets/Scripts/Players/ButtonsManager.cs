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
    public CharacterPathfinding character3Pathfinding;



    private void Start()
    {
        enabledShoot = false;
        enabledMove = false;
        enabledWall = false;

        pathfinding = Pathfinding.Instance;
        
    }

    private void Update()
    {
        if (Test.returnTurn)
        {
            shootButton.interactable = true;
            moveButton.interactable = true;
            wallButton.interactable = true;
            Test.returnTurn = false;
        }

        if (Test.isIATurn)
        {
            shootButton.interactable = false;
            moveButton.interactable = false;
            wallButton.interactable = false;

            totalBulletsText.text = "";
            totalWallsText.text = "";
        }

        else
        {
            
            if (Test.playerTurn == 1)
            {
                totalBulletsText.text = characterPathfinding.totalBullets.ToString();
                totalWallsText.text = characterPathfinding.totalWalls.ToString();

                if (enabledShoot)
                {

                    if (Input.GetMouseButtonDown(0) && characterPathfinding.totalBullets > 0)
                    {
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                        if (characterPathfinding.range.Contains(pathfinding.GetNode(x, y)))
                        {
                            GameObject bulletInstance = Instantiate(m_Bullet, characterPathfinding.transform.position, Quaternion.identity) as GameObject;
                            Vector3 shootDir = (mouseWorldPosition - bulletInstance.transform.position).normalized;
                            bulletInstance.GetComponent<PlayerBullet>().SetUp(shootDir, pathfinding.GetNode(x, y));
                            enabledShoot = false;
                            characterPathfinding.totalBullets--;

                            //Para no desbloquear el boton despues del disparo, comentar:
                            //shootButton.interactable = true;
                        }
                    }
                }

                if (enabledMove)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        characterPathfinding.SetTargetPosition(mouseWorldPosition);

                        //Para no desbloquear el boton despues del disparo, comentar:
                        //moveButton.interactable = true;
                    }
                }

                if (enabledWall)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                        //Ademas de isWalkeable, characterPathfinding.range.Contains(pathfinding.GetNode(x, y)??
                        if (pathfinding.GetNode(x, y).isWalkeable && characterPathfinding.totalWalls > 0)
                        {
                            enabledWall = false;
                            characterPathfinding.totalWalls--;
                            Debug.Log("Muro puesto");

                            pathfinding.GetNode(x, y).SetIsTallWall(true);
                            pathfinding.GetNode(x, y).SetIsWalkeable(false);
                            Test.stringGrid.GetGridObject(x, y).AddLetter("TW");
                            Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                            GameObject tWallInstance = Instantiate(m_TWall, localPost, Quaternion.identity) as GameObject;
                            //Test.tallWallList.Add(pathfinding.GetNode(x, y));

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
                        }
                    }
                }
            }

            else if (Test.playerTurn == 2)
            {
                totalBulletsText.text = character2Pathfinding.totalBullets.ToString();
                totalWallsText.text = character2Pathfinding.totalWalls.ToString();

                if (enabledShoot)
                {

                    if (Input.GetMouseButtonDown(0) && character2Pathfinding.totalBullets > 0)
                    {
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                        if (character2Pathfinding.range.Contains(pathfinding.GetNode(x, y)))
                        {
                            GameObject bulletInstance = Instantiate(m_Bullet, character2Pathfinding.transform.position, Quaternion.identity) as GameObject;
                            Vector3 shootDir = (mouseWorldPosition - bulletInstance.transform.position).normalized;
                            bulletInstance.GetComponent<PlayerBullet>().SetUp(shootDir, pathfinding.GetNode(x, y));
                            enabledShoot = false;
                            character2Pathfinding.totalBullets--;

                            //Para no desbloquear el boton despues del disparo, comentar:
                            //shootButton.interactable = true;
                        }
                    }
                }

                if (enabledMove)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        character2Pathfinding.SetTargetPosition(mouseWorldPosition);

                        //Para no desbloquear el boton despues del disparo, comentar:
                        //moveButton.interactable = true;
                    }
                }

                if (enabledWall)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                        if (pathfinding.GetNode(x, y).isWalkeable && character2Pathfinding.totalWalls > 0)
                        {
                            enabledWall = false;
                            character2Pathfinding.totalWalls--;
                            Debug.Log("Muro puesto");

                            pathfinding.GetNode(x, y).SetIsTallWall(true);
                            pathfinding.GetNode(x, y).SetIsWalkeable(false);
                            Test.stringGrid.GetGridObject(x, y).AddLetter("TW");
                            Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                            GameObject tWallInstance = Instantiate(m_TWall, localPost, Quaternion.identity) as GameObject;
                            //Test.tallWallList.Add(pathfinding.GetNode(x, y));

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
                        }
                    }
                }
            }

            else if (Test.playerTurn == 3)
            {
                totalBulletsText.text = character3Pathfinding.totalBullets.ToString();
                totalWallsText.text = character3Pathfinding.totalWalls.ToString();

                if (enabledShoot)
                {

                    if (Input.GetMouseButtonDown(0) && character3Pathfinding.totalBullets > 0)
                    {
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                        if (character3Pathfinding.range.Contains(pathfinding.GetNode(x, y)))
                        {
                            GameObject bulletInstance = Instantiate(m_Bullet, character3Pathfinding.transform.position, Quaternion.identity) as GameObject;
                            Vector3 shootDir = (mouseWorldPosition - bulletInstance.transform.position).normalized;
                            bulletInstance.GetComponent<PlayerBullet>().SetUp(shootDir, pathfinding.GetNode(x, y));
                            enabledShoot = false;
                            character3Pathfinding.totalBullets--;

                            //Para no desbloquear el boton despues del disparo, comentar:
                            //shootButton.interactable = true;
                        }
                    }
                }

                if (enabledMove)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        character3Pathfinding.SetTargetPosition(mouseWorldPosition);

                        //Para no desbloquear el boton despues del disparo, comentar:
                        //moveButton.interactable = true;
                    }
                }

                if (enabledWall)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                        if (pathfinding.GetNode(x, y).isWalkeable && character3Pathfinding.totalWalls > 0)
                        {
                            enabledWall = false;
                            character3Pathfinding.totalWalls--;
                            Debug.Log("Muro puesto");

                            pathfinding.GetNode(x, y).SetIsTallWall(true);
                            pathfinding.GetNode(x, y).SetIsWalkeable(false);
                            Test.stringGrid.GetGridObject(x, y).AddLetter("TW");
                            Vector3 localPost = pathfinding.GetGrid().GetWorldPosition(x, y) + new Vector3(pathfinding.GetGrid().GetCellSize(), pathfinding.GetGrid().GetCellSize()) * .5f;
                            GameObject tWallInstance = Instantiate(m_TWall, localPost, Quaternion.identity) as GameObject;
                            //Test.tallWallList.Add(pathfinding.GetNode(x, y));

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
                        }
                    }
                }
            }
        }

        /*if (CharacterPathfinding.totalBullets == 0)
            shootButton.interactable = false;

        if (CharacterPathfinding.totalWalls == 0)
            wallButton.interactable = false;*/

    }

    public void Fire()
    {
        enabledMove = false;
        enabledWall = false;
        enabledShoot = true;

        shootButton.interactable = false;
        wallButton.interactable = false;
    }

    public void Move()
    {
        enabledShoot = false;
        enabledWall = false;
        enabledMove = true;

        moveButton.interactable = false;
        wallButton.interactable = false;
    }

    public void CreateWall()
    {
        enabledShoot = false;
        enabledMove = false;
        enabledWall = true;

        wallButton.interactable = false;
        shootButton.interactable = false;
        moveButton.interactable = false;
    }

    public void PassTurn()
    {
        //shootButton.interactable = false;
        //moveButton.interactable = false;
        //wallButton.interactable = false;

        //if (Test.IATurn == 1)
        //    Test.IATurn = 2;

        //else if (Test.IATurn == 2)
        //    Test.IATurn = 1;


        Test.isIATurn = true;

        if (Test.IATurn == 1)
            EnemyPathfinding.IAStart = true;

        else if (Test.IATurn == 2)
            Enemy2Pathfinding.IAStart = true;

        else if (Test.IATurn == 3)
            Enemy3Pathfinding.IAStart = true;

        if (Test.playerTurn == 1)
            Test.playerTurn = 2;

        else if (Test.playerTurn == 2)
            Test.playerTurn = 3;

        else if (Test.playerTurn == 3)
            Test.playerTurn = 1;


    }
}
