using UnityEngine;
using UnityEngine.UIElements;
using Unity.Cinemachine;

public class ShadowHand : MonoBehaviour
{
    [Tooltip("Stats")]
    public float allowedDistanceFromPlayer = 5f;
    public float shorteningIntensity = 1f;
    public float shorteningInterval = 2f;
    public float ligtheningTimer = 3f;
    public float moveSpeed = 2f;
    private bool isAttacking = false;
    private bool enlightened = false;
    public GameObject player;
    public GameObject camera;
    private ShadowManager shadowManager;
    private CinemachineImpulseSource impulseSource;
    private SnapAttackPoint occupiedPoint;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player GameObject is not assigned.");
            return;
        }

        if (enlightened)
        {
            lightAversion();
            return;
        }
        else
        {
            ligtheningTimer = 2f;
        }

        if (isAttacking)
        {
            return; // Skip movement while attacking
        }

        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer > shadowManager.shadowSpawnDistance + 2f)
        {
            Death();
            return;
        }

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        Vector3 rotationDirection = Quaternion.LookRotation(directionToPlayer).eulerAngles;

        if (distanceFromPlayer > allowedDistanceFromPlayer)
        {
            transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
        }
        else if (distanceFromPlayer <= allowedDistanceFromPlayer)
        {
            shorteningInterval += Time.deltaTime;
            if (shorteningInterval >= 2f)
            {
                allowedDistanceFromPlayer -= shorteningIntensity;
                shorteningInterval = 0f; // Reset the interval timer
            }
        }
        
        transform.rotation = Quaternion.Euler(rotationDirection.x, rotationDirection.y, rotationDirection.z);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AttackBoat();
        }
    }
    public void CameraShake()
    {
        if (impulseSource != null)
        {
            // To wywo³a wstrz¹s
            impulseSource.GenerateImpulse();
        }
        else
        {
            Debug.LogWarning("Brak CinemachineImpulseSource na obiekcie kamery!");
        }
    }

    public void AttackBoat()
    {
        Debug.Log("Shadow Hand is attacking the boat!");
        isAttacking = true;
        player.GetComponent<BoatMovement>().isAttacked = true;

        SnapAttackPoint closestPoint = FindNearestAvailableSnapPoint();

        if (closestPoint != null)
        {
            // Rezerwujemy punkt
            occupiedPoint = closestPoint;
            occupiedPoint.isOccupied = true;

            // Snapowanie: ustawiamy pozycjê i rotacjê dok³adnie tam gdzie punkt
            // UWAGA: Robimy rêkê dzieckiem punktu, ¿eby buja³a siê razem z ³odzi¹!
            transform.SetParent(occupiedPoint.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            Debug.Log("Shadow Hand snapped to point!");
        }

        CameraShake();
        Debug.Log("Boat is attacked! Movement and rotation are locked.");

        // Add boat tremble, increase fog intensity, lock player movement
        
    }

    public void Death()
    {
        if (isAttacking)
        {
            if (occupiedPoint != null)
            {
                occupiedPoint.isOccupied = false;
                occupiedPoint = null;
            }
            transform.SetParent(null);
            // Delete boat tremble, decrease fog intensity, unlock player movement
        }

        Debug.Log("Shadow Hand has been defeated!");
        Destroy(gameObject);
    }

    public void lightAversion()
    {
        ligtheningTimer -= Time.deltaTime;
        if (isAttacking)
        {
            if (ligtheningTimer <= 0f)
            {
                Death();
            }
            return;
        }

        if (ligtheningTimer <= 0f)
        {
            Vector3 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;
            transform.position += directionAwayFromPlayer * 3 * moveSpeed * Time.deltaTime;
        }
    }

    public bool setEnlightened(bool value)
    {
        enlightened = value;
        return enlightened;
    }

    public void Initialize(ShadowManager manager, GameObject playerReference, GameObject cameraReference)
    {
        shadowManager = manager;
        player = playerReference;
        camera = cameraReference;
    }

    void OnDestroy()
    {
        if (shadowManager != null)
        {
            shadowManager.RemoveShadowHand(this);
        }
    }
    private SnapAttackPoint FindNearestAvailableSnapPoint()
    {
        // ZnajdŸ wszystkie punkty z tagiem
        GameObject[] points = GameObject.FindGameObjectsWithTag("BoatSnapPoint");
        SnapAttackPoint bestPoint = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject pointObj in points)
        {
            SnapAttackPoint sp = pointObj.GetComponent<SnapAttackPoint>();
            if (sp != null && !sp.isOccupied)
            {
                float distance = Vector3.Distance(transform.position, pointObj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestPoint = sp;
                }
            }
        }
        return bestPoint;
    }
}
