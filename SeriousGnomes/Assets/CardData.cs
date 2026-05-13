using UnityEngine;

// Dit zorgt ervoor dat we dit bestandstype kunnen aanmaken via het rechtermuisknop-menu in Unity!
[CreateAssetMenu(fileName = "Nieuwe Kaart", menuName = "CardCollection/Cards")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int cost;
    
    [TextArea(3, 5)] // Dit maakt het tekstvak in Unity wat groter om makkelijker te typen
    public string description;
    
    public Sprite artwork;
}