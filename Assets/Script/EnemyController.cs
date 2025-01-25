using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Idle()
    {
        animator.SetBool("isShooting", false);
        animator.SetBool("isHurt", false);
    }

    public void Shoot()
    {
        animator.SetTrigger("shootTrigger");
    }

    public void Hurt()
    {
        animator.SetTrigger("hurtTrigger");
    }

    public void Die()
    {
        animator.SetBool("isDead", true);
    }
}
