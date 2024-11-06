using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // load the home screen and is used with buttons
    public void LoadHomeScreen()
    {
        SceneManager.LoadScene("HomeScreen"); 
    }
}
