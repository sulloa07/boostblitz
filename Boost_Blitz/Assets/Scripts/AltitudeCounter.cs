using UnityEngine;
using TMPro;

public class AltitudeCounter : MonoBehaviour
{
    public Transform rocket;

    // TextMeshProUGUI component to display the altitude score
    private TextMeshProUGUI altitudeText;

    // Current altitude score
    private int currentScore;

    // Static variable to hold the high score
    public static int highScore;

    // Reference to the HighScoreDisplay component
    private HighScoreDisplay highScoreDisplay;

    void Start()
    {
        // Get the TextMeshProUGUI component attached to this GameObject
        altitudeText = GetComponent<TextMeshProUGUI>();

        // Load the saved high score from PlayerPrefs, defaulting to 0 if not set
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Find and assign the HighScoreDisplay component in the scene
        highScoreDisplay = FindObjectOfType<HighScoreDisplay>();
    }

    void Update()
    {
        // Calculate the current score based on the rocket's Y position
        currentScore = Mathf.FloorToInt(rocket.position.y + 3f);

        // Update the altitude text to display the current score
        altitudeText.text = "Score: " + currentScore;

        // Check if the current score exceeds the high score
        if (currentScore > highScore)
        {
            highScore = currentScore; // Update the high score
            UpdateHighScore();        // Update the high score display
        }
    }

    // Method to start the launch and display the altitude score
    public void StartLaunch()
    {
        // Activate the altitude text UI
        altitudeText.gameObject.SetActive(true);
    }

    // Method to update the high score display and save it
    private void UpdateHighScore()
    {
        if (highScoreDisplay != null)
        {
            // Update the high score display with the new high score
            highScoreDisplay.UpdateHighScore(highScore);
        }
    }
}
