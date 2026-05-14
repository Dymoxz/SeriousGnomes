using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI roundNumberText;
    [SerializeField] private Button endPlayerTurn;
    [SerializeField] private Button endEnemyTurn;
    public void UpdateRoundNumber(int roundNumber)
    {
        roundNumberText.text = $"Round {roundNumber}/10";
    }

    public void SetEndPlayerTurnButtonActive(bool active)
    {
        endPlayerTurn.gameObject.SetActive(active);
    }

    public void SetEndEnemyTurnButtonActive(bool active)
    {
        endEnemyTurn.gameObject.SetActive(active);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
