using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public Player player;
    private List<GameObject> currentCards = new List<GameObject>();
    private static System.Random rnd = new System.Random();

    private bool enemyHasAttacked = false;
    private bool playerHasAttacked = false;

    public int roundNumber = 0; 

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
      
        initGame();
        //if player clicks start:{initGame}
    }

    void Update()
    {
        
    }

 

    private void initGame()
    {
        setCards();
        //other init game logic (populat crtain random tils)
        //...


        player.startTurn();
    }

    private void setCards()
    {
        //select 4 random cards from players deck
        int amountCardsToDisplay = 4;
        currentCards = player.deck.OrderBy(x => rnd.Next()).Take(amountCardsToDisplay).ToList(); 
        
        //move current cards to bottom of screen
        float startX = -3f;
        float spacing = 4f;
        float y = 4f;
        float z = 49f;

        for (int i = 0; i < currentCards.Count; i++)
        {
            GameObject card = currentCards[i];

            Vector3 targetPosition = new Vector3(
                startX + (i * spacing),
                y,
                z
            );

            card.transform.position = targetPosition;
        }
    }

    public void endPlayerTurn()
    {
        playerHasAttacked = true;
        if (enemyHasAttacked)
        {
            startRound();
        }
        else
        {
            //enemy turn logic
            //Enemy.StartTurn(); this method will also call endEnemyTurn() once the attack is done
        }
    }

    public void endEnemyTurn()
    {
        enemyHasAttacked = true;
        if (playerHasAttacked)
        {
            startRound();
        }
        else
        {
            player.startTurn();
        }
    }



    private void startRound()
    {
        //do round calculations
        //...


        //start turn for player or enemy depending on the round number, odd for player & even for enemy. 
        //this means players always start first, but this can be changed in the future if we want to add a coin flip at the start of the game
        if (roundNumber % 2 == 1)
        {
            player.startTurn();
        }
        else
        {
            //enemy turn logic
            //Enemy.StartTurn(); this method will also call endEnemyTurn() once the attack is done
        }
        roundNumber++;
    }


}
