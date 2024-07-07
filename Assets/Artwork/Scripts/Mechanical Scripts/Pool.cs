using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPo2ol : MonoBehaviour
{   
    //have made these public for getting them across scripts
    public GameObject prefab;
    public int initialSize = 10;
    private List<GameObject> pool;

    private void Start()
    {
        
    }

    void Awake()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetObject(Vector3 position)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(true);
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab, position, Quaternion.identity);
        newObj.SetActive(true);
        pool.Add(newObj);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
