using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public Player player;

    private static System.Random rnd = new System.Random();
    public GridGenerator gridGenerator;
    private bool enemyHasAttacked = false;
    private bool playerHasAttacked = false;

    private bool isPlayerTurn = true;

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
        player.deck = deckGenerator.GenerateDeck();
        player.deck = player.deck.OrderBy(x => rnd.Next()).ToList();


        //other init game logic (populat crtain random tils)
        //...
    }

    private void SetCards()
    {
        PushRemainingToQueue();

        DealToHand();

        PositionHandCards();

        UpdateStackDisplay();
    }

    private void DealToHand()
    {
        while (player.hand.Count < 4 && player.cardQueue.Count > 0)
        {
            Card card = player.cardQueue.Dequeue();
            player.hand.Add(card);
        }
    }

    private void PushRemainingToQueue()
    {
        // Only enqueue cards not already in hand or queue
        List<Card> remaining = player.deck
            .Where(c => !player.hand.Contains(c) && !player.cardQueue.Contains(c))
            .ToList();

        foreach (Card card in remaining)
        {
            player.cardQueue.Enqueue(card);
            card.transform.SetParent(deckGenerator.queueParent);
            card.gameObject.SetActive(false);
        }
    }

    private void PositionHandCards()
    {
        float startX = -4.5f;
        float spacing = 3f;

        for (int i = 0; i < player.hand.Count; i++)
        {
            Card card = player.hand[i];
            card.transform.SetParent(deckGenerator.handParent);
            card.transform.localPosition = new Vector3(startX + (i * spacing), 0f, 0f);
            card.SetStartPosition(card.transform.position);
            card.gameObject.SetActive(true);
            card.SetInteractable(isPlayerTurn);
        }
    }

    public void OnCardPlayed(Card card)
    {
        // Remove from hand
        player.PlayCard(card);

        // Add to back of queue (cycles to the end)
        card.isLocked = false;
        card.gameObject.SetActive(false);
        card.transform.SetParent(deckGenerator.queueParent);

        // Draw next card from front of queue into hand
        DrawOneCard();
        PositionHandCards();
        UpdateStackDisplay();
    }

    private void DrawOneCard()
    {
        if (player.cardQueue.Count == 0) return;

        Card next = player.cardQueue.Dequeue();
        player.hand.Add(next);
    }

    private void UpdateStackDisplay()
    {
        // Show the front card of the queue (next to be drawn)
        if (player.cardQueue.Count > 0)
        {
            Card frontCard = player.cardQueue.Peek();
            frontCard.gameObject.SetActive(true);
            frontCard.transform.SetParent(deckGenerator.queueParent);
            frontCard.transform.localPosition = new Vector3(-10f, 0f, 0f);
            frontCard.SetStartPosition(frontCard.transform.position);
        }
    }




    public void EndPlayerTurn()
    {
        playerHasAttacked = true;
        uiManager.SetEndPlayerTurnButtonActive(false);
        isPlayerTurn = false;


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
            isPlayerTurn = true;
            player.StartTurn();
            uiManager.SetEndPlayerTurnButtonActive(true);
        }
    }



    private void StartRound()
    {
        //do round calculations
        //...
        playerHasAttacked = false;
        enemyHasAttacked = false;

        SetCards();

        //start turn for player or enemy depending on the round number, odd for player & even for enemy. 
        //this means players always start first, but this can be changed in the future if we want to add a coin flip at the start of the game
        if (roundNumber % 2 == 0)
        {
            isPlayerTurn = true;
            player.StartTurn();
            
            uiManager.SetEndPlayerTurnButtonActive(true);
        }
        else
        {
            isPlayerTurn = false;
            //enemy turn logic
            //Enemy.StartTurn(); this method will also call endEnemyTurn() once the attack is done

            uiManager.SetEndEnemyTurnButtonActive(true);
        }
        roundNumber++;
        uiManager.UpdateRoundNumber(roundNumber);


    }


}
