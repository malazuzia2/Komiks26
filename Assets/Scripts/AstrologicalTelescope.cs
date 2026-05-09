using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AstrologicalTelescope : PickableItem
{
    [Header("Ustawienia Lunety")]
    public GameObject telescopeUI;      // Przeci¹gnij tu obiekt TelescopeView
    public Image constellationLines;    // Przeci¹gnij tu obrazek linii
    public float zoomFOV = 15f;         // Przybli¿enie (im mniejsze, tym wiêkszy zoom)

    [Header("Logika Gwiazd")]
    public Transform starTargetGroup;   // Przeci¹gnij tu StarTargetGroup z nieba
    public float tolerance = 10f;        // Jak dok³adnie trzeba wycelowaæ (stopnie)

    private float defaultFOV;
    private bool isZooming = false;
    private bool isHeld = false;
    private bool solved = false;

    void Start()
    {
        defaultFOV = Camera.main.fieldOfView;
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
        ExitZoom();
    }


    void EnterZoom()
    {
        isZooming = true;
        Camera.main.fieldOfView = zoomFOV;  
        telescopeUI.SetActive(true);      
         
        int starsLayer = LayerMask.NameToLayer("Stars");
        if (starsLayer != -1)
        { 
            Camera.main.cullingMask |= (1 << starsLayer);
        }
    }

    void ExitZoom()
    {
        isZooming = false;
        Camera.main.fieldOfView = defaultFOV; 
        telescopeUI.SetActive(false);        
         
        int starsLayer = LayerMask.NameToLayer("Stars");
        if (starsLayer != -1)
        { 
            Camera.main.cullingMask &= ~(1 << starsLayer);
        }
    }
    void Update()
    {
        if (!isHeld) return;

         if (Mouse.current.rightButton.isPressed)
        {
            EnterZoom();

             if (NavigationManager.Instance.isSearchingPhase)
            {
                CheckStars();
            }
        }
        else
        {
            ExitZoom();
        }
    }

    [Range(0, 1)] public float minOpacity = 0.2f;  

    void CheckStars()
    {
        if (starTargetGroup == null) return;
         
        Vector3 dirToStars = (starTargetGroup.position - Camera.main.transform.position).normalized;
        float angle = Vector3.Angle(Camera.main.transform.forward, dirToStars);

        Debug.Log("Aktualny k¹t do gwiazd: " + angle);

        float f = Mathf.Clamp01(1f - (angle / 20f));
         
        float finalAlpha = Mathf.Lerp(minOpacity, 1f, f);
         
        if (constellationLines != null)
        {
            constellationLines.color = new Color(constellationLines.color.r, constellationLines.color.g, constellationLines.color.b, finalAlpha);
        }
         
        if (angle < tolerance)
        {
            constellationLines.color = Color.cyan;  
            NavigationManager.Instance.OnStarsMatched();
            solved = true;

            if (starTargetGroup != null)
            {
                starTargetGroup.gameObject.SetActive(false);
            }
        }
    }
}