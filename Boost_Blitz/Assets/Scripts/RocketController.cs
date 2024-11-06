using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RocketController : MonoBehaviour
{
    // Movement settings
    public float speed = 5f;                     // Horizontal movement speed
    public float launchSpeed = 5f;               // Initial vertical launch speed
    public float boostPower = 1f;                // Current boost power
    public float boostMultiplier = 2f;           // Multiplier applied during boost
    public float boostRegenRate = 0.1f;          // Rate at which boost regenerates
    public float tiltAmount = 15f;               // Amount of tilt when moving

    // Knockback settings
    public float knockbackForce = 5f;            // Force applied during knockback
    public float knockbackDuration = 0.2f;       // Duration of knockback effect

    // UI elements
    public TextMeshProUGUI countdownText;        // Countdown text UI
    public Slider boostSlider;                   // Boost power slider UI
    public Slider healthSlider;                  // Health slider UI
    public GameObject gameOverPanel;             // Game Over panel UI

    // Particle effects
    public ParticleSystem[] exhaustParticles;    // Array of exhaust particle systems
    public GameObject deathExplosion;            // Explosion effect on death
    public GameObject asteroidExplosion;         // Explosion effect for asteroids

    // Sound effects
    public AudioSource boostSound;               // Sound when boosting 
    public AudioSource launchSound;              // Sound when launching
    public AudioSource asteroidHit;              // Sound when hitting asteroid
    public AudioSource destroyAsteroidSound;     // Sound when destroying asteroid
    public AudioSource deathExplosionSound;      // Sound when destroying rocket
    public AudioSource healthRegenSound;         // Sound when hitting heart 

    
    // Health settings
    public int maxHealth = 100;                   // Maximum health
    private int currentHealth;                    // Current health

    // Other references
    public AltitudeCounter altitudeCounter;       // Reference to altitude counter

    // Speed settings
    public float speedIncreaseRate = 0.05f;      // Rate at which launch speed increases over time

    // Other variables
    private Rigidbody rb;                        // Rigidbody component
    private Coroutine bounceCoroutine;           // Reference to the bounce text coroutine
    private float targetTilt = 0f;               // Target tilt angle
    public bool isLaunched = false;              // Flag to check if rocket has launched
    private bool isBoosting = false;              // Flag to check if boost is active

    void Start()
    {
        // Get and configure the Rigidbody component
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Stop all exhaust particle systems initially
        foreach (var particle in exhaustParticles)
        {
            particle.Stop();
        }

        // Start the launch countdown coroutine
        StartCoroutine(LaunchCountdown());

        // Setup boost slider UI
        if (boostSlider != null)
        {
            boostSlider.maxValue = 1f;
            boostSlider.value = boostPower;
            boostSlider.gameObject.SetActive(false);
        }

        // Setup health slider UI
        if (healthSlider != null)
        {
            currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthSlider.gameObject.SetActive(false);
        }

        // Hide the Game Over panel initially
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Hide the death explosion effect initially
        if (deathExplosion != null)
        {
            deathExplosion.SetActive(false);
        }

        // Hide the asteroid explosion effect initially
        if (asteroidExplosion != null)
        {
            asteroidExplosion.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLaunched)
        {
            MoveRocket(); // Rocket go up
            ApplyTilt(); // Rocket sideways movement
            RegenerateBoost(); // Gradually regenerate boost
            IncreaseLaunchSpeedOverTime(); // Gradually increase launch speed
        }

        // Update boost slider UI if active
        if (boostSlider != null && boostSlider.gameObject.activeSelf)
        {
            boostSlider.value = boostPower;
        }

        // Update health slider UI if active
        if (healthSlider != null && healthSlider.gameObject.activeSelf)
        {
            healthSlider.value = currentHealth;
        }
    }

    // Handles collision events with other game objects
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            HandleAsteroidCollision(collision);
        }
        else if (collision.gameObject.CompareTag("HealthBooster"))
        {
            IncreaseHealth(20);
            healthRegenSound.Play();
            Destroy(collision.gameObject);
        }
    }

    // Moves the rocket based on player input
        void MoveRocket()
    {
        // Get horizontal input and apply movement
        float moveX = Input.GetAxis("Horizontal") * speed;
        rb.velocity = new Vector3(moveX, rb.velocity.y, 0);

        // Apply launch speed if launched
        if (isLaunched)
        {
            rb.velocity = new Vector3(rb.velocity.x, launchSpeed, rb.velocity.z);
        }

        // Check for boost activation
        if (Input.GetKey(KeyCode.W) && boostPower >= 1f && !isBoosting)
        {
            StartCoroutine(UseBoost());
        }

        // Bound the rocket within the camera frame
        BoundRocketToCamera();
    }

    void BoundRocketToCamera()
    {
        // Get the camera's visible bounds
        float halfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        
        // Get the current position of the rocket
        Vector3 position = transform.position;

        // Clamp the x position to keep it within the camera bounds
        position.x = Mathf.Clamp(position.x, -halfWidth, halfWidth);

        // Apply the clamped position back to the rocket
        transform.position = position;
    }


    // Applies a tilt to the rocket based on horizontal movement (this was a proud moment)
    void ApplyTilt()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        targetTilt = -horizontalInput * tiltAmount;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetTilt);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // Regenerates boost power over time when not boosting
    void RegenerateBoost()
    {
        if (!isBoosting && boostPower < 1f)
        {
            boostPower += boostRegenRate * Time.deltaTime;
            boostPower = Mathf.Clamp01(boostPower);
        }
    }

    // Gradually increases the launch speed over time
    void IncreaseLaunchSpeedOverTime()
    {
        launchSpeed += speedIncreaseRate * Time.deltaTime;
    }

    // Coroutine to handle the boost mechanics
    IEnumerator UseBoost()
    {
        isBoosting = true;
        float originalLaunchSpeed = launchSpeed;
        launchSpeed *= boostMultiplier;
        boostSound.Play();

        // Deplete boost power over time
        while (boostPower > 0)
        {
            boostPower -= Time.deltaTime * 0.15f;
            yield return null;
        }

        // Reset launch speed and boost state
        launchSpeed = originalLaunchSpeed;
        isBoosting = false;
    }

    // Coroutine to handle the launch countdown sequence
    IEnumerator LaunchCountdown()
    {
        countdownText.gameObject.SetActive(true);
        countdownText.text = "Press space bar to initiate launch!";

        // Start bouncing text animation
        bounceCoroutine = StartCoroutine(BounceText(countdownText));

        // Wait until space bar is pressed
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        // Stop bouncing text
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
        }
        countdownText.transform.localScale = Vector3.one;

        // Start countdown
        countdownText.text = "Launching in 3";
        exhaustParticles[0].Play();
        yield return new WaitForSeconds(1f);

        countdownText.text = "Launching in 2";
        exhaustParticles[1].Play();
        yield return new WaitForSeconds(1f);

        countdownText.text = "Launching in 1";
        exhaustParticles[2].Play();

        launchSound.Play();
        yield return new WaitForSeconds(1f);
        
        countdownText.text = "Launch!";
        yield return new WaitForSeconds(1f);

        // Hide countdown text and activate launch
        countdownText.gameObject.SetActive(false);
        isLaunched = true;

        // Show boost and health sliders
        if (boostSlider != null)
        {
            boostSlider.gameObject.SetActive(true);
        }

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
        }

        // Start altitude counter
        if (altitudeCounter != null)
        {
            altitudeCounter.StartLaunch();
        }
    }

    // Coroutine to create a bouncing text animation effect
    IEnumerator BounceText(TextMeshProUGUI text)
    {
        Vector3 initialPosition = text.transform.localPosition;
        Vector3 targetPosition = initialPosition + new Vector3(0, 20f, 0);

        while (true)
        {
            // Move text up
            for (float t = 0; t < 1; t += Time.deltaTime * 2)
            {
                text.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);
                yield return null;
            }

            // Move text back down
            for (float t = 0; t < 1; t += Time.deltaTime * 2)
            {
                text.transform.localPosition = Vector3.Lerp(targetPosition, initialPosition, t);
                yield return null;
            }
        }
    }

    // Handles collisions specifically with asteroids
    void HandleAsteroidCollision(Collision collision)
    {
        // Calculate knockback direction
        Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
        knockbackDirection.y = Mathf.Abs(knockbackDirection.y); // Ensure upward knockback
        StartCoroutine(ApplyKnockback(knockbackDirection));

        if (isBoosting)
        {
            // Destroy asteroid without taking damage
            Destroy(collision.gameObject);

            // Instantiate asteroid explosion effect
            if (asteroidExplosion != null)
            {
                Debug.Log("exploding Asteroid");
                GameObject explosionClone = Instantiate(asteroidExplosion, collision.transform.position, Quaternion.identity);
                explosionClone.SetActive(true); // Ensure the clone is active
                destroyAsteroidSound.Play();
                Destroy(explosionClone, 2f);   // Destroy clone after 2 seconds
            }
        }
        else
        {
            // Take damage from the asteroid
            AsteroidController asteroid = collision.gameObject.GetComponent<AsteroidController>();
            if (asteroid != null)
            {
                int damage = asteroid.GetAsteroidDamage();
                asteroidHit.Play();
                TakeDamage(damage);
            }
        }
    }

    // Coroutine to apply knockback force to the rocket
    IEnumerator ApplyKnockback(Vector3 direction)
    {
        float elapsed = 0f;
        Vector3 initialVelocity = rb.velocity;

        while (elapsed < knockbackDuration)
        {
            float knockbackStep = knockbackForce * (1 - (elapsed / knockbackDuration));
            rb.velocity = initialVelocity + direction * knockbackStep;

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.velocity = initialVelocity;
    }

    // Reduces the rocket's health by the specified damage amount
    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            ExplodeAndGameOver();
        }

        // Update health slider UI
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    // Increases the rocket's health by the specified amount, without exceeding max health
    void IncreaseHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // Update health slider UI
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    // Handles the rocket's explosion and triggers the Game Over panel
    void ExplodeAndGameOver()
    {
        if (deathExplosion != null)
        {
            Debug.Log("exploding Rocket");
            deathExplosion.SetActive(true);
            deathExplosionSound.Play();
            Instantiate(deathExplosion, transform.position, transform.rotation);
        }

        // Deactivate the rocket object
        gameObject.SetActive(false);

        // Show Game Over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}
