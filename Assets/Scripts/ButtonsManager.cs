using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
    public Button shootButton;
    public Button moveButton;
    public GameObject m_Bullet;          
    public Transform m_Shooter;
    //public Transform targetPosition;

    public static bool enabledShoot;
    public static bool enabledMove;

    Pathfinding pathfinding;
    [SerializeField] private CharacterPathfinding characterPathfinding;

    private void Start()
    {
        enabledShoot = false;

        pathfinding = Pathfinding.Instance;
        
    }

    private void Update()
    {

        if (enabledShoot)
        {

            if (Input.GetMouseButtonDown(0))  //Space de momento, pero bloquear movimiento y usar raton
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

                if (characterPathfinding.range.Contains(pathfinding.GetNode(x, y))){
                    GameObject bulletInstance = Instantiate(m_Bullet, m_Shooter.position, Quaternion.identity) as GameObject;
                    Vector3 shootDir = (mouseWorldPosition - bulletInstance.transform.position).normalized;
                    bulletInstance.GetComponent<Bullet>().SetUp(shootDir); 
                    enabledShoot = false;

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
