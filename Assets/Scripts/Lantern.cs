using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Lantern : PickableItem
{
    [Header ("Flashlight")]
    public Light flashlightLight;
    public float maxBattery = 100f;      
    public float currentBattery;
    public float drainRate = 1f;

    [Header ("Flickering")]
    public float flickerThreshold = 30f;   
    public float minIntensity = 0.5f;    
    private float baseIntensity;
    public float maxTimeBetweenGlitches = 3.0f; 
    public float minTimeBetweenGlitches = 0.2f;  
    public float glitchDuration = 0.05f;

    [Header("Shaking")]
    public float shakeDuration = 0.5f;  
    public float shakeAmount = 0.05f;  
    private bool isShaking = false;
    public float shakeSpeed = 20f;     // Jak szybko latarka lata lewo-prawo
    public float shakeDistance = 0.1f; // Jak daleko wychyla si� na boki

    private float flickerTimer;
    private bool isGlitching = false; 
    public float fastFlickerInterval = 0.05f;  

    private bool isOn = false;
    private bool isHeld = false;

    [Header("Shadow Targeting")]
    public float shadowHitRange = 20f;
    public LayerMask shadowHandLayer = ~0;
    private ShadowHand currentTargetedShadowHand;

    public float raycastRadius = 0.25f; // Increase this to widen the area the flashlight detects (tweak in Inspector)
    [Header("Sounds")]
    public AudioClip pickupSound;
    public AudioSource failureHum;
    public AudioClip clickSound;

    private void Start()
    {
        if (flashlightLight != null)
        {
            baseIntensity = flashlightLight.intensity;
            flashlightLight.enabled = false;
        }
        currentBattery = maxBattery;
    }
    
    public override void OnPickUp(Transform hand)
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, 0.7f);
        }

        base.OnPickUp(hand);  
        isHeld = true;
        
    }

    public override void OnDrop()
    {
        base.OnDrop();  
        isHeld = false;
         
        isOn = false;
        if (flashlightLight != null) flashlightLight.enabled = false;
    }

    void Update()
    {
        if (!isHeld) return;

        if (Mouse.current.rightButton.wasPressedThisFrame && !isShaking)
        {
            if (clickSound != null)
            {
                AudioSource.PlayClipAtPoint(clickSound, transform.position, 0.7f);
            }
            ToggleLight();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame && !isShaking)
        {
            StartCoroutine(ShakeAndRecharge());
        }

        if (isOn && !isShaking)
        {
            HandleBattery();
            UpdateShadowRaycast();
        }
    }


    void UpdateShadowRaycast()
    {
        if (flashlightLight == null)
            return;

        Vector3 origin = flashlightLight.transform.position;
        Vector3 direction = flashlightLight.transform.forward;

        // Use SphereCastAll to create a wider detection area (approximate cone)
        RaycastHit[] hits = Physics.SphereCastAll(origin, raycastRadius, direction, shadowHitRange, shadowHandLayer, QueryTriggerInteraction.Ignore);

        if (hits == null || hits.Length == 0)
        {
            if (currentTargetedShadowHand != null)
            {
                currentTargetedShadowHand.setEnlightened(false);
                currentTargetedShadowHand = null;
            }
            return;
        }

        // Sort by distance so nearest hits are processed first
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // Find the first hit that has a ShadowHand component
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            if (hit.collider.TryGetComponent<ShadowHand>(out ShadowHand shadowHand))
            {
                if (currentTargetedShadowHand != null && currentTargetedShadowHand != shadowHand)
                {
                    currentTargetedShadowHand.setEnlightened(false);
                }

                currentTargetedShadowHand = shadowHand;
                currentTargetedShadowHand.setEnlightened(true);
                return;
            }
        }

        // No ShadowHand found in any hit
        if (currentTargetedShadowHand != null)
        {
            currentTargetedShadowHand.setEnlightened(false);
            currentTargetedShadowHand = null;
        }
    }

    IEnumerator ShakeAndRecharge()
    {
        isShaking = true;

        Vector3 originalPos = transform.localPosition;
        Quaternion originalRot = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
             
            float xOffset = Mathf.Sin(elapsed * shakeSpeed) * shakeDistance;

             transform.localPosition = new Vector3(originalPos.x + xOffset, originalPos.y, originalPos.z);

             float tilt = xOffset * 50f;  
            transform.localRotation = originalRot * Quaternion.Euler(0, 0, tilt);

            yield return null;
        }
         
        transform.localPosition = originalPos;
        transform.localRotation = originalRot; 
        Recharge(maxBattery);

        if (currentBattery > 0 && !isOn)
        {
            isOn = true;
            flashlightLight.enabled = true;
        }

        isShaking = false;
    }


    void ToggleLight()
    {
        if (currentBattery > 0)
        {
            isOn = !isOn;
            flashlightLight.enabled = isOn;
        }
    }

    void HandleBattery()
    {
        currentBattery -= drainRate * Time.deltaTime;
        currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);

        float totalPercent = currentBattery / maxBattery;
        flashlightLight.intensity = Mathf.Lerp(minIntensity, baseIntensity, totalPercent);

         if (currentBattery < flickerThreshold && currentBattery > 0)
        {
             if (!failureHum.isPlaying)
             {
                failureHum.Play();
             }

             failureHum.pitch = Mathf.Lerp(1.5f, 1.0f, currentBattery / flickerThreshold);

            float f = currentBattery / flickerThreshold;
            flickerTimer -= Time.deltaTime;

            if (flickerTimer <= 0)
            {
                if (!isGlitching)
                {
                    isGlitching = true;
                    flashlightLight.enabled = false;
                    flickerTimer = glitchDuration;
                     
                }
                else
                {
                    isGlitching = false;
                    flashlightLight.enabled = true;
                    float nextWait = Mathf.Lerp(minTimeBetweenGlitches, maxTimeBetweenGlitches, f);
                    flickerTimer = nextWait * Random.Range(0.8f, 1.2f);
                }
            }
        }
        else
        {
             if (failureHum.isPlaying)
             {
                failureHum.Stop();
             }

            if (currentBattery > flickerThreshold)
            {
                flashlightLight.enabled = true;
                isGlitching = false;
            }
        }

        if (currentBattery <= 0)
        {
            isOn = false;
            flashlightLight.enabled = false;
        }
    }


    public void Recharge(float amount)
    {
        currentBattery += amount;
        currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);
    }
}
