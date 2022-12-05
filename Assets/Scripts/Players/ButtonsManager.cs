using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
    public Button shootButton;
    public Button moveButton;
    public GameObject m_Bullet;
    //public Transform targetPosition;

    public static bool enabledShoot;
    public static bool enabledMove;

    Pathfinding pathfinding;
    public CharacterPathfinding characterPathfinding;
    public Character2Pathfinding character2Pathfinding;

    private void Start()
    {
        enabledShoot = false;
        enabledMove = false;

        pathfinding = Pathfinding.Instance;
        
    }

    private void Update()
    {
        if (Test.enemyTurn)
        {
            shootButton.interactable = false;
            moveButton.interactable = false;
        }


        if (Test.playerTurn == 1)
        {
            if (enabledShoot)
            {

                if (Input.GetMouseButtonDown(0))  //Space de momento, pero bloquear movimiento y usar raton
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                    if (characterPathfinding.range.Contains(pathfinding.GetNode(x, y))){
                        GameObject bulletInstance = Instantiate(m_Bullet, characterPathfinding.transform.position, Quaternion.identity) as GameObject;
                        Vector3 shootDir = (mouseWorldPosition - bulletInstance.transform.position).normalized;
                        bulletInstance.GetComponent<Bullet>().SetUp(shootDir, pathfinding.GetNode(x, y)); 
                        enabledShoot = false;

                        if (!moveButton.interactable)
                        {
                            Test.playerTurn = 2;
                            shootButton.interactable = true;
                            moveButton.interactable = true;
                        }

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

                    if (!shootButton.interactable)
                    {
                        Test.playerTurn = 2;
                        shootButton.interactable = true;
                        moveButton.interactable = true;
                    }

                    //Para no desbloquear el boton despues del disparo, comentar:
                    //moveButton.interactable = true;
                }
            }
        }

        else if (Test.playerTurn == 2)
        {
            if (enabledShoot)
            {

                if (Input.GetMouseButtonDown(0))  //Space de momento, pero bloquear movimiento y usar raton
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                    if (character2Pathfinding.range.Contains(pathfinding.GetNode(x, y)))
                    {
                        GameObject bulletInstance = Instantiate(m_Bullet, character2Pathfinding.transform.position, Quaternion.identity) as GameObject;
                        Vector3 shootDir = (mouseWorldPosition - bulletInstance.transform.position).normalized;
                        bulletInstance.GetComponent<Bullet>().SetUp(shootDir, pathfinding.GetNode(x, y));
                        enabledShoot = false;

                        if (!moveButton.interactable)
                        {
                            Test.playerTurn = 1;
                            shootButton.interactable = true;
                            moveButton.interactable = true;
                        }

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

                    if (!shootButton.interactable)
                    {
                        Test.playerTurn = 1;
                        shootButton.interactable = true;
                        moveButton.interactable = true;
                    }

                    //Para no desbloquear el boton despues del disparo, comentar:
                    //moveButton.interactable = true;
                }
            }
        }

    }

    public void Fire()
    {
        enabledMove = false;

        enabledShoot = true;
        shootButton.interactable = false;
    }

    public void Move()
    {
        enabledShoot = false;

        enabledMove = true;
        moveButton.interactable = false;
    }
}
