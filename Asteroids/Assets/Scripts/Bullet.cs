using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a bullet.
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>
    /// Bullet speed.
    /// </summary>
    [Tooltip("How fast the bullet will travel")]
    [SerializeField]
    private float bulletSpeed = 4;
    /// <summary>
    /// Bullet damage.
    /// </summary>
    [Tooltip("Amount of damage to do to an Asteroid")]
    [SerializeField]
    private int bulletDamage = 1;
    /// <summary>
    /// Force applied to Asteroid when destroyed by bullet.
    /// </summary>
    [Tooltip("Force to apply to the Asteroid when it gets exploded by a bullet")]
    [SerializeField]
    private float explosionForce = 50.0f;

    /// <summary>
    /// Cache of the Transform component.
    /// </summary>
    private Transform transformComp;

    /// <summary>
    ///  Used to cache Component or things to only do once.
    /// </summary>
    void Awake ()
    {
        transformComp = GetComponent<Transform>();
	}

    /// <summary>
    /// Used to enable the pooled bullet.
    /// </summary>
    /// <param name="position">Spawn position</param>
    /// <param name="direction">Spawn direction</param>
    public void Enable(Vector3 position, Vector3 direction)
    {
        transformComp.position = position;
        transformComp.eulerAngles = direction;

        gameObject.SetActive(true);
    }
    /// <summary>
    /// Used to disable the pooled bullet.
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Update the bullet.
    /// </summary>
    void Update()
    {
        transformComp.position += transformComp.up * bulletSpeed * Time.deltaTime;

        if (ScreenManager.GetInstance().IsOffScreen(transformComp.position, 2.0f))
        {
            Disable();
        }
	}

    /// <summary>
    /// Entered a collision with an object
    /// </summary>
    /// <param name="collider">Object we collided with</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //JF: Check we've collided with an Asteroid
        if (collider.tag == AsteroidManager.TAG)
        {
            Asteroid asteroid = collider.GetComponent<Asteroid>();
            //JF: Only increase the score when the asteroid is destroyed.
            if (asteroid.Shot(bulletDamage, transformComp.position, explosionForce))
            {
                GameManager.GetInstance().IncreaseScore(asteroid.GetPoints());
            }
            Disable();
        }
    }

    /// <summary>
    /// Get the Bullet damage.
    /// </summary>
    /// <returns></returns>
    public int GetBulletDamage()
    {
        return bulletDamage;
    }
}
