using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container class to allow us to view/edit items in the Inspector.
/// </summary>
[System.Serializable]
public class BulletPrefab : PoolPrefabData<BulletManager.BulletTypes>
{
}
/// <summary>
/// Container class to allow us to view/edit items in the Inspector.
/// </summary>
[System.Serializable]
public class BulletPool : PoolByType<Bullet, BulletManager.BulletTypes, BulletPrefab>
{
    public BulletPool()
    {
    }

    /// <summary>
    /// Disable all pooled bullets.
    /// </summary>
    public void DisableAll()
    {
        foreach (BulletManager.BulletTypes key in byType.Keys)
        {
            Bullet[] bullets = byType[key];
            for (int i = 0; i < bullets.Length; ++i)
            {
                bullets[i].Disable();
            }
        }
    }
}

/// <summary>
/// Manages Bullets and its Pool.
/// </summary>
public class BulletManager : MonoBehaviour
{
    /// <summary>
    /// Tag applied to Bullet GameObjects.
    /// </summary>
    public const string TAG = "Bullet";

    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static BulletManager instance;
    /// <summary>
    /// Singleton Accessor.
    /// </summary>
    /// <returns>Instance of class</returns>
    public static BulletManager GetInstance()
    {
        return instance;
    }

    /// <summary>
    /// Different types of bullets.
    /// </summary>
    public enum BulletTypes
    {
        Basic,
    }

    /// <summary>
    /// Stores Bullet Pool.
    /// </summary>
    [Tooltip("Bullet Pool - set up prefab and spawn amounts")]
    [SerializeField]
    public BulletPool bulletPool = new BulletPool();

    /// <summary>
    /// Called when GameObject is woken.
    /// </summary>
    private void Awake()
    {
        instance = this;

        bulletPool.CreatePool();
    }

    /// <summary>
    /// Called when game is restarted - reset.
    /// </summary>
    public void GameRestart()
    {
        bulletPool.DisableAll();
    }

    /// <summary>
    /// Find an inactive Bullet of a certain type.
    /// </summary>
    /// <param name="type">Bullet type.</param>
    /// <returns>Inactive Bullet - null when all active</returns>
    public Bullet FindInactiveBullet(BulletTypes type)
    {
        return bulletPool.FindInactive(type);
    }
}