using UnityEngine;
using UnityEngine.InputSystem; // Belangrijk: Gebruik de nieuwe Input System

public class InputController : MonoBehaviour
{
    [Tooltip("Sleep hier het GameObject met het UI3DObjectManager script naartoe.")]
    public UI3DObjectManager displayManager;

    [Header("Input Action Assets")]
    [Tooltip("Sleep hier de Input Action Asset naartoe die de WASD en Arrow toetsen bevat.")]
    public InputActionAsset inputActions;

    // Groepen van input actions (Action Maps)
    private InputActionMap _playerControls;
    private InputActionMap _arrowControls;

    void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("Input Actions Asset is niet ingesteld! De input zal niet werken.");
            enabled = false;
            return;
        }

        // Zoek de Input Action Maps in het Asset
        // Zorg ervoor dat de namen "Player" en "Player1" correct zijn in je Input Asset!
        _playerControls = inputActions.FindActionMap("Player");
        _arrowControls = inputActions.FindActionMap("Player1");

        if (_playerControls == null || _arrowControls == null)
        {
            Debug.LogError("Input Action Maps (PlayerControls of ArrowControls) niet gevonden in het Asset! Controleer de namen.");
            enabled = false;
            return;
        }
        
        // Koppel de events (wat moet er gebeuren als de actie wordt geactiveerd?)
        // We gebruiken += in plaats van .AddListener(), wat de standaard .NET manier is
        _playerControls.actionTriggered += OnPlayerControlsActivated;
        _arrowControls.actionTriggered += OnArrowControlsActivated;
    }

    private void OnEnable()
    {
        // Activeer de Input Action Maps wanneer dit script actief wordt
        _playerControls?.Enable();
        _arrowControls?.Enable();
    }

    private void OnDisable()
    {
        // Deactiveer de Input Action Maps wanneer dit script inactief wordt
        // Vergeet niet de events ook te ontkoppelen als je dit script op een GameObject hebt dat vaak wordt geactiveerd/gedeactiveerd
        _playerControls?.Disable();
        _arrowControls?.Disable();
    }
    
    // Optioneel, ontkoppel events bij destroy om geheugenlekken te voorkomen
    private void OnDestroy()
    {
        if (_playerControls != null)
        {
            _playerControls.actionTriggered -= OnPlayerControlsActivated;
        }
        if (_arrowControls != null)
        {
            _arrowControls.actionTriggered -= OnArrowControlsActivated;
        }
    }

    private void OnPlayerControlsActivated(InputAction.CallbackContext context)
    {
        // Controleren of de actie net is ingedrukt (Performed) én of de displayManager aanwezig is
        if (context.phase == InputActionPhase.Performed && displayManager != null)
        {
            // 1. Lees de Vector2 waarde van de input actie (moet ingesteld zijn als Vector2 in het Input Asset)
            Vector2 movement = context.ReadValue<Vector2>();

            // 2. Controleer of de bewegingswaarde NIET (0, 0) is. 
            // Een waarde groter dan 0 betekent dat er een knop is ingedrukt.
            // sqrMagnitude is efficiënter dan Magnitude.
            if (movement.sqrMagnitude > 0.01f) // 0.01f is een kleine drempel
            {
                // Object 1 (Index 0) wordt geactiveerd door WASD
                Debug.Log("WASD/Player input gedetecteerd. Activeer object 0.");
                displayManager.SetupDisplayByIndex(0);
            }
        }
    }

    private void OnArrowControlsActivated(InputAction.CallbackContext context)
    {
        // Controleren of de actie net is ingedrukt (Performed) én of de displayManager aanwezig is
        if (context.phase == InputActionPhase.Performed && displayManager != null)
        {
            // 1. Lees de Vector2 waarde van de input actie
            Vector2 movement = context.ReadValue<Vector2>();

            // 2. Controleer of de bewegingswaarde NIET (0, 0) is.
            if (movement.sqrMagnitude > 0.01f)
            {
                // Object 2 (Index 1) wordt geactiveerd door de pijltoetsen
                Debug.Log("Pijltoets input gedetecteerd. Activeer object 1.");
                displayManager.SetupDisplayByIndex(1);
            }
        }
    }
}