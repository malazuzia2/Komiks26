using UnityEngine;

public class PickableItem : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    private Collider col;
     
    private Vector3 originLocalPosition;
    private Quaternion originLocalRotation;
    private Transform originParent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
         
        originLocalPosition = transform.localPosition;
        originLocalRotation = transform.localRotation;
        originParent = transform.parent;  
    }

    public void Interact()
    { 
    }

    public virtual void OnPickUp(Transform hand)
    {
        rb.isKinematic = true;      
        col.enabled = false;        

        transform.SetParent(hand);   
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public virtual void OnDrop()
    { 
        transform.SetParent(originParent);
         
        transform.localPosition = originLocalPosition;
        transform.localRotation = originLocalRotation;

        rb.isKinematic = true;  
        col.enabled = true;   
    }
}