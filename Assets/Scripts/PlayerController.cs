using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector3 targetPosition;
    private bool isMoving;
    private GridManager gridManager;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            HandleInput();
        }
        MoveToTarget();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    Vector3 selectedTilePosition = hit.collider.transform.position;
                    if (gridManager != null && !gridManager.IsObstacleAtPosition(selectedTilePosition))
                    {
                        MoveTo(selectedTilePosition);
                    }
                    else
                    {
                        Debug.Log("Cannot move to grid with obstacle!");
                    }
                }
            }
        }
    }

    void MoveTo(Vector3 destination)
    {
        targetPosition = new Vector3(destination.x, transform.position.y, destination.z); // Maintain current y-position
    }

    void MoveToTarget()
    {
        if (transform.position != targetPosition)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacles"))
        {
            // Handle collision with obstacles here
            Debug.Log("Player collided with obstacle!");
            // For example, stop movement or perform other actions
            isMoving = false;
            // Optionally, reset player position or apply other game logic
        }
    }

    public void SetGridManager(GridManager manager)
    {
        gridManager = manager;
    }
}
