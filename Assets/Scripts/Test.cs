using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    private GridManager<StringGridObject> stringGrid;
    private Pathfinding pathfinding;

    [SerializeField] private CharacterPathfinding characterPathfinding;

    private void Start()
    {
        pathfinding = new Pathfinding(10,10);
        stringGrid = new GridManager<StringGridObject>(10,10,10f, Vector3.zero, (GridManager<StringGridObject> g, int x, int y) => new StringGridObject(g, x, y));  // width, heigth, cellSize, OriginPosition

    }

    private void Update()
    {
        
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                Debug.Log("Click");

                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f, Color.green,2);
                }
            }
            characterPathfinding.SetTargetPosition(mouseWorldPosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkeable(!pathfinding.GetNode(x, y).isWalkeable);
            stringGrid.GetGridObject(mouseWorldPosition).AddLetter("X");
        }


            /*if (Input.GetKeyDown(KeyCode.A)) { grid.GetGridObject(worldPosition).AddLetter("A"); }
            if (Input.GetKeyDown(KeyCode.B)) { grid.GetGridObject(worldPosition).AddLetter("B"); }
            if (Input.GetKeyDown(KeyCode.C)) { grid.GetGridObject(worldPosition).AddLetter("C"); }

            if (Input.GetKeyDown(KeyCode.Alpha1)) { grid.GetGridObject(worldPosition).AddNumber("1"); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { grid.GetGridObject(worldPosition).AddNumber("2"); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { grid.GetGridObject(worldPosition).AddNumber("3"); }
            */
        }

}

public class StringGridObject
{
    private GridManager<StringGridObject> grid;
    private int x;
    private int y;

    private string letters;
    private string numbers;

    public StringGridObject(GridManager<StringGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        letters = "";
        numbers = "";
    }

    public void AddLetter(string letter)
    {
        letters += letter;
        grid.TriggerGridObjectChanged(x, y);
    }
    public void AddNumber(string number)
    {
        numbers += number;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return letters + "\n" + numbers;
    }


}
