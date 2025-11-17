using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// De datastructuur blijft hetzelfde.
[System.Serializable]
public struct DisplayEntry
{
    [Tooltip("De 3D Prefab die je in de UI wilt tonen.")]
    public GameObject object3DPrefab;

    [Tooltip("Het Panel waarbinnen het object moet verschijnen.")]
    public RectTransform targetUIPanel;

    // We verwijderen 'activationKeys' en 'isSetupAttempted' uit de struct.

    // Bijgehouden instanties
    [HideInInspector] public GameObject spawnedObject;
    [HideInInspector] public Camera renderCamera;
    [HideInInspector] public RenderTexture renderTexture;
}


// De hoofdklasse (MonoBehaviour)
public class UI3DObjectManager : MonoBehaviour
{
    [Header("1. Lijst van 3D Displays")]
    public DisplayEntry[] displayEntries;

    [Header("2. Globale Schaal & Marge Instellingen")]
    [Range(30f, 90f)]
    public float cameraFOV = 60f;
    [Range(0.0f, 0.5f)]
    public float marginScale = 0.1f; 

    [Header("3. Wereldpositie Instellingen")]
    [Tooltip("Afstand tussen de gespawnde 3D objecten in de wereld (langs de X-as).")]
    public float worldSeparationDistance = 1000f; 

    // Interne Render Texture instellingen
    private const string LayerName = "UI3DObject";
    private int _object3DLayer;

    // Start wordt nu gebruikt voor de Layer check en de voorbereiding van de data.
    void Start()
    {
        _object3DLayer = LayerMask.NameToLayer(LayerName);
        if (_object3DLayer == -1)
        {
            Debug.LogError($"Layer '{LayerName}' bestaat NIET. Zorg ervoor dat deze is aangemaakt in Tags and Layers!");
            enabled = false;
        }
    }

    void Update()
    {
        // Laat objecten draaien, maar de input-check is verdwenen.
        foreach (var entry in displayEntries)
        {
            if (entry.spawnedObject != null)
            {
                entry.spawnedObject.transform.Rotate(Vector3.up, 20f * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// PUBLIEKE METHODE: Initialiseert en toont een 3D object in de UI.
    /// Deze methode wordt nu aangeroepen vanuit een extern script.
    /// </summary>
    /// <param name="index">De index van de displayEntry die moet worden geactiveerd (0 voor de eerste, 1 voor de tweede, etc.).</param>
    public void SetupDisplayByIndex(int index)
    {
        if (_object3DLayer == -1)
        {
            Debug.LogError("Setup kan niet worden voltooid: UI3DObject Layer ontbreekt.");
            return;
        }

        if (index < 0 || index >= displayEntries.Length)
        {
            Debug.LogError($"Index {index} is ongeldig. Controleer de arraygrootte (max {displayEntries.Length - 1}).");
            return;
        }

        // Gebruik ref om de struct direct aan te passen in de array
        ref DisplayEntry entry = ref displayEntries[index];

        if (entry.spawnedObject != null)
        {
            Debug.LogWarning($"Display Entry {index} is al gespawnd. Negeer aanroep.");
            return;
        }
        
        if (entry.object3DPrefab == null || entry.targetUIPanel == null)
        {
            Debug.LogError($"Display Entry {index} is niet volledig geconfigureerd in de Inspector.");
            return;
        }
        
        // De oorspronkelijke interne setup logica
        Setup3DObjectDisplayInternal(ref entry, index);
    }
    
    
    // --- INTERNE SETUP FUNCTIE (Verplaatst en hernoemd) ---

    private void Setup3DObjectDisplayInternal(ref DisplayEntry entry, int index)
    {
        RectTransform targetUIPanel = entry.targetUIPanel;

        // --- Render Texture en UI Setup ---
        int width = Mathf.RoundToInt(targetUIPanel.rect.width);
        int height = Mathf.RoundToInt(targetUIPanel.rect.height);
        
        entry.renderTexture = new RenderTexture(width, height, 24);
        entry.renderTexture.Create();

        GameObject rawImageGO = new GameObject(targetUIPanel.name + "_3D_RawImage", typeof(RawImage));
        RawImage rawImage = rawImageGO.GetComponent<RawImage>();
        
        rawImage.texture = entry.renderTexture;
        rawImage.transform.SetParent(targetUIPanel, false);
        
        rawImage.rectTransform.anchorMin = Vector2.zero;
        rawImage.rectTransform.anchorMax = Vector2.one;
        rawImage.rectTransform.offsetMin = Vector2.zero;
        rawImage.rectTransform.offsetMax = Vector2.zero;
        
        // 3. Instantieer het 3D Object.
        entry.spawnedObject = Instantiate(entry.object3DPrefab);
        SetLayerRecursively(entry.spawnedObject.transform, _object3DLayer);
        
        // Geef het object een unieke, ver verwijderde positie
        entry.spawnedObject.transform.position = new Vector3(index * worldSeparationDistance, 0, 0);

        // 4. Maak de Render Camera
        GameObject camGO = new GameObject(targetUIPanel.name + "_3DCamera");
        entry.renderCamera = camGO.AddComponent<Camera>();
        
        entry.renderCamera.targetTexture = entry.renderTexture; 
        
        // Camera configuratie
        entry.renderCamera.clearFlags = CameraClearFlags.SolidColor;
        entry.renderCamera.backgroundColor = new Color(0f, 0f, 0f, 0f); 
        entry.renderCamera.cullingMask = 1 << _object3DLayer;
        entry.renderCamera.orthographic = false; // Perspective
        entry.renderCamera.fieldOfView = cameraFOV;
        
        // 5. Pas de camera-afstand aan.
        float requiredDistance = CalculateCameraDistanceToFit(entry.spawnedObject, entry.renderCamera, targetUIPanel, marginScale);

        // Plaats de camera op de berekende afstand
        Vector3 targetCenter = entry.spawnedObject.transform.position;
        entry.renderCamera.transform.position = targetCenter + new Vector3(0, 0, -requiredDistance);
        entry.renderCamera.transform.LookAt(targetCenter);

        Debug.Log($"3D Object '{entry.object3DPrefab.name}' (Index {index}) is succesvol gespawnd.");
    }
    
    // --- BEREKENINGS EN HULPFUNCTIES (ongewijzigd) ---

    private float CalculateCameraDistanceToFit(GameObject targetObject, Camera camera, RectTransform panelRect, float margin)
    {
        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 5f;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z); 
        float fitScale = 1.0f - margin * 2f;
        if (fitScale <= 0) fitScale = 0.1f; 

        float aspectRatio = panelRect.rect.width / panelRect.rect.height;
        float halfFOV = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;

        float requiredDistanceHeight = (objectSize / fitScale / 2f) / Mathf.Tan(halfFOV);
        float requiredDistanceWidth = (objectSize / fitScale / 2f) / (Mathf.Tan(halfFOV) * aspectRatio);

        float requiredDistance = Mathf.Max(requiredDistanceHeight, requiredDistanceWidth) + bounds.extents.z;

        return requiredDistance;
    }

    private void SetLayerRecursively(Transform parent, int layer)
    {
        parent.gameObject.layer = layer;
        foreach (Transform child in parent)
        {
            SetLayerRecursively(child, layer);
        }
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