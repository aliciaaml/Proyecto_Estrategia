using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsManager : MonoBehaviour
{
    public Button shootButton;
    public GameObject m_Bullet;          
    public Transform m_Shooter;
    public Transform targetPosition;

    private bool enabledShoot;

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
            //Hacer que seleccione un targetposition
            //Para eso necesito añadir variables a los nodos y mirar en este si hay enemigos
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);

            if (Input.GetKeyDown(KeyCode.Space))  //Space de momento, pero bloquear movimiento y usar raton
            {
                
                if (characterPathfinding.range.Contains(pathfinding.GetNode(x, y))){
                    GameObject bulletInstance = Instantiate(m_Bullet, m_Shooter.position, Quaternion.identity) as GameObject;
                    Vector3 shootDir = (mouseWorldPosition - bulletInstance.transform.position).normalized;
                    bulletInstance.GetComponent<Bullet>().SetUp(shootDir); 
                    enabledShoot = false;

                    //Para no desbloquear el boton despues del disparo, comentar:
                    shootButton.interactable = true;
                }
                

                
            }  
        }
    }

    public void Fire()
    {
        
        enabledShoot = true;   //Falta por bloquear el movimiento

        //Para bloquear el boton de disparar una vez pulsado:
        shootButton.interactable = false;
    }

    public void Move()
    {

    }


    //En caso de diferenciar el movedel hit:
    public void Hit()
    {

    }
}
