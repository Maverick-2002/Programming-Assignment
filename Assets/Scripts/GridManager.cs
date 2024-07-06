using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject obstaclePrefab;
    public GameObject playerPrefab;
    public int numberOfObstacles = 5;
    public Transform obstaclesParent;
    public Transform cubeParent;
    float offsetX = -4.5f;
    float offsetZ = -4.5f;

    private GameObject player;
    private void Awake()
    {
        SpawnObstacles();
    }
    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector3 position = new Vector3(x + offsetX, 0, y + offsetZ);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.name = $"Cube_{x}_{y}";
                cube.AddComponent<TileInfo>().SetPosition(x, y);
                cube.transform.parent = cubeParent;
            }
        }
    }


    void SpawnObstacles()
    {
        float offsetX = -4.5f;
        float offsetZ = -4.5f;
        int obstacleCount = 0;

        while (obstacleCount < 5)
        {
            int x = Random.Range(0, 10);
            int z = Random.Range(0, 10);
            Vector3 position = new Vector3(x + offsetX, 1f, z + offsetZ);

            if (!IsObstacleAtPosition(position))
            {
                GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity);
                obstacle.name = $"Obstacle_{obstacleCount}";
                obstacle.transform.parent = obstaclesParent;
                obstacle.tag = "Obstacles"; // Ensure obstacle has the correct tag
                obstacleCount++;
            }
        }
    }

    public bool IsObstacleAtPosition(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Obstacles"))
            {
                return true;
            }
        }
        return false;
    }  
}
