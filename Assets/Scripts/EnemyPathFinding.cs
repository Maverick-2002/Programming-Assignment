using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPathfinding : MonoBehaviour
{
    public Transform seeker;             
    private Transform player;            
    private List<Node> currentPath;      
    Grid grid;                          

    void Awake()
    {
        grid = GetComponent<Grid>();   
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player not found.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            FindPathAndMove(player.position); // find path and move towards player position
        }
    }

    //find path from seeker current position to target position
    void FindPathAndMove(Vector3 targetPosition)
    {
        // Get start and target nodes from world positions
        Node startNode = grid.NodeFromWorldPoint(seeker.position);
        Node targetNode = grid.NodeFromWorldPoint(targetPosition);

        // Find path using A* algorithm
        List<Node> path = FindPath(startNode, targetNode);

        if (path != null)
        {
            // Show Movement along the found path using coroutine for controlled movement
            StartCoroutine(MoveAlongPath(path));
        }
        else
        {
            Debug.LogWarning("Path not found!"); // Log a warning if no path is found
        }
    }

    // A* pathfinding algorithm
    List<Node> FindPath(Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node>();      // List of nodes to be checked
        HashSet<Node> closedSet = new HashSet<Node>();  // Set of nodes already checked

        openSet.Add(startNode); // Add start node to open set

        while (openSet.Count > 0)
        {
            Node node = openSet[0];

            // Find node with lowest fCost in openSet
            for (int i = 1; i < openSet.Count; i++)
            {
                // Compare fCost; if equal, compare hCost to break ties
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost)
                {
                    node = openSet[i];
                }
            }

            // Move node from openSet to closedSet
            openSet.Remove(node);
            closedSet.Add(node);

            // Check if we reached the target node
            if (node == targetNode)
            {
                return RetracePath(startNode, targetNode); 
            }

            // Process each neighbour of the current node
            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                // Skip unwalkable nodes or nodes in the closed set
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue; 
                }

                // Calculate gCost for this neighbour
                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);

                // If this path is better (lower gCost), update neighbour's costs
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;  // Update gCost of neighbour
                    neighbour.hCost = GetDistance(neighbour, targetNode);  // Update hCost of neighbour
                    neighbour.parent = node;  // Set parent of neighbour to current node

                    // Add neighbour to openSet if not there
                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    
    List<Node> RetracePath(Node startNode, Node endNode)
    {
        // Retrace the path from endNode to startNode using parent references
        List<Node> path = new List<Node>();  // List to store the retrace path
        Node currentNode = endNode;         // Start from the end node

        
        while (currentNode != startNode)
        {
            path.Add(currentNode);          
            currentNode = currentNode.parent; 
        }

        // Reverse the path to get it from start to end
        path.Reverse();  

        grid.path = path; 

        return path;  
    }

    // Calculate the distance (cost) between two nodes using the Manhattan method
    int GetDistance(Node nodeA, Node nodeB)
    {
        // Calculate the absolute distance (cost) between two nodes
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);  
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // Calculate distance
        // Diagonal cost: 14, Vertical/Horizontal cost: 10
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);  // Diagonal cost: 14, Vertical/Horizontal cost: 10
        return 14 * dstX + 10 * (dstY - dstX);
    }

    IEnumerator MoveAlongPath(List<Node> path)// Coroutine to move the seeker along the calculated path
    {
        float speed = 2f;  // Adjust movement speed

        for (int i = 0; i < path.Count; i++)
        {
            Node currentNode = path[i];  // Get current node in path
            Vector3 targetPosition = grid.NodeToWorldPoint(currentNode); 
            targetPosition.y = seeker.position.y; // Maintain seeker height

            // Stop one block before the player's position
            if (i == path.Count - 1)  // Check if it's the last node in path
            {
                yield break; 
            }

            // Move towards the target position
            seeker.position = Vector3.MoveTowards(seeker.position, targetPosition, speed * Time.deltaTime);

            yield return null; 
        }
    }
}
