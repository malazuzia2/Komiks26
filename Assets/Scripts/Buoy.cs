using UnityEngine;
using System.Collections.Generic;

public class Buoy : MonoBehaviour
{
    public static List<Buoy> allBuoys = new List<Buoy>();

    public float safeRadius = 25f;
    public float fogPushDuration = 10f;  
    private bool isUsed = false;

    void OnEnable() { allBuoys.Add(this); }
    void OnDisable() { allBuoys.Remove(this); }

    public float Activate()
    {
        if (isUsed) return 0;
        isUsed = true;

        if (BuoySequenceManager.Instance != null)
        {
            BuoySequenceManager.Instance.OnBuoyCollected();
        }


        if (NavigationManager.Instance != null)
        {
            NavigationManager.Instance.StartLookingForStars();
        }

        Destroy(gameObject, 0.1f);
        return fogPushDuration;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, safeRadius);
    }
}