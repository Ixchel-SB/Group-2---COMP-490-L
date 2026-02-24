using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager3D : MonoBehaviour
{
    public GameObject pauseMenu3D;        // drag PauseMenu3D here
    public Transform playerHead;          // drag Main Camera here
    public MonoBehaviour playerController; // drag ThirdPersonController or PlayerArmature here
    public float distanceInFront = 2f;   // distance of menu from player

    private bool isPaused = false;

    void Start()
    {
        pauseMenu3D.SetActive(false);
    }

    void Update()
    {
        // Press P to pause
        if (Keyboard.current.pKey.wasPressedThisFrame && !isPaused)
            Pause();
    }

    void Pause()
    {
        Time.timeScale = 0f;
        playerController.enabled = false;

        pauseMenu3D.SetActive(true);

        // Position menu in front of the player
        pauseMenu3D.transform.position = playerHead.position + playerHead.forward * distanceInFront;

        // Rotate menu to face the player
        pauseMenu3D.transform.LookAt(playerHead);
        pauseMenu3D.transform.Rotate(0, 180f, 0); // flip because LookAt faces back

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        playerController.enabled = true;

        pauseMenu3D.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }
}
