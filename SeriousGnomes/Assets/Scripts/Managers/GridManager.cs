using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    private Dictionary<Vector3, GameObject> grid = new Dictionary<Vector3, GameObject>();//<coordinate, tile>

    public static GridManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Debug.Log("GridManager started");
    }

    private void Update()
    {
        // draait elke frame
    }

    public GameObject GetTile(Vector3 coordinates)
    {
        return grid[coordinates];
    }

    public void AddTileToGrid(Vector3 spawnPos, GameObject tile)
    {
        grid[spawnPos] = tile;
    }



}
