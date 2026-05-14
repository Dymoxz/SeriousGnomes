using UnityEngine;
using TMPro; // Vergeet deze niet voor TextMeshPro!
using System.Collections.Generic;

public class TipSystem : MonoBehaviour
{
    [Header("UI Elementen")]
    public TextMeshProUGUI tipTextElement;

    [Header("Instellingen")]
    [TextArea(3, 10)]
    public List<string> tips = new List<string>();

    private int currentTipIndex = 0;

    void Start()
    {
        // Laat direct een willekeurige of de eerste tip zien bij start
        ShowRandomTip();
    }

    public void ShowNextTip()
    {
        if (tips.Count == 0) return;

        // Ga naar de volgende tip, en spring terug naar 0 aan het einde
        currentTipIndex = (currentTipIndex + 1) % tips.Count;
        UpdateUI();
    }

    public void ShowRandomTip()
    {
        if (tips.Count == 0) return;

        currentTipIndex = Random.Range(0, tips.Count);
        UpdateUI();
    }

    private void UpdateUI()
    {
        tipTextElement.text = tips[currentTipIndex];
    }
}