using UnityEngine;

public class Flower : Entity, IPlaceableCard
{
    [Header("Card Data")]
    [SerializeField] private CardData cardData;
    public CardData CardData => cardData;


    public void OnPlace()
    {
        
        // animation, index(pijler) updates and shi

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
