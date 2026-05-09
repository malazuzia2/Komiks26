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
            Debug.Log("Item dropped!");
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