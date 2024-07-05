using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cubePrefab;

    void Start()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector3 position = new Vector3(x , 0, y );
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.name = $"Cube_{x}_{y}";
                cube.AddComponent<TileInfo>().SetPosition(x, y);
            }
        }
    }
}
