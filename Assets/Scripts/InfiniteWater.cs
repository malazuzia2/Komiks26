using System.Collections.Generic;
using UnityEngine;

public class InfiniteWater : MonoBehaviour
{
    public GameObject waterPrefab;     
    public Transform player;          
    public float tileSize = 100f;      

    private Vector2Int currentTile = new Vector2Int(0, 0);
    private Dictionary<Vector2Int, GameObject> activeTiles = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        UpdateTiles();
    }

    void Update()
    { 
        int playerX = Mathf.FloorToInt((player.position.x + tileSize / 2) / tileSize);
        int playerZ = Mathf.FloorToInt((player.position.z + tileSize / 2) / tileSize);

        if (playerX != currentTile.x || playerZ != currentTile.y)
        {
            currentTile = new Vector2Int(playerX, playerZ);
            UpdateTiles();
        }
    }

    void UpdateTiles()
    {
        List<Vector2Int> tilesNeeded = new List<Vector2Int>();
         
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                tilesNeeded.Add(new Vector2Int(currentTile.x + x, currentTile.y + z));
            }
        }
         
        List<Vector2Int> tilesToRemove = new List<Vector2Int>();
        foreach (var tile in activeTiles)
        {
            if (!tilesNeeded.Contains(tile.Key))
            {
                tilesToRemove.Add(tile.Key);
            }
        }

        foreach (var key in tilesToRemove)
        {
            Destroy(activeTiles[key]);
            activeTiles.Remove(key);
        }
         
        foreach (var tilePos in tilesNeeded)
        {
            if (!activeTiles.ContainsKey(tilePos))
            {
                Vector3 worldPos = new Vector3(tilePos.x * tileSize, 0, tilePos.y * tileSize);
                GameObject newTile = Instantiate(waterPrefab, worldPos, Quaternion.identity, transform);
                activeTiles.Add(tilePos, newTile);
            }
        }
    }

}
