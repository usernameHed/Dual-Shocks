using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// ObjectsPooler Description
/// </summary>
public class ObjectsPooler : ISingleton<ObjectsPooler>
{
    protected ObjectsPooler() { } // guarantee this will be always a singleton only - can't use the constructor!

    #region Attributes
    [System.Serializable]
    public class Pool
    {
        public GameData.Prefabs tag;
        public GameObject prefab;
        public int size;
        public bool shouldExpand = false;
    }

    [FoldoutGroup("GamePlay"), Tooltip("new pool"), SerializeField]
    private List<Pool> pools;

    private Dictionary<GameData.Prefabs, List<GameObject>> poolDictionary;

    #endregion

    #region Initialization

    private void Awake()
    {
        Debug.Log("init pool");
        InitPool();
    }

    /// <summary>
    /// initialise la pool
    /// </summary>
    private void InitPool()
    {
        poolDictionary = new Dictionary<GameData.Prefabs, List<GameObject>>();

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
    public GameObject SpawnFromPool(GameData.Prefabs tag, Vector3 position, Quaternion rotation, Transform parent)
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
    #endregion
}
