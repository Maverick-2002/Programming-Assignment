using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Grid : MonoBehaviour
{
    public Transform player;          
    public LayerMask unwalkableMask;    // Layer mask to define unwalkable areas
    public Vector3 gridWorldSize;       
    public float nodeRadius;            

    Node[,] grid;                       
    float nodeDiameter;                 
    int gridSizeX, gridSizeY;          

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);   // Calculate number of nodes along X
        gridSizeY = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);   // Calculate number of nodes along Y
        CreateGrid();                   // Initialize grid for path finidng
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];  // Initialize the grid array
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Iterate through each node position in the grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calculate world position of the node
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                // Check if node is walkable
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                // Create a new Node instance and assign it to the grid array
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // Get all neighboring nodes of a given node
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        // Iterate through potential neighboring positions
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Skip current node
                if (x == 0 && y == 0)
                    continue;

                // Calculate grid coordinates of neighboring node
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Ensure neighboring node is within grid bounds
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);  // Add neighboring node to list
                }
            }
        }

        return neighbours;
    }

    // Get the node from a given world position
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        // Calculate percentage of position across grid
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        // Clamp percentages to ensure within valid range
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Calculate grid coordinates from percentages
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];  // Return node at calculated grid position
    }

    // Get world position from a given node
    public Vector3 NodeToWorldPoint(Node node)
    {
        return node.worldPosition;  // Return world position of node
    }

    public List<Node> path;  // List to store path nodes

    // Draw gizmos for visualization
    void OnDrawGizmos()
    {
        // Draw wireframe cube representing the grid area
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.z));

        if (grid != null)
        {
            Node playerNode = NodeFromWorldPoint(player.position);  // Get player's current node

            foreach (Node n in grid)
            {
                // Set gizmo color based on node walkability and obstacles
                Gizmos.color = (n.walkable) ? Color.white : Color.red;

                // Highlight player current node in cyan
                if (playerNode == n)
                {
                    Gizmos.color = Color.cyan;
                }

                // Highlight path nodes in black
                if (path != null && path.Contains(n))
                {
                    Gizmos.color = Color.black;
                }

                // Draw cube gizmo representing each node
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
