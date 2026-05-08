using UnityEngine;

public class Radyjko : MonoBehaviour, IInteractable
{
    private AudioSource audioSource;
    private bool isOn = false;

    void Awake()
    { 
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    { 
        ToggleRadio();
    }

    void ToggleRadio()
    {
        isOn = !isOn;

        if (isOn)
        {
            audioSource.Play();
            Debug.Log("Radio on");
        }
        else
        {
            audioSource.Stop(); 
            Debug.Log("Radio off");
        }
    }

}
