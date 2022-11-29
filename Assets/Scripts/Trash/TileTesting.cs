using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTesting : MonoBehaviour
{
    TileMap tileMap;
    void Start()
    {
        TileMap tileMap = new TileMap(12, 12, 10f, Vector3.zero);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tileMap.SetTileMapSprite(mouseWorldPosition, TileMap.TileMapObject.TileMapSprite.Groud);
        }
    }
}
