using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float range = 5f;
    public LayerMask interactableLayer;
    public Transform handAnchor;

    private PickableItem currentlyHeldItem;

    void Update()
    { 
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentlyHeldItem == null)
            {
                TryPickUp();
            }
            else
            {
                DropItem();
            }
        }

    }

    void TryPickUp()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, interactableLayer))
        {
             IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }

             PickableItem item = hit.collider.GetComponent<PickableItem>();
            if (item != null)
            {
                currentlyHeldItem = item;
                currentlyHeldItem.OnPickUp(handAnchor);
            }
        }

    }

    void DropItem()
    {
        if (currentlyHeldItem != null)
        {
            currentlyHeldItem.OnDrop();
            currentlyHeldItem = null;
            Debug.Log("Dropped");
        }
    }


}