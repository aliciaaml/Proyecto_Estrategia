using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRID {

    private int width;
    private int height;
    private int[,] gridArray;   //Array multidimensional (2 dimensiones)
    private float cellSize;

  

    public GRID(int width, int height, float cellSize) {

        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];

        for(int x= 0; x<  gridArray.GetLength(0); x++){
            for(int y=0; y<gridArray.GetLength(1);y++){
                
                CreateWorldText(gridArray[x,y].ToString(),null,GetWorldPosition(x,y) + new Vector3(cellSize,cellSize)* .5f,30,Color.white,TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x,y+1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x+1,y),Color.white, 100f);
            }
            Debug.DrawLine(GetWorldPosition(width,0),GetWorldPosition(width,height),Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width,height),Color.white, 100f);
        }   

        
    }

    //Convierte x e y en posiciones del escenario
    
    private Vector3 GetWorldPosition(int x, int y){

        return new Vector3(x,y)*cellSize;

    }

    //Crear texto en el escenario

    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000) {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }
        
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder) {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
