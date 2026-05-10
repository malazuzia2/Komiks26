using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem; 

public class BoatMovement : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float turnSpeed = 100f;
    private float originalMoveSpeed;
    private float originalTurnSpeed;
    public bool isAttacked = false;
    public List<GameObject> snapAttackPoints;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    [Header("Sounds")]
    //public AudioClip pantingSound;
    public AudioSource oddychanie;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalMoveSpeed = moveSpeed;
        originalTurnSpeed = turnSpeed;

        SimpleMessage.Instance.ShowMessage("Nothing there... So following the light it is, i guess...");

    }

    void Update()
    {
        checkIfAttacked();

        if (isAttacked)
        {
            moveSpeed = 0f;
            turnSpeed = 0f;
            return;
        }

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

         moveInput = 0;
        if (keyboard.wKey.isPressed)
        {
            moveInput = 1f;
             
        }
        if (keyboard.sKey.isPressed)
        {
            moveInput = -1f;
             
        }
            turnInput = 0;
        if (keyboard.dKey.isPressed)
        {
            turnInput = 1f;
             
        }
        if (keyboard.aKey.isPressed)
        {
            turnInput = -1f;
            
        }
        
    }

    void FixedUpdate()
    {
         if (Mathf.Abs(moveInput) > 0.1f)
         {
            rb.AddRelativeForce(Vector3.left * moveInput * moveSpeed);
            
            // rb.AddRelativeForce(Vector3.forward * moveInput * moveSpeed);
        }
        


        if (Mathf.Abs(turnInput) > 0.1f)
         {
            float rotation = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);

            rb.MoveRotation(rb.rotation * turnRotation);
            
        }

        bool czyLodzSieRusza = Mathf.Abs(moveInput) > 0.1f || Mathf.Abs(turnInput) > 0.1f;

        if (czyLodzSieRusza)
        {
            if (!oddychanie.isPlaying)
            {
                oddychanie.Play();
            }
        }
        else
        {
             if (oddychanie.isPlaying)
            {
                oddychanie.Stop();
            }
        }

    }

    public void checkIfAttacked()
    {
        if (snapAttackPoints == null || snapAttackPoints.Count == 0)
        {
            // No points -> not attacked
            isAttacked = false;
            moveSpeed = originalMoveSpeed;
            turnSpeed = originalTurnSpeed;
            return;
        }

        // Consider a point "occupied" if it is active and any of its descendant GameObjects has tag "shadowHand"
        bool occupied = snapAttackPoints.Any(point =>
            point != null
            && point.activeInHierarchy
            && point.GetComponentsInChildren<Transform>(true)
                    .Any(t => t != point.transform && t.gameObject.CompareTag("shadowHand"))
        );

        if (occupied)
        {
            isAttacked = true;
        }
        else
        {
            isAttacked = false;
            moveSpeed = originalMoveSpeed;
            turnSpeed = originalTurnSpeed;
        }
    }
    
}