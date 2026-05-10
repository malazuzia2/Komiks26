using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AstrologicalTelescope : PickableItem
{
    [Header("Luneta - UI")]
    public GameObject telescopeUI;
    public Image constellationLines;
    public float zoomFOV = 15f;
    [Range(0, 1)] public float minOpacity = 0.2f;

    [Header("Gwiazdy - Mechanika")]
    public Transform starTargetGroup;
    public float tolerance = 10f;
    public float holdTime = 4.0f; // Czas rozwi¹zania zagadki (4 sekundy)

    [Header("Efekty Wizualne")]
    public ParticleSystem starExplosionPrefab; // Prefab wybuchu (Particle System)
    public float maxUIEmission = 2.0f;
    public float maxStarBrightness = 5.0f;

    [Header("DŸwiêki")]
    public AudioClip pickupSound;
    public AudioClip finishedSound;

    // Prywatne zmienne pomocnicze
    private float currentHoldTimer = 0f;
    private bool isSolving = false;
    private float defaultFOV;
    private bool isZooming = false;
    private bool isHeld = false;
    private bool solved = false;
    private bool hasShownHint = false;


    private Material uiMat;
    private List<Material> currentStarMaterials = new List<Material>();

    void Start()
    {
        if (Camera.main != null)
            defaultFOV = Camera.main.fieldOfView;

        // Pobieramy materia³ z Image (wymaga Custom Shadera na UI Image)
        if (constellationLines != null)
        {
            // Tworzymy instancjê materia³u, ¿eby nie zmieniaæ assetu na dysku
            uiMat = Instantiate(constellationLines.material);
            constellationLines.material = uiMat;
        }
    }

    public override void OnPickUp(Transform hand)
    {
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, 0.7f);

        base.OnPickUp(hand);
        isHeld = true;


    }

    public override void OnDrop()
    {
        base.OnDrop();
        isHeld = false;
        ExitZoom();
    }

    void Update()
    {
        if (!isHeld || solved) return;

        if (!isZooming && !hasShownHint)
        {
            SimpleMessage.Instance.ShowMessage("Hold RBM to zoom in on the stars, and hold on there for a second.");
            hasShownHint = true;
        }


        if (Mouse.current.rightButton.isPressed)
        {
            EnterZoom();

            // Sprawdzamy postêp tylko jeœli NavigationManager na to pozwala
            if (NavigationManager.Instance != null && NavigationManager.Instance.isSearchingPhase)
            {
                CheckStars();
            }
        }
        else
        {
            ExitZoom();
            ResetHold();
        }
    }

    void EnterZoom()
    {
        if (isZooming) return;
        isZooming = true;
        Camera.main.fieldOfView = zoomFOV;
        telescopeUI.SetActive(true);

        int starsLayer = LayerMask.NameToLayer("Stars");
        if (starsLayer != -1) Camera.main.cullingMask |= (1 << starsLayer);
    }

    void ExitZoom()
    {
        if (!isZooming) return;
        isZooming = false;
        Camera.main.fieldOfView = defaultFOV;
        telescopeUI.SetActive(false);

        int starsLayer = LayerMask.NameToLayer("Stars");
        if (starsLayer != -1) Camera.main.cullingMask &= ~(1 << starsLayer);
    }

    void CheckStars()
    {
        if (starTargetGroup == null) return;

        Vector3 dirToStars = (starTargetGroup.position - Camera.main.transform.position).normalized;
        float angle = Vector3.Angle(Camera.main.transform.forward, dirToStars);

        if (angle < tolerance)
        {
            isSolving = true;
            currentHoldTimer += Time.deltaTime;

            // Obliczamy progres 0-1 w czasie holdTime (4s)
            float progress = Mathf.Clamp01(currentHoldTimer / holdTime);

            UpdateVisualEffects(progress);

            if (currentHoldTimer >= holdTime)
            {
                CompletePuzzle();
            }
        }
        else
        {
            // Jeœli gracz "zgubi" gwiazdy, p³ynnie resetujemy (lub nagle, zale¿nie od preferencji)
            ResetHold();
        }
    }

    void UpdateVisualEffects(float progress)
    {
        // 1. P³ynne opacity linii konstelacji (od minOpacity do 1.0)
        float currentAlpha = Mathf.Lerp(minOpacity, 1.0f, progress);
        constellationLines.color = new Color(1, 1, 1, currentAlpha);

        // 2. Zwiêkszanie Emission na UI (Shader musi mieæ parametr _EmissionPower)
        if (uiMat != null)
            uiMat.SetFloat("_EmissionPower", progress * maxUIEmission);

        // 3. Rozjaœnianie gwiazd w œwiecie (Shader gwiazd musi mieæ parametr _Brightness)
        foreach (Material mat in currentStarMaterials)
        {
            if (mat != null)
                mat.SetFloat("_Brightness", 1.0f + (progress * maxStarBrightness));
        }
    }

    void ResetHold()
    {
        if (isSolving)
        {
            currentHoldTimer = 0f;
            isSolving = false;
            UpdateVisualEffects(0); // Wraca do stanu pocz¹tkowego
        }
    }

    void CompletePuzzle()
    {
        solved = true;
        isSolving = false;

        if (finishedSound != null)
            AudioSource.PlayClipAtPoint(finishedSound, transform.position, 0.7f);

        // Uruchamiamy sekwencjê koñcow¹
        StartCoroutine(SolveAnimationSequence());

        if (NavigationManager.Instance != null)
            NavigationManager.Instance.OnStarsMatched();
    }

    IEnumerator SolveAnimationSequence()
    {
        // 1. Wybuch (Particle System)
        if (starExplosionPrefab != null)
        {
            Instantiate(starExplosionPrefab, starTargetGroup.position, Quaternion.identity);
        }

        // 2. P³ynny Dissolve gwiazd
        float elapsed = 0;
        float dissolveDuration = 2.0f;
        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            float dissolveProgress = elapsed / dissolveDuration;

            foreach (Material mat in currentStarMaterials)
            {
                if (mat != null)
                    mat.SetFloat("_DissolveAmount", dissolveProgress);
            }
            yield return null;
        }

        starTargetGroup.gameObject.SetActive(false);
    }

    // Wywo³ywane przez zewnêtrzny skrypt menad¿era zagadek
    public void SetNewTarget(Transform newStars, Sprite newImage)
    {
        starTargetGroup = newStars;
        constellationLines.sprite = newImage;
        solved = false;
        currentHoldTimer = 0;

        // Cache materia³ów gwiazd, aby nie szukaæ ich co klatkê
        currentStarMaterials.Clear();
        Renderer[] renderers = starTargetGroup.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            currentStarMaterials.Add(r.material);
        }

        UpdateVisualEffects(0);
    }
}