using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        // Pobieramy komponent Impulse Source
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void CameraShaker()
    {
        if (impulseSource != null)
        {
            // To wywoła wstrząs
            impulseSource.GenerateImpulse();
        }
        else
        {
            Debug.LogWarning("Brak CinemachineImpulseSource na obiekcie kamery!");
        }
    }
}
