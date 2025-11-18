using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem; 

[System.Serializable]
public struct DisplayEntry
{
    [Tooltip("De 3D Prefab die je in de UI wilt tonen.")]
    public GameObject object3DPrefab;

    [Tooltip("Het Panel waarbinnen het object moet verschijnen.")]
    public RectTransform targetUIPanel;

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
    
    [Tooltip("Verticale (Y-as) offset om de 3D objecten in de wereld te groeperen. (Optioneel, beïnvloedt de UI niet).")]
    public float worldYOffset = 0f; 

    [Header("4. UI Render Instellingen")]
    [Tooltip("Verticale (Y-as) verschuiving van de camera. Positief verplaatst het object omlaag in de UI.")]
    public float cameraLookAtYOffset = 0f;
    
    [Header("5. Setup Instellingen")]
    [Tooltip("Stel in op 'true' als de setup voltooid moet zijn voordat de speler kan beginnen.")]
    public bool requiresBothPlayersToSetup = true;


    private const string LayerName = "UI3DObject";
    private int _object3DLayer;
    public GameObject playbutton;
    private int _setupCount = 0; 
    
    // Bijgehouden welke devices al zijn ingesteld
    private readonly HashSet<InputDevice> _setupDevices = new HashSet<InputDevice>();


    void Start()
    {
        _object3DLayer = LayerMask.NameToLayer(LayerName);
        if (_object3DLayer == -1)
        {
            Debug.LogError($"Layer '{LayerName}' bestaat NIET. Zorg ervoor dat deze is aangemaakt in Tags and Layers!");
            enabled = false;
        }
        
        // Zorg ervoor dat de Play Button uit staat bij de start
        if (playbutton != null)
        {
            playbutton.SetActive(false);
        }
    }

    void Update()
    {
        // De rotatie blijft, die moet elke frame uitgevoerd worden.
        foreach (var entry in displayEntries)
        {
            if (entry.spawnedObject != null)
            {
                entry.spawnedObject.transform.Rotate(Vector3.up, 20f * Time.deltaTime);
            }
        }
    }


    /// <summary>
    /// PUBLIEKE METHODE: Wordt aangeroepen door de PlayerInput component.
    /// Bepaalt de index op basis van het apparaat dat triggert (Player 1 = index 0, Player 2 = index 2).
    /// </summary>
    public void OnSetupTrigger(InputAction.CallbackContext context)
    {
        // Alleen triggeren op het moment van de druk (Started/Performed is vaak de trigger)
        if (!context.performed) 
            return;

        // Het apparaat dat de trigger gaf
        InputDevice triggeringDevice = context.control.device;

        // 1. Controleer of het apparaat al geset-up is
        if (_setupDevices.Contains(triggeringDevice))
        {
            // Debug.LogWarning($"Apparaat {triggeringDevice.displayName} heeft de setup al voltooid. Negeer aanroep.");
            return;
        }

        int targetIndex = -1;
        
        // 2. Bepaal de target index op basis van het aantal reeds ingestelde apparaten
        if (_setupDevices.Count == 0)
        {
            // Eerste apparaat -> Player (Index 0)
            targetIndex = 0;
            // Debug.Log($"Apparaat 1 ({triggeringDevice.displayName}) start setup voor index {targetIndex} (Player).");
        }
        else if (_setupDevices.Count == 1)
        {
            // Tweede apparaat -> Player1 (Index 2)
            targetIndex = 2;
            // Debug.Log($"Apparaat 2 ({triggeringDevice.displayName}) start setup voor index {targetIndex} (Player1).");
        }
        else
        {
            // Debug.Log("Maximaal 2 spelers zijn al geset-up. Negeer verdere setup.");
            return;
        }

        // 3. Valideer index en array grootte
        if (targetIndex >= displayEntries.Length)
        {
            Debug.LogError($"Setup kan niet worden voltooid: DisplayEntries array is te klein (minimaal grootte {targetIndex + 1} vereist voor index {targetIndex}).");
            return;
        }
        
        // 4. Voer de setup uit en voeg het apparaat toe
        if (targetIndex != -1)
        {
            SetupDisplayByIndex(targetIndex);
            // BELANGRIJK: Voeg het apparaat pas toe NADAT de SetupDisplayByIndex succesvol is uitgevoerd
            // (dit voorkomt dat we het apparaat toevoegen als de prefab/panel niet ingesteld zijn)
            _setupDevices.Add(triggeringDevice);
        }
    }
    
    
    // --- PUBLIEKE SETUP METHODE (ongewijzigd) ---
    public void SetupDisplayByIndex(int index)
    {
        if (_object3DLayer == -1) { Debug.LogError("Setup kan niet worden voltooid: UI3DObject Layer ontbreekt."); return; }
        if (index < 0 || index >= displayEntries.Length) { Debug.LogError($"Index {index} is ongeldig. Controleer de arraygrootte (max {displayEntries.Length - 1})."); return; }
        
        ref DisplayEntry entry = ref displayEntries[index];

        if (entry.spawnedObject != null) { Debug.LogWarning($"Display Entry {index} is al gespawnd. Negeer aanroep."); return; }
        if (entry.object3DPrefab == null || entry.targetUIPanel == null) { Debug.LogError($"Display Entry {index} is niet volledig geconfigureerd in de Inspector."); return; }
        
        Setup3DObjectDisplayInternal(ref entry, index);
    }
    
    
    // --- INTERNE SETUP FUNCTIE (Playbutton logica gefixt) ---
    private void Setup3DObjectDisplayInternal(ref DisplayEntry entry, int index)
    {
        RectTransform targetUIPanel = entry.targetUIPanel;

        // ... (Render Texture en UI Setup code ongewijzigd) ...
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
        entry.spawnedObject.transform.position = new Vector3(index * worldSeparationDistance, worldYOffset, 0);

        // Audio Logica (Veiligheidscheck)
        // if (AudioManager.Instance != null) { AudioManager.Instance.Play("join"); }
        // else { Debug.LogWarning("AudioManager.Instance is null. Geluid 'join' kon niet worden afgespeeld."); }
        
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
        
        // 5. Pas de camera-afstand en positie aan.
        float requiredDistance = CalculateCameraDistanceToFit(entry.spawnedObject, entry.renderCamera, targetUIPanel, marginScale);
        Vector3 targetCenter = entry.spawnedObject.transform.position;
        Vector3 adjustedTargetCenter = targetCenter + new Vector3(0, cameraLookAtYOffset, 0);
        entry.renderCamera.transform.position = adjustedTargetCenter + new Vector3(0, 0, -requiredDistance);
        entry.renderCamera.transform.LookAt(adjustedTargetCenter);

        // Controle voor Play Button activatie
        _setupCount++; 

        // ⭐ GEFIXT: Gebruikt de _setupDevices.Count, wat de meest nauwkeurige teller is van het aantal actieve spelers.
        int requiredCount = requiresBothPlayersToSetup ? 2 : 1;
        
        if (_setupDevices.Count >= requiredCount && playbutton != null && !playbutton.activeSelf)
        {
             // Dit zorgt ervoor dat de knop pas actief wordt als de setup van de tweede (of eerste) speler voltooid is.
            playbutton.SetActive(true);
            Debug.Log($"Setup count is {_setupDevices.Count}. 'playbutton' is geactiveerd.");
        }
    }
    
    // ... (Hulpfuncties blijven ongewijzigd) ...

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