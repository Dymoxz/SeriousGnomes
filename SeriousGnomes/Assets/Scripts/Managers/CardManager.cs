using UnityEngine;
using TMPro;

public class CardManager : MonoBehaviour
{
    [Header("UI Elementen")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;

    [Header("3D Model")]
    public Transform modelParent; // empty child transform where the model spawns
    private GameObject currentModel;

    [Header("Data Bron (Optioneel)")]
    public CardData cardData;

    [Header("Handmatige Invoer (Als Card Data leeg is)")]
    public string manualTitle;
    public int manualCost;
    [TextArea(3, 5)] public string manualDescription;
    public GameObject manualModelPrefab;

    

    private void Start()
    {
        RefreshCard();
    }

    public void RefreshCard()
    {
        if (cardData != null)
            UpdateCard(cardData.cardName, cardData.cost, cardData.description, cardData.artwork);
        else
            UpdateCard(manualTitle, manualCost, manualDescription, manualModelPrefab);
    }

    private void UpdateCard(string cardName, int cost, string desc, GameObject modelPrefab)
    {
        // Update text
        if (titleText != null) titleText.text = cardName;
        if (costText != null) costText.text = cost.ToString();
        if (descriptionText != null) descriptionText.text = desc;

        // Swap 3D model
        if (modelParent != null)
        {
            if (currentModel != null)
                Destroy(currentModel);

            if (modelPrefab != null)
            {
                currentModel = Instantiate(modelPrefab, modelParent.position, modelParent.rotation, modelParent);
                currentModel.transform.localPosition = Vector3.zero;
                currentModel.transform.localRotation = Quaternion.identity;
            }
        }
    }
}