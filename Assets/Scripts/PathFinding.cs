using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker;
    public GameObject targetPrefab;
    private Transform currentTarget;
    private List<Node> currentPath;
    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the mouse position to find where it hits in the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is part of the grid tagged as floor
                if (hit.collider.CompareTag("Floor"))
                {
                    // Get the position where the mouse click
                    Vector3 clickPosition = hit.point;

                    ClearTarget();// Destroy the current target

                    // Instantiate the target prefab at clicked cube
                    currentTarget = Instantiate(targetPrefab, clickPosition, Quaternion.identity).transform;
                }
            }
        }

        // Move to the current target 
        if (currentTarget != null)
        {
            FindPathAndMove(currentTarget.position);
        }
    }

    void FindPathAndMove(Vector3 targetPosition)
    {
        // Find path from seeker current position to the target position
        Node startNode = grid.NodeFromWorldPoint(seeker.position);
        Node targetNode = grid.NodeFromWorldPoint(targetPosition);

        // Find the path using the A* algorithm
        List<Node> path = FindPath(startNode, targetNode);

        if (path != null)
        {
            //Show Movement along the found path using coroutine for controlled movement
            StartCoroutine(MoveAlongPath(path));
        }
        else
        {
            Debug.LogWarning("Path not found!");
        }
    }

    List<Node> FindPath(Node startNode, Node targetNode)
    {
        // Find path using A* algorithm
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Find node with lowest fCost in openSet
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                // Compare fCost; if equal, compare hCost to break ties
                if (openSet[i].fCost < node.fCost || (openSet[i].fCost == node.fCost && openSet[i].hCost < node.hCost))
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
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

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

    int GetDistance(Node nodeA, Node nodeB)
    {
        // Calculate the absolute distance (cost) between two nodes
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // Calculate distance
        // Diagonal cost: 14, Vertical/Horizontal cost: 10
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        else
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

    IEnumerator MoveAlongPath(List<Node> path)// Coroutine to move the seeker along the calculated path
    {
        float speed = 2f; // Adjust speed value

        for (int i = 0; i < path.Count; i++)
        {
            Node currentNode = path[i];// Get current node in path
            Vector3 targetPosition = grid.NodeToWorldPoint(currentNode);
            targetPosition.y = seeker.position.y; // Maintain seeker height

            // Move towards the target position
            seeker.position = Vector3.MoveTowards(seeker.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    void ClearTarget()
    {
        if (currentTarget != null)
        {
            Destroy(currentTarget.gameObject);
            currentTarget = null;
        }
    }
}
