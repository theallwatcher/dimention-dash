using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DisplayEntry
{
    public RectTransform targetUIPanel;

    [HideInInspector] public GameObject spawnedObject;
    [HideInInspector] public Camera renderCamera;
    [HideInInspector] public RenderTexture renderTexture;
}

public class UI3DObjectManager : MonoBehaviour
{
    [Header("1. Lijst van 3D Displays")]
    public DisplayEntry[] displayEntries;

    [Header("2. Camera & UI Settings")]
    [Range(30f, 90f)] public float cameraFOV = 60f;
    [Range(0f, 0.5f)] public float marginScale = 0.1f;

    [Header("3. World Positioning")]
    public float worldSeparationDistance = 1000f;
    public float worldYOffset = 0f;
    public float cameraLookAtYOffset = 0f;

    [Header("4. Setup")]
    public bool requiresBothPlayersToSetup = true;
    public GameObject playbutton;

    private const string LayerName = "UI3DObject";
    private int _object3DLayer;
    private int _setupCount = 0;

    void Start()
    {
        _object3DLayer = LayerMask.NameToLayer(LayerName);
        if (_object3DLayer == -1)
        {
            Debug.LogError($"Layer '{LayerName}' bestaat NIET!");
            enabled = false;
            return;
        }

        if (playbutton != null) playbutton.SetActive(false);
    }

    void Update()
    {
        foreach (var entry in displayEntries)
        {
            if (entry.spawnedObject != null)
            {
                entry.spawnedObject.transform.Rotate(Vector3.up, 20f * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Wordt aangeroepen door Input_Manager nadat een speler is gespawnd
    /// </summary>
    public void SetupDisplayForPlayer(GameObject playerObject, int index)
    {
        if (_object3DLayer == -1 || playerObject == null) return;
        if (index < 0 || index >= displayEntries.Length) return;

        ref DisplayEntry entry = ref displayEntries[index];
        if (entry.spawnedObject != null) return;
        if (entry.targetUIPanel == null) return;

        // Gebruik de reeds gespawnde player prefab
        entry.spawnedObject = playerObject;

        // Zet de juiste layer
        SetLayerRecursively(entry.spawnedObject.transform, _object3DLayer);

        // Zet speler ver weg voor rendering
        entry.spawnedObject.transform.position = new Vector3(index * worldSeparationDistance, worldYOffset, 0);

        // Maak camera
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

        // Plaats RawImage in UI
        GameObject rawImageGO = new GameObject(entry.targetUIPanel.name + "_3D_RawImage", typeof(RawImage));
        RawImage rawImage = rawImageGO.GetComponent<RawImage>();
        rawImage.texture = entry.renderTexture;
        rawImage.transform.SetParent(entry.targetUIPanel, false);
        rawImage.rectTransform.anchorMin = Vector2.zero;
        rawImage.rectTransform.anchorMax = Vector2.one;
        rawImage.rectTransform.offsetMin = Vector2.zero;
        rawImage.rectTransform.offsetMax = Vector2.zero;

        // Camera afstand berekenen
        float requiredDistance = CalculateCameraDistanceToFit(entry.spawnedObject, entry.renderCamera, entry.targetUIPanel, marginScale);
        Vector3 targetCenter = entry.spawnedObject.transform.position + new Vector3(0, cameraLookAtYOffset, 0);
        entry.renderCamera.transform.position = targetCenter + new Vector3(0, 0, -requiredDistance);
        entry.renderCamera.transform.LookAt(targetCenter);

        // Update setup count en activeer playbutton
        _setupCount++;
        if (requiresBothPlayersToSetup)
        {
            if (_setupCount >= 2 && playbutton != null) playbutton.SetActive(true);
        }
        else
        {
            if (_setupCount >= 1 && playbutton != null) playbutton.SetActive(true);
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
            // spawnedObject wordt beheerd door Input_Manager, dus niet vernietigen
        }
    }
}
