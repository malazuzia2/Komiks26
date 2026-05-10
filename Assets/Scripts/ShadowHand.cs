using UnityEngine;
using UnityEngine.UIElements;
using Unity.Cinemachine;
using System.Collections;

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
    //ZGRAJ DELAY Z ANIMACJ�
    public float attackDelay = 1f;
    public GameObject player;
    public GameObject camera;
    private ShadowManager shadowManager;
    private CinemachineImpulseSource impulseSource;
    private SnapAttackPoint occupiedPoint;


    private static bool hasShownFlashlightHint = false;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

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
            return; 
        }

        float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (!hasShownFlashlightHint && distanceFromPlayer < (allowedDistanceFromPlayer + 5f))
        {
            SimpleMessage.Instance.ShowMessage("Fuck, fuck, fuck... Where is my flashlight!");
            hasShownFlashlightHint = true;
        }

        if (distanceFromPlayer > shadowManager.shadowSpawnDistance + 2f)
        {
            Death();
            //DISOLVE R�KI
            return;
        }

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        Vector3 rotationDirection = Quaternion.LookRotation(directionToPlayer).eulerAngles;

        if (distanceFromPlayer > allowedDistanceFromPlayer)
        {
            transform.position += directionToPlayer * moveSpeed * Time.deltaTime;
            //ANIMACJA ZBLIZANIA SI� REKI
        }
        else if (distanceFromPlayer <= allowedDistanceFromPlayer)
        {
            shorteningInterval += Time.deltaTime;
            if (shorteningInterval >= 2f)
            {
                allowedDistanceFromPlayer -= shorteningIntensity;
                shorteningInterval = 0f; 
            }
        }
        
        transform.rotation = Quaternion.Euler(rotationDirection.x, rotationDirection.y, rotationDirection.z);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (isAttacking)
            return;

        isAttacking = true;
        //DODAJ ANIMACJ� ATAKU
        
        Collider selfCol = GetComponent<Collider>();
        if (selfCol != null)
            selfCol.isTrigger = true;

        StartCoroutine(AttackDelayCoroutine());
    }

    private IEnumerator AttackDelayCoroutine()
    {
        yield return new WaitForSeconds(attackDelay);

        if (this == null) yield break;

        AttackBoat();
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

    public void AttackBoat()
    {
        Debug.Log("Shadow Hand is attacking the boat!");
        player.GetComponent<BoatMovement>().isAttacked = true;

        SnapAttackPoint closestPoint = FindNearestAvailableSnapPoint();

        if (closestPoint != null)
        {
            occupiedPoint = closestPoint;
            occupiedPoint.isOccupied = true;

            transform.SetParent(occupiedPoint.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;


            Debug.Log("Shadow Hand snapped to point!");
        }

        CameraShake();
        Debug.Log("Boat is attacked! Movement and rotation are locked.");
        
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
        }
        //DODAJ DESOLVE
        Debug.Log("Shadow Hand has been defeated!");
        Destroy(gameObject);
    }

    public void lightAversion()
    {
        //DODAJ PARTICLE OPCJONALNIE
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
            //ANIMACJA RUCHU DO TY�U R�KI
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
