using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuoySequenceManager : MonoBehaviour
{
    public static BuoySequenceManager Instance;  

    public GameObject buoyPrefab;    
    public Transform player;         
    public float spawnDistance = 150f;  
    public float lateralSpread = 50f;

    private bool hintShown = false;
    private int buoysCollected = 0;

    void Update()
    {
         if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            SkipCurrentBuoy();
        }
    }
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

    public GameObject finalBuoyPrefab;

    public void OnBuoyCollected()
    {
        buoysCollected++;

        // Jeœli to pierwsza boja i jeszcze nie pokazywaliœmy wskazówki
        if (buoysCollected == 1 && !hintShown)
        {
            StartCoroutine(ShowTelescopeHintDelayed());
        }
    }

    private IEnumerator ShowTelescopeHintDelayed()
    {
        yield return new WaitForSeconds(1.5f);

        SimpleMessage.Instance.ShowMessage("Okay... What now? May the stars lead my way.");
        hintShown = true;
    }


    public void SpawnFinalBuoy()
    {

        SimpleMessage.Instance.ShowMessage("This one is weairdly... Different...");

        Vector3 spawnPos = player.position + player.forward * spawnDistance;
        spawnPos.y = player.position.y;

        Instantiate(finalBuoyPrefab, spawnPos, Quaternion.identity);
    }

    private void SkipCurrentBuoy()
    {
 
         foreach (Buoy buoy in Buoy.allBuoys.ToArray())
        {
            if (buoy != null) Destroy(buoy.gameObject);
        }

         OnBuoyCollected();

         if (buoysCollected < 3)  
        {
            SpawnNextBuoy();
        }
        else
        {
            SpawnFinalBuoy();
        }
    }


}