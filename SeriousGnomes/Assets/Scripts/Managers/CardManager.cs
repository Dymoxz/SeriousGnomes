using UnityEngine;
using UnityEngine.UI; // Nodig voor de Image component
using TMPro;

public class CardManager : MonoBehaviour
{
    [Header("UI Elementen")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;
    public Image artworkImage;

    [Header("Data Bron (Optioneel)")]
    public CardData cardData;

    [Header("Handmatige Invoer (Als Card Data leeg is)")]
    public string manualTitle;
    public int manualCost;
    [TextArea(3, 5)] public string manualDescription;
    public Sprite manualArtwork;

    private void Start()
    {
        RefreshCard();
    }

    public void RefreshCard()
    {
        if (cardData != null)
        {
            UpdateCard(cardData.cardName, cardData.cost, cardData.description, cardData.artwork);
        }
        else
        {
            UpdateCard(manualTitle, manualCost, manualDescription, manualArtwork);
        }
    }

    private void UpdateCard(string cardName, int cost, string desc, Sprite artworkSprite)
    {
        // 1. Update de tekst
        if (titleText != null) titleText.text = cardName;
        if (costText != null) costText.text = cost.ToString();
        if (descriptionText != null) descriptionText.text = desc;

        // 2. Update de 2D Artwork Image
        if (artworkImage != null)
        {
            if (artworkSprite != null)
            {
                artworkImage.sprite = artworkSprite;
                artworkImage.enabled = true; // Zorg dat de afbeelding zichtbaar is
            }
            else
            {
                artworkImage.enabled = false; // Verberg de Image als er geen artwork is toegewezen
            }
        }
    }
}