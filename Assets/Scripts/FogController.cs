using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("radius")]
    public float currentRadius = 40f;
    public float minRadius = 8f;
    public float maxRadius = 70f;

    [Header("Gestosc")]
    public float minParticleSize = 10f;
    public float maxParticleSize = 50f;
    public float highAlpha = 0.8f;
    public float lowAlpha = 0.3f;

    [Header("Odpychanie")]
    public float pushForce = 25f;
    public float shrinkSpeed = 2f;

    private float buoyTimer = 0f;  

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
        Lantern flashlight = FindFirstObjectByType<Lantern>();
        bool isLanternOn = (flashlight != null && flashlight.flashlightLight.enabled);
         
        bool activeBuoyNearby = false;
         
        List<Buoy> currentBuoys = new List<Buoy>(Buoy.allBuoys);
        foreach (Buoy buoy in currentBuoys)
        {
            if (buoy == null) continue;

            float distance = Vector3.Distance(transform.position, buoy.transform.position);
             
            if (distance < buoy.safeRadius)
            { 
                float duration = buoy.Activate();
                if (duration > 0)
                {
                    buoyTimer = duration;  
                }
            }
        }
         
        if (buoyTimer > 0)
        {
            buoyTimer -= Time.deltaTime;
            activeBuoyNearby = true;
        } 
        if (isLanternOn || activeBuoyNearby)
        {
            currentRadius = Mathf.MoveTowards(currentRadius, maxRadius, pushForce * Time.deltaTime);
        }
        else
        {
            currentRadius = Mathf.MoveTowards(currentRadius, minRadius, shrinkSpeed * Time.deltaTime);
        }

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