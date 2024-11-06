using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsScreenController : MonoBehaviour
{
    // load the controls scene and is used with buttons
    public void LoadControlsScene()
    {
        SceneManager.LoadScene("ControlsScene"); 
    }
}
