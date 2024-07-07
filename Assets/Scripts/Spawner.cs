using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Spawner Script:
 * Spawns different objects using object pooling.
 */

/* Attachables:
 *  1. Attach an ObjectPool script with proper initialization to the Spawner.
 */

public class Spawner : MonoBehaviour
{

    /* Spawner Pools */
    private ObjectPool yakshiPool;

    /* Spawning Lists */
    private List<Transform> yakshiSpawnPoints;

    /*(For Animators) Specific Instances*/
    private Animator yakshiAnimator;

    /* External References */
    [SerializeField] private GameObject yakshiSpawner;


    private void Awake()
    {
        yakshiSpawnPoints = new List<Transform>();
    }

    private void Start()
    {
        if (yakshiSpawner != null && yakshiSpawner.GetComponent<ObjectPool>())
        {
            yakshiPool = yakshiSpawner.GetComponent<ObjectPool>();
            foreach (Transform child in yakshiSpawner.transform)
            {
                yakshiSpawnPoints.Add(child);
            }
            if (yakshiSpawnPoints.Count > 0) StartCoroutine(SpawnYakshi());
            else Debug.LogError("No spawn points found under Yakshi Spawner.");
        }
        else Debug.LogError("Yakshi Spawner cannot be started. Ensure it has an ObjectPool component.");
    }

    /* Yakshi Spawner Coroutine */
    IEnumerator SpawnYakshi()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            if (yakshiSpawnPoints.Count == 0) yield break;

            int randomIndex = Random.Range(0, yakshiSpawnPoints.Count);
            Transform randomSpawnPoint = yakshiSpawnPoints[randomIndex];

            if (randomSpawnPoint == null) continue;

            GameObject yakshiInstance = yakshiPool.GetObject(randomSpawnPoint.position);
            if (yakshiInstance == null)
            {
                Debug.Log("Failed to get a Yakshi instance from the pool!");
                continue;
            }

            yield return new WaitForSeconds(5f);
            if (yakshiAnimator == null) yakshiAnimator = yakshiInstance.GetComponent<Animator>();
            if (yakshiAnimator != null)
            {
                yakshiAnimator.SetTrigger("fade");
                yield return new WaitForSeconds(1f);
            }

            yakshiPool.ReturnObject(yakshiInstance);

        }
    }
}
