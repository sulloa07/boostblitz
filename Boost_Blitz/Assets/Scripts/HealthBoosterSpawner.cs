using System.Collections;
using UnityEngine;

public class HealthBoosterSpawner : MonoBehaviour
{
    public GameObject healthBoosterPrefab;       // Prefab of the HealthBooster
    public Transform rocket;                     // Reference to the rocket's Transform
    public float minSpawnInterval = 5f;         // Minimum time between spawns
    public float maxSpawnInterval = 20f;         // Maximum time between spawns
    public float spawnRangeX = 8f;               // Horizontal range for spawning
    public float spawnOffsetY = 12f;             // Distance above the rocket to spawn HealthBoosters
    public float initialSpawnDelay = 10f;        // Delay before the first spawn

    private bool hasStartedSpawning = false;     // Tracks if spawning has started
    private RocketController rocketController;   // Reference to the rocket's controller script

    void Start()
    {
        // Check if the rocket reference is set
        if (rocket != null)
        {
            rocketController = rocket.GetComponent<RocketController>();
        }
        else
        {
            Debug.LogError("Rocket reference is missing in HealthBoosterSpawner.");
        }
    }

    void Update()
    {
        // Start spawning HealthBoosters if the rocket has launched and spawning hasn't started yet
        if (rocketController != null && rocketController.isLaunched && !hasStartedSpawning)
        {
            StartCoroutine(SpawnHealthBoosters());
            hasStartedSpawning = true;
        }
    }

    // Coroutine to handle random spawn intervals for HealthBoosters
    IEnumerator SpawnHealthBoosters()
    {
        // Initial delay before starting the spawn loop
        yield return new WaitForSeconds(initialSpawnDelay);

        while (true)
        {
            SpawnHealthBooster();

            // Wait for a random interval before spawning the next HealthBooster
            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    // Spawns a single HealthBooster at a random position above the rocket
    void SpawnHealthBooster()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        float spawnY = rocket.position.y + spawnOffsetY;
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0);

        Instantiate(healthBoosterPrefab, spawnPosition, Quaternion.identity);
    }
}
