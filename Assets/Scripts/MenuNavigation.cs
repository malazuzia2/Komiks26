using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[System.Serializable]
public struct TooltipData
{
    public Button button;
    [TextArea] public string tooltipText;
}

public class MenuNavigation : MonoBehaviour
{
    public List<Button> buttons;
    public List<Sprite> normalSprites;
    public List<Sprite> selectedSprites;

    private int selectedIndex = 0;

    void OnEnable()
    {
        selectedIndex = 0;
        UpdateVisuals();
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!gameObject.activeInHierarchy) return;

        if (keyboard.wKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex - 1 + buttons.Count) % buttons.Count;
            UpdateVisuals();
        }
        else if (keyboard.sKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex + 1) % buttons.Count;
            UpdateVisuals();
        }

        if (keyboard.spaceKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame)
        {
            buttons[selectedIndex].onClick.Invoke();
        }
    }


    void UpdateVisuals()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            // Podmieniamy obrazek w komponencie Image przycisku
            Image img = buttons[i].GetComponent<Image>();

            if (i == selectedIndex)
            {
                img.sprite = selectedSprites[i]; // Ustaw wybrany
                buttons[i].Select();             // Opcjonalnie: ustawia focus
            }
            else
            {
                img.sprite = normalSprites[i];   // Ustaw zwyk³y
            }
        }
    }
}