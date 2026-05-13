using UnityEngine;
using System.Collections.Generic;
public class GridGenerator : MonoBehaviour
{
    [Header("Grid Setup")]
    [Tooltip("Drag all your tile variations in here!")]
    public GameObject[] tilePrefabs;
    public GameObject middleTile;
    public int columns = 4; // X-axis
    public int rows = 5;    // Z-axis
    private int middle => rows / 2; // For potential future use (like placing a special tile in the center)

    [Header("Spacing & Layout")]
    [Tooltip("The absolute minimum space between tiles.")]
    public float minSpacing = 0.05f;

    [Tooltip("Space left around the very edge of the parent plane.")]
    public float edgePadding = 0.5f;

    [Tooltip("How high above the parent's surface the grid should sit.")]
    public float elevationY = 0.1f;

    [ContextMenu("Generate Grid")]
    public void GenerateBoard()
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogWarning("Please assign at least one Tile Prefab to the array!");
            return;
        }

        MeshFilter parentMesh = GetComponent<MeshFilter>();
        if (parentMesh == null || parentMesh.sharedMesh == null)
        {
            Debug.LogError("The parent object needs a MeshFilter to calculate exact bounds!");
            return;
        }

        // We grab the first valid prefab in the array just to figure out the general sizing 
        // (Assuming all your variations are roughly the same base size, like 4m)
        MeshFilter referenceTileMesh = null;
        foreach (var prefab in tilePrefabs)
        {
            if (prefab != null)
            {
                referenceTileMesh = prefab.GetComponentInChildren<MeshFilter>();
                break;
            }
        }

        if (referenceTileMesh == null || referenceTileMesh.sharedMesh == null)
        {
            Debug.LogError("At least one Tile Prefab needs a MeshFilter so we can measure it!");
            return;
        }

        ClearGrid();

        // 1. Get accurate WORLD sizes for the parent
        Vector3 parentLocalSize = parentMesh.sharedMesh.bounds.size;
        float worldWidth = parentLocalSize.x * transform.lossyScale.x;
        float worldDepth = parentLocalSize.z * transform.lossyScale.z;

        // 2. Calculate usable area
        float usableWidth = worldWidth - (2 * edgePadding);
        float usableDepth = worldDepth - (2 * edgePadding);

        // 3. Calculate max square tile size based on our reference tile
        float maxTileX = (usableWidth - ((columns - 1) * minSpacing)) / columns;
        float maxTileZ = (usableDepth - ((rows - 1) * minSpacing)) / rows;
        float targetTileSize = Mathf.Min(maxTileX, maxTileZ);

        // 4. Calculate dynamic spacing
        float actualSpacingX = columns > 1 ? (usableWidth - (columns * targetTileSize)) / (columns - 1) : 0;
        float actualSpacingZ = rows > 1 ? (usableDepth - (rows * targetTileSize)) / (rows - 1) : 0;

        // 5. Calculate total physical grid dimensions
        float gridTotalWidth = (columns * targetTileSize) + ((columns - 1) * actualSpacingX);
        float gridTotalDepth = (rows * targetTileSize) + ((rows - 1) * actualSpacingZ);

        // 6. Find exact mathematical center of the parent
        Vector3 parentTrueCenter = transform.TransformPoint(parentMesh.sharedMesh.bounds.center);

        float startX = parentTrueCenter.x - (gridTotalWidth / 2f) + (targetTileSize / 2f);
        float startZ = parentTrueCenter.z - (gridTotalDepth / 2f) + (targetTileSize / 2f);
        float parentSurfaceY = transform.TransformPoint(parentMesh.sharedMesh.bounds.max).y;

        // 7. Spawn the randomized grid
        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                GameObject selectedPrefab;
                if (z == middle)
                {
                    // Pick a random prefab from the array
                    selectedPrefab = middleTile;
                } else
                {
                    // Pick a random prefab from the array
                    selectedPrefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                }


                // Skip if you accidentally left an empty slot in the array
                if (selectedPrefab == null) continue;

                // Get the specific mesh data for THIS exact randomly selected tile
                MeshFilter tileMesh = selectedPrefab.GetComponentInChildren<MeshFilter>();
                if (tileMesh == null || tileMesh.sharedMesh == null) continue;

                // Dynamically calculate the scale and pivot offset for this specific variation
                Vector3 tileLocalSize = tileMesh.sharedMesh.bounds.size;
                Vector3 requiredLocalScale = new Vector3(
                    targetTileSize / (tileLocalSize.x * transform.lossyScale.x),
                    1f / transform.lossyScale.y,
                    targetTileSize / (tileLocalSize.z * transform.lossyScale.z)
                );

                Vector3 tileWorldScale = new Vector3(targetTileSize / tileLocalSize.x, 1f, targetTileSize / tileLocalSize.z);
                Vector3 tilePivotOffsetWorld = new Vector3(
                    tileMesh.sharedMesh.bounds.center.x * tileWorldScale.x,
                    0,
                    tileMesh.sharedMesh.bounds.center.z * tileWorldScale.z
                );

                // Target center position
                float targetX = startX + (x * (targetTileSize + actualSpacingX));
                float targetZ = startZ + (z * (targetTileSize + actualSpacingZ));

                Vector3 spawnPosition = new Vector3(
                    targetX - tilePivotOffsetWorld.x,
                    parentSurfaceY + elevationY,
                    targetZ - tilePivotOffsetWorld.z
                );

                GameObject newTile = Instantiate(selectedPrefab, spawnPosition, selectedPrefab.transform.rotation, transform);

                // Find the GridManager in the scene, even in Edit Mode
                GridManager gridManager = FindFirstObjectByType<GridManager>();
                if (gridManager != null)
                {
                    gridManager.AddTileToGrid(spawnPosition, newTile);
                }
                else
                {
                    Debug.LogWarning("No GridManager found in the scene.");
                }

                newTile.name = $"Tile_{x}_{z}";
                newTile.transform.localScale = requiredLocalScale;
            }
        }

        Debug.Log($"Randomized Grid Generated! Array Size: {tilePrefabs.Length}");
    }

    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}