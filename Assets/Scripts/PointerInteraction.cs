using UnityEngine;
using UnityEngine.InputSystem;

public class PointerInteraction : MonoBehaviour
{
    public float interactDistance = 5f; 
    public LayerMask interactableLayer; 

    void Update()
    {
         Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

         if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
             Debug.Log("Pointer najecha³ na: " + hit.collider.name);

             if (Pointer.current.press.isPressed || Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("Klikniêto w: " + hit.collider.name);
             }
        }
    }

     void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * interactDistance);
    }
}