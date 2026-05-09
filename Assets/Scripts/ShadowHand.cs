using UnityEngine;
using UnityEngine.UIElements;

public class ShadowHand : MonoBehaviour
{
    [Tooltip("Stats")]
    public float allowedDistanceFromPlayer = 5f;
    public float shorteningIntensity = 1f;
    public float shorteningInterval = 2f;
    public float trembleIntensity = 0.1f;
    public float ligtheningTimer = 3f;
    public float moveSpeed = 2f;
    private bool isAttacking = false;
    private bool enlightened = false;
    public GameObject player;
    private ShadowManager shadowManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        }
        else
        {
            ligtheningTimer = 3f;
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

        if (distanceFromPlayer > allowedDistanceFromPlayer)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
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
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Boat"))
        {
            AttackBoat();
        }
    }

    public void AttackBoat()
    {
        Debug.Log("Shadow Hand is attacking the boat!");
        isAttacking = true;
    }

    public void Death()
    {
        if (isAttacking)
        {
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
            transform.position += directionAwayFromPlayer * 2 * moveSpeed * Time.deltaTime;
        }
    }

    public bool setEnlightened(bool value)
    {
        enlightened = value;
        return enlightened;
    }

    public void Initialize(ShadowManager manager, GameObject playerReference)
    {
        shadowManager = manager;
        player = playerReference;
    }

    void OnDestroy()
    {
        if (shadowManager != null)
        {
            shadowManager.RemoveShadowHand(this);
        }
    }
}
