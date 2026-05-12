using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private List<Entity> entities;
    [SerializeField] private TileState state; //0 is default, 1 is sunny, 2 is frozen


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
           
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
