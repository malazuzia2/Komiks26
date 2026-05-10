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

    public float holdTime = 3.0f;
    private float currentHoldTimer = 0f;
    private bool isSolving = false;

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
        Color currentColor = constellationLines.color;

        if (angle < tolerance)
        {
            isSolving = true;
            currentHoldTimer += Time.deltaTime;

            float progress = currentHoldTimer / holdTime;

             float currentOpacity = Mathf.Lerp(0.2f, 1.0f, progress);

            constellationLines.color = currentColor;

            if (currentHoldTimer >= holdTime)
            {
                CompletePuzzle();
            }
        }
        else
        {
            ResetHold();
            float f = Mathf.Clamp01(1f - (angle / 20f));
            float finalAlpha = Mathf.Lerp(minOpacity, 1f, f);
            constellationLines.color = new Color(1, 1, 1, finalAlpha);
        }
    

    }
    void ResetHold()
    {
        if (isSolving)
        {
            currentHoldTimer = 0f;
            isSolving = false;
             constellationLines.color = new Color(1, 1, 1, minOpacity);
        }
    }

    void CompletePuzzle()
    {
        if (finishedSound != null)
        {
            AudioSource.PlayClipAtPoint(finishedSound, transform.position, 0.7f);
        }
        solved = true;
        isSolving = false;
        if (starTargetGroup != null) starTargetGroup.gameObject.SetActive(false);

        NavigationManager.Instance.OnStarsMatched();
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
