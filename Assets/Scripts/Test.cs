using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private GRID grid;

    private void Start()
    {
        grid = new GRID(12, 12, 10f, new Vector3 (0, 0));  // width, heigth, cellSize, OriginPosition

        //Walls
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);
        grid.SetValue(Random.Range(3, 9), Random.Range(0, 12), 1);

        //Players
        grid.SetValue(Random.Range(9, 12), Random.Range(0, 5), 2);
        grid.SetValue(Random.Range(9, 12), Random.Range(5, 8), 2);
        grid.SetValue(Random.Range(9, 12), Random.Range(8, 12), 2);

        //Enemies
        grid.SetValue(Random.Range(0, 3), Random.Range(0, 5), 3);
        grid.SetValue(Random.Range(0, 3), Random.Range(5, 8), 3);
        grid.SetValue(Random.Range(0, 3), Random.Range(8, 12), 3);
    }

    private void Update()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        /*if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(worldPosition, 56);
        }*/

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(worldPosition));
        }
    }
}