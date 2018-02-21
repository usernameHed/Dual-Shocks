using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// list d'objets poolé au démarage !
/// </summary>
/*[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int pooledAmount = 20;
    public bool shouldExpand = false;
}*/

/// <summary>
/// ObjectsPooler Description
/// </summary>
public class ObjectsPooler : MonoBehaviour
{
    #region Attributes
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool shouldExpand = false;
    }

    [FoldoutGroup("GamePlay"), Tooltip("new pool"), SerializeField]
    private List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDictionary;





    private static ObjectsPooler instance;
    public static ObjectsPooler GetSingleton
    {
        get { return instance; }
    }

    
    /*

    public List<ObjectPoolItem> itemsToPool;

    private List<GameObject> pooledObjects; //list de TOUT les objets...
    */
    #endregion

    #region Initialization

    /// <summary>
    /// Init 
    /// </summary>
    public void SetSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Awake()
    {
        SetSingleton();
        InitPool();
    }

    /// <summary>
    /// initialise la pool
    /// </summary>
    private void InitPool()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool  in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    #endregion

    #region Core
    /// <summary>
    /// access object from pool
    /// </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("pool with tag: " + tag + "doesn't exist");
            return (null);
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        if (objectToSpawn.activeSelf)
        {
            Debug.Log("on a dépassé la limite ! que faire ?");
            //poolDictionary[tag].Enqueue(objectToSpawn);

            foreach (Pool pool in pools)
            {
                if (pool.tag == tag)
                {
                    if (pool.shouldExpand)
                    {
                        poolDictionary[tag].Enqueue(objectToSpawn);
                        objectToSpawn = Instantiate(pool.prefab, position, rotation, parent);
                    }
                    else
                    {
                        Debug.Log("pas d'expantion, on réutilise un objet déja actif !!");
                        break;
                    }
                }
            }
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.SetParent(parent);

        IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObject != null)
        {
            pooledObject.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return (objectToSpawn);
    }

    /*
    /// <summary>
    /// on a un objet actif... on fait quoi ? on en créé un autre ?
    /// </summary>
    /// <param name="tag"></param>
    private bool ErrorExpand(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("pool with tag: " + tag + "doesn't exist");
            return (false);
        }
        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                if (pool.shouldExpand)
                {
                    //ici créé un objet dans la queue !
                    Queue<GameObject> objectPool = poolDictionary[tag];
                    GameObject obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                    poolDictionary[tag] = objectPool;
                    return (true);
                }
                else
                    return (false);
            }
        }

        return (true);
    }
    */

    /*
    /// <summary>
    /// retourn un objet actuellement désactivé
    /// </summary>
    /// <returns></returns>
    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
            {
                pooledObjects[i].SetActive(true);
                return pooledObjects[i];
            }
        }


        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                if (item.shouldExpand)
                {
                    GameObject obj = Instantiate(item.objectToPool, transform) as GameObject;
                    if (!obj.activeSelf)
                        obj.SetActive(true);
                    pooledObjects.Add(obj);
                    return (obj);
                }
            }
        }
        return (null);
    }
    */
    ////////////////////////////////////////////////////////////// Unity functions

    #endregion
}
