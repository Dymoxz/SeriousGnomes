using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    //sustainability index's
    [SerializeField] protected double soilQuality;
    [SerializeField] protected double plantDiversity;
    [SerializeField] protected double groundwaterLevel;
    [SerializeField] protected double biodiversity;

    [SerializeField] protected double health;
    [SerializeField] protected double strength;
    [SerializeField] protected double moneyGenerating;

    public double GetHealth () { return health; }
    public double GetStrength () { return strength; }

    public void GetDamaged(double damage)
    {
        health -= damage;
    }

    public void Die()
    {
        //
    }

    public void Spawn(Vector3 position)
    { 
        transform.position = position;
    }

    public double GetSoilQuality () { return soilQuality; }
    public double GetPlantDiversity () { return plantDiversity; }
    public double GetGroundwaterLevel () { return groundwaterLevel; }
    public double GetBiodiversity () { return biodiversity; }

}
