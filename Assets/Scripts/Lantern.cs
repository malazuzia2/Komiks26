using UnityEngine;
using UnityEngine.InputSystem;

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

    private float flickerTimer;
    private bool isGlitching = false; 
    public float fastFlickerInterval = 0.05f;  

    private bool isOn = false;
    private bool isHeld = false;

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
        if (isHeld && Mouse.current.rightButton.wasPressedThisFrame)
        {
            ToggleLight();
        }
        if (isOn) {
            HandleBattery();
        }
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
        else if (currentBattery > flickerThreshold)
        {
            flashlightLight.enabled = true;
            isGlitching = false;
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
