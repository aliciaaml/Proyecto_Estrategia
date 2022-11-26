using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPathfinding : MonoBehaviour
{
    public float speed = 5f;

    Vector2 lastClickedPos;
    bool moving;
    List<Vector3> pathVectorList;
    int currentPathIndex;


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
                }
            }
        }
    }

    private void StopMoving()
    {
        pathVectorList = null;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);

        if(pathVectorList!= null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }
}
