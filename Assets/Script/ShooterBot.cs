using UnityEngine;

public class ShooterBot : MonoBehaviour
{
    public Animator animator;
    public GameObject bubblePrefab;
    public Transform shootPoint; 
    public float bubbleSpeed = 5f;
    public float shootingInterval = 2f;

    public GameObject enemyPrefab;

    private bool canShoot = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) // Shooting
        {
            ShootBubble();
        }

        if (Input.GetKeyDown(KeyCode.S)) // Hurt
        {
            PlayHurtAnimation();
        }

        if (Input.GetKeyDown(KeyCode.W)) // Death
        {
            PlayDeathAnimation();
        }
    }

    void ShootBubble()
    {
    if (!canShoot) return;

    animator.ResetTrigger("ShootTrigger");
    animator.SetTrigger("ShootTrigger");

    GameObject bubble = Instantiate(bubblePrefab, transform.position, transform.rotation);

    bubble.transform.SetParent(transform);

    Rigidbody rb = bubble.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.velocity = transform.forward * bubbleSpeed;
    }

    Destroy(bubble, 6f); 

    canShoot = false;
    Invoke(nameof(ResetShooting), shootingInterval);
    }


    void ResetShooting()
    {
        canShoot = true;
    }

    void PlayHurtAnimation()
    {
        animator.ResetTrigger("HurtTrigger");
        animator.SetTrigger("HurtTrigger");
    }

    void PlayDeathAnimation()
    {
        animator.ResetTrigger("DeathTrigger");
        animator.SetTrigger("DeathTrigger");
        enemyPrefab.SetActive(false);
    }
}
