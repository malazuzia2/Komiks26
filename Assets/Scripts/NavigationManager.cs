using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;

    [Header("Etapy Gry")]
    public GameObject[] starGroups;      // Przeci¹gnij tu 3 grupy gwiazd z nieba
    public Sprite[] constellationUI;    // Przeci¹gnij tu 3 obrazki PNG

    private int currentStage = 0;       // Numer obecnego etapu (0, 1, 2)
    public bool isSearchingPhase = false;

    void Awake() { Instance = this; }

    void Start()
    {
        // Na starcie wszystkie gwiazdy s¹ wy³¹czone
        foreach (GameObject group in starGroups) group.SetActive(false);
    }

    public void StartLookingForStars()
    {
        if (currentStage >= starGroups.Length)
        {
             return;
        }

        isSearchingPhase = true;

         starGroups[currentStage].SetActive(true);
         
        AstrologicalTelescope telescope = FindObjectOfType<AstrologicalTelescope>();
        telescope.SetNewTarget(starGroups[currentStage].transform, constellationUI[currentStage]);
    }

    public void OnStarsMatched()
    {
        if (!isSearchingPhase) return;
        isSearchingPhase = false;
         
        currentStage++;

        if (BuoySequenceManager.Instance != null)
        {
            BuoySequenceManager.Instance.SpawnNextBuoy();
        }
    }
}