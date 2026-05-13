using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Grip Instellingen")]
    public LayerMask gridLayer;
    public float heightOffset = 0.7f;
    private bool isDragging = false;
    private GameObject currentHoveredTile;
    private Vector3 startPosition;
    public float snapThreshold = 1.5f;
    private bool isLocked = false;

    public Entity entity;


    void Start()
    {
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (isLocked)
            return;
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;
        Vector3? closestTilePos = FindClosestTile();

        if (closestTilePos.HasValue)
        {
            Vector3 tilePos = closestTilePos.Value;
            // Snap to tile position + height offset
            transform.position = new Vector3(tilePos.x, heightOffset, tilePos.z);
            startPosition = transform.position;
            isLocked = true;

            //spawn new asset on tile
            this.gameObject.SetActive(false);
            entity.Spawn(startPosition);
            
            
        }
        else
        {
            transform.position = startPosition;
            isLocked = false;
        }
    }

    void Update()
    {
        if (isDragging)
        {
            MoveWithMouse();
        }
    }

    void MoveWithMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, heightOffset, 0));

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            transform.position = ray.GetPoint(rayDistance);
        }
    }

    private Vector3? FindClosestTile()
    {
        if (GridManager.Instance == null || GridManager.Instance.grid.Count == 0)
        {
            Debug.LogWarning("GridManager not found or grid is empty!");
            return null;
        }

        Vector3? bestTarget = null;
        float closestDistance = snapThreshold;
        Vector3 cardPos = transform.position;

        Debug.Log($"FindClosestTile: Checking {GridManager.Instance.grid.Count} tiles. Card at {cardPos}");

        foreach (Vector3 tileCoords in GridManager.Instance.grid.Keys)
        {
            // Only compare X and Z (horizontal distance)
            Vector3 cardPosXZ = new Vector3(cardPos.x, 0, cardPos.z);
            Vector3 tilePosXZ = new Vector3(tileCoords.x, 0, tileCoords.z);

            float distance = Vector3.Distance(cardPosXZ, tilePosXZ);

    
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = tileCoords;
            }
        }

        if (bestTarget.HasValue)
        {
            Debug.Log($"Closest tile found: {bestTarget.Value} at distance {closestDistance}");
        }
        else
        {
            Debug.Log($"No tile within snapThreshold ({snapThreshold})");
        }

        return bestTarget;
    }
}