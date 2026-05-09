using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private Transform player;
    public float destroyDistance = 50f;  

    void Start()
    { 
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;
         
        float distance = Vector3.Distance(transform.position, player.position); 
        if (distance > destroyDistance && transform.position.z < player.position.z)
        {
            Destroy(gameObject);
        }
    }
}