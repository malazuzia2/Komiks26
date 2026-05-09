using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public Transform player;

    [Header("Ustawienia Spawnu")]
    public float minSpawnDistance = 100f;  
    public float maxSpawnDistance = 200f;  
    public float lateralRange = 60f;       
    public float spawnRate = 2f;           

    private float nextSpawnTime;

    void Update()
    {
        if (player == null) return;

        if (Time.time > nextSpawnTime)
        {
            SpawnObject();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnObject()
    {
        if (prefabs.Length == 0) return; 
        float randomForward = Random.Range(minSpawnDistance, maxSpawnDistance); 
        float randomSide = Random.Range(-lateralRange, lateralRange);
        
        Vector3 spawnPos = player.position + (player.forward * randomForward) + (player.right * randomSide);
        spawnPos.y = player.position.y;

        GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Length)];
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        Instantiate(selectedPrefab, spawnPos, randomRotation);
    }

}