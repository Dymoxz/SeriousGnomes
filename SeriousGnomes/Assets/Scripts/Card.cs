using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Grip Instellingen")]
    public LayerMask gridLayer;
    public float heightOffset = 0.5f;
    private bool isDragging = false;
    private GameObject currentHoveredTile;
    private Vector3 startPosition;
    public float snapThreshold = 1.5f;
    public bool isLocked = false;

    [Header("Unit Spawning")]
    public GameObject unitPrefab;
    private GameObject spawnedUnit;
    private bool hasSpawned = false;

    [Header("Tile Highlighting")]
    public Color highlightColor = Color.yellow; // Change to orange in Inspector
    private GameObject highlightedTile;
    private Color originalColor;

    void Start()
    {
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (isLocked)
            return;

        if (!hasSpawned)
        {
            spawnedUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity);
            hasSpawned = true;
            isDragging = true;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Unhighlight tile
        UnhighlightTile();

        if (spawnedUnit == null) return;

        Vector3? closestTilePos = FindClosestTile();

        if (closestTilePos.HasValue)
        {
            Vector3 tilePos = closestTilePos.Value;

            spawnedUnit.transform.position = new Vector3(tilePos.x, tilePos.y + heightOffset, tilePos.z);

            isLocked = true;
            Debug.Log("Unit placed on tile!");
        }
        else
        {
            Destroy(spawnedUnit);
            hasSpawned = false;
            Debug.Log("Unit discarded - not on a tile");
        }

        Destroy(gameObject);
    }

    void Update()
    {
        if (isDragging && spawnedUnit != null)
        {
            MoveUnitWithMouse();
            UpdateTileHighlight();
        }
    }

    void MoveUnitWithMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, heightOffset, 0));

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            spawnedUnit.transform.position = ray.GetPoint(rayDistance);
        }
    }

    void UpdateTileHighlight()
    {
        Vector3? closestTilePos = FindClosestTile();
        //Debug.Log("UpadeHighlight" + closestTilePos);

        if (closestTilePos.HasValue)
        {
            Debug.Log("closestilepos has value");
            GameObject tileToHighlight = GridManager.Instance.grid[closestTilePos.Value];

            if (tileToHighlight != highlightedTile)
            {
                Debug.Log("highlighted till exists");
                UnhighlightTile();
                HighlightTile(tileToHighlight);
            }
        }
        else
        {
            UnhighlightTile();
        }
    }

    void HighlightTile(GameObject tile)
    {
        highlightedTile = tile;

        // Try to get renderer on this object first
        Renderer renderer = tile.GetComponent<Renderer>();

        // If not found, search in children
        if (renderer == null)
        {
            renderer = tile.GetComponentInChildren<Renderer>();
            Debug.Log("Renderer found in children");
        }

        if (renderer != null)
        {
            Debug.Log("Renderer exists");
            renderer.material = new Material(renderer.material);
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", highlightColor);
        }
        else
        {
            Debug.LogWarning("No Renderer found on tile or its children!");
        }
    }

    void UnhighlightTile()
    {
        if (highlightedTile != null)
        {
            Renderer renderer = highlightedTile.GetComponentInChildren<Renderer>();

            if (renderer != null)
            {
                renderer.material.DisableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", Color.black);
            }

            highlightedTile = null;
        }
    }

    private Vector3? FindClosestTile()
    {
        Vector3? bestTarget = null;
        float closestDistance = snapThreshold;
        Vector3 unitPos = spawnedUnit.transform.position;

        foreach (Vector3 tileCoords in GridManager.Instance.grid.Keys)
        {
            Vector3 unitPosXZ = new Vector3(unitPos.x, 0, unitPos.z);
            Vector3 tilePosXZ = new Vector3(tileCoords.x, 0, tileCoords.z);
            float distance = Vector3.Distance(unitPosXZ, tilePosXZ);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = tileCoords;
            }
        }

        return bestTarget;
    }
}