using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuResumeButton : MonoBehaviour
{
    public GameObject resumeSavedGameButton;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        bool hasSave = PlayerPrefs.GetInt("HasSavedGame", 0) == 1;
        resumeSavedGameButton.SetActive(hasSave); 
    }

    public void ResumeSavedGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
