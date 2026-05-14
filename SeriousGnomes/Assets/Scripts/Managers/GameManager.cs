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
        UpdateStackDisplay();


        //other init game logic (populat crtain random tils)
        //...
    }

    private void SetCards()
    {
        // Recycle stack if not enough cards left to fill hand
        int availableCount = player.deck.Count(c => !player.hand.Contains(c) && !player.cardsInStack.Contains(c));
        if (availableCount < 4 - player.hand.Count && player.cardsInStack.Count > 0)
        {
            while (player.cardsInStack.Count > 0)
            {
                Card recycled = player.cardsInStack.Pop();
                recycled.isLocked = false;
                recycled.gameObject.SetActive(false);
            }
        }

        List<Card> newCards = GetHand();
        player.hand.AddRange(newCards);

        List<Card> remainingCards = player.deck
            .Where(c => !player.hand.Contains(c) && !player.cardsInStack.Contains(c))
            .OrderBy(x => rnd.Next())
            .ToList();

        foreach (Card card in remainingCards)
        {
            player.cardsInStack.Push(card);
            card.transform.SetParent(deckGenerator.stackParent);
            card.gameObject.SetActive(false);
        }

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

        // Show top of stack
        UpdateStackDisplay();
    }

    private List<Card> GetHand()
    {
        return player.deck
            .Where(c => !player.hand.Contains(c) && !player.cardsInStack.Contains(c))
            .OrderBy(x => rnd.Next())
            .Take(4 - player.hand.Count) 
            .ToList();
    }

    public void OnCardPlayed(Card card)
    {
        player.PlayCard(card);
        card.transform.SetParent(deckGenerator.stackParent);
        UpdateStackDisplay();
        SetCards(); // reuse same method to fill the gap
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
