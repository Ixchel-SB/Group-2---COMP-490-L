using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadNextScene : MonoBehaviour
{
    public float delay = 3f; // seconds

    void Start()
    {
        Invoke("LoadNextScene", delay);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("SampleScene"); 
        // Later change this to your intro cutscene scene name
    }
}