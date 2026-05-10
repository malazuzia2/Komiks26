using UnityEngine;

public class SkyFollow : MonoBehaviour
{
    public Transform player;  
    private Vector3 offset;   

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
         
        offset = transform.position - player.position;
    }

    void LateUpdate()  
    {
        if (player == null) return;
         
        transform.position = player.position + offset;
    }
}