using UnityEngine;

public class SurfaceFiller : MonoBehaviour
{
    [Header("Tile Settings")]
    [Tooltip("Plaats hier je 6 (of meer) landschap varianten!")]
    public GameObject[] tilePrefabs;

    [Tooltip("Hoe hoog de tegels boven de plane zweven (bijv. 0.01 tegen Z-fighting)")]
    public float elevationY = 0.01f;

    [ContextMenu("Vul Oppervlakte")]
    public void FillSurface()
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogWarning("Voeg minimaal 1 prefab toe aan de array!");
            return;
        }

        MeshFilter parentMesh = GetComponent<MeshFilter>();
        if (parentMesh == null || parentMesh.sharedMesh == null)
        {
            Debug.LogError("Parent heeft geen MeshFilter!");
            return;
        }

        // Zoek de eerste geldige prefab om de basismaten mee te berekenen
        MeshFilter referenceTileMesh = null;
        GameObject refPrefab = null;
        foreach (var prefab in tilePrefabs)
        {
            if (prefab != null)
            {
                referenceTileMesh = prefab.GetComponentInChildren<MeshFilter>();
                refPrefab = prefab;
                break;
            }
        }

        if (referenceTileMesh == null) return;

        ClearSurface();

        // 1. Bereken de exacte wereld-afmetingen van je parent plane
        Vector3 parentLocalSize = parentMesh.sharedMesh.bounds.size;
        float parentWorldWidth = parentLocalSize.x * transform.lossyScale.x;
        float parentWorldDepth = parentLocalSize.z * transform.lossyScale.z;

        // 2. Bereken de originele grootte van je tegel prefab
        Vector3 tileLocalSize = referenceTileMesh.sharedMesh.bounds.size;
        float tileWorldWidth = tileLocalSize.x * refPrefab.transform.localScale.x;
        float tileWorldDepth = tileLocalSize.z * refPrefab.transform.localScale.z;

        // 3. Bereken automatisch het aantal rijen en kolommen (geen gaps, originele size)
        // We gebruiken RoundToInt zodat het zo dicht mogelijk bij de randen van je plane komt
        int columns = Mathf.RoundToInt(parentWorldWidth / tileWorldWidth);
        int rows = Mathf.RoundToInt(parentWorldDepth / tileWorldDepth);

        // Zorg dat er altijd minimaal 1 tegel geplaatst wordt
        columns = Mathf.Max(1, columns);
        rows = Mathf.Max(1, rows);

        // 4. Bereken startpositie om het hele blok perfect te centreren op de parent
        float gridTotalWidth = columns * tileWorldWidth;
        float gridTotalDepth = rows * tileWorldDepth;

        Vector3 parentTrueCenter = transform.TransformPoint(parentMesh.sharedMesh.bounds.center);
        float startX = parentTrueCenter.x - (gridTotalWidth / 2f) + (tileWorldWidth / 2f);
        float startZ = parentTrueCenter.z - (gridTotalDepth / 2f) + (tileWorldDepth / 2f);
        float parentSurfaceY = transform.TransformPoint(parentMesh.sharedMesh.bounds.max).y;

        // 5. Genereer de wereld
        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                GameObject selectedPrefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                if (selectedPrefab == null) continue;

                MeshFilter specifiekeTileMesh = selectedPrefab.GetComponentInChildren<MeshFilter>();

                // Compenseer voor vreemde pivot points van 3D modellen
                Vector3 pivotOffsetWorld = new Vector3(
                    specifiekeTileMesh.sharedMesh.bounds.center.x * selectedPrefab.transform.localScale.x,
                    0,
                    specifiekeTileMesh.sharedMesh.bounds.center.z * selectedPrefab.transform.localScale.z
                );

                float targetX = startX + (x * tileWorldWidth);
                float targetZ = startZ + (z * tileWorldDepth);

                Vector3 spawnPosition = new Vector3(
                    targetX - pivotOffsetWorld.x,
                    parentSurfaceY + elevationY,
                    targetZ - pivotOffsetWorld.z
                );

                GameObject newTile = Instantiate(selectedPrefab, spawnPosition, selectedPrefab.transform.rotation, transform);
                newTile.name = $"WorldTile_{x}_{z}";

                // CRUCIAAL: We behouden de originele size door de scale van de parent (die de tegels uitrekt) teniet te doen
                newTile.transform.localScale = new Vector3(
                    selectedPrefab.transform.localScale.x / transform.lossyScale.x,
                    selectedPrefab.transform.localScale.y / transform.lossyScale.y,
                    selectedPrefab.transform.localScale.z / transform.lossyScale.z
                );
            }
        }

        Debug.Log($"Wereld Gegenereerd! ({columns}x{rows} tegels geplaatst).");
    }

    [ContextMenu("Clear Oppervlakte")]
    public void ClearSurface()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}