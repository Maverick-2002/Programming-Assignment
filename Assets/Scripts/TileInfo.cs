using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    // Public variables to store the coordinates of the tile
    public int x; 
    public int y; 

    public void SetPosition(int x, int y)
    {
        // Set the x and y coordinates of the tile
        this.x = x;
        this.y = y;
    }
}
