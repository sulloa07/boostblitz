using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    // Initial health of the asteroid
    public int health = 3;

    // Reference to the rocket's Transform component
    public Transform rocket;

    // Rigidbody component of the asteroid
    private Rigidbody rb;

    // Enum to define different sizes of asteroids
    public enum AsteroidSize { Small, Medium, Large, Huge }

    // Current size of the asteroid
    public AsteroidSize asteroidSize;

    void Start()
    {
        // Get the Rigidbody component attached to this asteroid
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // If the asteroid has fallen below a certain point relative to the rocket, destroy it
        if (transform.position.y < rocket.position.y - 20f)
        {
            Destroy(gameObject);
        }
    }

    // Method to apply damage to the asteroid
    public void TakeDamage(int damageAmount)
    {
        // Reduce the asteroid's health by the damage amount
        health -= damageAmount;

        // If health drops to zero or below, destroy the asteroid
        if (health <= 0)
        {
            DestroyAsteroid();
        }
    }

    // Method to handle the destruction of the asteroid
    void DestroyAsteroid()
    {
        // Destroy this asteroid GameObject
        Destroy(gameObject);
    }

    // Method to get the damage value based on the asteroid's size
    public int GetAsteroidDamage()
    {
        switch (asteroidSize)
        {
            case AsteroidSize.Small:
                return 10;
            case AsteroidSize.Medium:
                return 20;
            case AsteroidSize.Large:
                return 30;
            case AsteroidSize.Huge:
                return 50;
            default:
                return 10;
        }
    }
}
