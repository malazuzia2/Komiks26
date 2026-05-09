using UnityEngine;

public class BuoySequenceManager : MonoBehaviour
{
    public static BuoySequenceManager Instance;  

    public GameObject buoyPrefab;    
    public Transform player;         
    public float spawnDistance = 150f;  
    public float lateralSpread = 50f;  

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnNextBuoy();
    }

    public void SpawnNextBuoy()
    {
        Vector3 spawnPos = player.position + (player.forward * spawnDistance);

        spawnPos += player.right * Random.Range(-lateralSpread, lateralSpread);

        spawnPos.y = player.position.y;

        Instantiate(buoyPrefab, spawnPos, Quaternion.identity);
    }
}