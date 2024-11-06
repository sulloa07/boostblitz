using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    // Arrays of different asteroid prefabs
    public GameObject[] smallAsteroids;
    public GameObject[] mediumAsteroids;
    public GameObject[] largeAsteroids;
    public GameObject[] hugeAsteroids;

    public Transform rocket;

    // Spawn configuration variables
    public float initialSpawnInterval = 0.7f; // Starting time between spawns
    public float spawnRangeX = 8f;            // Horizontal range for spawning
    public float asteroidSpeed = 0.1f;        // Speed of falling asteroids
    public float spawnOffsetY = 10f;          // Distance above the rocket to spawn asteroids
    public float currentSpawnInterval;        // Current spawn interval displayed in the Inspector

    // Internal state variables
    private bool hasStartedSpawning = false;          // Tracks if spawning has started
    private RocketController rocketController;        // Reference to the rocket's controller script

    void Start()
    {
        if (rocket != null)
        {
            // Get the RocketController component from the rocket
            rocketController = rocket.GetComponent<RocketController>();
        }

        // Initialize currentSpawnInterval to the initial spawn interval
        currentSpawnInterval = initialSpawnInterval;
    }

    void Update()
    {
        // Start spawning asteroids if the rocket has launched and spawning hasn't started yet
        if (rocketController != null && rocketController.isLaunched && !hasStartedSpawning)
        {
            StartSpawningAsteroids();
        }
    }

    // Begins the asteroid spawning process
    void StartSpawningAsteroids()
    {
        hasStartedSpawning = true;
        // Repeatedly call SpawnAsteroid at intervals of currentSpawnInterval
        InvokeRepeating("SpawnAsteroid", 0f, currentSpawnInterval);
        // Start the coroutine to adjust spawn rate over time
        StartCoroutine(AdjustSpawnRate());
    }

    // Coroutine to adjust the spawn rate based on the rocket's altitude
    IEnumerator AdjustSpawnRate()
    {
        while (hasStartedSpawning)
        {
            // Wait for 1 second before adjusting the spawn rate
            yield return new WaitForSeconds(1f);

            // Calculate altitude factor based on the rocket's Y position
            float altitudeFactor = rocket.position.y * 0.001f;
            // Decrease the spawn interval as altitude increases, clamped between 0.2f and initialSpawnInterval
            currentSpawnInterval = Mathf.Clamp(initialSpawnInterval - altitudeFactor, 0.2f, initialSpawnInterval);

            // Update the spawn interval by cancelling and restarting InvokeRepeating
            CancelInvoke("SpawnAsteroid");
            InvokeRepeating("SpawnAsteroid", 0f, currentSpawnInterval);
        }
    }

    // Spawns a single asteroid at a random position above the rocket
    void SpawnAsteroid()
    {
        // Generate a random X position within the spawn range
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        // Calculate the Y position based on the rocket's current position plus the spawn offset
        float spawnY = rocket.position.y + spawnOffsetY;
        // Define the spawn position
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0);

        // Select an asteroid prefab based on predefined probabilities
        GameObject asteroidPrefab = SelectAsteroidPrefab();
        // Instantiate the selected asteroid at the spawn position with no rotation
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);

        // Get the Rigidbody component of the spawned asteroid
        Rigidbody asteroidRb = asteroid.GetComponent<Rigidbody>();
        if (asteroidRb != null)
        {
            // Set the asteroid's velocity to make it fall downwards
            asteroidRb.velocity = new Vector3(0, -asteroidSpeed, 0);
        }

        // Assign the rocket reference to the AsteroidController script of the spawned asteroid
        AsteroidController asteroidController = asteroid.GetComponent<AsteroidController>();
        if (asteroidController != null)
        {
            asteroidController.rocket = rocket; // Ensure the rocket is assigned here
        }
        else
        {
            Debug.LogWarning("AsteroidController component missing on asteroid prefab.");
        }
    }

    // Selects an asteroid prefab based on random probabilities
    GameObject SelectAsteroidPrefab()
    {
        // Generate a random value between 0 and 1
        float randomValue = Random.Range(0f, 1f);

        if (randomValue < 0.25f)
        {
            // 25% chance to spawn a small asteroid
            return smallAsteroids[Random.Range(0, smallAsteroids.Length)];
        }
        else if (randomValue < 0.25)
        {
            // 25% chance to spawn a medium asteroid
            return mediumAsteroids[Random.Range(0, mediumAsteroids.Length)];
        }
        else if (randomValue < 0.25f)
        {
            // 25% chance to spawn a large asteroid
            return largeAsteroids[Random.Range(0, largeAsteroids.Length)];
        }
        else
        {
            // 15% chance to spawn a huge asteroid
            return hugeAsteroids[Random.Range(0, hugeAsteroids.Length)];
        }
    }
}
