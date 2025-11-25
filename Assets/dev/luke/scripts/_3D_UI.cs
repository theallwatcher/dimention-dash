using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Display_3D_Entry
{
    // Nieuw: Initiële rotatie in Euler hoeken (X, Y, Z)
    public Vector3 initialRotation; 
    
    public bool rotateObject; 
    
    public GameObject objectPrefab; 
    public RectTransform targetUIPanel;

    [HideInInspector] public GameObject spawnedObject;
    [HideInInspector] public Camera renderCamera;
    [HideInInspector] public RenderTexture renderTexture;
}

public class _3D_UI : MonoBehaviour
{
    [Header("1. Lijst van 3D Displays")]
    public Display_3D_Entry[] displayEntries;

    [Header("2. Camera & UI Settings")]
    [Range(30f, 90f)] public float cameraFOV = 60f;
    [Range(0f, 0.5f)] public float marginScale = 0.1f;

    [Header("3. World Positioning")]
    public float worldSeparationDistance = 1000f;
    public float worldXOffset = 0f;
    public float worldYOffset = 0f;
    public float worldZOffset = 0f;
    public float cameraLookAtYOffset = 0f;

    [Header("4. Setup")]
    public bool requiresBothPlayersToSetup = false;
    public GameObject playbutton;

    private const string LayerName = "UI3DObject";
    private int _object3DLayer;
    private int _setupCount = 0;

    void Start()
    {
        _object3DLayer = LayerMask.NameToLayer(LayerName);
        if (_object3DLayer == -1)
        {
            Debug.LogError($"Layer '{LayerName}' bestaat NIET! Zorg ervoor dat deze Layer is toegevoegd in Project Settings -> Tags and Layers.");
            enabled = false;
            return;
        }

        SetupAllDisplays(); 
        if (playbutton != null) playbutton.SetActive(false);
    }
    
    void OnValidate()
    {
        RecalculateCameraPositions();
    }

    private void SetupAllDisplays()
    {
        for (int i = 0; i < displayEntries.Length; i++)
        {
            SetupDisplayForObject(i);
        }
        
        CheckSetupComplete();
    }

    void Update()
    {
        // De doorlopende rotatie
        foreach (var entry in displayEntries)
        {
            if (entry.spawnedObject != null && entry.rotateObject)
            {
                // Gebruikt lokale rotatie om het object rond zijn eigen Y-as te laten draaien
                entry.spawnedObject.transform.Rotate(Vector3.up, 20f * Time.deltaTime, Space.Self);
            }
        }
    }

    public void RecalculateCameraPositions()
    {
        if (!Application.isPlaying && !Application.isEditor) return;

        foreach (var entry in displayEntries)
        {
            if (entry.renderCamera != null && entry.spawnedObject != null && entry.targetUIPanel != null)
            {
                // Deel 1: Object positie en rotatie aanpassen
                float currentXSeparation = entry.spawnedObject.transform.position.x;
                
                // Rotatie opnieuw toepassen als de initialRotation variabele is gewijzigd
                entry.spawnedObject.transform.rotation = Quaternion.Euler(entry.initialRotation); 
                
                // Positie opnieuw toepassen
                entry.spawnedObject.transform.position = new Vector3(
                    currentXSeparation, // De X-separatie blijft behouden
                    worldYOffset, 
                    worldZOffset
                );

                // Deel 2: Camera aanpassen
                entry.renderCamera.fieldOfView = cameraFOV;
                float requiredDistance = CalculateCameraDistanceToFit(entry.spawnedObject, entry.renderCamera, entry.targetUIPanel, marginScale);
                Vector3 targetCenter = entry.spawnedObject.transform.position + new Vector3(0, cameraLookAtYOffset, 0);
                entry.renderCamera.transform.position = targetCenter + new Vector3(0, 0, -requiredDistance);
                entry.renderCamera.transform.LookAt(targetCenter);
            }
        }
    }

    public void SetupDisplayForObject(int index)
    {
        if (_object3DLayer == -1) return;
        if (index < 0 || index >= displayEntries.Length) return;

        ref Display_3D_Entry entry = ref displayEntries[index];
        
        if (entry.objectPrefab == null) { Debug.LogWarning($"DisplayEntry {index} heeft geen Prefab toegewezen."); return; }
        if (entry.spawnedObject != null) return;
        if (entry.targetUIPanel == null) return;

        // 1. Spawn het object
        entry.spawnedObject = Instantiate(entry.objectPrefab);

        // 2. Rotatie instellen
        // *** WIJZIGING HIER: Pas de initiële rotatie toe ***
        entry.spawnedObject.transform.rotation = Quaternion.Euler(entry.initialRotation);
        
        // 3. Positie instellen
        SetLayerRecursively(entry.spawnedObject.transform, _object3DLayer);
        entry.spawnedObject.transform.position = new Vector3(
            (index * worldSeparationDistance) + worldXOffset, 
            worldYOffset,                                     
            worldZOffset                                      
        );

        // 4. Maak de Render Camera en Render Texture
        GameObject camGO = new GameObject(entry.targetUIPanel.name + "_3DCamera");
        entry.renderCamera = camGO.AddComponent<Camera>();
        
        entry.renderCamera.targetTexture = new RenderTexture(
            Mathf.RoundToInt(entry.targetUIPanel.rect.width),
            Mathf.RoundToInt(entry.targetUIPanel.rect.height),
            24);
        entry.renderTexture = entry.renderCamera.targetTexture;

        entry.renderCamera.clearFlags = CameraClearFlags.SolidColor;
        entry.renderCamera.backgroundColor = new Color(0, 0, 0, 0);
        entry.renderCamera.cullingMask = 1 << _object3DLayer;
        entry.renderCamera.fieldOfView = cameraFOV;

        // 5. Plaats RawImage in UI
        GameObject rawImageGO = new GameObject(entry.targetUIPanel.name + "_3D_RawImage", typeof(RawImage));
        RawImage rawImage = rawImageGO.GetComponent<RawImage>();
        rawImage.texture = entry.renderTexture;
        rawImage.transform.SetParent(entry.targetUIPanel, false);
        
        rawImage.rectTransform.anchorMin = Vector2.zero;
        rawImage.rectTransform.anchorMax = Vector2.one;
        rawImage.rectTransform.offsetMin = Vector2.zero;
        rawImage.rectTransform.offsetMax = Vector2.zero;

        // 6. Herbereken de camerapositie
        RecalculateCameraPositions(); 
        
        // 7. Update setup count
        _setupCount++;
        CheckSetupComplete();
    }

    private void CheckSetupComplete()
    {
        if (playbutton == null) return;

        if (requiresBothPlayersToSetup)
        {
            if (_setupCount >= 2) playbutton.SetActive(true);
        }
        else
        {
            if (_setupCount >= displayEntries.Length && displayEntries.Length > 0) playbutton.SetActive(true);
        }
    }

    private float CalculateCameraDistanceToFit(GameObject targetObject, Camera camera, RectTransform panelRect, float margin)
    {
        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 5f;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);

        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float fitScale = Mathf.Max(0.1f, 1f - margin * 2f);

        float aspectRatio = panelRect.rect.width / panelRect.rect.height;
        float halfFOV = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;

        float requiredDistanceHeight = (objectSize / fitScale / 2f) / Mathf.Tan(halfFOV);
        float requiredDistanceWidth = (objectSize / fitScale / 2f) / (Mathf.Tan(halfFOV) * aspectRatio);

        return Mathf.Max(requiredDistanceHeight, requiredDistanceWidth) + bounds.extents.z;
    }

    private void SetLayerRecursively(Transform parent, int layer)
    {
        parent.gameObject.layer = layer;
        foreach (Transform child in parent) SetLayerRecursively(child, layer);
    }

    void OnDestroy()
    {
        foreach (var entry in displayEntries)
        {
            if (entry.renderTexture != null) { entry.renderTexture.Release(); Destroy(entry.renderTexture); }
            if (entry.renderCamera != null) { Destroy(entry.renderCamera.gameObject); }
            if (entry.spawnedObject != null) { Destroy(entry.spawnedObject); }
        }
    }
}