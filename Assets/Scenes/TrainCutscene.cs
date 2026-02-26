using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Add this for Input System

public class TrainCutscene : MonoBehaviour
{
    [Header("Train Movement (Disabled for now)")]
    public Transform train;
    public Vector3 moveDirection = new Vector3(1, 0, 0);
    public float moveSpeed = 5f;
    public float distanceToMove = 30f;
    
    [Header("Camera Settings")]
    public Camera cutsceneCamera;
    public Vector3 cameraOffset = new Vector3(0, 2, -10);
    public Vector3 cameraRotation = new Vector3(0, 0, 0);
    
    [Header("Auto Load")]
    public float cutsceneDuration = 5f;
    public string nextSceneName = "Scenes/SampleScene";
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isPlaying = false;
    
    void Start()
    {
        Debug.Log("=== TRAIN CUTSCENE STARTED ===");
        
        if (train == null)
            train = transform;
            
        startPosition = train.position;
        targetPosition = startPosition + (moveDirection.normalized * distanceToMove);
        
        // Setup camera (optional)
        if (cutsceneCamera != null)
        {
            cutsceneCamera.transform.SetParent(train);
            cutsceneCamera.transform.localPosition = cameraOffset;
            cutsceneCamera.transform.localEulerAngles = cameraRotation;
        }
        else
        {
            Debug.Log("No camera assigned - that's okay for now");
        }
        
        // FORCE the next scene to load after cutsceneDuration seconds
        Debug.Log("Will load next scene in " + cutsceneDuration + " seconds");
        Invoke("ForceLoadNextScene", cutsceneDuration);
        
        // Don't try to move the train yet
        // StartCutscene();
    }
    
    public void StartCutscene()
    {
        isPlaying = true;
        Debug.Log("Train movement would start here (disabled for now)");
    }
    
    void Update()
    {
        // Skip with spacebar using NEW Input System
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Cutscene skipped by player");
            ForceLoadNextScene();
        }
        
        // Movement is disabled for now
        /*
        if (!isPlaying) return;
        
        // Move train
        train.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
        
        // Check if reached target
        if (Vector3.Distance(train.position, targetPosition) < 0.5f)
        {
            isPlaying = false;
            Debug.Log("Train reached destination");
        }
        */
    }
    
    void ForceLoadNextScene()
    {
        Debug.Log("=== LOADING SAMPLE SCENE NOW ===");
        SceneManager.LoadScene(nextSceneName);
    }
    
    // Visual aid for setup
    void OnDrawGizmosSelected()
    {
        if (train != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = Application.isPlaying ? startPosition : train.position;
            Vector3 end = start + (moveDirection.normalized * distanceToMove);
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(end, 0.5f);
        }
    }
}
