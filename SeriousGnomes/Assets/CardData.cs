using UnityEngine;

[CreateAssetMenu(fileName = "Nieuwe Kaart", menuName = "CardCollection/Cards")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int cost;

    public Entity entityPrefab;

    [TextArea(3, 5)]
    public string description;
    
    public Sprite artwork;
}