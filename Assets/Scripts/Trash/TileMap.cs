using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    GridManager<TileMapObject> grid;

    public TileMap(int width, int height, float cellSize, Vector3 originPosition)
    {
        grid = new GridManager<TileMapObject>(width, height, cellSize, originPosition, (GridManager<TileMapObject> g, int x, int y) => new TileMapObject(grid, x, y));
    }

    public void SetTileMapSprite(Vector3 worldPosition, TileMapObject.TileMapSprite tileMapSprite)
    {
        TileMapObject tileMapObject = grid.GetGridObject(worldPosition);

        if (tileMapObject != null)
            tileMapObject.SetTileMapSprite(tileMapSprite);
    }

    public class TileMapObject
    {
        public enum TileMapSprite
        {
            None,
            Groud
        }

        GridManager<TileMapObject> grid;
        int x;
        int y;
        TileMapSprite tileMapSprite;

        public TileMapObject(GridManager<TileMapObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetTileMapSprite(TileMapSprite tileMapSprite)
        {
            this.tileMapSprite = tileMapSprite;
            grid.TriggerGridObjectChanged(x, y);
        }

        public override string ToString()
        {
            return tileMapSprite.ToString();
        }
    }
}
