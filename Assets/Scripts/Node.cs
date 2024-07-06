using UnityEngine;

public class Node
{
    public bool walkable;        
    public Vector3 worldPosition; 
    public int gridX;            
    public int gridY;            

    public int gCost;            // Cost from the starting node to this node
    public int hCost;            // Heuristic cost from this node to the target node
    public Node parent;          // Parent node used for tracing the path

    // Initialize the node with information
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    // Calculate the fCost of the node
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
