using UnityEngine;
using System.Collections;

public class gemThrown : MonoBehaviour
{
    private float sphereRadius = 1f;
    [SerializeField] private float GemInAirMultiplier = 3f;
    private bool isInAir = false;
    private bool canPickUpGem = false;

    private void Start()
    {
        StartCoroutine(EnablePickup());
    }

    private IEnumerator EnablePickup()
    {
        yield return new WaitForSeconds(1f);
        canPickUpGem = true;
    }

    void Update()
    {
        if (!canPickUpGem)
            return;

        isInAir = true;
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Ground"))
            {
                isInAir = false;
                break;
            }
        }

        float detectionRadius = isInAir ? sphereRadius * GemInAirMultiplier : sphereRadius;
        Collider[] detectionColliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in detectionColliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.hasGem = true;
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);

        if (isInAir)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, sphereRadius * GemInAirMultiplier);
        }
    }
}
