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

    private Dictionary<string, List<GameObject>> poolDictionary;





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
        Debug.Log("init pool");
        SetSingleton();
        InitPool();
    }

    /// <summary>
    /// initialise la pool
    /// </summary>
    private void InitPool()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();

        foreach(Pool pool  in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Add(obj);
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

        List<GameObject> objFromTag = poolDictionary[tag];

        for (int i = 0; i < objFromTag.Count; i++)
        {
            if (objFromTag[i] && !objFromTag[i].activeSelf)
            {
                //ici on récupère un objet de la pool !

                objFromTag[i].SetActive(true);
                objFromTag[i].transform.position = position;
                objFromTag[i].transform.rotation = rotation;
                objFromTag[i].transform.SetParent(parent);

                IPooledObject pooledObject = objFromTag[i].GetComponent<IPooledObject>();

                if (pooledObject != null)
                {
                    pooledObject.OnObjectSpawn();
                }

                return (objFromTag[i]);
            }
        }

        Debug.Log("ici on a raté ! tout les objets de la pools sont complet !!");
        foreach (Pool pool in pools)
        {
            if (pool.tag == tag)
            {
                if (pool.shouldExpand)
                {
                    GameObject obj = Instantiate(pool.prefab, transform);
                    //obj.SetActive(false);
                    objFromTag.Add(obj);


                    obj.SetActive(true);
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;
                    obj.transform.SetParent(parent);

                    IPooledObject pooledObject = obj.GetComponent<IPooledObject>();

                    if (pooledObject != null)
                    {
                        pooledObject.OnObjectSpawn();
                    }

                    return (obj);


                }
                else
                {
                    Debug.LogError("pas d'expantion, error");

                    break;
                }
            }
        }


        return (null);
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
