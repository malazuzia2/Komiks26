using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Ustawienia Interakcji")]
    public float range = 5f;
    public LayerMask interactableLayer;
    public Transform handAnchor;        // Miejsce "rêki" pod kamer¹

    [Header("Stan Gracza")]
    private PickableItem currentlyHeldItem; // Aktualnie trzymany przedmiot
    private HighlightEffect lastHighlighted; // Ostatnio podœwietlony obiekt

    void Update()
    {
        // 1. Wysy³amy promieñ idealnie ze œrodka ekranu
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Sprawdzamy czy patrzymy na coœ interaktywnego na warstwie Interactable
        if (Physics.Raycast(ray, out hit, range, interactableLayer))
        {
            // --- LOGIKA PODŒWIETLANIA ---
            HighlightEffect highlight = hit.collider.GetComponent<HighlightEffect>();

            if (highlight != null)
            {
                if (lastHighlighted != highlight)
                {
                    // Wy³¹czamy poprzednie podœwietlenie
                    if (lastHighlighted != null) lastHighlighted.ToggleHighlight(false);

                    // W³¹czamy nowe podœwietlenie na obiekcie
                    highlight.ToggleHighlight(true);
                    lastHighlighted = highlight;
                }
            }

            // --- LOGIKA KLIKNIÊCIA (LPM) ---
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
            // --- JEŒLI NIC NIE WIDZIMY ---
            ResetHighlight();

            // Klikniêcie w pust¹ przestrzeñ trzymaj¹c przedmiot - upuœæ go
            if (Mouse.current.leftButton.wasPressedThisFrame && currentlyHeldItem != null)
            {
                DropCurrentItem();
            }
        }
    }

    void HandleNewInteraction(GameObject obj)
    {
        // Interakcja (Radio / Prze³¹czniki)
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