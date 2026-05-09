using System.Collections.Generic;
using UnityEngine;

public class ShadowManager : MonoBehaviour
{
    [Tooltip("Settings")]
    public float shadowSpawnDistance = 10f;
    public float shadowSpawnInterval = 10f;
    public int maxShadowHands = 5;
    public GameObject shadowHandPrefab;
    public GameObject player;
    public GameObject camera;

    [SerializeField]
    private List<ShadowHand> activeShadowHands = new List<ShadowHand>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (shadowHandPrefab == null)
        {
            Debug.LogWarning("Shadow Hand Prefab is not assigned.");
            return;
        }
        if (player == null)
        {
            Debug.LogWarning("Player GameObject is not assigned.");
            return;
        }
        if (Time.time >= shadowSpawnInterval)
        {
            SpawnShadowHand();
            shadowSpawnInterval = Time.time + shadowSpawnInterval; // Reset the timer
        }
    }

    void SpawnShadowHand()
    {
        if (activeShadowHands.Count >= maxShadowHands)
        {
            Debug.LogWarning("Maximum number of shadow hands reached.");
            return;
        }

        float angle = Random.Range(0f, 180f);
        Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
        Vector3 spawnPosition = player.transform.position + direction * shadowSpawnDistance;

        Vector3 directionToPlayer = (player.transform.position - spawnPosition).normalized;
        Quaternion baseRotation = shadowHandPrefab.transform.rotation;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        Quaternion finalRotation = baseRotation * lookRotation;
        
        GameObject shadowHandObject = Instantiate(shadowHandPrefab, spawnPosition, finalRotation);
        ShadowHand shadowHand = shadowHandObject.GetComponent<ShadowHand>();

        if (shadowHand == null)
        {
            Debug.LogError("Shadow Hand Prefab does not contain a ShadowHand component.");
            Destroy(shadowHandObject);
            return;
        }

        shadowHand.Initialize(this, player, camera);
        activeShadowHands.Add(shadowHand);
    }

    public void RemoveShadowHand(ShadowHand shadowHand)
    {
        if (activeShadowHands.Remove(shadowHand) == false)
        {
            Debug.LogWarning("Tried to remove a ShadowHand that was not tracked by ShadowManager.");
        }
    }
}
