using UnityEngine;

public class MapViewer : MonoBehaviour
{
    [Header("Map Settings")]
    public Texture2D oldMapTexture; // Drag your Old Map texture here
    public Transform playerHead;
    public float distanceInFront = 2f;
    public float mapWidth = 2f;
    public float mapHeight = 1.5f;
    
    private GameObject currentMapInstance;
    private bool isMapOpen = false;
    private GameObject player;
    private MonoBehaviour playerController;
    private Camera mainCamera;
    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;
    private GameObject playerModel;
    private PauseManager3D pauseManager; // Reference to pause manager

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main;
        
        // Find PauseManager
        pauseManager = FindObjectOfType<PauseManager3D>();
        
        if (player != null)
        {
            // Find player controller
            playerController = player.GetComponent<MonoBehaviour>();
            if (playerController == null)
            {
                Transform playerArmature = player.transform.Find("PlayerArmature");
                if (playerArmature != null)
                    playerController = playerArmature.GetComponent<MonoBehaviour>();
            }
            
            // Find player model to hide
            SkinnedMeshRenderer skinnedMesh = player.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
                playerModel = skinnedMesh.gameObject;
            else
                playerModel = player;
        }
        
        Debug.Log("MapViewer initialized");
    }
    
    void Update()
    {
        // Check if game is paused before allowing map to open
        bool isGamePaused = false;
        if (pauseManager != null)
        {
            isGamePaused = pauseManager.IsGamePaused();
        }
        
        // Only allow map to open if:
        // 1. Player has the map
        // 2. Game is NOT paused
        // 3. Map is not already open (or we're closing it)
        if (GameProgressManager.Instance != null && GameProgressManager.Instance.HasMap() && !isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (isMapOpen)
                    CloseMap();
                else
                    OpenMap();
            }
        }
        
        // Also handle closing map if game gets paused while map is open
        if (isMapOpen && isGamePaused)
        {
            CloseMap();
        }
    }
    
    // Public method for PauseManager to check if map is open
    public bool IsMapOpen()
    {
        return isMapOpen;
    }
    
    void OpenMap()
    {
        Debug.Log("OpenMap called");
        isMapOpen = true;
        
        // FREEZE PLAYER
        if (playerController != null)
        {
            playerController.enabled = false;
            Debug.Log("Player controller disabled");
        }
        
        // HIDE PLAYER MODEL
        if (playerModel != null)
        {
            playerModel.SetActive(false);
            Debug.Log("Player model hidden");
        }
        
        // Store camera position
        if (mainCamera != null)
        {
            originalCameraPos = mainCamera.transform.position;
            originalCameraRot = mainCamera.transform.rotation;
        }
        
        // Create a flat plane with the Old Map texture
        if (oldMapTexture != null && playerHead != null)
        {
            // Create a new GameObject for the map
            currentMapInstance = new GameObject("OldMap");
            
            // Add a Quad (flat plane) mesh
            MeshFilter meshFilter = currentMapInstance.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateQuadMesh();
            
            // Add a MeshRenderer
            MeshRenderer renderer = currentMapInstance.AddComponent<MeshRenderer>();
            
            // Create a material with your texture
            Material mapMaterial = new Material(Shader.Find("Unlit/Texture"));
            mapMaterial.mainTexture = oldMapTexture;
            renderer.material = mapMaterial;
            
            // Position in front of player
            Vector3 mapPosition = playerHead.position + playerHead.forward * distanceInFront;
            currentMapInstance.transform.position = mapPosition;
            
            // Scale the quad to desired size
            currentMapInstance.transform.localScale = new Vector3(mapWidth, mapHeight, 1);
            
            // Face the player
            currentMapInstance.transform.LookAt(playerHead);
            currentMapInstance.transform.Rotate(0, 180f, 0);
            
            Debug.Log($"2D Map created at position: {mapPosition}");
        }
        else
        {
            Debug.LogError($"oldMapTexture: {oldMapTexture != null}, playerHead: {playerHead != null}");
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void CloseMap()
    {
        Debug.Log("CloseMap called");
        isMapOpen = false;
        
        // UNFREEZE PLAYER
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("Player controller enabled");
        }
        
        // SHOW PLAYER MODEL
        if (playerModel != null)
        {
            playerModel.SetActive(true);
            Debug.Log("Player model shown");
        }
        
        // Restore camera position
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPos;
            mainCamera.transform.rotation = originalCameraRot;
            Debug.Log("Camera restored");
        }
        
        // Destroy map instance
        if (currentMapInstance != null)
        {
            Destroy(currentMapInstance);
            currentMapInstance = null;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // Helper method to create a simple quad mesh
    Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(0.5f, -0.5f, 0),
            new Vector3(-0.5f, 0.5f, 0),
            new Vector3(0.5f, 0.5f, 0)
        };
        
        int[] triangles = new int[]
        {
            0, 2, 1,
            2, 3, 1
        };
        
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        
        return mesh;
    }
}
