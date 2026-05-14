using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public Player player;
    private List<Card> cardsInHand = new List<Card>();
    private static System.Random rnd = new System.Random();
    public GridGenerator gridGenerator;
    private bool enemyHasAttacked = false;
    private bool playerHasAttacked = false;

    public DeckGenerator deckGenerator;



    [SerializeField] private int roundNumber = 0;

    [SerializeField] private UIManager uiManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
      
        InitGame();
        StartRound();
        //if player clicks start:{initGame}
    }

    void Update()
    {
        
    }

 

    private void InitGame()
    {
        gridGenerator.ClearGrid();
        gridGenerator.GenerateBoard();
        Debug.Log("Managing Deck");
        player.deck = deckGenerator.GenerateDeck();
        SetCards();

        //other init game logic (populat crtain random tils)
        //...
    }

    private void SetCards()
    {
        int amountCardsToDisplay = 4;
        cardsInHand = player.deck
            .Where(c => !player.cardsInStack.Contains(c))
            .OrderBy(x => rnd.Next())
            .Take(amountCardsToDisplay)
            .ToList();

        float startX = -4.5f;
        float spacing = 3f; 
        float y = 0f;
        float z = 0f;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Card card = cardsInHand[i];
            Vector3 localPosition = new Vector3(startX + (i * spacing), y, z);
            card.transform.localPosition = localPosition;      
            card.SetStartPosition(card.transform.position);      
            card.gameObject.SetActive(true);
            card.SetInteractable(false);
        }
    }



    public void OnCardPlayed(Card card)
    {
        cardsInHand.Remove(card);
        player.AddToStack(card);
        card.transform.SetParent(deckGenerator.stackParent); // moves it under Stack in hierarchy
        UpdateStackDisplay();
    }

    private void UpdateStackDisplay()
    {
        // Position the top card of the stack to the left
        if (player.cardsInStack.Count > 0)
        {
            Card topCard = player.cardsInStack.Peek();
            topCard.gameObject.SetActive(true);

            topCard.transform.localPosition = new Vector3(-10f, 0f, 0f);
            topCard.SetStartPosition(topCard.transform.position);
        }
    }

    public void RemoveCardFromHand(Card card)
    {
        cardsInHand.Remove(card);
        player.deck.Remove(card); // remove from deck too so it can't be redrawn
    }

    public void EndPlayerTurn()
    {
        playerHasAttacked = true;
        uiManager.SetEndPlayerTurnButtonActive(false);

        

        if (enemyHasAttacked)
        {
            StartRound();
        }
        else
        {
            //enemy turn logic
            //Enemy.StartTurn(); this method will also call endEnemyTurn() once the attack is done
            uiManager.SetEndEnemyTurnButtonActive(true);
        }
    }

    public void EndEnemyTurn()
    {
        enemyHasAttacked = true;
        uiManager.SetEndEnemyTurnButtonActive(false);
        if (playerHasAttacked)
        {
            StartRound();
        }
        else
        {
            player.StartTurn(cardsInHand);
            uiManager.SetEndPlayerTurnButtonActive(true);
        }
    }



    private void StartRound()
    {
        //do round calculations
        //...
        playerHasAttacked = false;
        enemyHasAttacked = false;

        //start turn for player or enemy depending on the round number, odd for player & even for enemy. 
        //this means players always start first, but this can be changed in the future if we want to add a coin flip at the start of the game
        if (roundNumber % 2 == 0)
        {
            player.StartTurn(cardsInHand);
            uiManager.SetEndPlayerTurnButtonActive(true);
        }
        else
        {
            //enemy turn logic
            //Enemy.StartTurn(); this method will also call endEnemyTurn() once the attack is done
            uiManager.SetEndEnemyTurnButtonActive(true);
        }
        roundNumber++;
        uiManager.UpdateRoundNumber(roundNumber);


    }


}
