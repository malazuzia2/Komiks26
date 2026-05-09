using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;
    public bool isSearchingPhase = false;  

    void Awake() { Instance = this; }
     
    public void StartLookingForStars()
    {
        isSearchingPhase = true; 
    }
     
    public void OnStarsMatched()
    {
        if (!isSearchingPhase) return;

        isSearchingPhase = false; 
         
        if (BuoySequenceManager.Instance != null)
        {
            BuoySequenceManager.Instance.SpawnNextBuoy();
        }
    }
}