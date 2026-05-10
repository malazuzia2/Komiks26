using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class BigSnake : MonoBehaviour
{
    [Header("Instance settings (applies to spawned snake)")]
    public float appearFreezeMin = 5f;            // Minimum freeze duration
    public float appearFreezeMax = 8f;            // Maximum freeze duration
    public float instanceMoveSpeed = 6f;          // Movement speed along waypoints
    public AudioClip roarClip;                    // Optional roar clip used if prefab has no AudioSource
    public ShadowManager shadowManagerToFreeze;   // optional default; manager can override

    // runtime
    private bool isActiveInstance = false;

    // instance-mode fields
    private Transform[] instanceWaypoints;
    private int currentWaypoint = 0;
    private float instanceElapsed = 0f;
    private float instanceDuration = 0f;
    private Transform player;

    private CinemachineImpulseSource impulseSource;

    // store states we modify so we can restore them
    private BoatMovement boatMovementRef;
    private bool boatMovementEnabledBefore;


    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (impulseSource == null)
        {
            Debug.LogWarning("BigSnake instance missing CinemachineImpulseSource - camera shake will not occur.");
        }
    }

    public void SwitchToInstanceMode(BigSnakeManager manager)
    {
        if (manager == null)
        {
            Debug.LogWarning("BigSnake.SwitchToInstanceMode called with null manager - instance will not start.");
            return;
        }

        isActiveInstance = true;

        instanceWaypoints = manager.waypoints;

        instanceDuration = Random.Range(appearFreezeMin, appearFreezeMax);

        player = manager.player;

        StartCoroutine(PerformAppearanceRoutine());
    }

    private IEnumerator PerformAppearanceRoutine()
    {
        PlayRoarAndShake();


        instanceElapsed = 0f;

        while (instanceElapsed < instanceDuration)
        {
            instanceElapsed += Time.deltaTime;
            yield return null;
        }


        Destroy(gameObject);
    }

    private void PlayRoarAndShake()
    {
        AudioSource a = GetComponent<AudioSource>();
        if (a != null)
        {
            if (roarClip != null) a.PlayOneShot(roarClip);
            else a.Play();
        }
        else if (roarClip != null)
        {
            AudioSource.PlayClipAtPoint(roarClip, transform.position, 1f);
        }
        CameraShake();
    }

    


    // minimal waypoint movement for instance-mode snake
    void Update()
    {
        if (!isActiveInstance) return;
        HandleInstanceMovement();
    }

    public void CameraShake()
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
        else
        {
            Debug.LogWarning("Brak CinemachineImpulseSource na obiekcie kamery!");
        }
    }

    public float instanceRotationSpeed = 720f; // degrees per second (configurable)

    private void HandleInstanceMovement()
    {
        if (instanceWaypoints == null || instanceWaypoints.Length == 0) return;
        if (currentWaypoint >= instanceWaypoints.Length) currentWaypoint = 0;

        Transform target = instanceWaypoints[currentWaypoint];
        if (target == null) return;

        // direction to target
        Vector3 dir = target.position - transform.position;

        // Move toward target (as before)
        Vector3 move = dir.normalized * instanceMoveSpeed * Time.deltaTime;
        if (move.sqrMagnitude >= dir.sqrMagnitude)
        {
            // reached
            transform.position = target.position;
            currentWaypoint = (currentWaypoint + 1) % instanceWaypoints.Length;
        }
        else
        {
            transform.position += move;
        }

        Vector3 horizontalDir = new Vector3(dir.x, 0f, dir.z);
        const float minSqrMagnitudeForRotation = 0.0001f; // small threshold
        if (horizontalDir.sqrMagnitude > minSqrMagnitudeForRotation)
        {
            Quaternion desired = Quaternion.LookRotation(horizontalDir.normalized, Vector3.up);
            float maxDegreesThisFrame = instanceRotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, maxDegreesThisFrame);
        }
    }
}
