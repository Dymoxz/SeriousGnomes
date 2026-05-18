using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Grip Instellingen")]
    public LayerMask gridLayer;
    public float heightOffset = 0.7f;
    private bool isDragging = false;
    private Vector3 startPosition;
    private bool isInteractable = false;

    [Tooltip("If highlighting fails to trigger when hovering near edges, increase this value.")]
    public float snapThreshold = 1.5f;
    public bool isLocked = false;

    public CardData cardData;
    public SpriteRenderer artworkRenderer;

    [Header("Text Components")]
    public TextMeshPro itemNameTextComponent;
    public TextMeshPro priceTextComponent;

    public Vector2 targetArtworkSize = new Vector2(2f, 2f);

    [Header("Highlight Settings (Emission)")]
    public Color glowColor = Color.yellow;
    [Tooltip("How bright the tile lights up! (Requires Post-Processing Bloom for a true 'glow' effect)")]
    public float glowIntensity = 3.0f;

    private GameObject currentHoveredTile;
    private Renderer hoveredTileRenderer;

    private Color originalEmissionColor;
    private bool originallyHadEmissionEnabled;

    [Header("Ghost Effect Settings")]
    [Range(0f, 1f)]
    public float ghostAlpha = 0.5f; // How transparent the ghost is
    public Color ghostTint = Color.white; // Optional color tint (e.g., light blue)
    private System.Collections.Generic.Dictionary<Renderer, Color[]> originalColors = new System.Collections.Generic.Dictionary<Renderer, Color[]>();

    // NEW: Keep track of the entity while dragging
    private Entity draggedEntityInstance;

    public void SetInteractable(bool state)
    {
        isInteractable = state;
    }

    void Start()
    {
        startPosition = transform.position;
        UpdateCardVisuals();
    }

    void OnMouseDown()
    {
        if (isLocked || !isInteractable)
            return;

        isDragging = true;

        // 1. Hide the card's UI/Meshes
        ToggleCardVisuals(false);

        // 2. Spawn the entity prefab as a visual preview and attach it to the card
        if (cardData != null && cardData.entityPrefab != null && draggedEntityInstance == null)
        {
            draggedEntityInstance = cardData.entityPrefab.Spawn(transform.position);
            draggedEntityInstance.transform.SetParent(this.transform);
            draggedEntityInstance.transform.localPosition = Vector3.zero;

            ApplyGhostEffect(draggedEntityInstance.gameObject);

            // Turn off colliders on the preview temporarily so it doesn't block the mouse raycasts while dragging
            Collider[] colliders = draggedEntityInstance.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
        }
    }

    void OnMouseUp()
    {
        if (!isInteractable) return;
        isDragging = false;

        RemoveHighlight();

        GameObject closestTile = FindClosestTileObject();

        if (closestTile != null)
        {
            Renderer tileRenderer = closestTile.GetComponentInChildren<Renderer>();
            Vector3 snapPos = tileRenderer != null ? tileRenderer.bounds.center : closestTile.transform.position;

            transform.position = new Vector3(snapPos.x, heightOffset, snapPos.z);
            startPosition = transform.position;
            isLocked = true;

            // 3a. Finalize the preview entity into the actual placed entity
            if (draggedEntityInstance != null)
            {
                draggedEntityInstance.transform.SetParent(null);
                draggedEntityInstance.transform.position = startPosition;

                // Re-enable colliders for gameplay
                Collider[] colliders = draggedEntityInstance.GetComponentsInChildren<Collider>(true);
                foreach (Collider col in colliders)
                {
                    col.enabled = true;
                }

                Tile targetTileComponent = closestTile.GetComponent<Tile>();
                if (targetTileComponent != null)
                {
                    targetTileComponent.AddEntity(draggedEntityInstance);
                }

                ResetGhostEffect(draggedEntityInstance.gameObject);

                draggedEntityInstance = null; // Clear reference
            }

            // Restore card visuals behind the scenes so it looks normal if drawn again later
            ToggleCardVisuals(true);

            this.gameObject.SetActive(false);
            GameManager.Instance.OnCardPlayed(this);
        }
        else
        {
            // 3b. Invalid placement: destroy the preview and restore the card
            if (draggedEntityInstance != null)
            {
                Destroy(draggedEntityInstance.gameObject);
                draggedEntityInstance = null;
            }

            ToggleCardVisuals(true);

            transform.position = startPosition;
            isLocked = false;
        }
    }

    private void ApplyGhostEffect(GameObject target)
    {
        originalColors.Clear();
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            // Store original colors
            Color[] colors = new Color[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
            {
                if (rend.materials[i].HasProperty("_Color"))
                {
                    colors[i] = rend.materials[i].color;

                    // Create ghost color: Keep tint but apply ghostAlpha
                    Color ghostCol = ghostTint;
                    ghostCol.a = ghostAlpha;

                    rend.materials[i].color = ghostCol;
                }
            }
            originalColors[rend] = colors;
        }
    }

    private void ResetGhostEffect(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            if (originalColors.ContainsKey(rend))
            {
                Color[] colors = originalColors[rend];
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    if (rend.materials[i].HasProperty("_Color"))
                    {
                        rend.materials[i].color = colors[i];
                    }
                }
            }
        }
    }
    public void SetStartPosition(Vector3 position)
    {
        startPosition = position;
        transform.position = position;
    }

    void Update()
    {
        if (isDragging)
        {
            MoveWithMouse();
        }
    }

    public void UpdateCardVisuals()
    {
        if (cardData != null)
        {
            if (itemNameTextComponent != null)
            {
                itemNameTextComponent.text = cardData.cardName;
            }

            if (priceTextComponent != null)
            {
                priceTextComponent.text = cardData.cost.ToString();
            }

            if (artworkRenderer != null && cardData.artwork != null)
            {
                artworkRenderer.sprite = cardData.artwork;

                Vector2 spriteSize = artworkRenderer.sprite.bounds.size;

                float scaleX = targetArtworkSize.x / spriteSize.x;
                float scaleY = targetArtworkSize.y / spriteSize.y;

                float scaleFactor = Mathf.Min(scaleX, scaleY);
                artworkRenderer.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            }
        }
    }

    // --- NEW HELPER METHOD ---
    // Toggles all renderers on the card, making it easily disappear/reappear
    private void ToggleCardVisuals(bool isVisible)
    {
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in allRenderers)
        {
            // Make sure we NEVER hide the preview entity by accident
            if (draggedEntityInstance != null && r.transform.IsChildOf(draggedEntityInstance.transform))
                continue;

            r.enabled = isVisible;
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

        UpdateHoveredTile();
    }

    private void UpdateHoveredTile()
    {
        GameObject newHoveredTile = FindClosestTileObject();

        if (newHoveredTile != currentHoveredTile)
        {
            RemoveHighlight();
            currentHoveredTile = newHoveredTile;

            if (currentHoveredTile != null)
            {
                hoveredTileRenderer = currentHoveredTile.GetComponentInChildren<Renderer>();

                if (hoveredTileRenderer != null)
                {
                    Material mat = hoveredTileRenderer.material;

                    originallyHadEmissionEnabled = mat.IsKeywordEnabled("_EMISSION");
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        originalEmissionColor = mat.GetColor("_EmissionColor");
                    }
                    else
                    {
                        originalEmissionColor = Color.black;
                    }

                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", glowColor * glowIntensity);
                }
            }
        }
    }

    private void RemoveHighlight()
    {
        if (hoveredTileRenderer != null)
        {
            Material mat = hoveredTileRenderer.material;

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor("_EmissionColor", originalEmissionColor);
            }

            if (!originallyHadEmissionEnabled)
            {
                mat.DisableKeyword("_EMISSION");
            }

            hoveredTileRenderer = null;
            currentHoveredTile = null;
        }
    }

    private GameObject FindClosestTileObject()
    {
        if (GridManager.Instance == null || GridManager.Instance.grid.Count == 0)
        {
            return null;
        }

        GameObject bestTarget = null;
        float closestDistance = snapThreshold;
        Vector3 cardPos = transform.position;
        Vector3 cardPosXZ = new Vector3(cardPos.x, 0, cardPos.z);

        foreach (var kvp in GridManager.Instance.grid)
        {
            GameObject tileObj = kvp.Value;
            if (tileObj == null) continue;

            if (!tileObj.CompareTag("PlayerTile")) continue;

            Tile tileComponent = tileObj.GetComponent<Tile>();
            if (tileComponent != null && !tileComponent.IsEmpty()) continue;

            Renderer tileRenderer = tileObj.GetComponentInChildren<Renderer>();
            Vector3 targetPosXZ;

            if (tileRenderer != null)
            {
                targetPosXZ = new Vector3(tileRenderer.bounds.center.x, 0, tileRenderer.bounds.center.z);
            }
            else
            {
                targetPosXZ = new Vector3(tileObj.transform.position.x, 0, tileObj.transform.position.z);
            }

            float distance = Vector3.Distance(cardPosXZ, targetPosXZ);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = tileObj;
            }
        }

        return bestTarget;
    }
}