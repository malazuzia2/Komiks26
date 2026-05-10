using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SimpleMessage : MonoBehaviour
{
    public static SimpleMessage Instance;

    [SerializeField] private GameObject messagePanel; // Ca³y obiekt z UI
    [SerializeField] private TextMeshProUGUI textElement;

    private bool isVisible = false;

    private void Awake()
    {
        Instance = this;
        messagePanel.SetActive(false); // Wy³¹cz na start
    }

    private void Update()
    {
        // Jeœli panel jest widoczny i klikniêto lewy przycisk myszy
        if (isVisible && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HideMessage();
        }
    }

    public void ShowMessage(string text)
    {
        textElement.text = text;
        messagePanel.SetActive(true);
        isVisible = true;

        // Opcjonalnie: odblokuj kursor, ¿eby gracz czu³, ¿e mo¿e klikn¹æ
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideMessage()
    {
        messagePanel.SetActive(false);
        isVisible = false;

        // Powrót do stanu gry (jeœli nie jesteœmy w menu)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}