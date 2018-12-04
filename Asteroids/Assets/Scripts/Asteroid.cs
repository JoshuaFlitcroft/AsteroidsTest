using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages an Asteroid.
/// </summary>
public class Asteroid : MonoBehaviour
{
    /// <summary>
    /// Asteroid type.
    /// </summary>
    [Tooltip("Type of asteroid")]
    [SerializeField]
    private AsteroidManager.AsteroidTypes asteroidType;
    /// <summary>
    /// Child Asteroid type.
    /// </summary>
    [Tooltip("Type of asteroid to spawn when destroyed")]
    [SerializeField]
    private AsteroidManager.AsteroidTypes childAsteroidType;

    /// <summary>
    /// Starting health for the asteroid.
    /// </summary>
    [Tooltip("Amount of health the asteroid has")]
    [SerializeField]
    private int startHealth;
    /// <summary>
    /// Amount of damage to the player.
    /// </summary>
    [Tooltip("Amount of damage this will do to the player")]
    [SerializeField]
    private int damage;
    /// <summary>
    /// Amount of points to give when destroyed.
    /// </summary>
    [Tooltip("Number of points awarded when destroyed")]
    [SerializeField]
    private int points;
    /// <summary>
    /// Number of children to spawn
    /// </summary>
    [Tooltip("Number of children to spawn when destroyed - 0 for no children")]
    [SerializeField]
    private int splitCount;
    /// <summary>
    /// Amount to offset spawned children.
    /// </summary>
    [Tooltip("Amount to offset spawned children when destroyed")]
    [SerializeField]
    private float splitOffset;

    /// <summary>
    /// Cache of the transform component.
    /// </summary>
    private Transform transformComp;
    /// <summary>
    /// Cache of the Rigidbody2D component.
    /// </summary>
    private Rigidbody2D rigidbodyComp;

    /// <summary>
    /// Current health for the asteroid.
    /// </summary>
    private int currentHealth;


    /// <summary>
    ///  Used to cache Component or things to only do once.
    /// </summary>
    private void Awake()
    {
        transformComp = GetComponent<Transform>();
        rigidbodyComp = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Used to enable the pooled asteroid.
    /// </summary>
    /// <param name="position">Spawn position</param>
    /// <param name="rotation">Spawn rotation</param>
    /// <param name="startForce">Spawn force</param>
    public void Enable(Vector3 position, Vector3 rotation, Vector2 startForce)
    {
        transformComp.position = position;
        transformComp.eulerAngles = rotation;

        currentHealth = startHealth;

        gameObject.SetActive(true);

        rigidbodyComp.AddForce(startForce);
    }
    /// <summary>
    /// Used to disable the pooled asteroid.
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Update the asteroid.
    /// </summary>
    void Update ()
    {
	    if (ScreenManager.GetInstance().IsOffScreen(transformComp.position, 2.0f))
        {
            Disable();
        }
	}

    /// <summary>
    /// Get the current health of the Asteroid.
    /// </summary>
    /// <returns>Asteroid health</returns>
    public int GetHealth()
    {
        return currentHealth;
    }
    /// <summary>
    /// Get the amount of damage the Asteroid does.
    /// </summary>
    /// <returns>Asteroid damage</returns>
    public int GetDamage()
    {
        return damage;
    }
    /// <summary>
    /// Get the number of points for the Asteroid.
    /// </summary>
    /// <returns>Asteroid points</returns>
    public int GetPoints()
    {
        return points;
    }

    /// <summary>
    /// Split the Asteroid into child pieces.
    /// </summary>
    /// <param name="impactPoint">Point to split asteroid away from</param>
    /// <param name="explosionForce">Amount of force to apply to split children</param>
    public void SplitAsteroid(Vector3 impactPoint, float explosionForce)
    {
        Disable();

        if (splitCount == 0)
            return;

        //JF: Calculate direction to place new children.
        Vector3 direction = transformComp.position - impactPoint;
        direction.Normalize();
        direction *= splitOffset;

        //JF: Calculate angle for offseting children explosion direction.
        float rotateAmount = 360.0f / splitCount;
        //JF: Rotate by half the amount so the first doesn't move towards the bullet.
        direction = Quaternion.Euler(0, 0, rotateAmount / 2.0f) * direction;

        for (int i = 0; i < splitCount; ++i)
        {
            Asteroid childAsteroid = AsteroidManager.GetInstance().FindInactiveAsteroid(childAsteroidType);
            if (childAsteroid)
            {
                childAsteroid.Enable(transformComp.position + direction, new Vector3(0, 0, Random.Range(0, 360.0f)), direction * explosionForce);
                //JF: Rotate direction by amount for next child.
                direction = Quaternion.Euler(0, 0, rotateAmount) * direction;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Used to decrease the health of the asteroid. Returns true when destroyed.
    /// </summary>
    /// <param name="damage">Damage to apply to Asteroid</param>
    /// <param name="bulletPosition">Position of the bullet that hit it</param>
    /// <param name="explosionForce">Amount of force to use to explode children</param>
    /// <returns>true when split, false when still alive</returns>
    public bool Shot(int damage, Vector3 bulletPosition, float explosionForce)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            SplitAsteroid(bulletPosition, explosionForce);
            return true;
        }
        return false;
    }
}
