using UnityEngine;
using UnityEngine.InputSystem; 

public class CursorManager : MonoBehaviour
{
    void Start()
    {
         LockCursor();
    }

    void Update()
    {
         if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

         if (Pointer.current.press.isPressed)
        {
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;                  
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;    
        Cursor.visible = true;                   
    }
}