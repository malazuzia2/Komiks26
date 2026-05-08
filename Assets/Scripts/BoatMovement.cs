using UnityEngine;
using UnityEngine.InputSystem; 

public class BoatMovement : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float turnSpeed = 100f;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
         var keyboard = Keyboard.current;
        if (keyboard == null) return;

         moveInput = 0;
        if (keyboard.wKey.isPressed) moveInput = 1f;
        if (keyboard.sKey.isPressed) moveInput = -1f;

         turnInput = 0;
        if (keyboard.dKey.isPressed) turnInput = 1f;
        if (keyboard.aKey.isPressed) turnInput = -1f;
    }

    void FixedUpdate()
    {
         if (Mathf.Abs(moveInput) > 0.1f)
        {
            rb.AddRelativeForce(Vector3.forward * moveInput * moveSpeed);
        }

         if (Mathf.Abs(turnInput) > 0.1f)
        {
            float rotation = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}