using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputController : MonoBehaviour
{
    [Tooltip("Sleep hier het GameObject met het UI3DObjectManager script naartoe.")]
    public UI3DObjectManager displayManager;

    [Header("Input Action Assets")]
    [Tooltip("Dit asset bevat de Player en Player1 maps.")]
    public InputActionAsset inputActions;
    
    // Deze acties blijven, maar we luisteren naar beide
    private InputAction _playerMoveAction; 
    private InputAction _arrowMoveAction; 

    private const string MoveActionName = "Move";
    
    // ⭐ NIEUW: Opslag om bij te houden welk apparaat welke index al gespawnd heeft.
    private readonly Dictionary<InputDevice, int> _deviceIndexMap = new Dictionary<InputDevice, int>();
    
    // De index die we willen spawnen voor de Player1 map (Index 1 i.p.v. 2)
    private const int Player1Index = 1; 


    void Awake()
    {
        if (inputActions == null || displayManager == null)
        {
            if (inputActions == null) Debug.LogError("Input Actions Asset is niet ingesteld!");
            if (displayManager == null) Debug.LogError("UI3DObjectManager is niet ingesteld!");
            enabled = false;
            return;
        }

        InputActionMap playerMap = inputActions.FindActionMap("Player");
        InputActionMap player1Map = inputActions.FindActionMap("Player1"); // Naam is "Player1"
        
        if (playerMap == null || player1Map == null)
        {
            Debug.LogError("Input Action Maps (Player of Player1) niet gevonden! Controleer de namen.");
            enabled = false;
            return;
        }

        _playerMoveAction = playerMap.FindAction(MoveActionName);
        _arrowMoveAction = player1Map.FindAction(MoveActionName); // Naam in script was _arrowMoveAction
        
        if (_playerMoveAction == null || _arrowMoveAction == null)
        {
            Debug.LogError($"De actie '{MoveActionName}' is niet gevonden in beide Maps!");
            enabled = false;
            return;
        }

        // ⭐ GEWIJZIGD: Beide acties roepen nu dezelfde Handler aan.
        // De Handler bepaalt intern welke index gespawnd moet worden.
        _playerMoveAction.performed += OnMoveActivated;
        _arrowMoveAction.performed += OnMoveActivated;
    }

    private void OnEnable()
    {
        _playerMoveAction?.Enable();
        _arrowMoveAction?.Enable();
    }

    private void OnDisable()
    {
        _playerMoveAction?.Disable();
        _arrowMoveAction?.Disable();
    }
    
    private void OnDestroy()
    {
        if (_playerMoveAction != null) _playerMoveAction.performed -= OnMoveActivated;
        if (_arrowMoveAction != null) _arrowMoveAction.performed -= OnMoveActivated;
    }


    /// <summary>
    /// Handler voor BEIDE Player Move Actions.
    /// Deze functie bepaalt aan de hand van het triggering device welke index gespawnd moet worden.
    /// </summary>
    private void OnMoveActivated(InputAction.CallbackContext context)
    {
        Vector2 movement = context.ReadValue<Vector2>();

        // We willen alleen een actie triggeren als er daadwerkelijke beweging is (dit filtert 0,0 input)
        if (movement.sqrMagnitude < 0.01f)
        {
            return;
        }

        InputDevice triggeringDevice = context.control.device;

        // 1. Controleer of dit apparaat al een index heeft gespawnd
        if (_deviceIndexMap.ContainsKey(triggeringDevice))
        {
            // Apparaat heeft al gespawnd. Negeer deze aanroep.
            // Dit zorgt ervoor dat een apparaat Index 0 niet twee keer kan spawnen.
            return;
        }

        // 2. Bepaal de target index op basis van de Action Map die getriggerd is
        int targetIndex = -1;
        string mapName = context.action.actionMap.name;

        if (mapName == "Player")
        {
            targetIndex = 0; // Player Map (WASD / Controller 1) -> Index 0
        }
        else if (mapName == "Player1")
        {
            targetIndex = Player1Index; // Player1 Map (Arrows / Controller 2) -> Index 1
        }
        
        // 3. Spawnen en apparaat vastleggen
        if (targetIndex != -1)
        {
            // Spawnen alleen als de Index geldig is en de displayManager bestaat.
            if (targetIndex < displayManager.displayEntries.Length)
            {
                Debug.Log($"Setup (Index {targetIndex}) geactiveerd door Map '{mapName}' en device '{triggeringDevice.name}'.");
                displayManager.SetupDisplayByIndex(targetIndex);
                
                // ⭐ BELANGRIJK: Leg dit apparaat vast zodat het niet opnieuw kan triggeren
                _deviceIndexMap.Add(triggeringDevice, targetIndex);
            }
            else
            {
                Debug.LogError($"Index {targetIndex} is groter dan de array grootte van UI3DObjectManager.");
            }
        }
    }
}