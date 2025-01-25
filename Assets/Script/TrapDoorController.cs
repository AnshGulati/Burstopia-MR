using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoorController : MonoBehaviour
{
    public GameObject enemy;
    public float popSpeed = 2f;
    public GameObject trapdoor;

    void Start()
    {
        enemy.SetActive(false);
        // Set the initial position of the enemy to be slightly below its final position
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
        Vector3 targetPos = new Vector3(startPos.x, startPos.y + 1.2f, startPos.z);
        float time = 0f;

        while (time < 1f)
        {
            enemy.transform.position = Vector3.Lerp(startPos, targetPos, time);
            time += Time.deltaTime * popSpeed;
            yield return null;
        }

        enemy.transform.position = targetPos;
        // Delay deactivating the trapdoor
        yield return new WaitForSeconds(1.5f);
        trapdoor.SetActive(false);
    }

    public void OpenTrapDoor()
    {
        ActivateEnemy();
    }
}