using UnityEngine;

public class LoadSavedPosition : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("HasSavedGame", 0) == 1)
        {
            CharacterController controller = GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            transform.position = new Vector3(
                PlayerPrefs.GetFloat("PlayerX"),
                PlayerPrefs.GetFloat("PlayerY"),
                PlayerPrefs.GetFloat("PlayerZ")
            );

            if (controller != null)
                controller.enabled = true;

            Debug.Log("Loaded saved position!");
        }
    }

}
