using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class TrapDoorController : MonoBehaviour
{
    public GameObject enemy;
    public float popSpeed = 2f;
    private Animator doorAnimator;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        enemy.SetActive(false);
        // Set the initial position of the enemy to be a bit below its final position
        enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y - 1f, enemy.transform.position.z);
    }

    public void ActivateEnemy()
    {
        enemy.SetActive(true);
        StartCoroutine(PopEnemyIn());
    }

    private IEnumerator PopEnemyIn()
    {
        Vector3 startPos = enemy.transform.position;
        Vector3 targetPos = new Vector3(startPos.x , startPos.y + 01.2f, startPos.z); 
        float time = 0f;

        while (time < 1f)
        {
            // Interpolate position from start to target
            enemy.transform.position = Vector3.Lerp(startPos, targetPos, time);
            time += Time.deltaTime * popSpeed;
            yield return null;
        }

        // Ensure the final position is exactly the target position
        enemy.transform.position = targetPos;
    }

    public void OpenTrapDoor()
    {
        doorAnimator.SetTrigger("Open");
        ActivateEnemy();
    }
}
