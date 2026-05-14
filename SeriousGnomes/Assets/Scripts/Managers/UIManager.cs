using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public void LoadMatchScene()
    {
        SceneManager.LoadScene("MatchScene");
    }

    public void LoadVictoryCanvas()
    {
        //create victory canvas and set it active
    }

    public void LoadLostCanvas()
    {
        //create lost canvas and set it active
    }
}
