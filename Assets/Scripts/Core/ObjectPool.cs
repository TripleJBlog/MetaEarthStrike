using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaEarthStrike.Core
{
    [System.Serializable]
    public class PooledObject
    {
        public GameObject prefab;
        public int poolSize = 20;
        public List<GameObject> pooledObjects = new List<GameObject>();
    }

    public class ObjectPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        public PooledObject[] pooledObjects;
        
        private Dictionary<GameObject, List<GameObject>> objectPools = new Dictionary<GameObject, List<GameObject>>();
        private Dictionary<GameObject, GameObject> activeObjects = new Dictionary<GameObject, GameObject>();
        
        private void Awake()
        {
            InitializePools();
        }
        
        private void InitializePools()
        {
            // Initialize predefined pools
            foreach (PooledObject pooledObject in pooledObjects)
            {
                if (pooledObject.prefab != null)
                {
                    CreatePool(pooledObject.prefab, pooledObject.poolSize);
                }
            }
        }
        
        public void CreatePool(GameObject prefab, int poolSize)
        {
            if (objectPools.ContainsKey(prefab))
                return;
                
            List<GameObject> pool = new List<GameObject>();
            
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                pool.Add(obj);
            }
            
            objectPools[prefab] = pool;
        }
        
        public GameObject GetObject(GameObject prefab)
        {
            if (!objectPools.ContainsKey(prefab))
            {
                CreatePool(prefab, 10); // Default pool size
            }
            
            List<GameObject> pool = objectPools[prefab];
            
            // Find inactive object in pool
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeInHierarchy)
                {
                    GameObject obj = pool[i];
                    obj.SetActive(true);
                    activeObjects[obj] = prefab;
                    return obj;
                }
            }
            
            // If no inactive objects, create a new one
            GameObject newObj = Instantiate(prefab);
            pool.Add(newObj);
            activeObjects[newObj] = prefab;
            return newObj;
        }
        
        public void ReturnObject(GameObject obj)
        {
            if (activeObjects.ContainsKey(obj))
            {
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                activeObjects.Remove(obj);
            }
        }
        
        public void ReturnAllObjects()
        {
            foreach (GameObject obj in activeObjects.Keys)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                }
            }
            activeObjects.Clear();
        }
        
        public void ExpandPool(GameObject prefab, int additionalSize)
        {
            if (!objectPools.ContainsKey(prefab))
            {
                CreatePool(prefab, additionalSize);
                return;
            }
            
            List<GameObject> pool = objectPools[prefab];
            
            for (int i = 0; i < additionalSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                pool.Add(obj);
            }
        }
        
        public int GetActiveObjectCount(GameObject prefab)
        {
            int count = 0;
            foreach (GameObject obj in activeObjects.Keys)
            {
                if (activeObjects[obj] == prefab && obj.activeInHierarchy)
                {
                    count++;
                }
            }
            return count;
        }
        
        public int GetPoolSize(GameObject prefab)
        {
            if (objectPools.ContainsKey(prefab))
            {
                return objectPools[prefab].Count;
            }
            return 0;
        }
        
        private void OnDestroy()
        {
            // Clean up all pooled objects
            foreach (var pool in objectPools.Values)
            {
                foreach (GameObject obj in pool)
                {
                    if (obj != null)
                    {
                        DestroyImmediate(obj);
                    }
                }
            }
            objectPools.Clear();
            activeObjects.Clear();
        }
    }
} 