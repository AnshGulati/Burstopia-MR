using GorillaZilla;
using System.Collections;
using UnityEngine;

public class ShooterBot : MonoBehaviour
{
    public float lookSpeed = 3f;
    public float minFireRate = 2f;
    public float maxFireRate = 4f;
    public float bubbleSpeed = 2f;
    public GameObject bubblePrefab;
    private bool canAttack = false;
    public Animator animator;

    // Health System
    public int health = 100;
    public int damage = 10;
    public Material characterMaterial;
    public Color flashColor = Color.red;
    public float flashDuration = 0.2f;
    private Color originalEmissionColor;
    public int maxHealth = 100;
    public HealthBar healthBar;
    

    public GameObject bubbleParticlePrefab;
    public ParticleSystem bubbleJetpack;

    // Transforms
    public Transform shootPoint;
    public Transform pivot;
    private float nextFireTime;
    private Transform player;
    public GameObject trapdoor;

    public AudioSource shoot;
    public AudioSource hurt;
    public AudioSource die;
    public AudioSource jetpackSound;

    // Collision
    public GameObject enemyPrefab;
    private bool destroyOnCollision = true;
    public LayerMask collideLayer;

    private void Start()
    {
        if (characterMaterial.HasProperty("_EmissionColor"))
        {
            originalEmissionColor = characterMaterial.GetColor("_EmissionColor");
        }

        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        player = Camera.main.transform; // Assign player transform
        Invoke(nameof(EnableShooting), 6f);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Hand")) // Check if MR hand touches the enemy
    //    {
    //        Destroy(enemyPrefab); // Destroy the enemy
    //    }
    //}

    private void EnableShooting()
    {
        canAttack = true;
        nextFireTime = Time.time + Random.Range(minFireRate, maxFireRate);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!destroyOnCollision) return;
        if (collideLayer.Contains(other.gameObject.layer))
        {
            TakeDamage();
            //Die();
            //Destroy(gameObject);
            //die.Play();
        }
    }

    private void Awake()
    {
        bubbleJetpack.Play();
        jetpackSound.Play();
        trapdoor.SetActive(false);
    }

    void FixedUpdate()
    {
        LookAtPlayer();

        if (canAttack && Time.time >= nextFireTime)
        {
            FireBullet(); 
            nextFireTime = Time.time + Random.Range(minFireRate, maxFireRate); // Randomize next fire interval
        }
    }

    private void TakeDamage()
    {
        animator.ResetTrigger("HurtTrigger");
        animator.SetTrigger("HurtTrigger");
        hurt.Play();
        health -= damage;
        healthBar.SetHealth(health);
        StartCoroutine(FlashDamageEffect());

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.ResetTrigger("DeathTrigger");
        animator.SetTrigger("DeathTrigger");
        die.Play();
        bubbleJetpack.Stop();
        jetpackSound.Stop();

        if (bubbleParticlePrefab != null)
        {
            var particle = Instantiate(bubbleParticlePrefab, transform.position, Quaternion.identity);
            particle.transform.localScale = Vector3.one * 0.05f;
        }

        Destroy(gameObject);
        healthBar.gameObject.SetActive(false);
    }

    void LookAtPlayer()
    {
        if (!player) return;

        Vector3 directionToPlayer = player.position - pivot.position;
        directionToPlayer.y = 0; // Only rotate on Y-axis

        if (directionToPlayer.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            pivot.rotation = Quaternion.Slerp(pivot.rotation, targetRotation, Time.deltaTime * lookSpeed);
        }

        // Adjust shoot direction
        shootPoint.forward = (player.position - shootPoint.position).normalized;
    }

    void FireBullet()
    {
        if (!canAttack) return;

        animator.ResetTrigger("ShootTrigger");
        animator.SetTrigger("ShootTrigger");
        shoot.Play();

        Vector3 spawnPoint = shootPoint.position + shootPoint.forward * 0.1f;
        GameObject bulletGO = Instantiate(bubblePrefab, spawnPoint, shootPoint.rotation);

        Rigidbody bulletRB = bulletGO.GetComponent<Rigidbody>();
        if (bulletRB)
        {
            bulletRB.velocity = shootPoint.forward * bubbleSpeed;
        }

        //bulletGO.GetComponent<Rigidbody>().AddForce(shootPoint.forward * bubbleSpeed);

        Destroy(bulletGO, 10f);
    }

    IEnumerator FlashDamageEffect()
    {
        if (characterMaterial.HasProperty("_EmissionColor"))
        {
            characterMaterial.SetColor("_EmissionColor", flashColor * 3f);
        }
        yield return new WaitForSeconds(flashDuration);
        characterMaterial.SetColor("_EmissionColor", originalEmissionColor);
    }
}
