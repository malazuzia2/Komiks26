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
         Vector3 origin = Camera.main.transform.position;

         Vector3 lookDirection = Camera.main.transform.forward;
        lookDirection.y = 0;  
        lookDirection.Normalize();

         Vector3 spawnPos = origin + lookDirection * spawnDistance;

        spawnPos += Camera.main.transform.right * Random.Range(-lateralSpread, lateralSpread);

        spawnPos.y = player.position.y;

        Instantiate(buoyPrefab, spawnPos, Quaternion.identity);
    }
}