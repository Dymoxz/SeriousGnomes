using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<GameObject> cards = new List<GameObject>();
    [SerializeField] public List<GameObject> chosenCards = new List<GameObject>();

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectChosenCards(List<GameObject> cards)
    {
        foreach (GameObject card in cards)
        {
            chosenCards.Add(card);  
        }
        
    }
}
