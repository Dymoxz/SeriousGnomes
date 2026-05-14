using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public List<Card> cards = new List<Card>();
    [SerializeField] public List<Card> deck = new List<Card>(); //8 cards in deck, 4 cards in hand

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectDeck(List<Card> cards)
    {
        foreach (Card card in cards) //placeholder for now
        {
            deck.Add(card);  
        }
        
    }

    public void StartTurn(List<Card> currentCards) 
    {
        Debug.Log($"StartTurn called with {currentCards.Count} cards");
        foreach (Card card in currentCards)
        {
            Debug.Log($"Setting interactable: {card.name}");
            card.SetInteractable(true);
        }
    }

    public void EndTurn() //called when player clicks end turn button
    {
        foreach (Card card in deck)
            card.SetInteractable(false);

        GameManager.Instance.EndPlayerTurn();
    }
}
