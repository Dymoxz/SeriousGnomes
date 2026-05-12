using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    //sustainability index's
    [SerializeField] protected double soilQuality;
    [SerializeField] protected double plantDiversity;
    [SerializeField] protected double groundwaterLevel;
    [SerializeField] protected double biodiversity;

    [SerializeField] protected double health;
    [SerializeField] protected double damage;


}
