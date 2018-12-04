using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the Player ship.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Cache of the Rigidbody2D Component.
    /// </summary>
    private Rigidbody2D rigidbodyComp;
    /// <summary>
    /// Cache of the Transform Component.
    /// </summary>
    private Transform transformComp;
    /// <summary>
    /// Cache of the Sprite Renderer Component.
    /// </summary>
    private SpriteRenderer spriteRenderComp;
    /// <summary>
    /// Cache of the Color used on the Sprite Renderer material.
    /// </summary>
    private Color shipColor;

    /// <summary>
    /// Position to spawn the bullets.
    /// </summary>
    [Tooltip("Transform to spawn the bullets at")]
    [SerializeField]
    private Transform bulletSpawn;

    /// <summary>
    /// Amount of force to apply to ship when forward is pressed.
    /// </summary>
    [Tooltip("Force to apply to the ship to move forwards")]
    [SerializeField]
    private float forwardThrust = 5.0f;
    /// <summary>
    /// Amount of force to apply to the ship when backwards is pressed.
    /// </summary>
    [Tooltip("Force to apply to the ship to move backwards")]
    [SerializeField]
    private float backwardThrust = 2.0f;
    /// <summary>
    /// Maximum force(velocity) for the ship.
    /// </summary>
    [Tooltip("Maximum amount of force the ship can have")]
    [SerializeField]
    private float maxThrust = 4.0f;
    /// <summary>
    /// Cached squared version of maxThrust
    /// </summary>
    private float maxThrustSqrd;
    /// <summary>
    /// Speed to rotate the ship.
    /// </summary>
    [Tooltip("How fast to rotate the ship")]
    [SerializeField]
    private float rotationSpeed = 3.0f;

    /// <summary>
    /// Reference to the Transform of the UI element which shows the Health.
    /// </summary>
    [Tooltip("Transfor of UI element for the Health")]
    [SerializeField]
    private RectTransform healthBar;
    /// <summary>
    /// Starting Player health.
    /// </summary>
    [Tooltip("Player Health")]
    [SerializeField]
    private float startHealth = 100;
    /// <summary>
    /// Amount of time Player is invulnerable.
    /// </summary>
    [Tooltip("How long the Player is invulnerable for after taking damage")]
    [SerializeField]
    private float invulnerableLength = 2.0f;
    /// <summary>
    /// Current Player Health
    /// </summary>
    private float health;
    /// <summary>
    /// Invulnerable timer - also used to know when active/inactive.
    /// </summary>
    private float invulernableTimer;

    /// <summary>
    /// Stores the current bullet type for the Player - pickups would change this.
    /// </summary>
    private BulletManager.BulletTypes currentBullet = BulletManager.BulletTypes.Basic;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start ()
    {
        rigidbodyComp = GetComponent<Rigidbody2D>();
        transformComp = GetComponent<Transform>();
        spriteRenderComp = GetComponent<SpriteRenderer>();
        shipColor = spriteRenderComp.material.color;

        //JF: So that we don't have to keep calculating it or SquareRoot when we don't need to.
        maxThrustSqrd = maxThrust * maxThrust;

        GameRestart();
	}

    /// <summary>
    /// Called when game is restarted - reset.
    /// </summary>
    public void GameRestart()
    {
        // GameObject turned off when dead, make sure its on.
        gameObject.SetActive(true);
        transformComp.position = new Vector3(0, 0, 0);
        transformComp.eulerAngles = Vector3.zero;

        health = startHealth;
        UpdateHealthBar();

        invulernableTimer = 0;
    }
	
	/// <summary>
    /// Update the Player - Fixed because of force applying.
    /// </summary>
	void FixedUpdate()
    {
        //JF: Make sure the game is active.
        if (GameManager.GetInstance().GetGameState() != GameManager.GameStates.Playing)
        {
            return;
        }

        //JF: Check whether we're invulnerable.
        if (invulernableTimer > 0)
        {
            invulernableTimer -= Time.deltaTime;
            if (invulernableTimer < 0) //JF: No longer invulnerable.
            {
                shipColor.a = 1.0f;
            }
            else
            {
                //JF: Flash the ship when invulnerable
                shipColor.a = Mathf.Lerp(0.3f, 0.8f, Mathf.Sin(invulernableTimer * 20));
            }
            spriteRenderComp.material.color = shipColor;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontal) > 0)
        {
            transformComp.Rotate(Vector3.back * horizontal * rotationSpeed);
        }
        if (vertical > 0)
        {
            rigidbodyComp.AddForce(transformComp.up * vertical * forwardThrust);
        }
        else if(vertical < 0)
        {
            rigidbodyComp.AddForce(transformComp.up * vertical * backwardThrust);
        }

        //JF: Check whether we've gone to the other side of the screen (if not same position returned).
        transformComp.position = ScreenManager.GetInstance().TryTeleportToOtherSide(transformComp.position, 0);

        //JF: Check whether we've exceeded the maximum thrust allowed.
        if (rigidbodyComp.velocity.sqrMagnitude > maxThrustSqrd)
        {
            //JF: Set to max when exceeded.
            rigidbodyComp.velocity = rigidbodyComp.velocity.normalized * maxThrust;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Bullet bullet = BulletManager.GetInstance().FindInactiveBullet(currentBullet);
            if (bullet)
            {
                bullet.Enable(bulletSpawn.position, transformComp.eulerAngles);
            }
        }
    }

    /// <summary>
    /// Entered a collision with an object
    /// </summary>
    /// <param name="collision">Object we collided with</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionGO = collision.gameObject;
        //JF: Only checking for Asteroids at the moment.
        if (collisionGO.tag == AsteroidManager.TAG)
        {
            Asteroid asteroid = collisionGO.GetComponent<Asteroid>();
            TakeDamage(asteroid.GetDamage());
            //JF: We've been hit, split the Asteroid regardless of its health.
            asteroid.SplitAsteroid(transformComp.position, 400.0f);
        }
    }

    /// <summary>
    /// Update Health bar UI element.
    /// </summary>
    public void UpdateHealthBar()
    {
        Vector3 scale = healthBar.localScale;
        scale.x = Mathf.Clamp(health / startHealth, 0.0f, 1.0f);
        healthBar.localScale = scale;
    }
    /// <summary>
    /// Taken damage - decrease health and check for death.
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    public void TakeDamage(int damage)
    {
        //JF: Don't bother when invulnerable.
        if (invulernableTimer > 0)
        {
            return;
        }

        health -= damage;
        UpdateHealthBar();
        if (health <= 0) //JF: We're dead - Game Over.
        {
            gameObject.SetActive(false);
            GameManager.GetInstance().SetGameState(GameManager.GameStates.GameOver);
            return;
        }

        invulernableTimer = invulnerableLength;
    }
    /// <summary>
    /// Get the current Health for the Player.
    /// </summary>
    /// <returns>Current health</returns>
    public float GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Get the current Bullet used.
    /// </summary>
    /// <returns>Current bullet</returns>
    public BulletManager.BulletTypes GetCurrentBulletType()
    {
        return currentBullet;
    }
    /// <summary>
    /// Set the current Bullet used.
    /// </summary>
    /// <param name="type">New bullet type</param>
    public void SetCurrentBulletType(BulletManager.BulletTypes type)
    {
        currentBullet = type;
    }
}
