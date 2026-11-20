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

    // Update is called once per frame
    void Update()
    {
        isInAir = false;
        //collider for the gem tedecting if it hit a player
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player") && canPickUpGem)
            {
                PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.hasGem = true;
                    Destroy(gameObject);
                    return;
                }
            }
            //if the gem is in the air increase the sphere radius to detect players that are further away
            else if (!collider.CompareTag("Ground"))
            {
                isInAir = true;
                Collider[] groundColliders = Physics.OverlapSphere(transform.position, sphereRadius * GemInAirMultiplier);

                foreach (Collider groundCollider in groundColliders)
                {
                    if (groundCollider.CompareTag("Player") && canPickUpGem)
                    {
                        PlayerMovement playerMovement = groundCollider.GetComponent<PlayerMovement>();
                        if (playerMovement != null)
                        {
                            playerMovement.hasGem = true;
                            Destroy(gameObject);
                            return;
                        }
                    }
                }
            }
            else if (collider.CompareTag("Ground"))
            {
                isInAir = false;
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
}
