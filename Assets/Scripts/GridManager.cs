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
        SpawnObstacles();// Spawn obstacles
    }

    void Start()
    {
        
        GenerateGrid();// Generate grid of cubes
    }
    void GenerateGrid()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                // Calculate position for each cube in the grid
                Vector3 position = new Vector3(x + offsetX, 0, y + offsetZ);
                // Instantiate a cube prefab at the position
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.name = $"Cube_{x}_{y}"; //name the cube
                cube.AddComponent<TileInfo>().SetPosition(x, y); // Add TileInfo component and call set position
                cube.transform.parent = cubeParent; // Set grid cube under the parent 
            }
        }
    }
    void SpawnObstacles()
    {
        int obstacleCount = 0;

        while (obstacleCount < numberOfObstacles)
        {
            // Generate random coordinates for obstacles 
            int x = Random.Range(0, 10);
            int z = Random.Range(0, 10);

            // Calculate position for the obstacle
            Vector3 position = new Vector3(x + offsetX, 1f, z + offsetZ);

            // Check if the position is not occupied by the player or enemy
            if (!IsObstacleAtPosition(position) && !IsPlayerAtPosition(position))
            {
                // Instantiate an obstacle prefab at the calculated position
                GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity);
                obstacle.name = $"Obstacle_{obstacleCount}"; //name the obstacle
                obstacle.transform.parent = obstaclesParent; // Set Obstacles under the parent 
                obstacle.tag = "Obstacles"; // Assign Tag to Obstacles
                obstacleCount++;
            }
        }
    }

    // check if there is an obstacle at a given position
    bool IsObstacleAtPosition(Vector3 position)
    {
        // Check for colliders in a small sphere around the given position
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);

        // Loop through all colliders
        foreach (Collider collider in colliders)
        {
            // Check if collider has "Obstacles" tag
            if (collider.CompareTag("Obstacles"))
            {
                return true;
            }
        }

        return false;
    }

    // check if there is a player at a given position
    bool IsPlayerAtPosition(Vector3 position)
    {
        // Check for colliders in a small sphere around the given position
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);

        // Loop through all colliders
        foreach (Collider collider in colliders)
        {
            // Check if collider has "Player" tag
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
