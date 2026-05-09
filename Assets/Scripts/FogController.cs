using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("Dystans")]
    public float currentRadius = 40f;
    public float minRadius = 8f;
    public float maxRadius = 70f;

    [Header("Ustawienia Gêstoœci przy £odzi")]
    public float minParticleSize = 10f;  // Ma³e, ale wystarczaj¹ce by siê nak³adaæ
    public float maxParticleSize = 50f;  // Wielkie na horyzoncie
    public float highAlpha = 0.8f;       // Mocno zas³aniaj¹ce (blisko)
    public float lowAlpha = 0.3f;        // Bardziej przeŸroczyste (daleko)

    private ParticleSystem fogParticles;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.ShapeModule fogShape;
    private ParticleSystem.EmissionModule emissionModule;

    void Start()
    {
        fogParticles = GetComponent<ParticleSystem>();
        mainModule = fogParticles.main;
        fogShape = fogParticles.shape;
        emissionModule = fogParticles.emission;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
    }

    void Update()
    {
        // 1. Logika latarki
        Lantern flashlight = FindFirstObjectByType<Lantern>();
        if (flashlight != null && flashlight.flashlightLight.enabled)
            currentRadius = Mathf.MoveTowards(currentRadius, maxRadius, 25f * Time.deltaTime);
        else
            currentRadius = Mathf.MoveTowards(currentRadius, minRadius, 2f * Time.deltaTime);

        currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);
         
        float t = (currentRadius - minRadius) / (maxRadius - minRadius);
          
        mainModule.startSize = Mathf.Lerp(minParticleSize, maxParticleSize, t);

        Color c = mainModule.startColor.color;
        c.a = Mathf.Lerp(highAlpha, lowAlpha, t);  
        mainModule.startColor = c;
         
        emissionModule.rateOverTime = Mathf.Lerp(400, 50, t);
         
        fogShape.radius = currentRadius;
         
        RenderSettings.fogStartDistance = currentRadius * 0.1f;
        RenderSettings.fogEndDistance = currentRadius * 1.1f;
    }
}