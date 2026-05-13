using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardManager : MonoBehaviour
{
    [Header("UI Elementen (Sleep deze hierheen)")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;
    public Image artworkImage;

    [Header("Data Bron (Optioneel)")]
    public CardData cardData; // ScriptableObject

    [Header("Handmatige Invoer (Als Card Data leeg is)")]
    public string manualTitle;
    public int manualCost;
    [TextArea(3, 5)] public string manualDescription;
    public Sprite manualArtwork;

    // OnValidate ensures that the card updates immediately in the Editor 
    // when you change a value or drag in a scriptable object.

    private void OnValidate()
    {
        RefreshCard();
    }

    public void RefreshCard()
    {
        if (cardData != null)
        {
            // Use the data from the ScriptableObject
            UpdateUI(cardData.cardName, cardData.cost, cardData.description, cardData.artwork);
        }
        else
        {
            // Use the manual fields from the Inspector
            UpdateUI(manualTitle, manualCost, manualDescription, manualArtwork);
        }
    }

    private void UpdateUI(string name, int cost, string desc, Sprite art)
    {
        if (titleText != null) titleText.text = name;
        if (costText != null) costText.text = cost.ToString();
        if (descriptionText != null) descriptionText.text = desc;
        if (artworkImage != null) artworkImage.sprite = art;
    }
}