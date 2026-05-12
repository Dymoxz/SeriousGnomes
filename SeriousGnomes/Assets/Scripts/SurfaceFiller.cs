using UnityEngine;

public class SurfaceFiller : MonoBehaviour
{
    [Header("Tile Settings")]
    public GameObject[] tilePrefabs;
    public float elevationY = 0.01f;

    [Header("Decoration (Bomen/Rotsen)")]
    public GameObject[] decorationPrefabs;

    [Tooltip("Kans per tegel dat er iets op groeit (0.0 = niks, 1.0 = op elke tegel)")]
    [Range(0f, 1f)]
    public float decorationDensity = 0.2f;

    [Tooltip("Moet de decoratie in het midden van de tegel staan, of een beetje willekeurig verschoven?")]
    public float randomOffset = 1.0f;

    private GameObject decorationParent; // Om de hierarchy netjes te houden

    [ContextMenu("Vul Oppervlakte")]
    public void FillSurface()
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogWarning("Voeg minimaal 1 prefab toe aan de array!");
            return;
        }

        MeshFilter parentMesh = GetComponent<MeshFilter>();
        if (parentMesh == null || parentMesh.sharedMesh == null) return;

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

        // Maak een apart mapje in de hierarchy voor alle bomen
        decorationParent = new GameObject("Decorations");
        decorationParent.transform.SetParent(this.transform);
        decorationParent.transform.localPosition = Vector3.zero;

        Vector3 parentLocalSize = parentMesh.sharedMesh.bounds.size;
        float parentWorldWidth = parentLocalSize.x * transform.lossyScale.x;
        float parentWorldDepth = parentLocalSize.z * transform.lossyScale.z;

        Vector3 tileLocalSize = referenceTileMesh.sharedMesh.bounds.size;
        float tileWorldWidth = tileLocalSize.x * refPrefab.transform.localScale.x;
        float tileWorldDepth = tileLocalSize.z * refPrefab.transform.localScale.z;

        int columns = Mathf.Max(1, Mathf.RoundToInt(parentWorldWidth / tileWorldWidth));
        int rows = Mathf.Max(1, Mathf.RoundToInt(parentWorldDepth / tileWorldDepth));

        float gridTotalWidth = columns * tileWorldWidth;
        float gridTotalDepth = rows * tileWorldDepth;

        Vector3 parentTrueCenter = transform.TransformPoint(parentMesh.sharedMesh.bounds.center);
        float startX = parentTrueCenter.x - (gridTotalWidth / 2f) + (tileWorldWidth / 2f);
        float startZ = parentTrueCenter.z - (gridTotalDepth / 2f) + (tileWorldDepth / 2f);
        float parentSurfaceY = transform.TransformPoint(parentMesh.sharedMesh.bounds.max).y;

        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                // 1. TEGEL PLAATSEN
                GameObject selectedPrefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                if (selectedPrefab == null) continue;

                MeshFilter specifiekeTileMesh = selectedPrefab.GetComponentInChildren<MeshFilter>();
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
                newTile.transform.localScale = new Vector3(
                    selectedPrefab.transform.localScale.x / transform.lossyScale.x,
                    selectedPrefab.transform.localScale.y / transform.lossyScale.y,
                    selectedPrefab.transform.localScale.z / transform.lossyScale.z
                );

                // 2. DECORATIE (BOMEN) PLAATSEN
                if (decorationPrefabs != null && decorationPrefabs.Length > 0 && Random.value < decorationDensity)
                {
                    GameObject randomTree = decorationPrefabs[Random.Range(0, decorationPrefabs.Length)];
                    if (randomTree != null)
                    {
                        // Beetje willekeur in de positie zodat het niet perfect uitgelijnd lijkt
                        float offsetX = Random.Range(-randomOffset, randomOffset);
                        float offsetZ = Random.Range(-randomOffset, randomOffset);
                        Vector3 treePos = new Vector3(targetX + offsetX, parentSurfaceY + elevationY, targetZ + offsetZ);

                        // Willekeurige Y rotatie (0 tot 360 graden) voor natuurlijke look
                        Quaternion treeRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                        GameObject newTree = Instantiate(randomTree, treePos, treeRot, decorationParent.transform);
                        newTree.name = "Tree";
                    }
                }
            }
        }

        Debug.Log($"Wereld + Decoratie Gegenereerd!");
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