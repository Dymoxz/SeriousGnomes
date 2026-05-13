using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public List<GameObject> cards = new List<GameObject>();
    [SerializeField] public List<GameObject> deck = new List<GameObject>(); //8 cards in deck, 4 cards in hand

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectDeck(List<GameObject> cards)
    {
        foreach (GameObject card in cards) //placeolder
        {
            deck.Add(card);  
        }
        
    }

    public void startTurn() 
    {
        foreach (GameObject card in deck)
        {
            //card.enable(); enable cards from deck to be moved and played

        }
    }

    public void endTurn() //called when player clicks end turn button
    {
        //cards.disable(); disable cards to be moved and played
        GameManager.Instance.endPlayerTurn();
    }
}
