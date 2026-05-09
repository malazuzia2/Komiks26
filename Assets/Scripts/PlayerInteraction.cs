using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public float range = 5f;
    public LayerMask interactableLayer;
    public Transform handAnchor;

    public float cameraShakeIntensity = 3f;
    public float cameraShakeDuration = 2f;

    private PickableItem currentlyHeldItem;
    private Camera mainCamera;
    private Vector3 originalCameraPosition;

    void Start()
    {
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.localPosition;
    }

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
            CameraShake();
            Debug.Log("Dropped");
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

}