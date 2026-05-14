using UnityEngine;

public interface IPlaceableCard
{
    CardData CardData { get; }
    void OnPlace();
}
