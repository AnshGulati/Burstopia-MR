using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEnemy : MonoBehaviour
{
    public LayerMask playerLayer;
    Transform player;
    public float movementSpeed;
    public float rotatationSpeed;
    public float upForce = .1f;
    public Animator animator;
    public AudioSource audioSource;

    void Start()
    {
        player = Camera.main.transform;
        transform.position += Vector3.up * .5f;
        GetComponent<DestroyOnCollision>().destroyOnCollision = false;
    }

    
    void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody>();

        Vector3 forward = player.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(forward.normalized, Vector3.up);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotatationSpeed * Time.fixedDeltaTime));

        Vector3 targetPoint = player.TransformPoint(Vector3.forward * .5f);
        rb.MovePosition(Vector3.Lerp(rb.position, targetPoint, movementSpeed * Time.fixedDeltaTime));
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.rigidbody != null && playerLayer.Contains(other.rigidbody.gameObject.layer))
        {
            audioSource.Stop();
            animator.enabled = false;
            GetComponent<DestroyOnCollision>().destroyOnCollision = true;
        }
    }
}
