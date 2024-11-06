using UnityEngine;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI highScoreText;

    void Start()
    {
        highScoreText = GetComponent<TextMeshProUGUI>();

        // Load the saved high score from PlayerPrefs, defaulting to 0 if not set
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + savedHighScore;
    }

    // This method will be called by the AltitudeCounter when the high score is updated
    public void UpdateHighScore(int newHighScore)
    {
        // Update the high score text with the new value
        highScoreText.text = "High Score: " + newHighScore;

        // Save the new high score to PlayerPrefs for persistence
        PlayerPrefs.SetInt("HighScore", newHighScore);
    }
}
