using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject obstaclePrefab;
    public GameObject playerPrefab;
    public int numberOfObstacles = 5;
    public Transform obstaclesParent;

    private GameObject player;

    void Start()
    {
        GenerateGrid();
        SpawnObstacles();
        SpawnPlayer();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector3 position = new Vector3(x, 0, y);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.name = $"Cube_{x}_{y}";
                cube.AddComponent<TileInfo>().SetPosition(x, y);
                cube.transform.parent = transform;
            }
        }
    }

    void SpawnObstacles()
    {
        for (int i = 0; i < numberOfObstacles; i++)
        {
            int x = Random.Range(0, 10);
            int z = Random.Range(0, 10);
            Vector3 position = new Vector3(x, 1f, z);
            if (!IsObstacleAtPosition(position))
            {
                GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity);
                obstacle.name = $"Obstacle_{i}";
                obstacle.transform.parent = obstaclesParent;
                obstacle.tag = "Obstacles"; // Ensure obstacle has the correct tag
            }
            else
            {
                i--;
            }
        }
    }

    void SpawnPlayer()
    {
        Vector3 position = new Vector3(0, 1f, 0);
        player = Instantiate(playerPrefab, position, Quaternion.identity);
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetGridManager(this);
        }
        else
        {
            Debug.LogError("Player prefab is missing PlayerController component!");
        }
    }

    public bool IsObstacleAtPosition(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f); // Adjust radius as needed
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Obstacles"))
            {
                return true;
            }
        }
        return false;
    }
}
