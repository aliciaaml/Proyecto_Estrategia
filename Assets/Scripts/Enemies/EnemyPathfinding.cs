using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    public float speed = 15f;
    Pathfinding pathfinding;
    List<Vector3> pathVectorList;
    int currentPathIndex;
    List<Vector3> secondPathVectorList;
    int secondPathIndex;

    public List<PathNode> range = new List<PathNode>();
    public bool IAEnd;

    public GameObject bulletAmmo;
    public GameObject m_TWall;

    public static int totalWalls = 3;

    public GameObject m_Bullet;
    int totalBullets = 3;


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
        if (Test.isIATurn && Test.IATurn == 1)
        {
            Debug.Log("Turno IA 1");
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

                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }

            else
            {
                currentPathIndex++;

                if (currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList = null;
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
                }
            }
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
        float minDistPlayerFromAmmo = 10000;
        PathNode closestPlayerFromAmmo = null;


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
                    }
                }
            }

            if(closestAmmo != null) //Se encuentra jugador en rango de munición
            {
                List<PathNode> myAmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(closestAmmo.x, closestAmmo.y));

                foreach (PathNode myNode in myAmmoRange) //Guardar player en rango de munición
                {
                    if (pathfinding.GetNode(myNode.x, myNode.y).isPlayer) //Se encuentra player en rango de munición
                    {
                        if (minDistPlayerFromAmmo > Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(myNode.x, myNode.y)))
                        {
                            closestPlayerFromAmmo = myNode;
                            minDistPlayerFromAmmo = Vector2.Distance(new Vector2(closestAmmo.x, closestAmmo.y), new Vector2(myNode.x, myNode.y));
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

                    if (totalBullets > 0 && PlayerHealth.currentHealth > 20) //Tiene balas y vida closestPlayer > 20 (Aquí mirar la del closestPlayer)
                    {
                        Debug.Log("Tiene balas y vida closestPlayer > 20");
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHiding.x, choosenHalfHiding.y));
                        
                        //Disparar desde choosenHalfHiding

                        pathfinding.GetGrid().GetXY(transform.position, out int x, out int y);

                        if (pathfinding.GetNode(x,y).isHalfHiding){

                            Debug.Log("ajñsldfkjasdlfkjsdfkñjsdñflk");

                            GameObject bulletInstance = Instantiate(m_Bullet, transform.position, Quaternion.identity) as GameObject;
                            Vector3 shootDir = (pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y) - bulletInstance.transform.position).normalized;
                            bulletInstance.GetComponent<Bullet>().SetUp(shootDir, pathfinding.GetNode(closestPlayer.x, closestPlayer.y)); 

                            totalBullets-=1;
                            Debug.Log("Quedan: " + totalBullets);


                        }
                    }

                    else //O no tiene balas o vida closestPlayer < 20
                    {
                        Debug.Log("O no tiene balas o vida closestPlayer < 20");
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHiding.x, choosenHalfHiding.y));
                    }
                }

                else if (tallWall != null) //En rango de closestPlayer hay tallWall
                {
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

                    Debug.Log("En rango de closestPlayer hay tallWall");
                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenFullHiding.x, choosenFullHiding.y));
                }

                else //Si ataca al enemigo no tiene muros en los que esconderse despues
                {
                    
                    if (farthestHiding != null && totalBullets > 0) //En rango de IA hay cualquier Wall && IA tiene balas
                    {
                        Debug.Log("En rango de IA hay cualquier Wall && IA tiene balas");
                        //Disparar (FALTA)
                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(farthestHiding.x, farthestHiding.y)); //Va al escondite más lejano

                    }

                    else //No tiene muros cerca o no tiene balas
                    {
                        if (ammoPlayer != null) //Hay balas en rango de closestPlayer
                        {
                            Debug.Log("Hay balas en rango de closestPlayer");
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(ammoPlayer.x, ammoPlayer.y));
                            //Y dispara
                        }

                        else if (closestAmmo != null) //Hay balas en rango de IA
                        {
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

                    if (totalBullets > 0 && PlayerHealth.currentHealth > 20) //Tiene balas y vida closestPlayer > 20 (Aquí mirar la del closestPlayer)
                    {
                        Debug.Log("Tiene balas y vida closestPlayer > 20");
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHiding.x, choosenHalfHiding.y));
                        //Disparar desde choosenHalfHiding
                    }

                    else //O no tiene balas o vida closestPlayer < 20
                    {
                        Debug.Log("O no tiene balas o vida closestPlayer < 20");
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestPlayer.x, closestPlayer.y), pathfinding.GetGrid().GetWorldPosition(choosenHalfHiding.x, choosenHalfHiding.y));

                    }
                }

                else if (tallWall != null && totalBullets > 0) //Tiene muro alto en rango y balas
                {
                    //Dispara
                    SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(choosenFullHiding.x, choosenFullHiding.y));
                }

                else if (smallWall != null && totalBullets > 0) //Tiene muro bajo en rango y balas
                {
                    //Dispara
                    SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(choosenHalfHiding.x, choosenHalfHiding.y));
                }

                else
                {
                    if (closestAmmo != null) //Hay munición en rango
                    {
                        if (closestAmmo != null && closestPlayerFromAmmo != null) //Hay munición en rango y en el rango del player hay munición
                        {
                            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y));
                            //Dispara
                        }

                        else if (closestAmmo != null) //Hay munición en rango
                        {
                            SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y));
                        }

                        else
                        {
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

                PathNode smallWall = null;
                PathNode choosenHalfHiding = null;
                PathNode tallWall = null;
                PathNode choosenFullHiding = null;
                PathNode ammoNearIA = null;
                PathNode closestPlayerToAttack = null;
                PathNode playerNearAmmo = null;
                float minDistPlayerAmmo = 10000;

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

                    if (pathfinding.GetNode(node.x, node.y).isAmmo)   //Hay munición
                    {
                        ammoNearIA = node;
                    }

                    if (pathfinding.GetNode(node.x, node.y).isPlayer)   //Hay jugador
                    {
                        closestPlayerToAttack = node;
                    }
                }

                if (ammoNearIA != null) //Hay balas en el rango de closestIA
                {
                    //Encontrar jugador en rango de balas
                    List<PathNode> AmmoRange = pathfinding.GetRangeList(pathfinding.GetNode(ammoNearIA.x, ammoNearIA.y));

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
                }

                if (closestPlayerToAttack != null) //Hay Player en rango de closestIA
                {
                    List<PathNode> IAPlayerRange = pathfinding.GetRangeList(pathfinding.GetNode(closestPlayerToAttack.x, closestPlayerToAttack.y)); //Obtener rango de closestPlayerToAttack
                    PathNode smallWallPlayer = null;
                    PathNode choosenHalfHidingAfterJump = null;
                    PathNode tallWallPlayer = null;
                    PathNode choosenFullHidingAfterJump = null;
                    PathNode ammoNearPlayer = null;
                    Vector2 vectorNodoClosestPlayer = new Vector2(closestPlayerToAttack.x, closestPlayerToAttack.y);

                    foreach (PathNode node in IARange)
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
                    }

                    if (smallWallPlayer != null)
                    {
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
                    }

                    if (tallWallPlayer != null)
                    {
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
                    }

                    if (EnemyHealth.currentHealth >= 50) //IA vida >= 50
                    {
                        if (smallWallPlayer != null) //En rango de closestPlayer hay smallWall
                        {
                            if (totalBullets > 0 && PlayerHealth.currentHealth > 20) //Tiene balas && vida del closestPlayer > 20 (Aquí mirar la del closestPlayer)
                            {
                                //Salta y ataca
                                SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                                //y se esconde en choosenHalfHidingAfterJump (hace 3 paths) (FALTA)
                                //Y dispara (FALTA)
                            }

                            else
                            {
                                //Salta y ataca
                                SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                                //y se esconde en choosenHalfHidingAfterJump (hace 3 paths) (FALTA)
                            }
                        }

                        else if (tallWallPlayer != null) //En rango de closestPlayer hay tallWall
                        {
                            //Salta y ataca
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                            //y se esconde en choosenFullHidingAfterJump (hace 3 paths) (FALTA)
                        }

                        else if (totalBullets > 0) //IA tiene balas
                        {
                            //Salta y ataca
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                            //y se ALEJA EN DIRECCIÓN CONTRARIA 3 casillas (-vector2?) (hace 3 paths) (FALTA)
                            //Y dispara (FALTA)
                        }

                        else //No hay muros cerca del closestPlayer de la closestIA o no tiene munición
                        {
                            if (ammoNearPlayer != null) //Balas en rango de closestPlayer
                            {
                                //Salta y ataca
                                SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                                //y voy a las balas (FALTA)
                                //y disparo (FALTA)
                            }

                            else if (ammoNearIA != null) //balas en el rango de closestIA
                            {
                                if (playerNearAmmo != null) //Hay Player en rango de munición
                                {
                                    //Salta y va a las balas
                                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                                    //Y dispara (FALTA)                                                                                         
                                }
                                else
                                {
                                    //Salta y va a las balas
                                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                                }
                            }
                            else //No hay muros cerca del closestPlayer de la closestIA o no tiene munición y no hay balas en el rango de la closestIA
                            {
                                //Salta y ataca
                                SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                                //y se ALEJA EN DIRECCIÓN CONTRARIA 3 casillas (-vector2?) (hace 3 paths) (FALTA)
                            }
                        }
                    }
                    else //IA vida < 50
                    {
                        if (tallWallPlayer != null) //En rango de closestPlayer hay tallWall
                        {
                            //Salta y ataca
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                            //y se esconde en choosenFullHidingAfterJump (hace 3 paths) (FALTA)
                        }
                        else if (smallWallPlayer != null) //En rango de closestPlayer hay smallWall
                        {
                            if (totalBullets > 0 && PlayerHealth.currentHealth > 20) //Tiene balas && vida del closestPlayer > 20 (Aquí mirar la del closestPlayer)
                            {
                                //Salta y ataca
                                SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                                //y se esconde en choosenHalfHidingAfterJump (hace 3 paths) (FALTA)
                                //y dispara (FALTA)
                            }

                            else
                            {
                                //Salta y ataca
                                SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                                //y se esconde en choosenHalfHidingAfterJump (hace 3 paths) (FALTA)
                            }

                        }
                        else if (totalBullets > 0) //IA tiene balas
                        {
                            //Salta y ataca
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(closestPlayerToAttack.x, closestPlayerToAttack.y));
                            //y se ALEJA EN DIRECCIÓN CONTRARIA 3 casillas (-vector2?) (hace 3 paths) (FALTA)
                            //y dispara (FALTA)
                        }
                        else //No hay muros en rango de player, poca vida y no tiene munición
                        {
                            if (ammoNearIA != null) //balas en el rango de closestIA
                            {
                                if (playerNearAmmo != null) //Hay Player en rango de munición
                                {
                                    //Salta y va a las balas
                                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                                    //Y dispara (FALTA)                                                                                         
                                }
                                else
                                {
                                    //Salta y va a las balas
                                    SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                                }
                            }
                            else //No hay muros cerca del closestPlayer de la closestIA o no tiene munición y no hay balas en el rango de la closestIA
                            {
                                if (closestAmmo != null) //Hay balas en su propio rango
                                {
                                    if (closestPlayerFromAmmo != null) //En rango de balas hay player
                                    {
                                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                                        //Disparar (FALTA)
                                    }
                                    else
                                    {
                                        SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                                    }
                                }
                                else //No hay balas en su propio rango
                                {
                                    IACreatesWall(closestPlayer, nodoActual); //Crea un muro entre IA y player
                                }
                            }
                        }
                    }
                }
                else //No hay player en rango de closestIA
                {
                    if (ammoNearIA != null) //Hay balas en rango de closestIA
                    {
                        if (playerNearAmmo != null) //Hay Player en rango de munición
                        {
                            //Salta y va a las balas
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                            //Y dispara (FALTA)                                                                                         
                        }
                        else
                        {
                            //Salta y va a las balas
                            SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(ammoNearIA.x, ammoNearIA.y));
                        }
                    }
                    else if (farthestHiding != null) //En rango de closestIA hay cualquier Wall
                    {
                        //Salta y va al escondite más lejano
                        SetTwoTargetsPosition(pathfinding.GetGrid().GetWorldPosition(closestIA.x, closestIA.y), pathfinding.GetGrid().GetWorldPosition(farthestHiding.x, farthestHiding.y));
                    }
                    else
                    {
                        if (closestAmmo != null) //Hay balas en su propio rango
                        {
                            if (closestPlayerFromAmmo != null) //En rango de balas hay player
                            {
                                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                                //Disparar (FALTA)
                            }
                            else
                            {
                                SetTargetPosition(pathfinding.GetGrid().GetWorldPosition(closestAmmo.x, closestAmmo.y)); //Va a las balas
                            }
                        }
                        else //No hay balas en su propio rango
                        {
                            IACreatesWall(closestPlayerToAttack, nodoActual); //Crea un muro entre IA y player
                        }
                    }
                }
            }

            else //No hay nadie en rango
            {
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

                //Busco el hidding mas cercano al player más cercano
                foreach (PathNode node in range) //Guardar munición y escondites en rango
                {
                    if (pathfinding.GetNode(node.x, node.y).isTWall || pathfinding.GetNode(node.x, node.y).isSWall)
                    {
                        List<PathNode> neighbours = pathfinding.GetNeighbourList(pathfinding.GetNode(node.x, node.y));
                        foreach (PathNode neighbourNode in neighbours)
                        {
                            if (range.Contains(neighbourNode))
                            {
                                if (minDistHiding > Vector2.Distance(new Vector2(closestPlayerNotInRange.x, closestPlayerNotInRange.y), new Vector2(neighbourNode.x, neighbourNode.y)))
                                {
                                    minDistHiding = Vector2.Distance(new Vector2(closestPlayerNotInRange.x, closestPlayerNotInRange.y), new Vector2(neighbourNode.x, neighbourNode.y));
                                    closestHidingToPlayer = neighbourNode;
                                }
                            }

                        }
                    }
                }

                if (totalBullets <= 1)
                {
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
                            //Y dispara (FALTA)
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

        IAEnd = true;
        Test.ammoReload++;
    }

    void PassToPlayerTurn()
    {
        IAEnd = false;
        Test.returnTurn = true;
        Test.isIATurn = false;
        
        if (Test.playerTurn == 1) 
            Test.playerTurn = 2;

        else if (Test.playerTurn == 2) 
            Test.playerTurn = 1;
    }

    void IACreatesWall(PathNode closestPlayer, PathNode actualNode)
    {
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
    }
}
