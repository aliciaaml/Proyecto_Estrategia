using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    public float speed = 15f;
    Pathfinding pathfinding;
    List<Vector3> pathVectorList;
    int currentPathIndex;
    public List<PathNode> range = new List<PathNode>();
    public bool IAEnd;

    public GameObject bulletAmmo;

    public static int totalWalls = 3;
    public static int totalBullets = 3;

    //List<Vector3> secondPathVectorList;
    //int secondPathIndex;;

    void Start()
    {
        pathfinding = Pathfinding.Instance;
        pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
        pathfinding.GetNode(x, y).SetIsIA(true);
        range = pathfinding.GetRangeList(pathfinding.GetNode(x, y));
        IAEnd = false;
    }
    public void Update()
    {
        if (Test.IATurn)
        {
            pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
            TakeDecisions(pathfinding.GetNode(x, y));
            if(IAEnd) PassToPlayerTurn(); //Ha terminado con las decisiones, pasa a player
        }
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];

            if (Vector3.Distance(transform.position, targetPosition) > 1)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                //float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }

            else
            {
                currentPathIndex++;

                if (currentPathIndex >= pathVectorList.Count)
                {
                    //DoSecondPath();
                    StopMoving();
                }
            }
        }
    }

    private void StopMoving()
    {
        //secondPathVectorList = null;
        pathVectorList = null;

        foreach (PathNode node in range)
        {
            pathfinding.GetNode(node.x, node.y).SetIsInRange(false);
        }

        pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);

        if (pathfinding.GetNode(x, y).isPlayer || pathfinding.GetNode(x, y).isIA)
        {
            ButtonsManager.enabledMove = true;
        }

        pathfinding.GetNode(x, y).SetIsIA(true);
        range = pathfinding.GetRangeList(pathfinding.GetNode(x, y));
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;

        foreach (PathNode node in range)
        {
            pathfinding.GetNode(node.x, node.y).SetIsInRange(true);
        }

        pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
        pathfinding.GetNode(x, y).SetIsIA(false);

        pathVectorList = pathfinding.FindPath(GetPosition(), targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            ButtonsManager.enabledMove = false;
            pathVectorList.RemoveAt(0);
        }
    }

    void TakeDecisions(PathNode nodoActual)
    {

        range = pathfinding.GetRangeList(pathfinding.GetNode(nodoActual.x, nodoActual.y));
        PathNode closestPlayer = null;
        PathNode closestIA = null;
        int minDist = 1000;

        foreach (PathNode node in range) //Guardar jugador o IA más cercanos
        {
            if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra jugador en rango
            {
                if (pathfinding.CalculateDistanceCost(node, nodoActual) < minDist)
                {
                    closestPlayer = node;

                    if (Test.saltado)
                        pathfinding.GetNode(node.x, node.y).SetIsIA(false);//Si la IA ya ha habia saltado a un jugador no lo vuelve a saltar

                    //HandleMovement();  // Cuando la IA se quede en una casilla de jugador y no hay muros cerca, ni otro jugador se mueve aleatoriamente el rango m�ximo
                }
            }

            else  //No se encuentra jugador en rango
            {
                if (pathfinding.GetNode(node.x, node.y).isIA) //Se encuentra IA en rango
                {
                    if (pathfinding.CalculateDistanceCost(node, nodoActual) < minDist)
                    {
                        if (!Test.saltado)
                        {
                            closestIA = node;
                        }
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //INICIO DEL ALGORITMO
        //////////////////////////////////////////////////////////////////////////////////////////////
        
        if (closestPlayer != null) //Hay jugador en rango
        {
            Debug.Log("Hay Player en rango");
            List<PathNode> playerRange = pathfinding.GetRangeList(pathfinding.GetNode(closestPlayer.x, closestPlayer.y)); //Obtener rango de player

            if (EnemyHealth.currentHealth >= 50) //Vida IA >= 50% (Al poner + de un enemy cambiar esto)
            {
                Debug.Log("Vida IA >= 50%");

                PathNode smallWall = null;
                PathNode choosenHalfHiding = null;
                PathNode tallWall = null;
                PathNode choosenFullHiding = null;
                PathNode closestPlayerAmmo = null;

                foreach (PathNode node in playerRange)
                {
                    if (pathfinding.GetNode(node.x, node.y).isSWall)   //Hay muro bajo
                    {
                        smallWall = node;
                    }

                    if (pathfinding.GetNode(node.x, node.y).isTWall)   //Hay muro alto
                    {
                        tallWall = node;
                    }

                    if (pathfinding.GetNode(node.x, node.y).isAmmo)   //Hay munición
                    {
                        closestPlayerAmmo = node;
                    }
                }

                if(smallWall != null) //En rango de closestPlayer hay smallWall
                {
                    Debug.Log("En rango de closestPlayer hay smallWall");
                    //Coger choosenHalfHiding más lejano a player de ese smallWall

                    if(totalBullets > 0 && PlayerHealth.currentHealth > 20) //Tiene balas y vida closestPlayer > 20 (Aquí mirar la del closestPlayer)
                    {
                        Debug.Log("Tiene balas y vida closestPlayer > 20");
                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y)); //Va a player
                        //Ir de player al choosenHalfHiding
                        //Disparar desde choosenHalfHiding
                    }
                    else //O no tiene balas o vida closestPlayer < 20
                    {
                        Debug.Log("O no tiene balas o vida closestPlayer < 20");
                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y)); //Va a player
                        //Ir de player al choosenHalfHiding
                    }
                }

                else if(tallWall != null) //En rango de closestPlayer hay tallWall
                {
                    Debug.Log("En rango de closestPlayer hay tallWall");
                    SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y));
                    //Ir de player a tallWall
                }

                else //Si ataca al enemigo no tiene muros en los que esconderse despues
                {
                    range = pathfinding.GetRangeList(pathfinding.GetNode(nodoActual.x, nodoActual.y));

                    PathNode closestAmmo = null;
                    PathNode farthestHiding = null; 
                    int minDistAmmo = 1000;
                    int maxDistHiding = 1;

                    foreach (PathNode node in range) //Guardar munición y escondites en rango
                    {
                        if (pathfinding.GetNode(node.x, node.y).isAmmo) //Se encuentra munición en rango
                        {
                            if (pathfinding.CalculateDistanceCost(node, nodoActual) < minDistAmmo)
                            {
                                closestAmmo = node;
                            }
                        }
                        if (pathfinding.GetNode(node.x, node.y).isHalfHiding || pathfinding.GetNode(node.x, node.y).isFullHiding)
                        {
                            if (pathfinding.CalculateDistanceCost(node, nodoActual) >= maxDistHiding)
                            {
                                farthestHiding = node;
                            }
                        }
                    }

                    if (farthestHiding != null && totalBullets > 0) //En rango de IA hay cualquier Wall && IA tiene balas
                    {
                        Debug.Log("En rango de IA hay cualquier Wall && IA tiene balas");
                        //Disparar (FALTA)
                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(farthestHiding.x, farthestHiding.y)); //Va al escondite más lejano

                    }

                    else //No tiene muros cerca o no tiene balas
                    {
                        if(closestPlayerAmmo != null) //Hay balas en rango de closestPlayer
                        {
                            Debug.Log("Hay balas en rango de closestPlayer");
                            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y)); //Ataca al player
                            //Y Va a las balas //SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayerAmmo.x, closestPlayerAmmo.y)); 
                            //Y dispara
                        }

                        else if (closestAmmo != null) //Hay balas en rango de IA
                        {
                            List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(closestAmmo.x, closestAmmo.y));
                            PathNode playerNearAmmo = null;
                            int minDistPlayer = 1000;

                            foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                            {
                                if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                                {
                                    if (pathfinding.CalculateDistanceCost(node, nodoActual) < minDistPlayer)
                                    {
                                        playerNearAmmo = node;
                                    }
                                }
                            }

                            if (playerNearAmmo != null) //Hay player en rango de balas
                            {
                                Debug.Log("Hay player en rango de balas");
                                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y));
                                //Y Dispara (FALTA)
                            }

                            else //No hay player en rango de balas
                            {
                                Debug.Log("No hay player en rango de balas");
                                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y));
                            }

                        }

                        else //Imposible llegar a balas
                        {
                            Debug.Log("Imposible llegar a balas");
                            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y));
                            //Y vuelve a su posición inicial (FALTA)
                        }
                    }
                }



            }

            else //Vida IA < 50%
            {
                Debug.Log("Vida IA < 50%");
            }


            

            /*
            if (choosenHide != null) //Desde player llego a muro
            {
                Debug.Log("Llega a muro");
                //SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenHide.x, choosenHide.y));
                if (nodoActual.x + 1 == closestPlayer.x && nodoActual.y + 1 == closestPlayer.y)
                {
                    Debug.Log("Misma posici�n que player");
                    SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(choosenHide.x, choosenHide.y));
                }

                //pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y); //mis x e y

                //while (pathfinding.GetNode(x, y) != pathfinding.GetNode(choosenHide.x, choosenHide.y))
                //{
                //    pathfinding.GetGrid().GetXY(GetPosition(), out int x1, out int y1); //mis x e y
                //    x = x1;
                //    y = y1;
                //}
                //SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(choosenHide.x, choosenHide.y));

            }

            else //No llega a muro desde player
            {
                foreach (PathNode node in playerRange)
                {
                    if (pathfinding.GetNode(node.x, node.y).isIA)
                    {
                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(node.x, node.y));
                    }
                }
            }*/

            
        }

        else //No hay Player en rango
        {
            Debug.Log("No hay Player en rango");

            if (closestIA != null) //Hay IA en rango (No lo desarrollo de momento xq solo hay una IA)        
            {
                Debug.Log("Hay IA en rango");
                
                List<PathNode> IARange = pathfinding.GetRangeList(pathfinding.GetNode(closestIA.x, closestIA.y)); //Obtener rango de IA amiga

                PathNode smallWall = null;
                PathNode choosenHalfHiding = null;
                PathNode tallWall = null;
                PathNode choosenFullHiding = null;

                foreach (PathNode node in IARange)
                {
                    if (pathfinding.GetNode(node.x, node.y).isSWall)   //Hay muro bajo
                    {
                        smallWall = node;
                    }

                    if (pathfinding.GetNode(node.x, node.y).isTWall)   //Hay muro alto
                    {
                        tallWall = node;
                    }
                }

                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y)); //Va a IA amiga
            }

            else //No hay nadie en rango
            {
                range = pathfinding.GetRangeList(pathfinding.GetNode(nodoActual.x, nodoActual.y));

                PathNode closestAmmo = null;
                PathNode closestHiding = null; //De momento es el lugar que tiene + cerca para esconderse, pero debería ser el más cercano a player
                int minDistAmmo = 1000;
                int minDistHiding = 1000;

                foreach (PathNode node in range) //Guardar munición y escondites en rango
                {
                    if (pathfinding.GetNode(node.x, node.y).isAmmo) //Se encuentra munición en rango
                    {
                        if (pathfinding.CalculateDistanceCost(node, nodoActual) < minDistAmmo)
                        {
                            closestAmmo = node;
                        }
                    }
                    if(pathfinding.GetNode(node.x, node.y).isHalfHiding || pathfinding.GetNode(node.x, node.y).isFullHiding)
                    {
                        if (pathfinding.CalculateDistanceCost(node, nodoActual) < minDistHiding)
                        {
                            closestHiding = node;
                        }
                    }
                }

                if (totalBullets <= 1)
                {
                    if(closestAmmo != null) //Hay munición en rango
                    {
                        Debug.Log("Hay munición en rango");
                        List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(closestAmmo.x, closestAmmo.y));
                        PathNode playerNearAmmo = null;
                        int minDistPlayer = 1000;

                        foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                        {
                            if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                            { 
                                if (pathfinding.CalculateDistanceCost(node, nodoActual) < minDistPlayer)
                                {
                                    playerNearAmmo = node;
                                }
                            }
                        }

                        if (playerNearAmmo != null) //Hay Player en rango de munición
                        {
                            Debug.Log("Hay Player en rango de munición");
                            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                            //Y dispara (FALTA)
                        }

                        else //No hay Player en rango de munición
                        {
                            Debug.Log("No hay Player en rango de munición");
                            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                        }
                    }

                    else if (closestHiding != null) //No hay balas cerca, pero hay muros
                    {
                        Debug.Log("No hay balas cerca, pero hay muros");
                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestHiding.x, closestHiding.y)); //Va al hiding más cercano (DEBERÍA SER + CERCANO A PLAYER)
                    }

                    else //No hay player, ni IA, ni balas ni muros cerca
                    {
                        Debug.Log("No hay player, ni IA, ni balas ni muros cerca");
                        //Crea un muro entre él y el player
                    }
                }

                else if (closestHiding != null) //Hay muro cerca y voy bien de munición
                {
                    Debug.Log("Hay muro cerca y voy bien de munición");
                    SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestHiding.x, closestHiding.y)); //Va al hiding más cercano (DEBERÍA SER + CERCANO A PLAYER)
                }

                else //No hay player, ni IA, ni balas ni muros cerca
                {
                    Debug.Log("No hay player, ni IA, ni balas ni muros cerca");
                    //Crea un muro entre él y el player
                }
            }
        }
        IAEnd = true;
    }

    void PassToPlayerTurn()
    {
        IAEnd = false;
        Test.returnTurn = true;
        Test.IATurn = false;
        
        if (Test.playerTurn == 1) 
            Test.playerTurn = 2;

        else if (Test.playerTurn == 2) 
            Test.playerTurn = 1;

    }

    //private void DoSecondPath()
    //{
    //    pathVectorList = null;

    //    if (secondPathVectorList != null)
    //    {
    //        while (secondPathIndex < secondPathVectorList.Count)
    //        {
    //            Vector3 targetPosition = secondPathVectorList[secondPathIndex];

    //            if (Vector3.Distance(transform.position, targetPosition) > 1)
    //            {
    //                Vector3 moveDir = (targetPosition - transform.position).normalized;

    //                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
    //                transform.position = transform.position + moveDir * speed * Time.deltaTime;
    //            }

    //            else
    //            {
    //                secondPathIndex++;
    //            }
    //        }
    //        StopMoving();
    //    }
    //    else
    //    {
    //        StopMoving();
    //    }
    //}

    //public void SetTwoTargetsPosition(Vector3 targetPosition, Vector3 secondTargetPosition)
    //{
    //    currentPathIndex = 0;
    //    secondPathIndex = 0;

    //    foreach (PathNode node in range)
    //    {
    //        pathfinding.GetNode(node.x, node.y).SetIsInRange(true);
    //    }

    //    pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
    //    pathfinding.GetNode(x, y).SetIsFriend(false);

    //    pathVectorList = pathfinding.FindPath(GetPosition(), targetPosition);
    //    secondPathVectorList = pathfinding.FindPath(targetPosition, secondTargetPosition); //Ir del enemigo a muro

    //    if (pathVectorList != null && pathVectorList.Count > 1)
    //    {
    //        pathVectorList.RemoveAt(0);
    //    }

    //    if (secondPathVectorList != null && secondPathVectorList.Count > 1)
    //    {
    //        secondPathVectorList.RemoveAt(0);
    //    }
    //}
}
