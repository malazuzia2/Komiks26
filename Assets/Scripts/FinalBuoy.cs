using UnityEngine;

public class FinalBuoy : MonoBehaviour
{
    public float finishRadius = 15f;

    void Update()
    {
         float distance = Vector3.Distance(transform.position, Camera.main.transform.position);

        if (distance < finishRadius)
        {
             GameUIManager uiManager = FindObjectOfType<GameUIManager>();
            if (uiManager != null)
            {
                uiManager.ShowGameOver();
            }

             Destroy(gameObject);
        }
    }
}