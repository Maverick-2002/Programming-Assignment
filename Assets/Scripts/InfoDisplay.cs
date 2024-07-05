using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InfoDisplay : MonoBehaviour
{ 
    public Text positionText;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red); // ray for debugging

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileInfo tileInfo = hit.transform.GetComponent<TileInfo>();
            if (tileInfo != null)
            {
                positionText.text = $"Unit Position: ({tileInfo.x}, {tileInfo.y})";
            }
        }
        else
        {
            positionText.text = "Unit Position: Na";
        }
    }
}


