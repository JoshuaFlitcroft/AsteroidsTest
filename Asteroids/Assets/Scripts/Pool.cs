using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the Prefab/Pool data for setting up the pool - needs to be overidden to view in Inspector.
/// </summary>
/// <typeparam name="E">Identifier for Object</typeparam>
[System.Serializable]
public class PoolPrefabData<E>
{
    /// <summary>
    /// Stores the type of the object - identifier.
    /// </summary>
    [Tooltip("Type of object")]
    [SerializeField]
    private E type;
    /// <summary>
    /// Stores the object to pool.
    /// </summary>
    [Tooltip("Object to pool")]
    [SerializeField]
    private GameObject prefab;
    /// <summary>
    /// Stores how many objects to pool.
    /// </summary>
    [Tooltip("Amount to pool")]
    [SerializeField]
    private int poolAmount;

    /// <summary>
    /// Get the object to pool.
    /// </summary>
    /// <returns>Object to pool</returns>
    public GameObject GetPrefab()
    {
        return prefab;
    }
    /// <summary>
    /// Get the prefab type - identifier.
    /// </summary>
    /// <returns>Prefab type</returns>
    public E GetPrefabType()
    {
        return type;
    }
    /// <summary>
    /// Get the amount we want to pool.
    /// </summary>
    /// <returns>Amount to pool</returns>
    public int GetPoolAmount()
    {
        return poolAmount;
    }
}

/// <summary>
/// Stores the objects that have been pooled - needs to be overidden to view in Inspector.
/// </summary>
/// <typeparam name="T">Class/MonoBehaviour on the objects - what we store a reference to</typeparam>
/// <typeparam name="E">Identifier to use and seperate by</typeparam>
/// <typeparam name="P">Data of the objects to pool</typeparam>
[System.Serializable]
public class PoolByType<T, E, P> where T : MonoBehaviour where P : PoolPrefabData<E>
{
    /// <summary>
    /// Stores the data about the object we want to pool.
    /// </summary>
    [Tooltip("Info about the object to pool")]
    [SerializeField]
    protected P[] toPool;
    /// <summary>
    /// Parent for the pooled objects.
    /// </summary>
    [Tooltip("Parent for the pooled objects")]
    [SerializeField]
    protected Transform poolRoot;

    /// <summary>
    /// Stores the pooled objects by their type/identifier.
    /// </summary>
    protected Dictionary<E, T[]> byType = new Dictionary<E, T[]>();

    public PoolByType()
    {
    }

    /// <summary>
    /// Create the pool.
    /// </summary>
    public void CreatePool()
    {
        //JF: Store by list first so that we can easily add new ones.
        Dictionary<E, List<T>> byTypeList = new Dictionary<E, List<T>>();

        for (int i = 0; i < toPool.Length; ++i)
        {
            P prefabData = toPool[i];
            //JF: Create instances based on amount to pool.
            for (int j = 0; j < prefabData.GetPoolAmount(); ++j)
            {
                GameObject go = GameObject.Instantiate<GameObject>(prefabData.GetPrefab());
                go.SetActive(false); //JF: Make sure they start off.
                go.transform.parent = poolRoot;

                T component = go.GetComponent<T>(); //JF: Get the component to store them by
                List<T> typeList = null;
                byTypeList.TryGetValue(prefabData.GetPrefabType(), out typeList);
                //JF: List didn't exist, create a new one and add to Dictionary.
                if (typeList == null)
                {
                    typeList = new List<T>();
                    byTypeList.Add(prefabData.GetPrefabType(), typeList);
                }
                typeList.Add(component);
            }
        }

        //JF: Now convert to array as pool won't grow.
        foreach (E key in byTypeList.Keys)
        {
            List<T> list = byTypeList[key];
            T[] array = new T[list.Count];
            for (int i = 0; i < list.Count; ++i)
            {
                array[i] = list[i];
            }
            byType.Add(key, array);
        }
    }

    /// <summary>
    /// Find the first inactive object matching a certain type - can be null if non inactive.
    /// </summary>
    /// <param name="type">Type/Identifier to pick object by</param>
    /// <returns>Inactive object or null if non exist</returns>
    public T FindInactive(E type)
    {
        T[] objects = byType[type];
        int count = 0;
        while (count < 10000)
        {
            count += 1;
            int index = Random.Range(0, objects.Length);
            T obj = objects[index];
            if (!obj.isActiveAndEnabled)
            {
                return obj;
            }
        }

        return null;
    }
}