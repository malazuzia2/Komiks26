using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class AstrologicalTelescope : PickableItem
{
    [Header("Luneta")]
    public GameObject telescopeUI;       
    public Image constellationLines;     
    public float zoomFOV = 15f;         

    [Header("Gwiazdy")]
    public Transform starTargetGroup;  
    public float tolerance = 10f;

    [Header("Sounds")]
    public AudioClip pickupSound;
    public AudioClip finishedSound;

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

        Debug.Log("Kat do gwiazd: " + angle);

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
            if (finishedSound != null)
            {
                AudioSource.PlayClipAtPoint(finishedSound, transform.position, 0.7f);
            }
            if (starTargetGroup != null)
            {
                StartCoroutine(FadeOutStars(starTargetGroup.gameObject));

                

                starTargetGroup.gameObject.SetActive(false);
            }
        }

    }
    public void SetNewTarget(Transform newStars, Sprite newImage)
    {
        starTargetGroup = newStars;          
        constellationLines.sprite = newImage;  
        solved = false;                  

        constellationLines.color = new Color(1, 1, 1, minOpacity);
    }


    IEnumerator FadeOutStars(GameObject group)
    {
        float duration = 3.0f; 
        float elapsed = 0; 
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime; 
            yield return null;
        }
        group.SetActive(false);
    }
}
