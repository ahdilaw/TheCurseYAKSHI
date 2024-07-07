using System.Collections;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public ObjectPool yakshiPool;
    public ObjectPool ghostPool;
    public ObjectPool spiritPool;
    public ObjectPool ratPool;
    public ObjectPool batPool;
    public ObjectPool deadbodyPool;
    public ObjectPool keyPool;
    public ObjectPool livesPool;
    public ObjectPool batteriesPool;

    public int maxRatCount = 10;
    public int maxBatCount = 10;

    private GameObject[] yakshiSpawnPoints;
    private GameObject[] ghostSpawnPoints;
    private GameObject[] spiritSpawnPoints;
    private GameObject[] ratSpawnPoints;
    private GameObject[] batSpawnPoints;
    private GameObject[] deadbodySpawnPoints;
    private GameObject[] keySpawnPoints;
    private PolygonCollider2D spawnArea;

    void Start()
    {
        yakshiSpawnPoints = GameObject.FindGameObjectsWithTag("Yakshi_SP");
        ghostSpawnPoints = GameObject.FindGameObjectsWithTag("Ghost_SP");
        spiritSpawnPoints = GameObject.FindGameObjectsWithTag("Spirit_SP");
        ratSpawnPoints = GameObject.FindGameObjectsWithTag("Rat_SP");
        batSpawnPoints = GameObject.FindGameObjectsWithTag("Bat_SP");
        deadbodySpawnPoints = GameObject.FindGameObjectsWithTag("Deadbody_SP");
        keySpawnPoints = GameObject.FindGameObjectsWithTag("Key_SP");
        spawnArea = GetComponent<PolygonCollider2D>();

        CheckInitialization();

        StartCoroutine(SpawnYakshi());
        StartCoroutine(SpawnGhosts());
        StartCoroutine(SpawnSpirits());
        SpawnRats();
        SpawnBats();
        SpawnDeadbodies();
        SpawnKeys();
        SpawnLivesAndBatteries();
    }

    void CheckInitialization()
    {
        if (yakshiPool == null) Debug.Log("Yakshi Pool is not assigned!");
        if (ghostPool == null) Debug.Log("Ghost Pool is not assigned!");
        if (spiritPool == null) Debug.Log("Spirit Pool is not assigned!");
        if (ratPool == null) Debug.Log("Rat Pool is not assigned!");
        if (batPool == null) Debug.Log("Bat Pool is not assigned!");
        if (deadbodyPool == null) Debug.Log("Deadbody Pool is not assigned!");
        if (keyPool == null) Debug.Log("Key Pool is not assigned!");
        if (livesPool == null) Debug.Log("Lives Pool is not assigned!");
        if (batteriesPool == null) Debug.Log("Batteries Pool is not assigned!");

        if (yakshiSpawnPoints.Length == 0) Debug.Log("No Yakshi Spawn Points found!");
        if (ghostSpawnPoints.Length == 0) Debug.Log("No Ghost Spawn Points found!");
        if (spiritSpawnPoints.Length == 0) Debug.Log("No Spirit Spawn Points found!");
        if (ratSpawnPoints.Length == 0) Debug.Log("No Rat Spawn Points found!");
        if (batSpawnPoints.Length == 0) Debug.Log("No Bat Spawn Points found!");
        if (deadbodySpawnPoints.Length == 0) Debug.Log("No Deadbody Spawn Points found!");
        if (keySpawnPoints.Length == 0) Debug.Log("No Key Spawn Points found!");
        if (spawnArea == null) Debug.Log("PolygonCollider2D is not assigned or found!");
    }

    IEnumerator SpawnYakshi()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (yakshiSpawnPoints.Length == 0) continue;

            int randomIndex = Random.Range(0, yakshiSpawnPoints.Length);
            GameObject randomSpawnPoint = yakshiSpawnPoints[randomIndex];
            if (randomSpawnPoint == null) continue;

            GameObject yakshiInstance = yakshiPool.GetObject(randomSpawnPoint.transform.position);
            if (yakshiInstance == null) Debug.Log("Failed to get a Yakshi instance from the pool!");

            yield return new WaitForSeconds(3f);
            yakshiPool.ReturnObject(yakshiInstance);
        }
    }

    IEnumerator SpawnGhosts()
    {
        while (true)
        {
            foreach (GameObject spawnPoint in ghostSpawnPoints)
            {
                if (spawnPoint == null)
                {
                    Debug.Log("One of the ghostSpawnPoints is null!");
                    continue;
                }

                int numberOfGhosts = Random.Range(0, 4);
                for (int i = 0; i < numberOfGhosts; i++)
                {
                    GameObject ghostInstance = ghostPool.GetObject(spawnPoint.transform.position);
                    if (ghostInstance == null)
                    {
                        Debug.Log("Failed to get a ghost instance from the pool!");
                    }
                }
            }
            yield return new WaitForSeconds(10f);
        }
    }

    IEnumerator SpawnSpirits()
    {
        while (true)
        {
            foreach (GameObject spawnPoint in spiritSpawnPoints)
            {
                if (spawnPoint == null)
                {
                    Debug.Log("One of the spiritSpawnPoints is null!");
                    continue;
                }

                int numberOfSpirits = Random.Range(0, 3);
                for (int i = 0; i < numberOfSpirits; i++)
                {
                    GameObject spiritInstance = spiritPool.GetObject(spawnPoint.transform.position);
                    if (spiritInstance == null)
                    {
                        Debug.Log("Failed to get a spirit instance from the pool!");
                    }
                }
            }
            yield return new WaitForSeconds(10f);
        }
    }

    void SpawnRats()
    {
        foreach (GameObject spawnPoint in ratSpawnPoints)
        {
            if (spawnPoint == null)
            {
                Debug.Log("One of the ratSpawnPoints is null!");
                continue;
            }

            int numberOfRats = Random.Range(0, maxRatCount + 1);
            for (int i = 0; i < numberOfRats; i++)
            {
                GameObject ratInstance = ratPool.GetObject(spawnPoint.transform.position);
                if (ratInstance == null)
                {
                    Debug.Log("Failed to get a rat instance from the pool!");
                }
            }
        }
    }

    void SpawnBats()
    {
        foreach (GameObject spawnPoint in batSpawnPoints)
        {
            if (spawnPoint == null)
            {
                Debug.Log("One of the batSpawnPoints is null!");
                continue;
            }

            int numberOfBats = Random.Range(0, maxBatCount + 1);
            for (int i = 0; i < numberOfBats; i++)
            {
                GameObject batInstance = batPool.GetObject(spawnPoint.transform.position);
                if (batInstance == null)
                {
                    Debug.Log("Failed to get a bat instance from the pool!");
                }
            }
        }
    }

    void SpawnDeadbodies()
    {
        foreach (GameObject spawnPoint in deadbodySpawnPoints)
        {
            if (spawnPoint == null)
            {
                Debug.Log("One of the deadbodySpawnPoints is null!");
                continue;
            }

            GameObject deadbodyInstance = deadbodyPool.GetObject(spawnPoint.transform.position);
            if (deadbodyInstance == null)
            {
                Debug.Log("Failed to get a deadbody instance from the pool!");
            }
        }
    }

    void SpawnKeys()
    {
        foreach (GameObject spawnPoint in keySpawnPoints)
        {
            if (spawnPoint == null)
            {
                Debug.Log("One of the keySpawnPoints is null!");
                continue;
            }

            GameObject keyInstance = keyPool.GetObject(spawnPoint.transform.position);
            if (keyInstance == null)
            {
                Debug.Log("Failed to get a key instance from the pool!");
            }
        }
    }

    void SpawnLivesAndBatteries()
    {
        if (spawnArea == null) return;

        Bounds bounds = spawnArea.bounds;
        Vector2 min = bounds.min;
        Vector2 max = bounds.max;

        int numberOfLives = Random.Range(1, 6);
        for (int i = 0; i < numberOfLives; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
            if (spawnArea.OverlapPoint(randomPosition))
            {
                GameObject lifeInstance = livesPool.GetObject(randomPosition);
                if (lifeInstance == null)
                {
                    Debug.Log("Failed to get a life instance from the pool!");
                }
            }
        }

        int numberOfBatteries = Random.Range(1, 6);
        for (int i = 0; i < numberOfBatteries; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
            if (spawnArea.OverlapPoint(randomPosition))
            {
                GameObject batteryInstance = batteriesPool.GetObject(randomPosition);
                if (batteryInstance == null)
                {
                    Debug.Log("Failed to get a battery instance from the pool!");
                }
            }
        }
    }
}
