using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenController : MonoBehaviour
{
    // load the launch scene and is used with buttons
    public void LoadLaunchScene()
    {
        SceneManager.LoadScene("LaunchScene"); 
    }
}
