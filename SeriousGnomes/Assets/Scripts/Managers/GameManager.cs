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
        SetCards();

        //other init game logic (populat crtain random tils)
        //...
    }

    private void SetCards()
    {
        //select 4 random cards from players deck
        int amountCardsToDisplay = 4;
        cardsInHand = player.deck.OrderBy(x => rnd.Next()).Take(amountCardsToDisplay).ToList(); 
        
        //move current cards to bottom of screen
        float startX = -3f;
        float spacing = 4f;
        float y = 4f;
        float z = 49f;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Card card = cardsInHand[i];

            Vector3 targetPosition = new Vector3(
                startX + (i * spacing),
                y,
                z
            );

            card.transform.position = targetPosition;
            card.SetStartPosition(targetPosition); // tell the card this is its new home
        }
    }
    public void SetStartPosition(Vector3 position)
    {
        startPosition = position;
        transform.position = position;
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
