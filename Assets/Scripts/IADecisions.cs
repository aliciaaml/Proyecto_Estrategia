using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IADecisions : MonoBehaviour
{
    private Pathfinding pathfinding;
    public static bool enemyTurn = true;
    public List<PathNode> range = new List<PathNode>();
    public CharacterPathfinding enemyPathfinding;

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = enemyPathfinding.pathfinding;

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyTurn)
        {
            pathfinding.GetGrid().GetXY(enemyPathfinding.GetPosition(), out int x, out int y);
            TakeDecisions(pathfinding.GetNode(x,y));
            enemyTurn = false;
            
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    void TakeDecisions(PathNode nodoActual)
    {
        range = pathfinding.GetRangeList(pathfinding.GetNode(nodoActual.x, nodoActual.y));

        foreach (PathNode node in range)
        {
            if (pathfinding.GetNode(node.x, node.y).isFriend) //Ver si hay algun jugador dentro del rango
            {
                Debug.Log("Hay jugador");
            }
            else Debug.Log("No hay jugador");
        }
        
    }
}
