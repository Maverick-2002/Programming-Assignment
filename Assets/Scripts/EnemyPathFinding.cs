using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPathfinding : MonoBehaviour
{
    public Transform seeker; // Enemy or seeker transform
    private Transform player; // Reference to the player transform
    private List<Node> currentPath; // Current path to follow
    Grid grid; // Reference to the Grid class instance

    void Awake()
    {
        grid = GetComponent<Grid>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player in the scene

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player is tagged as 'Player'.");
        }
    }

    void Update()
    {
        // Move towards the player if one exists
        if (player != null)
        {
            FindPathAndMove(player.position);
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

            // Stop one block before the player's position
            if (i == path.Count - 1) // -2 to stop one block before (assuming one block distance)
            {
                yield break; // Exit coroutine
            }

            // Move towards the target position
            seeker.position = Vector3.MoveTowards(seeker.position, targetPosition, speed * Time.deltaTime);

            yield return null;
        }

        Debug.Log("Reached final target!");
    }
}
