using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour
{
    public Text positionText;

    void Update()
    {
        // Cast a ray from the main camera through the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Draw a Gizmos in for visualization
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

        // Perform a raycast to detect collisions with objects in the scene
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the ray hit an object with a TileInfo component attached
            TileInfo tileInfo = hit.transform.GetComponent<TileInfo>();
            if (tileInfo != null)
            {
                positionText.text = $"Unit Position: ({tileInfo.x}, {tileInfo.y})";//Update UI
            }
        }
        else
        {
            positionText.text = "Unit Position: Na";//Updte UI
        }
    }
}
