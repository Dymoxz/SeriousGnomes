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

    // We store the original state so we don't break materials that already have emission
    private Color originalEmissionColor;
    private bool originallyHadEmissionEnabled;


    public void SetInteractable(bool state)
    {
        isInteractable = state;
    }

    void Start()
    {
        startPosition = transform.position;

        // Gewoon direct de visuals updaten. Instantiate is niet meer nodig voor Sprites!
        UpdateCardVisuals();
    }

    void OnMouseDown()
    {
        if (isLocked || !isInteractable)
            return;
        isDragging = true;
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

            this.gameObject.SetActive(false);
            Entity spawnedEntity = cardData.entityPrefab.Spawn(startPosition);

            Tile targetTileComponent = closestTile.GetComponent<Tile>();
            if (targetTileComponent != null)
            {
                targetTileComponent.AddEntity(spawnedEntity);
            }

            GameManager.Instance.OnCardPlayed(this);
        }
        else
        {
            transform.position = startPosition;
            isLocked = false;
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
            // --- NIEUW: Update de 3D tekst ---
            if (itemNameTextComponent != null)
            {
                itemNameTextComponent.text = cardData.cardName;
            }

            if (priceTextComponent != null)
            {
                priceTextComponent.text = cardData.cost.ToString();
            }


            // --- BESTAAND: Update en schaal de Sprite ---
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