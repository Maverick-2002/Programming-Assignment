using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker; // Player or seeker transform
    public GameObject targetPrefab; // Prefab representing the target
    private Transform currentTarget; // Reference to the current target instance
    private List<Node> currentPath; // Current path to follow
    Grid grid; // Reference to the Grid class instance

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        // Check for mouse click (left button) or touch input
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits something in the scene
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is part of the grid
                if (hit.collider.CompareTag("Floor"))
                {
                    // Get the position where the mouse click or touch occurred
                    Vector3 clickPosition = hit.point;

                    // Remove existing target if any
                    ClearTarget();

                    // Instantiate the target prefab at the clicked position
                    currentTarget = Instantiate(targetPrefab, clickPosition, Quaternion.identity).transform;
                }
            }
        }

        // Move towards the current target if one exists
        if (currentTarget != null)
        {
            FindPathAndMove(currentTarget.position);
        }
    }

    void FindPathAndMove(Vector3 targetPosition)
    {
        // Find path from seeker's current position to the target position
        Node startNode = grid.NodeFromWorldPoint(seeker.position);
        Node targetNode = grid.NodeFromWorldPoint(targetPosition);

        List<Node> path = FindPath(startNode, targetNode);

        if (path != null)
        {
            // Move along the found path
            StartCoroutine(MoveAlongPath(path));
        }
        else
        {
            Debug.LogWarning("Path not found!");
        }
    }

    List<Node> FindPath(Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null; // No path found
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path; // Store the path in the grid for visualization

        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    IEnumerator MoveAlongPath(List<Node> path)
    {
        float speed = 2f; // Adjust speed value

        for (int i = 0; i < path.Count; i++)
        {
            Node currentNode = path[i];
            Vector3 targetPosition = grid.NodeToWorldPoint(currentNode);
            targetPosition.y = seeker.position.y; // Maintain the original y-coordinate

            // Use MoveTowards for controlled movement with stopping
            seeker.position = Vector3.MoveTowards(seeker.position, targetPosition, speed * Time.deltaTime);

            // Optional: Check if close enough to target before yielding
            if (Vector3.Distance(seeker.position, targetPosition) <= 0.1f)
            {
                yield return null;
            }
        }

        Debug.Log("Reached final target!");
    }


    void ClearTarget()
    {
        // Destroy the current target if it exists
        if (currentTarget != null)
        {
            Destroy(currentTarget.gameObject);
            currentTarget = null;
        }
    }
}
