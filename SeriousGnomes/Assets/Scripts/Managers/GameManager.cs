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

    private void cycleCards() 
    {
        List<GameObject> cards = player.chosenCards;

    }

    private void initGame()
    {
        int amountCardsToDisplay = 4;
        currentCards = player.chosenCards.OrderBy(x => rnd.Next()).Take(amountCardsToDisplay).ToList(); //select 4 random cards from players deck
        //movw current cards to bottom of screen
        float startX = -3f;
        float spacing = 4f;

        for (int i = 0; i < currentCards.Count; i++)
        {
            GameObject card = currentCards[i];

            Vector3 targetPosition = new Vector3(
                startX + (i * spacing),
                4f,
                49f
            );

            card.transform.position = targetPosition;
        }
    }

}
