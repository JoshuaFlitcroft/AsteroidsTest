using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container class to allow us to view/edit items in the Inspector.
/// </summary>
[System.Serializable]
public class AsteroidPrefab : PoolPrefabData<AsteroidManager.AsteroidTypes>
{
    public AsteroidPrefab()
    {
    }
}
/// <summary>
/// Container class to allow us to view/edit items in the Inspector.
/// </summary>
[System.Serializable]
public class AsteroidPool : PoolByType<Asteroid, AsteroidManager.AsteroidTypes, AsteroidPrefab>
{
    public AsteroidPool()
    {
    }

    /// <summary>
    /// Disable all pooled Asteroids.
    /// </summary>
    public void DisableAll()
    {
        foreach (AsteroidManager.AsteroidTypes key in byType.Keys)
        {
            Asteroid[] asteroids = byType[key];
            for (int i = 0; i < asteroids.Length; ++i)
            {
                asteroids[i].Disable();
            }
        }
    }
}

/// <summary>
/// Manages Asteroids and its Pool.
/// </summary>
public class AsteroidManager : MonoBehaviour
{
    /// <summary>
    /// Tag applied to Asteroid GameObjects.
    /// </summary>
    public const string TAG = "Asteroid";

    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static AsteroidManager instance;
    /// <summary>
    /// Singleton Accessor.
    /// </summary>
    /// <returns>Instance of class</returns>
    public static AsteroidManager GetInstance()
    {
        return instance;
    }

    /// <summary>
    /// Different types of Asteroids.
    /// </summary>
    public enum AsteroidTypes
    {
        Large1,
        Large2,
        Medium1,
        Small1,
    }

    /// <summary>
    /// Stores the Asteroids which can be spawned into the scene.
    /// </summary>
    [Tooltip("Which Asteroids we want to spawn")]
    [SerializeField]
    private AsteroidTypes[] spawnableAsteroids;
    /// <summary>
    /// Initial velocity for spawned Asteroids.
    /// </summary>
    [Tooltip("Initial velocity to give spawned Asteroids")]
    [SerializeField]
    private Vector2 spawnSpeed = new Vector2(80.0f, 90.0f);

    /// <summary>
    /// Stores Asteroid Pool.
    /// </summary>
    [Tooltip("Asteroid Pool - set up prefab and spawn amounts")]
    [SerializeField]
    public AsteroidPool asteroidPool = new AsteroidPool();

    /// <summary>
    /// Length of time between Asteroids spawn.
    /// </summary>
    [Tooltip("Length of time between spawning Asteroids")]
    [SerializeField]
    private float spawnLimit = 4.0f;
    /// <summary>
    /// Amount to reduce spawnLimit by after spawning.
    /// </summary>
    [Tooltip("Amount to reduce spawn time by")]
    [SerializeField]
    private float spawnLimitModifier = 0.99f;
    /// <summary>
    /// Minimum spawn time - spawnLimit can't be below this
    /// </summary>
    [Tooltip("Minimum spawn time")]
    [SerializeField]
    private float minimumSpawnLimit = 0.5f;
    /// <summary>
    /// Stores the timer for spawning.
    /// </summary>
    private float spawnTimer;

    /// <summary>
    /// Called when GameObject is woken.
    /// </summary>
    private void Awake()
    {
        instance = this;

        asteroidPool.CreatePool();
    }

    /// <summary>
    /// Called when game is restarted - reset.
    /// </summary>
    public void GameRestart()
    {
        asteroidPool.DisableAll();
    }
	
	/// <summary>
    /// Update the manager.
    /// </summary>
	void Update ()
    {
        //JF: Make sure the game state id valid.
        if (GameManager.GetInstance().GetGameState() != GameManager.GameStates.Playing)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnLimit)
        {
            spawnTimer = 0f;
            spawnLimit = Mathf.Max(spawnLimit * spawnLimitModifier, minimumSpawnLimit);

            //JF: Pick a random type from spawnable - can be waited.
            Asteroid asteroid = FindInactiveAsteroid(spawnableAsteroids[Random.Range(0, spawnableAsteroids.Length)]);
            if (asteroid)
            {
                Vector3 position = new Vector3(0, 0, 0);
                //JF: Choose the side of the screen to spawn on.
                int side = Random.Range(0, 4);
                switch (side)
                {
                    case 0: // Left side
                        position.x = ScreenManager.GetInstance().LeftEdge();
                        position.y = Random.Range(ScreenManager.GetInstance().BottomEdge(), ScreenManager.GetInstance().TopEdge());
                        break;
                    case 1: // Right side
                        position.x = ScreenManager.GetInstance().RightEdge();
                        position.y = Random.Range(ScreenManager.GetInstance().BottomEdge(), ScreenManager.GetInstance().TopEdge());
                        break;
                    case 2: // Top side
                        position.x = Random.Range(ScreenManager.GetInstance().LeftEdge(), ScreenManager.GetInstance().RightEdge());
                        position.y = ScreenManager.GetInstance().TopEdge();
                        break;
                    case 3: // Bottom side
                        position.x = Random.Range(ScreenManager.GetInstance().LeftEdge(), ScreenManager.GetInstance().RightEdge());
                        position.y = ScreenManager.GetInstance().BottomEdge();
                        break;
                }

                Transform playerTrans = GameManager.GetInstance().GetPlayer().GetComponent<Transform>();
                //JF: Set initial force to fire towards the player.
                asteroid.Enable(position, new Vector3(0, 0, Random.Range(0, 360.0f)), new Vector2(playerTrans.position.x - position.x, playerTrans.position.y - position.y) * Random.Range(spawnSpeed.x, spawnSpeed.y));
            }
        }
	}

    /// <summary>
    /// Find an inactive Asteroid of a certain type.
    /// </summary>
    /// <param name="type">Asteroid type.</param>
    /// <returns>Inactive Asteroid - null when all active</returns>
    public Asteroid FindInactiveAsteroid(AsteroidTypes type)
    {
        return asteroidPool.FindInactive(type);
    }
}