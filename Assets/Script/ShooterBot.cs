using System.Collections;
using System.Collections.Generic;
using GorillaZilla;
using UnityEngine;
public class ShooterBot : MonoBehaviour
{
    public float lookSpeed = 1;
    //public Transform shootPoint;
    //public float fireRate;
    public float fireTimer;
    public float bubbleSpeed = 5f;
    public GameObject bubblePrefab;
    public LayerMask laserLayer;
    public bool canAttack = true;
    public Animator animator;

    // Health System
    public int health = 100;
    public int damage = 25;
    public Material characterMaterial;
    public Color flashColor = Color.red;
    public float flashDuration = 0.2f;
    private Color originalEmissionColor;
    public int maxHealth = 100;
    public HealthBar healthBar;

    public GameObject enemyPrefab;
    public bool damageOnCollision = true;
    public LayerMask collideLayer;
    public GameObject bubbleParticlePrefab;
    public ParticleSystem bubbleJetpack;

    // New tranforms
    public Transform shootPoint;  // Shooting point (for firing direction)
    public Transform pivot;  // Pivot point (for rotating the enemy body)
    public float initialDelay = 6f; // Delay before the first shot
    public float subsequentFireRate = 3f; // Rate after the first shot (3 seconds)

    private void Start()
    {
        // Store original emission color
        if (characterMaterial.HasProperty("_EmissionColor"))
        {
            originalEmissionColor = characterMaterial.GetColor("_EmissionColor");
        }

        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth); // Set initial health
    }

    private void Awake()
    {
        bubbleJetpack.Play();
        fireTimer = initialDelay;
    }
    void FixedUpdate()
    {
        LookAtPlayer();
        if(canAttack)
        {
                if (fireTimer >= initialDelay + subsequentFireRate)
                {
                    FireBullet();
                    fireTimer = 0;
                }
                fireTimer += Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!damageOnCollision) return;
        if (collideLayer.Contains(other.gameObject.layer))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        animator.ResetTrigger("HurtTrigger");
        animator.SetTrigger("HurtTrigger");
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
        bubbleJetpack.Stop();

        if (bubbleParticlePrefab != null)
        {
            var particle = Instantiate(bubbleParticlePrefab, transform.position, transform.rotation);
            particle.transform.localScale = Vector3.one * .05f;
        }

        Destroy(gameObject);
        healthBar.gameObject.SetActive(false);
    }

    void LookAtPlayer()
    {
        Transform playerHead = Camera.main.transform;
        Vector3 headPos = playerHead.position;

        // Calculate the direction to the player
        Vector3 directionToPlayer = headPos - transform.position;

        // Ensure we only rotate along the Y-axis (keeping the rotation flat)
        //directionToPlayer.y = 0; // Remove any vertical offset for proper rotation

        // If there's still some direction to the player, rotate towards the player

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            Quaternion curRotation = pivot.rotation;
            //turretHead.LookAt(playerHead);
            //Quaternion targetRotation = turretHead.rotation;
            pivot.rotation = Quaternion.Slerp(curRotation, targetRotation, Time.fixedDeltaTime * lookSpeed);

            // New Changes
            // Adjust shooting direction (shootPoint) towards the player
            Vector3 shootDirection = headPos - shootPoint.position;
            //shootDirection.y = 0; // Prevent any vertical offset
            shootPoint.rotation = Quaternion.LookRotation(shootDirection, Vector3.up); // Rotate shoot point to face the player
    }

    void FireBullet()
    {
        if (!canAttack) return;
        animator.ResetTrigger("ShootTrigger");
        animator.SetTrigger("ShootTrigger");

        Vector3 spawnPoint = shootPoint.position + shootPoint.forward * .1f;
        var bulletGO = Instantiate(bubblePrefab, spawnPoint, shootPoint.rotation, transform);
        bulletGO.GetComponent<Rigidbody>().AddForce(shootPoint.forward * bubbleSpeed);
        Destroy(bulletGO, 10f);
    }

    IEnumerator FlashDamageEffect()
    {
        if (characterMaterial.HasProperty("_EmissionColor"))
        {
            characterMaterial.SetColor("_EmissionColor", flashColor * 3f); // Increase intensity
        }
        yield return new WaitForSeconds(flashDuration);
        characterMaterial.SetColor("_EmissionColor", originalEmissionColor); // Reset to normal
    }
}
