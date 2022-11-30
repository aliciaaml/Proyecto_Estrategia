using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPathfinding : MonoBehaviour
{
    public float speed = 5f;

    Vector2 lastClickedPos;
    bool moving;
    List<Vector3> pathVectorList;
    Pathfinding pathfinding;
    int currentPathIndex;
    public List<PathNode> range = new List<PathNode>();

    void Start()
    {
        pathfinding = Pathfinding.Instance;

        pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
        range = pathfinding.GetRangeList(pathfinding.GetNode(x, y));

    }
    public void Update()
    {
       HandleMovement();
    }

    private void HandleMovement()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];

            if (Vector3.Distance(transform.position, targetPosition) > 1)
            {
                Vector3 moveDir = (targetPosition- transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position,targetPosition);
                transform.position = transform.position + moveDir * speed * Time.deltaTime; 
            }

            else
            {
                currentPathIndex++;

                if (currentPathIndex >= pathVectorList.Count)
                {
                    StopMoving();
                    pathfinding.GetGrid().GetXY(GetPosition(), out int x, out int y);
                    range = pathfinding.GetRangeList(pathfinding.GetNode(x, y));
                }
            }
        }
    }

    private void StopMoving()
    {
        pathVectorList = null;
        
        foreach (PathNode node in range)
        {
            pathfinding.GetNode(node.x, node.y).SetIsInRange(false);
        }
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

        pathVectorList = pathfinding.FindPath(GetPosition(), targetPosition);

        if (pathVectorList!= null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }
}
