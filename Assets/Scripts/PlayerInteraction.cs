using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Ustawienia Interakcji")]
    public float range = 5f;
    public LayerMask interactableLayer;
    public Transform handAnchor;        // Miejsce "r�ki" pod kamer�

    [Header("Stan Gracza")]
    private PickableItem currentlyHeldItem; // Aktualnie trzymany przedmiot
    private HighlightEffect lastHighlighted; // Ostatnio pod�wietlony obiekt
    public float cameraShakeIntensity = 3f;
    public float cameraShakeDuration = 2f;
    private CinemachineImpulseSource impulseSource;

    private Camera mainCamera;
    private Vector3 originalCameraPosition;

    void Start()
    {
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.localPosition;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource == null)
        {
            impulseSource = gameObject.AddComponent<CinemachineImpulseSource>();
        }
    }

    void Update()
    {
        // 1. Wysy�amy promie� idealnie ze �rodka ekranu
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Sprawdzamy czy patrzymy na co� interaktywnego na warstwie Interactable
        if (Physics.Raycast(ray, out hit, range, interactableLayer))
        {
            // --- LOGIKA POD�WIETLANIA ---
            HighlightEffect highlight = hit.collider.GetComponent<HighlightEffect>();

            if (highlight != null)
            {
                if (lastHighlighted != highlight)
                {
                    // Wy��czamy poprzednie pod�wietlenie
                    if (lastHighlighted != null) lastHighlighted.ToggleHighlight(false);

                    // W��czamy nowe pod�wietlenie na obiekcie
                    highlight.ToggleHighlight(true);
                    lastHighlighted = highlight;
                }
            }

            // --- LOGIKA KLIKNI�CIA (LPM) ---
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (currentlyHeldItem == null)
                {
                    HandleNewInteraction(hit.collider.gameObject);
                }
                else
                {
                    DropCurrentItem();
                }
            }
        }
        else
        {
            // --- JE�LI NIC NIE WIDZIMY ---
            ResetHighlight();

            // Klikni�cie w pust� przestrze� trzymaj�c przedmiot - upu�� go
            if (Mouse.current.leftButton.wasPressedThisFrame && currentlyHeldItem != null)
            {
                DropCurrentItem();
            }
        }
    }

    public void CameraShake()
    {
        StartCoroutine(CameraShakeCoroutine());
    }

    IEnumerator CameraShakeCoroutine()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < cameraShakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / cameraShakeDuration;
            
            // Zmniejszanie intensywności shake'a w czasie
            float intensity = cameraShakeIntensity * (1f - progress);
            
            // Losowe przesunięcie kamery
            Vector3 randomOffset = Random.insideUnitSphere * intensity;
            mainCamera.transform.localPosition = originalCameraPosition + randomOffset;
            
            yield return null;
        }
        Debug.Log("Camera shake ended.");
        mainCamera.transform.localPosition = originalCameraPosition;
    }   

    void HandleNewInteraction(GameObject obj)
    {
        // Interakcja (Radio / Prze��czniki)
        IInteractable interactable = obj.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact();
        }

        // Podnoszenie (Latarka / Kostka)
        PickableItem item = obj.GetComponent<PickableItem>();
        if (item != null)
        {
            currentlyHeldItem = item;
            currentlyHeldItem.OnPickUp(handAnchor);
        }
    }

    void DropCurrentItem()
    {
        if (currentlyHeldItem != null)
        {
            currentlyHeldItem.OnDrop();
            currentlyHeldItem = null;
        }
    }

    void ResetHighlight()
    {
        if (lastHighlighted != null)
        {
            lastHighlighted.ToggleHighlight(false);
            lastHighlighted = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * range);
    }
}