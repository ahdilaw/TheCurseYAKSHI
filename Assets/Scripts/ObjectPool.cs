using System.Collections.Generic;
using UnityEngine;

/* ObjectPooling Custom Class:
 * A simple implementation of the ObjectPool category to implement parking and reuse.
 */

public class ObjectPool : MonoBehaviour
{
    /* Pool */
    private List<GameObject> pooledObjects;
    private int poolCount;

    /*Data Strc.*/
    public enum ObjectDropdown
    {
        Yakshi
    }

    /* External Resources */
    [SerializeField] private ObjectDropdown dropdown;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int maxPoolSize = 20;

    /*Yakshi Serializations*/
    [SerializeField] private Player yakshi_Player;
    [SerializeField] private GameObject yakshi_objectPlayer;
    [SerializeField] private HUD yakshi_HUD;

    private void Awake()
    {
        pooledObjects = new List<GameObject>();
    }

    private void Start()
    {
        if (prefab != null)
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject pooledObject = Instantiate(prefab);
                pooledObject.SetActive(false);
                pooledObjects.Add(pooledObject);
                InitializeObject(pooledObject);
            }
            poolCount = initialPoolSize;
        }
        else Debug.LogError("The object pool prefab is missing.");
    }

    private void InitializeObject(GameObject pooledObject)
    {
        switch (dropdown)
        {
            case ObjectDropdown.Yakshi:
                if (yakshi_Player != null && yakshi_objectPlayer != null)
                pooledObject.GetComponent<Yakshi>().Initialize(yakshi_Player, yakshi_objectPlayer, yakshi_HUD);
                break;
        }
    }

    /* Accessible Pool Functions as Public Instances */
    public GameObject GetObject(Vector3 position)
    {
        GameObject obj = FetchPoolObject(position);
        if (obj == null)
        {
            obj = InstantiateAndPool(position);
        }
        return obj;
    }

    private GameObject InstantiateAndPool(Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogError("The Object Pool has no valid prefab.");
            return null;
        }
        if (pooledObjects.Count >= maxPoolSize)
        {
            Debug.LogError("The Object Pool has exceeded its maximum pool size.");
            return null;
        }
        GameObject pooledObject = Instantiate(prefab);
        pooledObject.SetActive(false);
        pooledObjects.Add(pooledObject);
        poolCount++; 
        return FetchPoolObject(position);
    }

    private GameObject FetchPoolObject(Vector3 position)
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(true);
                poolCount--; 
                return obj;
            }
        }
        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj.activeInHierarchy)
        {
            obj.SetActive(false);
            poolCount++; 
        }
        else
        {
            Debug.LogError("Deactivated objects cannot be accepted for return into the pool.");
        }
    }

    public void DestroyAllObjects()
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                Destroy(obj);
            }
        }
        pooledObjects.Clear();
        poolCount = 0;
    }
}
