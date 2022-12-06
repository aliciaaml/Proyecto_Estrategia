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
    
    //List<Vector3> secondPathVectorList;
    //int secondPathIndex;;

    void Start()
    {
        pathfinding = Pathfinding.Instance;
        pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
        pathfinding.GetNode(x, y).SetIsIA(true);
        range = pathfinding.GetRangeList(pathfinding.GetNode(x, y));

    }
    public void Update()
    {
        if (Test.IATurn)
        {
            pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
            //Debug.Log(pathfinding.GetNode(x, y));
            TakeDecisions(pathfinding.GetNode(x, y));
            //enemyTurn = false;
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


    void TakeDecisions(PathNode nodoActual)
    {

        range = pathfinding.GetRangeList(pathfinding.GetNode(nodoActual.x, nodoActual.y));
        PathNode closestPlayer = null;
        PathNode closestIA = null;
        int minDist = 1000;
        //enemyTurn = false;

        foreach (PathNode node in range)
        {
            if (pathfinding.GetNode(node.x, node.y).isPlayer) //Hay jugador en rango?
            {

                Debug.Log("PLAYER EN RANGOOO");
                if (pathfinding.CalculateDistanceCost(node, nodoActual) < minDist)
                {
                    closestPlayer = node;

                    if (Test.saltado)
                        pathfinding.GetNode(node.x, node.y).SetIsIA(false);//Si la IA ya ha hab�a saltado a un jugador no lo vuelve a saltar

                    //HandleMovement();  // Cuando la IA se quede en una casilla de jugador y no hay muros cerca, ni otro jugador se mueve aleatoriamente el rango m�ximo
                }
            }

            else if (pathfinding.GetNode(node.x, node.y).isIA) //Hay IA en rango?
            {
                Debug.Log("IA EN RANGOOO");
                if (pathfinding.CalculateDistanceCost(node, nodoActual) < minDist)
                {
                    if (!Test.saltado)
                    {
                        closestIA = node;
                    }
                }
            }
        }

        
        if (closestPlayer != null) //Hay jugador en rango
        {
            Debug.Log("player in range");
            //Debug.Log(nodoActual.x);
            //Debug.Log(nodoActual.y);
            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y));

            List<PathNode> playerRange = pathfinding.GetRangeList(pathfinding.GetNode(closestPlayer.x, closestPlayer.y));
            PathNode choosenHide = null;

            foreach (PathNode node in playerRange)
            {
                if (pathfinding.GetNode(node.x, node.y).isFullHiding || pathfinding.GetNode(node.x, node.y).isHalfHiding)
                {
                    choosenHide = node;
                }
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
        
        /*else //No hay jugador en rango
        {

        }*/

        if (closestIA != null) //Hay IA en rango        //else
        {
            Debug.Log("IA in range");
            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y));

            List<PathNode> playerRange = pathfinding.GetRangeList(pathfinding.GetNode(closestIA.x, closestIA.y));
            PathNode choosenHide = null;

            foreach (PathNode node in playerRange)
            {
                if (pathfinding.GetNode(node.x, node.y).isFullHiding || pathfinding.GetNode(node.x, node.y).isHalfHiding)
                {
                    choosenHide = node;
                }
            }

            if (choosenHide != null) //Tiene muro cerca
            {
                if (nodoActual.x == closestIA.x && nodoActual.y == closestIA.y)
                    SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(choosenHide.x, choosenHide.y));
            }

            else
            {

            }
        }
    }
}
