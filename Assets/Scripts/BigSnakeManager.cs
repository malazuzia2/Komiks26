using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawner/manager for BigSnake instances. Attach to a scene object and assign `snakePrefab` (which should contain the `BigSnake` script).
/// </summary>
public class BigSnakeManager : MonoBehaviour
{
    [Header("Spawner (manager mode)")]
    public GameObject snakePrefab;                // Prefab that contains `BigSnake` script (used for spawning)
    public Transform player;                      // Player transform (boat)
    public float spawnInterval = 180f;            // Seconds between spawns
    public Transform[] waypoints;                 // Waypoints to assign to spawned snake
    public ShadowManager shadowManagerToFreeze;   // Optional: assign ShadowManager to freeze respawns. If null, instances will find all in scene.
    private bool hasSpawnedOnce = false;

    // runtime
    private float spawnTimer;

    void Awake()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        HandleSpawner();
    }

    private void HandleSpawner()
    {
        if (snakePrefab == null || player == null) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnSnake();
            if (!hasSpawnedOnce)
            {
                hasSpawnedOnce = true;
                spawnTimer = spawnInterval + 100f;
            }
            else
            {
                spawnTimer = spawnInterval;
            }
        }
    }


    private void SpawnSnake()
    {
        if (snakePrefab == null) return;

        Vector3 spawnPos;
        Quaternion rot;

        // Prefer first waypoint if available
        if (waypoints != null && waypoints.Length > 0 && waypoints[0] != null)
        {
            spawnPos = waypoints[0].position + (Vector3.up * 0.5f); // small offset to avoid ground clipping

            if (player != null)
            {
                // Face the player
                Vector3 dirToPlayer = (player.position - spawnPos);
                if (dirToPlayer.sqrMagnitude > 0.0001f)
                    rot = Quaternion.LookRotation(dirToPlayer.normalized, Vector3.up);
                else
                    rot = waypoints[0].rotation;
            }
            else
            {
                rot = waypoints[0].rotation;
            }
        }
        else
        {
            // Fallback: spawn in front of the player (original behavior)
            if (player == null) return;

            spawnPos = player.position + (player.forward * 12f) + (Vector3.up * 0.5f);
            rot = Quaternion.LookRotation((player.position - spawnPos).normalized, Vector3.up);
        }

        GameObject inst = Instantiate(snakePrefab, spawnPos, rot);
        BigSnake instScript = inst.GetComponent<BigSnake>();
        if (instScript == null)
        {
            Debug.LogError("snakePrefab must contain BigSnake script.");
            Destroy(inst);
            return;
        }

        // configure the instance
        instScript.SwitchToInstanceMode(this);
    }
}
