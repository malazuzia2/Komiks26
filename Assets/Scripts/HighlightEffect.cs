using UnityEngine;
using System.Collections.Generic;

public class HighlightEffect : MonoBehaviour
{
    private Renderer[] allRenderers;
    private List<Color[]> originalEmissionColors = new List<Color[]>();
    private bool isHighlighted = false;

    [Header("Ustawienia Podœwietlenia")]
    [ColorUsage(true, true)]
    public Color highlightColor = new Color(1.5f, 1.5f, 1.5f); // Jasne œwiecenie HDR

    void Start()
    {
        // 1. ZnajdŸ wszystkie komponenty Renderer w tym obiekcie i jego dzieciach
        allRenderers = GetComponentsInChildren<Renderer>();

        // 2. Zapamiêtaj oryginalne kolory Emission dla KA¯DEGO materia³u w KA¯DYM rendererze
        foreach (Renderer rend in allRenderers)
        {
            Material[] mats = rend.materials;
            Color[] colors = new Color[mats.Length];

            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i].HasProperty("_EmissionColor"))
                {
                    colors[i] = mats[i].GetColor("_EmissionColor");
                }
                else
                {
                    colors[i] = Color.black;
                }
            }
            originalEmissionColors.Add(colors);
        }
    }

    public void ToggleHighlight(bool on)
    {
        if (isHighlighted == on) return;
        isHighlighted = on;

        for (int r = 0; r < allRenderers.Length; r++)
        {
            Renderer rend = allRenderers[r];
            Material[] mats = rend.materials;

            for (int m = 0; m < mats.Length; m++)
            {
                if (mats[m].HasProperty("_EmissionColor"))
                {
                    if (on)
                    {
                        mats[m].EnableKeyword("_EMISSION");
                        mats[m].SetColor("_EmissionColor", highlightColor);
                    }
                    else
                    {
                        mats[m].SetColor("_EmissionColor", originalEmissionColors[r][m]);

                        // Wy³¹cz œwiecenie tylko jeœli oryginalny kolor by³ czarny
                        if (originalEmissionColors[r][m] == Color.black)
                            mats[m].DisableKeyword("_EMISSION");
                    }
                }
            }
            // Wa¿ne: musimy przypisaæ tablicê z powrotem do renderera
            rend.materials = mats;
        }
    }
}