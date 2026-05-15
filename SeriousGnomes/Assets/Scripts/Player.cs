using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public List<Card> cards = new List<Card>();
    [SerializeField] public List<Card> deck = new List<Card>(); //8 cards in deck, 4 cards in hand
    public List<Card> hand = new List<Card>();
    public Queue<Card> cardQueue = new Queue<Card>();
    //money property
    
    public void AddToHand(Card card)
    {
        hand.Add(card);
    }

    public void PlayCard(Card card)
    {
        hand.Remove(card);
        cardQueue.Enqueue(card);
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

    public void StartTurn() 
    {
        foreach (Card card in hand)
        {
            card.SetInteractable(true);
        }
    }

    public void EndTurn()
    {
        foreach (Card card in hand)
            card.SetInteractable(false);

        GameManager.Instance.EndPlayerTurn();
    }
}
