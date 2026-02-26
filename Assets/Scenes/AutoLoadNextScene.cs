using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadNextScene : MonoBehaviour
{
    public float delay = 4f; // seconds (increased to 4 for better readability)

    void Start()
    {
        Debug.Log("StoryTextScene: Will load train cutscene in " + delay + " seconds");
        Invoke("LoadNextScene", delay);
    }

    void LoadNextScene()
    {
        Debug.Log("StoryTextScene: Loading TrainCutscene");
        SceneManager.LoadScene("Scenes/TrainCutscene"); 
        // Changed from "SampleScene" to "Scenes/TrainCutscene"
    }
}