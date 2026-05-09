using UnityEngine;

public class FogController : MonoBehaviour
{
    public float currentRadius = 40f;
    public float minRadius = 5f;
    public float maxRadius = 60f;
    public float shrinkSpeed = 0.5f;
    public float pushForce = 20f;

    private ParticleSystem.ShapeModule fogShape;

    void Start()
    {
        fogShape = GetComponent<ParticleSystem>().shape;
         RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
    }

    void Update()
    {
        Lantern flashlight = FindFirstObjectByType<Lantern>();

        if (flashlight != null && flashlight.flashlightLight.enabled)
            currentRadius = Mathf.MoveTowards(currentRadius, maxRadius, pushForce * Time.deltaTime);
        else
            currentRadius -= shrinkSpeed * Time.deltaTime;

        currentRadius = Mathf.Clamp(currentRadius, minRadius, maxRadius);

         fogShape.radius = currentRadius;
         
        RenderSettings.fogStartDistance = currentRadius * 0.2f; 
        RenderSettings.fogEndDistance = currentRadius * 1.2f;    

    }

}
