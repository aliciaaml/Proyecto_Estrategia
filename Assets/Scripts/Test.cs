using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private GRID grid;
    private void Start()
    {
        grid = new GRID(10,10,10f, new Vector3 (0, 0));  // width, heigth, cellSize, OriginPosition
        
    }

    private void Update()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(worldPosition, 56);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(worldPosition));
        }
    }

}
