using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Dictionary<Vector3, GameObject> grid = new Dictionary<Vector3, GameObject>();//<coordinate, tile>

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

    public void AddTileToGrid(GameObject tile)
    {
        Vector3 tileCenter = tile.transform.position + new Vector3(1f, 0, 1f);
        grid[tileCenter] = tile;
    }

   


}
