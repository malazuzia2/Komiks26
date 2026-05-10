using UnityEngine;

// Automatycznie dodajemy potrzebne komponenty do efektu retro
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
[RequireComponent(typeof(AudioHighPassFilter))]
[RequireComponent(typeof(AudioDistortionFilter))]
public class Radyjko : MonoBehaviour, IInteractable
{
    [Header("Stacje Radiowe")]
    public AudioClip[] radioStations;
    public AudioClip staticNoiseClip; // DŸwiêk szumu miêdzy stacjami

    [Header("Ustawienia Retro")]
    [Range(0, 1)] public float radioDistortion = 0.3f;
    public float lowPassFreq = 3000f;  // Odcina wysokie tony (stary g³oœnik)
    public float highPassFreq = 1000f; // Odcina basy (ma³y g³oœnik)

    private AudioSource audioSource;
    private int currentStationIndex = -1; // -1 oznacza, ¿e radio jest wy³¹czone
    private bool isTuning = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Konfiguracja filtrów "starego radia"
        GetComponent<AudioLowPassFilter>().cutoffFrequency = lowPassFreq;
        GetComponent<AudioHighPassFilter>().cutoffFrequency = highPassFreq;
        GetComponent<AudioDistortionFilter>().distortionLevel = radioDistortion;

        // Ustawienia AudioSource dla 3D
        audioSource.spatialBlend = 1.0f; // DŸwiêk przestrzenny
        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    public void Interact()
    {
        NextStation();
    }

    void NextStation()
    {
        currentStationIndex++;

        // Jeœli indeks przekroczy liczbê stacji, wy³¹czamy radio
        if (currentStationIndex >= radioStations.Length)
        {
            currentStationIndex = -1;
            StopRadio();
        }
        else
        {
            PlayStation(radioStations[currentStationIndex]);
        }
    }

    void PlayStation(AudioClip clip)
    {
        // Opcjonalnie: krótki efekt szumu przy prze³¹czaniu
        if (staticNoiseClip != null)
        {
            audioSource.PlayOneShot(staticNoiseClip, 0.5f);
        }

        audioSource.clip = clip;
        audioSource.Play();
        Debug.Log("Radio Station: " + (currentStationIndex + 1));
    }

    void StopRadio()
    {
        audioSource.Stop();
        Debug.Log("Radio Off");
    }

    // Pozwala na aktualizacjê filtrów w czasie rzeczywistym w inspektorze
    void OnValidate()
    {
        var low = GetComponent<AudioLowPassFilter>();
        var high = GetComponent<AudioHighPassFilter>();
        var dist = GetComponent<AudioDistortionFilter>();

        if (low) low.cutoffFrequency = lowPassFreq;
        if (high) high.cutoffFrequency = highPassFreq;
        if (dist) dist.distortionLevel = radioDistortion;
    }
}