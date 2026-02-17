using UnityEngine;
using UnityEngine.InputSystem; // important if using new input system

public class PauseManager : MonoBehaviour
{
    public UnityEngine.InputSystem.PlayerInput playerInput;

    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        Time.timeScale = 0f;
        playerInput.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    void Resume()
    {
        Time.timeScale = 1f;
        playerInput.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }
}

