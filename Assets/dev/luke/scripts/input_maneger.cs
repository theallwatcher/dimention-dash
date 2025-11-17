using UnityEngine;
using UnityEngine.InputSystem; // Belangrijk: Gebruik de nieuwe Input System

public class InputController : MonoBehaviour
{
    [Tooltip("Sleep hier het GameObject met het UI3DObjectManager script naartoe.")]
    public UI3DObjectManager displayManager;

    [Header("Input Action Assets")]
    [Tooltip("Sleep hier de Input Action Asset naartoe die de WASD en Arrow toetsen bevat.")]
    public InputActionAsset inputActions;

    // Groepen van input actions
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
        _playerControls = inputActions.FindActionMap("PlayerControls");
        _arrowControls = inputActions.FindActionMap("ArrowControls");

        if (_playerControls == null || _arrowControls == null)
        {
             Debug.LogError("Input Action Maps (PlayerControls of ArrowControls) niet gevonden in het Asset!");
             enabled = false;
             return;
        }
        
        // Koppel de events (wat moet er gebeuren als de actie wordt geactiveerd?)
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
        _playerControls?.Disable();
        _arrowControls?.Disable();
    }

    private void OnPlayerControlsActivated(InputAction.CallbackContext context)
    {
        // Controleren of de actie net is ingedrukt (equivalent aan GetKeyDown)
        if (context.phase == InputActionPhase.Performed && displayManager != null)
        {
            // Object 1 (Index 0) wordt geactiveerd door WASD
            displayManager.SetupDisplayByIndex(0);
        }
    }

    private void OnArrowControlsActivated(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && displayManager != null)
        {
            // Object 2 (Index 1) wordt geactiveerd door de pijltoetsen
            displayManager.SetupDisplayByIndex(1);
        }
    }
}