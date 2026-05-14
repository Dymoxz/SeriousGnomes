using System.Collections.Generic;
using UnityEngine;

public class DeckGenerator : MonoBehaviour
{
    public Card cardPrefab;
    public Transform handParent;
    public Transform stackParent; 
    public List<CardData> availableCards;

    public List<Card> GenerateDeck()
    {
        Debug.Log("Generating Deck");
        List<Card> deck = new List<Card>();

        foreach (CardData cardData in availableCards)
        {
            Card card = Instantiate(cardPrefab, handParent); 
            card.cardData = cardData;
            card.name = cardData.cardName;
            card.gameObject.SetActive(false); 
            deck.Add(card);
        }

        return deck;
    }
}

