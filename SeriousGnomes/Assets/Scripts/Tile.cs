using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Entity> entities;
    [SerializeField] private TileStateEnum state; //0 is default, 1 is sunny, 2 is frozen


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    public bool IsEmpty()
    {
        return entities.Count == 0;
    }

    public void AddEntity(Entity ent)
    {
        entities.Add(ent);
    }

    void GetDamaged(double damage)
    {
        foreach (Entity ent in entities)
        {
            if (ent != null)
            {
                return;
            }
            ent.GetDamaged(damage);
            if (ent.GetHealth() <= 0)
            {
                //ent.Die();
            } 
        }
    }
}
