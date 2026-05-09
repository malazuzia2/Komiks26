using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
       public float cameraShakeIntensity = 3f;
    public float cameraShakeDuration = 2f;
    private CinemachineImpulseSource impulseSource;

    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.localPosition;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource == null)
        {
            impulseSource = gameObject.AddComponent<CinemachineImpulseSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraShaker()
    {
        if (impulseSource != null)
        {
            Vector3 impulseDirection = Random.insideUnitSphere.normalized;
            impulseSource.GenerateImpulse(impulseDirection * cameraShakeIntensity);
            Debug.Log("Camera shake triggered!");
        }
        else
        {
            Debug.LogError("CinemachineImpulseSource not found! Make sure camera has CinemachineImpulseListener.");
        }
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
