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

    private Rigidbody playerRb;
    public float minSpeedToSpawn = 1f;

    private float nextSpawnTime;

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Brak ³odzi");
            return;
        }

        if (playerRb == null)
        {
            playerRb = player.GetComponent<Rigidbody>();

            if (playerRb == null)
                playerRb = player.GetComponentInChildren<Rigidbody>();

            if (playerRb == null) return; 
        }

        if (playerRb.linearVelocity.magnitude > minSpeedToSpawn)
        {
            if (Time.time > nextSpawnTime)
            {
                SpawnObject();
                nextSpawnTime = Time.time + spawnRate;
            }
        }
        else
        {
            nextSpawnTime = Time.time + 0.5f;
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