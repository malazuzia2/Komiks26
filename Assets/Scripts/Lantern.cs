using UnityEngine;
using UnityEngine.InputSystem;

public class Lantern : PickableItem
{
    public Light flashlightLight;
    private bool isOn = false;
    private bool isHeld = false;
     
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
    }

    void ToggleLight()
    {
        if (flashlightLight != null)
        {
            isOn = !isOn;
            flashlightLight.enabled = isOn;
            Debug.Log("light " + isOn);
        }
    }


}
