using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Pathfinding : MonoBehaviour
{
    float speed = 15f;
    Pathfinding pathfinding;
    List<Vector3> pathVectorList;
    int currentPathIndex;
    List<Vector3> secondPathVectorList;
    int secondPathIndex;
    List<Vector3> thirdPathVectorList;
    int thirdPathIndex;

    List<PathNode> range = new List<PathNode>();
    public static bool IAStart;
    bool IAEnd;

    public List<GameObject> players = new List<GameObject>();
    GameObject playerGO;
    public GameObject m_TWall;
    public GameObject m_Bullet;
    int totalBullets = 3;
    bool shoot = false;
    PathNode target = null;


    void Start()
    {
        pathfinding = Pathfinding.Instance;
        pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
        pathfinding.GetNode(x, y).SetIsIA(true);
        range = pathfinding.GetRangeList(pathfinding.GetNode(x, y));
        IAEnd = false;
        IAStart = false;
    }

    public void Update()
    {
        //if (Test.isIATurn && Test.IATurn == 1)
        //{
        //    Debug.Log("Turno IA 1");
        //    pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
        //    TakeDecisions(pathfinding.GetNode(x, y));

        //    //if (IAEnd) PassToPlayerTurn(); //Ha terminado con las decisiones, pasa a player

        //}

        if (IAStart)
        {
            Debug.Log("Turno IA 2");
            pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
            TakeDecisions(pathfinding.GetNode(x, y));
            IAStart = false;

        }

        if (IAEnd) PassToPlayerTurn();
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

                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }

            else
            {
                currentPathIndex++;

                if (currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList = null;
                    if (secondPathVectorList == null && thirdPathVectorList == null)
                    {
                        IAEnd = true;
                        Debug.Log("IAEND 1");
                    }
                }
            }
        }

        else if (secondPathVectorList != null)
        {
            Vector3 targetPosition = secondPathVectorList[secondPathIndex];

            if (Vector3.Distance(transform.position, targetPosition) > 1)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }

            else
            {
                secondPathIndex++;

                if (secondPathIndex >= secondPathVectorList.Count)
                {
                    secondPathVectorList = null;
                    if (thirdPathVectorList == null)
                    {
                        IAEnd = true;
                        Debug.Log("IAEND 2");
                    }
                }
            }
        }

        else if (thirdPathVectorList != null)
        {
            Debug.Log("33333333333");
            Vector3 targetPosition = thirdPathVectorList[thirdPathIndex];

            if (Vector3.Distance(transform.position, targetPosition) > 1)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }

            else
            {
                thirdPathIndex++;

                if (thirdPathIndex >= thirdPathVectorList.Count)
                {
                    thirdPathVectorList = null;
                    IAEnd = true;
                    Debug.Log("IAEND 3");
                }
            }
        }

        else if (shoot)
        {
            GameObject bulletInstance = Instantiate(m_Bullet, transform.position, Quaternion.identity) as GameObject;
            Vector3 shootDir = (pathfinding.GetGrid().GetWorldPosition(target.x, target.y + 1) - bulletInstance.transform.position).normalized;
            Debug.Log(shootDir);
            bulletInstance.GetComponent<EnemyBullet>().SetUp(shootDir, target);
            totalBullets -= 1;
            Debug.Log("Quedan: " + totalBullets);

            shoot = false;
            target = null;
        }

        else
        {
            StopMoving();
        }
    }

    private void StopMoving()
    {
        foreach (PathNode node in range)
        {
            pathfinding.GetNode(node.x, node.y).SetIsInRange(false);
        }

        pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);

        pathfinding.GetNode(x, y).SetIsIA(true);
        range = pathfinding.GetRangeList(pathfinding.GetNode(x, y));

        //IAEnd = true;
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

    public void SetTwoTargetsPosition(Vector3 targetPosition, Vector3 secondTargetPosition)
    {
        currentPathIndex = 0;
        secondPathIndex = 0;

        foreach (PathNode node in range)
        {
            pathfinding.GetNode(node.x, node.y).SetIsInRange(true);
        }

        pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
        pathfinding.GetNode(x, y).SetIsIA(false);

        pathVectorList = pathfinding.FindPath(GetPosition(), targetPosition);
        secondPathVectorList = pathfinding.FindSecondPath(targetPosition, secondTargetPosition); //Ir del enemigo a muro

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }

        if (secondPathVectorList != null && secondPathVectorList.Count > 1)
        {
            secondPathVectorList.RemoveAt(0);
        }
    }

    public void SetThreeTargetsPosition(Vector3 targetPosition, Vector3 secondTargetPosition, Vector3 thirdTargetPosition)
    {
        currentPathIndex = 0;
        secondPathIndex = 0;
        thirdPathIndex = 0;

        foreach (PathNode node in range)
        {
            pathfinding.GetNode(node.x, node.y).SetIsInRange(true);
        }

        pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
        pathfinding.GetNode(x, y).SetIsIA(false);

        pathVectorList = pathfinding.FindPath(GetPosition(), targetPosition);
        secondPathVectorList = pathfinding.FindSecondPath(targetPosition, secondTargetPosition); //Ir del enemigo a muro
        thirdPathVectorList = pathfinding.FindSecondPath(secondTargetPosition, thirdTargetPosition); //Ir del enemigo a muro despues de saltar


        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }

        if (secondPathVectorList != null && secondPathVectorList.Count > 1)
        {
            secondPathVectorList.RemoveAt(0);
        }

        if (thirdPathVectorList != null && thirdPathVectorList.Count > 1)
        {
            thirdPathVectorList.RemoveAt(0);
        }
    }

    void TakeDecisions(PathNode nodoActual)
    {

        range = pathfinding.GetRangeList(pathfinding.GetNode(nodoActual.x, nodoActual.y));
        PathNode closestPlayer = null;
        PathNode closestIA = null;
        PathNode closestAmmo = null;
        PathNode farthestHiding = null;
        float minDistPlayer = 10000;
        float minDistIA = 10000;
        float minDistAmmo = 10000;
        float maxDistHiding = 0;

        PathNode closestPlayerNotInRange = null;
        PathNode closestHidingToPlayer = null;
        float minDistPlayerNotInRange = 10000;
        float minDistHiding = 10000;

        //Busco el player más cercano
        for (int x = 0; x < pathfinding.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < pathfinding.GetGrid().GetHeigth(); y++)
            {
                if (pathfinding.GetNode(x, y).isPlayer)
                {
                    if (minDistPlayerNotInRange > Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(x, y)))
                    {
                        closestPlayerNotInRange = pathfinding.GetNode(x, y);
                        minDistPlayerNotInRange = Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(x, y));

                    }
                }
            }
        }

        foreach (PathNode node in range) //Guardar jugador o IA más cercanos
        {
            if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra jugador en rango
            {
                if (minDistPlayer > Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(node.x, node.y)))
                {
                    closestPlayer = node;
                    minDistPlayer = Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(node.x, node.y));

                    //if (Test.saltado)
                    //pathfinding.GetNode(node.x, node.y).SetIsIA(false);//Si la IA ya ha habia saltado a un jugador no lo vuelve a saltar

                    //HandleMovement();  // Cuando la IA se quede en una casilla de jugador y no hay muros cerca, ni otro jugador se mueve aleatoriamente el rango m�ximo
                }
            }

            if (pathfinding.GetNode(node.x, node.y).isIA) //Se encuentra IA en rango
            {

                if (minDistIA > Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(node.x, node.y)))
                {
                    //if (!Test.saltado)
                    //{
                    closestIA = node;
                    minDistIA = Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(node.x, node.y));
                    //}
                }
            }

            if (pathfinding.GetNode(node.x, node.y).isAmmo) //Se encuentra munición en rango
            {
                if (minDistAmmo > Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(node.x, node.y)))
                {
                    closestAmmo = node;
                    minDistAmmo = Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(node.x, node.y));
                }
            }

            if (pathfinding.GetNode(node.x, node.y).isTWall || pathfinding.GetNode(node.x, node.y).isSWall) //Se encuentra escondite
            {
                List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(node.x, node.y));
                foreach (PathNode neighbourNode in neighbours)
                {
                    if (range.Contains(neighbourNode))
                    {
                        if (maxDistHiding < Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(neighbourNode.x, neighbourNode.y)))
                        {
                            maxDistHiding = Vector2.Distance(new Vector2(nodoActual.x, nodoActual.y), new Vector2(neighbourNode.x, neighbourNode.y));
                            farthestHiding = neighbourNode;
                        }

                        if (minDistHiding > Vector2.Distance(new Vector2(closestPlayerNotInRange.x, closestPlayerNotInRange.y), new Vector2(neighbourNode.x, neighbourNode.y)))
                        {
                            minDistHiding = Vector2.Distance(new Vector2(closestPlayerNotInRange.x, closestPlayerNotInRange.y), new Vector2(neighbourNode.x, neighbourNode.y));
                            closestHidingToPlayer = neighbourNode;
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

            //coger su game object
            foreach (GameObject player in players)
            {
                pathfinding.GetGrid().GetXY(player.transform.position, out int x, out int y);
                if (pathfinding.GetNode(x, y) == closestPlayer)
                {
                    playerGO = player;
                }

            }
            if (playerGO == null)
            {
                Debug.Log("null palyer");
            }

            if (gameObject.GetComponent<EnemyHealth>().currentHealth >= 50) //Vida IA >= 50% (Al poner + de un enemy cambiar esto)
            {
                Debug.Log("Vida IA >= 50%");

                PathNode smallWall = null;
                PathNode choosenHalfHiding = null;
                PathNode tallWall = null;
                PathNode choosenFullHiding = null;
                PathNode ammoPlayer = null;
                Vector2 vectorNodoClosestPlayer = new Vector2(closestPlayer.x, closestPlayer.y);


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
                        ammoPlayer = node;
                    }
                }

                if (smallWall != null) //En rango de closestPlayer hay smallWall
                {
                    Debug.Log("En rango de closestPlayer hay smallWall");

                    //Coge choosenHalfHiding más lejano a player de ese smallWall
                    float maxDist = 0;
                    List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(smallWall.x, smallWall.y));
                    foreach (PathNode neighbourNode in neighbours)
                    {
                        if (playerRange.Contains(neighbourNode))
                        {
                            if (maxDist < Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y)))
                            {
                                maxDist = Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y));
                                choosenHalfHiding = neighbourNode;
                            }
                        }

                    }

                    if (totalBullets > 0 && playerGO.GetComponent<PlayerHealth>().currentHealth > 20) //Tiene balas y vida closestPlayer > 20 (Aquí mirar la del closestPlayer)
                    {
                        Debug.Log("Tiene balas y vida closestPlayer > 20");
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHiding.x, choosenHalfHiding.y));

                        //Disparar desde choosenHalfHiding
                        shoot = true;
                        target = closestPlayer;
                    }

                    else //O no tiene balas o vida closestPlayer < 20
                    {
                        Debug.Log("O no tiene balas o vida closestPlayer < 20");
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHiding.x, choosenHalfHiding.y));
                    }
                }

                else if (tallWall != null) //En rango de closestPlayer hay tallWall
                {
                    Debug.Log("En rango de closestPlayer hay tallWall");

                    float maxDist = 0;
                    List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(tallWall.x, tallWall.y));
                    foreach (PathNode neighbourNode in neighbours)
                    {
                        if (playerRange.Contains(neighbourNode))
                        {
                            if (maxDist < Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y)))
                            {
                                maxDist = Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y));
                                choosenFullHiding = neighbourNode;
                            }
                        }

                    }

                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenFullHiding.x, choosenFullHiding.y));
                }

                else //Si ataca al enemigo no tiene muros en los que esconderse despues
                {

                    if (farthestHiding != null && totalBullets > 0) //En rango de IA hay cualquier Wall && IA tiene balas
                    {
                        Debug.Log("En rango de IA hay cualquier Wall && IA tiene balas");
                        GameObject bulletInstance = Instantiate(m_Bullet, transform.position, Quaternion.identity) as GameObject;
                        Vector3 shootDir = (pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y+1) - bulletInstance.transform.position).normalized;
                        bulletInstance.GetComponent<EnemyBullet>().SetUp(shootDir, closestPlayer);
                        totalBullets -= 1;
                        Debug.Log("Quedan: " + totalBullets);

                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(farthestHiding.x, farthestHiding.y)); //Va al escondite más lejano

                    }

                    else //No tiene muros cerca o no tiene balas
                    {
                        if (ammoPlayer != null) //Hay balas en rango de closestPlayer
                        {
                            Debug.Log("Hay balas en rango de closestPlayer");
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(ammoPlayer.x, ammoPlayer.y));
                            //Y dispara
                            shoot = true;
                            target = closestPlayer;
                        }

                        else if (closestAmmo != null) //Hay balas en rango de IA
                        {
                            Debug.Log("Hay balas en mi rango");
                            List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(closestAmmo.x, closestAmmo.y));
                            PathNode playerNearAmmo = null;
                            float minDistPlayerAmmo = 1000;

                            foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                            {
                                if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                                {
                                    if (minDistPlayerAmmo > Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y)))
                                    {
                                        playerNearAmmo = node;
                                        minDistPlayerAmmo = Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y));
                                    }
                                }
                            }

                            if (playerNearAmmo != null) //Hay player en rango de balas
                            {
                                Debug.Log("Hay player en rango de balas");
                                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y));
                                //Y Dispara
                                shoot = true;
                                target = playerNearAmmo;
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
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), transform.position);

                        }
                    }
                }
            }

            else //Vida IA < 50%
            {
                Debug.Log("Vida IA < 50%");

                PathNode smallWall = null;
                PathNode choosenHalfHiding = null;
                PathNode tallWall = null;
                PathNode choosenFullHiding = null;
                Vector2 vectorNodoClosestPlayer = new Vector2(closestPlayer.x, closestPlayer.y);


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
                }

                if (tallWall != null) //En rango de closestPlayer hay tallWall
                {
                    Debug.Log("En rango de closestPlayer hay tallWall");

                    //Coge choosenFullHiding más lejano a player de ese tallWall
                    float maxDist = 0;
                    List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(tallWall.x, tallWall.y));
                    foreach (PathNode neighbourNode in neighbours)
                    {
                        if (playerRange.Contains(neighbourNode))
                        {
                            if (maxDist < Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y)))
                            {
                                maxDist = Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y));
                                choosenFullHiding = neighbourNode;
                            }
                        }
                    }

                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenFullHiding.x, choosenFullHiding.y));

                }

                else if (smallWall != null) //En rango de closestPlayer hay smallWall
                {
                    Debug.Log("En rango de closestPlayer hay smallWall");

                    //Coge choosenHalfHiding más lejano a player de ese smallWall
                    float maxDist = 0;
                    List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(smallWall.x, smallWall.y));
                    foreach (PathNode neighbourNode in neighbours)
                    {
                        if (playerRange.Contains(neighbourNode))
                        {
                            if (maxDist < Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y)))
                            {
                                maxDist = Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y));
                                choosenHalfHiding = neighbourNode;
                            }
                        }
                    }

                    if (totalBullets > 0 && playerGO.GetComponent<PlayerHealth>().currentHealth > 20) //Tiene balas y vida closestPlayer > 20 (Aquí mirar la del closestPlayer)
                    {
                        Debug.Log("Tiene balas y vida closestPlayer > 20");
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHiding.x, choosenHalfHiding.y));
                        //Disparar desde choosenHalfHiding
                        shoot = true;
                        target = closestPlayer;
                    }

                    else //O no tiene balas o vida closestPlayer < 20
                    {
                        Debug.Log("O no tiene balas o vida closestPlayer < 20");
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHiding.x, choosenHalfHiding.y));

                    }
                }


                else //Si ataca al enemigo no tiene muros en los que esconderse despues
                {

                    if (farthestHiding != null && totalBullets > 0) //En rango de IA hay cualquier Wall && IA tiene balas
                    {
                        Debug.Log("En rango de IA hay cualquier Wall && IA tiene balas");
                        GameObject bulletInstance = Instantiate(m_Bullet, transform.position, Quaternion.identity) as GameObject;
                        Vector3 shootDir = (pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y+1) - bulletInstance.transform.position).normalized;
                        bulletInstance.GetComponent<EnemyBullet>().SetUp(shootDir, closestPlayer);
                        totalBullets -= 1;
                        Debug.Log("Quedan: " + totalBullets);

                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(farthestHiding.x, farthestHiding.y)); //Va al escondite más lejano

                    }

                    else //No tiene muros cerca o no tiene balas
                    {
                        if (closestAmmo != null) //Hay balas en rango de IA
                        {
                            Debug.Log("Hay balas en mi rango");
                            List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(closestAmmo.x, closestAmmo.y));
                            PathNode playerNearAmmo = null;
                            float minDistPlayerAmmo = 1000;

                            foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                            {
                                if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                                {
                                    if (minDistPlayerAmmo > Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y)))
                                    {
                                        playerNearAmmo = node;
                                        minDistPlayerAmmo = Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y));
                                    }
                                }
                            }

                            if (playerNearAmmo != null) //Hay player en rango de balas
                            {
                                Debug.Log("Hay player en rango de balas");
                                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y));
                                //Y Dispara
                                shoot = true;
                                target = playerNearAmmo;
                            }

                            else //No hay player en rango de balas
                            {
                                Debug.Log("No hay player en rango de balas");
                                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y));
                            }

                        }

                        else //Imposible llegar a balas
                        {
                            Debug.Log("Imposible llegar a balas, Crea muro");
                            IACreatesWall(closestPlayer, nodoActual);
                        }
                    }
                }
            }
        }

        else //No hay Player en rango
        {
            Debug.Log("No hay Player en rango");

            if (closestIA != null) //Hay IA en rango        
            {
                Debug.Log("Hay IA en rango");

                List<PathNode> IARange = pathfinding.GetRangeList(pathfinding.GetNode(closestIA.x, closestIA.y)); //Obtener rango de IA amiga

                PathNode ammoNearIA = null;
                PathNode farthestHidingIA = null;
                float maxDistHidingIA = 0;
                PathNode closestPlayerToAttack = null;


                foreach (PathNode node in IARange)
                {

                    if (pathfinding.GetNode(node.x, node.y).isAmmo)   //Hay munición
                    {
                        ammoNearIA = node;
                    }

                    if (pathfinding.GetNode(node.x, node.y).isPlayer)   //Hay jugador
                    {
                        closestPlayerToAttack = node;
                    }

                    if (pathfinding.GetNode(node.x, node.y).isTWall || pathfinding.GetNode(node.x, node.y).isSWall) //Se encuentra escondite
                    {
                        List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(node.x, node.y));
                        foreach (PathNode neighbourNode in neighbours)
                        {
                            if (IARange.Contains(neighbourNode))
                            {
                                if (maxDistHidingIA < Vector2.Distance(new Vector2(closestIA.x, closestIA.y), new Vector2(neighbourNode.x, neighbourNode.y)))
                                {
                                    maxDistHidingIA = Vector2.Distance(new Vector2(closestIA.x, closestIA.y), new Vector2(neighbourNode.x, neighbourNode.y));
                                    farthestHidingIA = neighbourNode;
                                }
                            }
                        }
                    }
                }

                if (closestPlayerToAttack != null) //Hay Player en rango de closestIA
                {
                    Debug.Log("Hay Player en rango de closestIA");

                    List<PathNode> IAPlayerRange = pathfinding.GetRangeList(pathfinding.GetNode(closestPlayerToAttack.x, closestPlayerToAttack.y)); //Obtener rango de closestPlayerToAttack
                    PathNode smallWallPlayer = null;
                    PathNode choosenHalfHidingAfterJump = null;
                    PathNode tallWallPlayer = null;
                    PathNode choosenFullHidingAfterJump = null;
                    PathNode ammoNearPlayer = null;
                    Vector2 vectorNodoClosestPlayer = new Vector2(closestPlayerToAttack.x, closestPlayerToAttack.y);

                    PathNode farthestNode = null;
                    float maxDistfarthestNode = 0;



                    foreach (PathNode node in IAPlayerRange)
                    {
                        if (pathfinding.GetNode(node.x, node.y).isSWall)   //Hay muro bajo
                        {
                            smallWallPlayer = node;
                        }

                        if (pathfinding.GetNode(node.x, node.y).isTWall)   //Hay muro alto
                        {
                            tallWallPlayer = node;
                        }

                        if (pathfinding.GetNode(node.x, node.y).isAmmo)   //Hay munición
                        {
                            ammoNearPlayer = node;
                        }

                        if (!pathfinding.GetNode(node.x, node.y).isPlayer && !pathfinding.GetNode(node.x, node.y).isIA && !pathfinding.GetNode(node.x, node.y).isTWall && !pathfinding.GetNode(node.x, node.y).isSWall)
                        {
                            if (maxDistfarthestNode < Vector2.Distance(new Vector2(closestPlayerToAttack.x, closestPlayerToAttack.y), new Vector2(node.x, node.y)))
                            {
                                farthestNode = node;
                                maxDistfarthestNode = Vector2.Distance(new Vector2(closestPlayerToAttack.x, closestPlayerToAttack.y), new Vector2(node.x, node.y));
                            }
                        }


                    }

                    //coger su game object
                    foreach (GameObject player in players)
                    {
                        pathfinding.GetGrid().GetXY(player.transform.position, out int x, out int y);
                        if (pathfinding.GetNode(x, y) == closestPlayerToAttack)
                        {
                            playerGO = player;
                        }

                    }

                    if(playerGO == null)
                    {
                        Debug.Log("null palyer");
                    }


                    if (gameObject.GetComponent<EnemyHealth>().currentHealth >= 50) //IA vida >= 50
                    {
                        Debug.Log("IA vida >= 50");

                        if (smallWallPlayer != null)
                        {
                            Debug.Log("En rango de closestPlayerToAtack hay smallWall");

                            //Coge choosenHalfHiding más lejano a player de ese smallWall
                            float maxDist = 0;
                            List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(smallWallPlayer.x, smallWallPlayer.y));
                            foreach (PathNode neighbourNode in neighbours)
                            {
                                if (IAPlayerRange.Contains(neighbourNode))
                                {
                                    if (maxDist < Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y)))
                                    {
                                        maxDist = Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y));
                                        choosenHalfHidingAfterJump = neighbourNode;
                                    }
                                }
                            }

                            if (totalBullets > 0 && playerGO.GetComponent<PlayerHealth>().currentHealth > 20) //Tiene balas && vida del closestPlayer > 20 (Aquí mirar la del closestPlayer)
                            {
                                Debug.Log("Tiene balas && vida del closestPlayerToAtack > 20");
                                //Salta, ataca, se esconde y dispara
                                SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHidingAfterJump.x, choosenHalfHidingAfterJump.y));
                                shoot = true;
                                target = closestPlayerToAttack;
                            }

                            else
                            {
                                Debug.Log("No tiene balas o vida del closestPlayerToAtack < 20");
                                //Salta, ataca y se esconde
                                SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHidingAfterJump.x, choosenHalfHidingAfterJump.y));
                            }


                        }

                        else if (tallWallPlayer != null) //En rango de closestPlayer hay tallWall
                        {
                            Debug.Log("En rango de closestPlayerToAtack hay tallWall");
                            //Coge choosenFullHiding más lejano a player de ese tallWall
                            float maxDist = 0;
                            List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(tallWallPlayer.x, tallWallPlayer.y));
                            foreach (PathNode neighbourNode in neighbours)
                            {
                                if (IAPlayerRange.Contains(neighbourNode))
                                {
                                    if (maxDist < Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y)))
                                    {
                                        maxDist = Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y));
                                        choosenFullHidingAfterJump = neighbourNode;
                                    }
                                }
                            }

                            //Salta, ataca y se esconde
                            SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(choosenFullHidingAfterJump.x, choosenFullHidingAfterJump.y));

                        }

                        else if (totalBullets > 0) //IA tiene balas
                        {
                            //Salta, ataca, se aleja y dispara
                            Debug.Log("tiene balas");
                            SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(farthestNode.x, farthestNode.y));
                            shoot = true;
                            target = closestPlayerToAttack;
                        }

                        else //No hay muros cerca del closestPlayer de la closestIA o no tiene munición
                        {
                            Debug.Log("No hay muros cerca del closestPlayer de la closestIA o no tiene munición");
                            if (ammoNearPlayer != null) //Balas en rango de closestPlayer
                            {
                                Debug.Log("Balas en rango de closestPlayerToAtack");
                                //Salta, ataca, va a las balas y dispara
                                SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(ammoNearPlayer.x, ammoNearPlayer.y));
                                shoot = true;
                                target = closestPlayerToAttack;
                            }

                            else if (ammoNearIA != null) //balas en el rango de closestIA
                            {
                                Debug.Log("Balas en rango de closestIA");
                                //Encontrar jugador en rango de balas
                                List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(ammoNearIA.x, ammoNearIA.y));
                                PathNode playerNearAmmo = null;
                                float minDistPlayerAmmo = 10000;

                                foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                                {
                                    if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                                    {
                                        if (minDistPlayerAmmo > Vector2.Distance(new Vector2(ammoNearIA.x, ammoNearIA.y), new Vector2(node.x, node.y)))
                                        {
                                            playerNearAmmo = node;
                                            minDistPlayerAmmo = Vector2.Distance(new Vector2(ammoNearIA.x, ammoNearIA.y), new Vector2(node.x, node.y));
                                        }
                                    }
                                }

                                if (playerNearAmmo != null) //Hay Player en rango de munición
                                {
                                    Debug.Log("Hay Player en rango de Balas");
                                    //Salta, va a las balas y dispara
                                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                                    shoot = true;
                                    target = playerNearAmmo;
                                }
                                else
                                {
                                    Debug.Log("No Hay Player en rango de Balas");
                                    //Salta y va a las balas
                                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                                }
                            }
                            else //No hay muros cerca del closestPlayer de la closestIA o no tiene munición y no hay balas en el rango de la closestIA
                            {
                                Debug.Log("No hay muros cerca del closestPlayer de la closestIA o no tiene munición y no hay balas en el rango de la closestIA");
                                //Salta, ataca y se aleja
                                SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(farthestNode.x, farthestNode.y));

                            }
                        }
                    }
                    else //IA vida < 50
                    {
                        Debug.Log("IA vida < 50");
                        if (tallWallPlayer != null) //En rango de closestPlayer hay tallWall
                        {
                            Debug.Log("En rango de closestPlayerToAtack hay tallWall");
                            //Coge choosenFullHiding más lejano a player de ese tallWall
                            float maxDist = 0;
                            List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(tallWallPlayer.x, tallWallPlayer.y));
                            foreach (PathNode neighbourNode in neighbours)
                            {
                                if (IAPlayerRange.Contains(neighbourNode))
                                {
                                    if (maxDist < Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y)))
                                    {
                                        maxDist = Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y));
                                        choosenFullHidingAfterJump = neighbourNode;
                                    }
                                }
                            }

                            //Salta, ataca y se esconde
                            SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(choosenFullHidingAfterJump.x, choosenFullHidingAfterJump.y));

                        }

                        else if (smallWallPlayer != null) //En rango de closestPlayer hay smallWall
                        {
                            Debug.Log("En rango de closestPlayerToAtack hay smallWall");
                            //Coge choosenHalfHiding más lejano a player de ese smallWall
                            float maxDist = 0;
                            List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(smallWallPlayer.x, smallWallPlayer.y));
                            foreach (PathNode neighbourNode in neighbours)
                            {
                                if (IAPlayerRange.Contains(neighbourNode))
                                {
                                    if (maxDist < Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y)))
                                    {
                                        maxDist = Vector2.Distance(vectorNodoClosestPlayer, new Vector2(neighbourNode.x, neighbourNode.y));
                                        choosenHalfHidingAfterJump = neighbourNode;
                                    }
                                }
                            }

                            if (totalBullets > 0 && playerGO.GetComponent<PlayerHealth>().currentHealth > 20) //Tiene balas && vida del closestPlayer > 20 (Aquí mirar la del closestPlayer)
                            {
                                Debug.Log("Tiene balas && vida del closestPlayerToAtack > 20");
                                //Salta, ataca, se esconde y dispara
                                SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHidingAfterJump.x, choosenHalfHidingAfterJump.y));
                                shoot = true;
                                target = closestPlayerToAttack;
                            }

                            else
                            {
                                Debug.Log("No tiene balas o vida del closestPlayerToAtack < 20");
                                //Salta, ataca y se esconde
                                SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHidingAfterJump.x, choosenHalfHidingAfterJump.y));

                            }

                        }

                        else if (totalBullets > 0) //IA tiene balas
                        {
                            Debug.Log("tiene balas");
                            //Salta, ataca, se aleja y dispara
                            SetThreeTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y), pathfinding.GetGrid().GetWorldPosition(farthestNode.x, farthestNode.y));
                            shoot = true;
                            target = closestPlayerToAttack;
                        }

                        else //No hay muros en rango de player, poca vida y no tiene munición
                        {

                            Debug.Log("No hay muros cerca del closestPlayer de la closestIA o no tiene munición");
                            if (ammoNearIA != null) //balas en el rango de closestIA
                            {
                                Debug.Log("Balas en rango de closestIA");
                                //Encontrar jugador en rango de balas
                                List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(ammoNearIA.x, ammoNearIA.y));
                                PathNode playerNearAmmo = null;
                                float minDistPlayerAmmo = 10000;

                                foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                                {
                                    if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                                    {
                                        if (minDistPlayerAmmo > Vector2.Distance(new Vector2(ammoNearIA.x, ammoNearIA.y), new Vector2(node.x, node.y)))
                                        {
                                            playerNearAmmo = node;
                                            minDistPlayerAmmo = Vector2.Distance(new Vector2(ammoNearIA.x, ammoNearIA.y), new Vector2(node.x, node.y));
                                        }
                                    }
                                }

                                if (playerNearAmmo != null) //Hay Player en rango de munición
                                {
                                    Debug.Log("Hay Player en rango de Balas");
                                    //Salta, va a las balas y dispara
                                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                                    shoot = true;
                                    target = playerNearAmmo;
                                }
                                else
                                {
                                    Debug.Log("No Hay Player en rango de Balas");
                                    //Salta y va a las balas
                                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                                }
                            }

                            else //No hay muros cerca del closestPlayer de la closestIA o no tiene munición y no hay balas en el rango de la closestIA
                            {
                                Debug.Log("No hay muros cerca del closestPlayer de la closestIA o no tiene munición y no hay balas en el rango de la closestIA");

                                if (closestAmmo != null) //Hay balas en su propio rango
                                {
                                    Debug.Log("Hay balas en mi rango");
                                    List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(closestAmmo.x, closestAmmo.y));
                                    PathNode playerNearAmmo = null;
                                    float minDistPlayerAmmo = 10000;

                                    foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                                    {
                                        if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                                        {
                                            if (minDistPlayerAmmo > Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y)))
                                            {
                                                playerNearAmmo = node;
                                                minDistPlayerAmmo = Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y));
                                            }
                                        }
                                    }

                                    if (playerNearAmmo != null) //En rango de balas hay player
                                    {
                                        Debug.Log("En rango de balas hay player");
                                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                                        //Disparar
                                        shoot = true;
                                        target = playerNearAmmo;
                                    }
                                    else
                                    {
                                        Debug.Log("En rango de balas no hay player");
                                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                                    }
                                }
                                else //No hay balas en su propio rango
                                {
                                    Debug.Log("No hay balas en su propio rango");
                                    IACreatesWall(closestPlayerNotInRange, nodoActual); //Crea un muro entre IA y player
                                }
                            }
                        }
                    }
                }
                else //No hay player en rango de closestIA
                {
                    if (ammoNearIA != null) //balas en el rango de closestIA
                    {
                        Debug.Log("balas en el rango de closestIA");
                        //Encontrar jugador en rango de balas
                        List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(ammoNearIA.x, ammoNearIA.y));
                        PathNode playerNearAmmo = null;
                        float minDistPlayerAmmo = 10000;

                        foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                        {
                            if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                            {
                                if (minDistPlayerAmmo > Vector2.Distance(new Vector2(ammoNearIA.x, ammoNearIA.y), new Vector2(node.x, node.y)))
                                {
                                    playerNearAmmo = node;
                                    minDistPlayerAmmo = Vector2.Distance(new Vector2(ammoNearIA.x, ammoNearIA.y), new Vector2(node.x, node.y));
                                }
                            }
                        }

                        if (playerNearAmmo != null) //Hay Player en rango de munición
                        {
                            Debug.Log("Hay Player en rango de munición");
                            //Salta y va a las balas
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                            //Y dispara
                            shoot = true;
                            target = playerNearAmmo;
                        }
                        else
                        {
                            Debug.Log("No hay Player en rango de munición");
                            //Salta y va a las balas
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                        }
                    }

                    else if (farthestHidingIA != null) //En rango de closestIA hay cualquier Wall
                    {
                        //Salta y va al escondite más lejano
                        Debug.Log("escondite en amigo iA");
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(farthestHidingIA.x, farthestHidingIA.y));
                    }

                    else //No hay muros cerca del closestPlayer de la closestIA o no tiene munición y no hay balas en el rango de la closestIA
                    {
                        Debug.Log("No hay muros cerca del closestPlayer de la closestIA o no tiene munición y no hay balas en el rango de la closestIA");
                        if (closestAmmo != null) //Hay balas en su propio rango
                        {
                            Debug.Log("Hay balas en mi rango");
                            List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(closestAmmo.x, closestAmmo.y));
                            PathNode playerNearAmmo = null;
                            float minDistPlayerAmmo = 10000;

                            foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                            {
                                if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                                {
                                    if (minDistPlayerAmmo > Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y)))
                                    {
                                        playerNearAmmo = node;
                                        minDistPlayerAmmo = Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y));
                                    }
                                }
                            }

                            if (playerNearAmmo != null) //En rango de balas hay player
                            {
                                Debug.Log("En rango de balas hay player");
                                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                                shoot = true;
                                target = playerNearAmmo;
                            }
                            else
                            {
                                Debug.Log("En rango de balas no hay player");
                                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                            }
                        }
                        else //No hay balas en su propio rango
                        {
                            Debug.Log("No hay balas en su propio rango");
                            IACreatesWall(closestPlayerNotInRange, nodoActual); //Crea un muro entre IA y player
                        }
                    }
                }
            }

            else //No hay nadie en rango
            {
                Debug.Log("No hay nadie en rango");
                if (totalBullets <= 1)
                {
                    Debug.Log("balas <= 1");
                    if (closestAmmo != null) //Hay munición en rango
                    {
                        Debug.Log("Hay munición en rango");
                        List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(closestAmmo.x, closestAmmo.y));
                        PathNode playerNearAmmo = null;
                        float minDistPlayerAmmo = 10000;

                        foreach (PathNode node in AmmoRange) //Guardar player en rango de munición
                        {
                            if (pathfinding.GetNode(node.x, node.y).isPlayer) //Se encuentra player en rango de munición
                            {
                                if (minDistPlayerAmmo > Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y)))
                                {
                                    playerNearAmmo = node;
                                    minDistPlayerAmmo = Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(node.x, node.y));
                                }
                            }
                        }

                        if (playerNearAmmo != null) //Hay Player en rango de munición
                        {
                            Debug.Log("Hay Player en rango de munición");
                            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                            shoot = true;
                            target = playerNearAmmo;
                        }

                        else //No hay Player en rango de munición
                        {
                            Debug.Log("No hay Player en rango de munición");
                            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                        }
                    }

                    else if (closestHidingToPlayer != null) //No hay balas cerca, pero hay muros
                    {
                        Debug.Log("No hay balas cerca, pero hay muros");
                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestHidingToPlayer.x, closestHidingToPlayer.y)); //Va al hiding más cercano al player más cercano
                    }

                    else //No hay player, ni IA, ni balas ni muros cerca
                    {
                        Debug.Log("No hay player, ni IA, ni balas ni muros cerca");
                        IACreatesWall(closestPlayerNotInRange, nodoActual); //Crea un muro entre él y el player
                    }
                }

                else if (closestHidingToPlayer != null) //Hay muro cerca y voy bien de munición
                {
                    Debug.Log("Hay muro cerca y voy bien de munición");
                    SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestHidingToPlayer.x, closestHidingToPlayer.y)); //Va al hiding más cercano al player más cercano
                }

                else //No hay player, ni IA, ni balas ni muros cerca
                {
                    Debug.Log("No hay player, ni IA, ni balas ni muros cerca");
                    IACreatesWall(closestPlayerNotInRange, nodoActual);
                }
            }
        }


        Test.ammoReload++;
    }

    void PassToPlayerTurn()
    {
        IAEnd = false;
        Test.returnTurn = true;
        Test.isIATurn = false;

        if (Test.enemy3.activeSelf)
            Test.IATurn = 3;

        else if (Test.enemy1.activeSelf)
            Test.IATurn = 1;

        else
            Test.IATurn = 2;
    }

    void IACreatesWall(PathNode closestPlayer, PathNode actualNode)
    {
        //Que pasa si el player esta justo al lado?

        int x;
        int y;

        if (closestPlayer.x == actualNode.x && closestPlayer.y > actualNode.y)
        {
            x = actualNode.x;
            y = actualNode.y + 1;
        }

        else if (closestPlayer.x > actualNode.x && closestPlayer.y > actualNode.y)
        {
            x = actualNode.x + 1;
            y = actualNode.y + 1;
        }

        else if (closestPlayer.x > actualNode.x && closestPlayer.y == actualNode.y)
        {
            x = actualNode.x + 1;
            y = actualNode.y;
        }

        else if (closestPlayer.x > actualNode.x && closestPlayer.y < actualNode.y)
        {
            x = actualNode.x + 1;
            y = actualNode.y - 1;
        }

        else if (closestPlayer.x == actualNode.x && closestPlayer.y < actualNode.y)
        {
            x = actualNode.x;
            y = actualNode.y - 1;
        }

        else if (closestPlayer.x < actualNode.x && closestPlayer.y < actualNode.y)
        {
            x = actualNode.x - 1;
            y = actualNode.y - 1;
        }

        else if (closestPlayer.x < actualNode.x && closestPlayer.y == actualNode.y)
        {
            x = actualNode.x - 1;
            y = actualNode.y;
        }

        else
        {
            x = actualNode.x - 1;
            y = actualNode.y + 1;
        }

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

        PassToPlayerTurn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ammo"))
        {
            totalBullets++;
            pathfinding.GetGrid().GetXY(other.gameObject.transform.position, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsAmmo(false);
            Destroy(other.gameObject);
        }
    }
}
