using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public List<Card> cards = new List<Card>();
    [SerializeField] public List<Card> deck = new List<Card>(); //8 cards in deck, 4 cards in hand
    public List<Card> hand = new List<Card>();
    public Stack<Card> cardsInStack = new Stack<Card>();

    public void AddToStack(Card card)
    {
        cardsInStack.Push(card);
    }

  


    public void AddToHand(Card card)
    {
        hand.Add(card);
    }

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
        foreach (Card card in currentCards)
        {
            card.SetInteractable(true);
        }
    }

    public void EndTurn(List<Card> currentCards) //called when player clicks end turn button
    {
        foreach (Card card in currentCards)
            card.SetInteractable(false);

        GameManager.Instance.EndPlayerTurn();
    }
}
