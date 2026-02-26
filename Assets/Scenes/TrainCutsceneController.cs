using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainCutsceneController : MonoBehaviour
{
    [Header("Train Movement")]
    public Transform train;
    public float moveSpeed = 5f;
    public float moveDistance = 30f;
    public Vector3 moveDirection = new Vector3(1, 0, 0);
    
    [Header("Camera")]
    public Camera cutsceneCamera;
    public Vector3 cameraOffset = new Vector3(0, 2, -10);
    
    [Header("Auto Load")]
    public float cutsceneDuration = 5f;
    public string nextSceneName = "Scenes/SampleScene";
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    
    void Start()
    {
        if (train == null)
            train = transform;
            
        startPosition = train.position;
        targetPosition = startPosition + (moveDirection.normalized * moveDistance);
        
        // Setup camera
        if (cutsceneCamera != null)
        {
            cutsceneCamera.transform.SetParent(train);
            cutsceneCamera.transform.localPosition = cameraOffset;
            cutsceneCamera.transform.LookAt(train);
        }
        
        // Start moving and auto-load next scene
        StartCutscene();
    }
    
    void StartCutscene()
    {
        Debug.Log("Train cutscene started - duration: " + cutsceneDuration + " seconds");
        isMoving = true;
        
        // Auto-load next scene after duration
        Invoke("LoadNextScene", cutsceneDuration);
    }
    
    void Update()
    {
        // COMBINED UPDATE METHOD
        
        // 1. Train movement
        if (isMoving)
        {
            train.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
            
            // Optional: Stop moving if reached target
            if (Vector3.Distance(train.position, targetPosition) < 0.5f)
            {
                isMoving = false;
            }
        }
        
        // 2. Skip cutscene with spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Cutscene skipped by player");
            LoadNextScene();
        }
    }
    
    void LoadNextScene()
    {
        Debug.Log("Train cutscene finished - loading: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}