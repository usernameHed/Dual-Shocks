using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// list d'objets poolé au démarage !
/// </summary>
[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int pooledAmount = 20;
    public bool shouldExpand = false;
}

/// <summary>
/// ObjectsPooler Description
/// </summary>
public class ObjectsPooler : MonoBehaviour
{
    #region Attributes
    private static ObjectsPooler instance;
    public static ObjectsPooler GetSingleton
    {
        get { return instance; }
    }


    public List<ObjectPoolItem> itemsToPool;

    private List<GameObject> pooledObjects; //list de TOUT les objets...

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
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.pooledAmount; i++)
            {
                GameObject obj = Instantiate(item.objectToPool, transform) as GameObject;
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    #endregion

    #region Core

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

    ////////////////////////////////////////////////////////////// Unity functions

    #endregion
}
