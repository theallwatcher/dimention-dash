using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;

[System.Serializable]
public struct PlayerJoinInput
{
    public InputActionAsset asset;
    public string actionMap;
    public string actionName;
}

public class Input_manager : MonoBehaviour
{
    [Header("Prefabs / Spawn")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("UI")]
    [SerializeField] private Image itemSlot1, itemSlot2;

    [Header("Player Materials")]
    [SerializeField] private Material[] playerMaterials; // 0 = Player1, 1 = Player2

    [Header("Managers")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UI3DObjectManager ui3DManager;

    [Header("Control Schemes")]
    [SerializeField] private string player1KeyboardScheme = "WASD";
    [SerializeField] private string player2KeyboardScheme = "Arrows";
    [SerializeField] private string gamepadScheme = "Gamepad";

    [Header("Join Inputs")]
    [SerializeField] private PlayerJoinInput player1Input; // WASD
    [SerializeField] private PlayerJoinInput player2Input; // Arrows

    private InputAction player1JoinAction;
    private InputAction player2JoinAction;

    private PlayerInventory playerOneInventory;
    private PlayerInventory playerTwoInventory;
    private PlayerMovement playerOneMovement;
    private PlayerMovement playerTwoMovement;

    private bool playerOneJoined = false;
    private bool playerTwoJoined = false;
    private bool wasdJoined = false;
    private bool arrowsJoined = false;

    private void Start()
    {
        // Enable join actions
        player1JoinAction = GetAction(player1Input);
        player2JoinAction = GetAction(player2Input);

        player1JoinAction?.Enable();
        player2JoinAction?.Enable();
        
        // Register performed callbacks so joins respect bindings/control schemes at runtime
        if (player1JoinAction != null)
            player1JoinAction.performed += Player1JoinPerformed;

        if (player2JoinAction != null)
            player2JoinAction.performed += Player2JoinPerformed;
    }

    private void OnDestroy()
    {
        if (player1JoinAction != null)
            player1JoinAction.performed -= Player1JoinPerformed;

        if (player2JoinAction != null)
            player2JoinAction.performed -= Player2JoinPerformed;
    }

    private void Player1JoinPerformed(InputAction.CallbackContext ctx)
    {
        HandleJoin(ctx, 0, player1KeyboardScheme);
    }

    private void Player2JoinPerformed(InputAction.CallbackContext ctx)
    {
        HandleJoin(ctx, 1, player2KeyboardScheme);
    }

    private void HandleJoin(InputAction.CallbackContext ctx, int desiredIndex, string defaultScheme)
    {
        if (desiredIndex == 0 && playerOneJoined) return;
        if (desiredIndex == 1 && playerTwoJoined) return;

        int index = desiredIndex;
        string controlScheme = defaultScheme;

        // Try to infer control scheme from the binding groups (these are set in the InputAction bindings)
        try
        {
            // Some Input System versions don't expose bindingIndex on the callback context.
            // Find the binding by matching the control path.
            string controlPath = ctx.control != null ? ctx.control.path : null;
            var bindings = ctx.action.bindings;
            int foundIndex = -1;
            for (int i = 0; i < bindings.Count; i++)
            {
                var b = bindings[i];
                if (!string.IsNullOrEmpty(b.path) && !string.IsNullOrEmpty(controlPath) && b.path == controlPath)
                {
                    foundIndex = i;
                    break;
                }
            }

            if (foundIndex >= 0)
            {
                var groups = bindings[foundIndex].groups;
                if (!string.IsNullOrEmpty(groups))
                {
                    var parts = groups.Split(new[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < parts.Length; i++)
                    {
                        var g = parts[i].Trim();
                        if (string.Equals(g, player1KeyboardScheme, System.StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(g, player2KeyboardScheme, System.StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(g, gamepadScheme, System.StringComparison.OrdinalIgnoreCase))
                        {
                            controlScheme = g;
                            break;
                        }
                    }
                }
            }
        }
        catch { }

        // Fallback: infer from device type
        var device = ctx.control != null ? ctx.control.device : null;
        if (!string.IsNullOrEmpty(controlScheme) == false || controlScheme == defaultScheme)
        {
            if (device is Gamepad)
            {
                controlScheme = gamepadScheme;
            }
        }

        // Spawn player in the desired fixed slot (0 = player1, 1 = player2)
        SpawnPlayer(index, device, controlScheme);

        if (index == 0) playerOneJoined = true; else playerTwoJoined = true;
    }

    private InputAction GetAction(PlayerJoinInput joinInput)
    {
        if (joinInput.asset == null) return null;
        var map = joinInput.asset.FindActionMap(joinInput.actionMap);
        if (map == null) return null;
        return map.FindAction(joinInput.actionName);
    }

    private void Update()
    {
        if (playerOneInventory != null && playerTwoInventory != null)
        {
            gameManager?.SetupPlayersPositions(playerOneInventory.transform, playerTwoInventory.transform);
        }
    }

    private void HandleKeyboardJoin()
    {
        // WASD join
        if (Keyboard.current == null) return;

        // Detect WASD keys (spawn player 1)
        bool wasdPressed = Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame ||
                           Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame;

        if (wasdPressed && !playerOneJoined)
        {
            SpawnPlayer(0, Keyboard.current, player1KeyboardScheme);
            wasdJoined = true;
            playerOneJoined = true;
        }

        // Detect Arrow keys (spawn player 2)
        bool arrowsPressed = Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame ||
                             Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame;

        if (arrowsPressed && !playerTwoJoined)
        {
            SpawnPlayer(1, Keyboard.current, player2KeyboardScheme);
            arrowsJoined = true;
            playerTwoJoined = true;
        }
    }

    private void HandleGamepadJoin()
    {
        foreach (var gp in Gamepad.all)
        {
            if (gp == null) continue;

            bool input = gp.leftStick.ReadValue().sqrMagnitude > 0.1f ||
                         gp.dpad.ReadValue().sqrMagnitude > 0.1f ||
                         gp.buttonSouth.wasPressedThisFrame;

            if (!input) continue;

            int index = !playerOneJoined ? 0 : 1;

            SpawnPlayer(index, gp, gamepadScheme);

            if (!playerOneJoined) playerOneJoined = true;
            else if (!playerTwoJoined) playerTwoJoined = true;

            // Stop na spawn
            return;
        }
    }

    private void SpawnPlayer(int index, InputDevice device, string controlScheme)
    {
        if (playerPrefab == null || spawnPoints.Length <= index) return;

        var player = PlayerInput.Instantiate(playerPrefab,
            controlScheme: controlScheme,
            pairWithDevice: device);

        // Ensure the instantiated PlayerInput uses the requested control scheme
        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.SwitchCurrentControlScheme(controlScheme, device);
        }

        // Rigidbody fix
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.useGravity = false;

        // Zwarte kracht uit
        Renderer[] renderers = player.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderers)
        {
            r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false;
        }

        // Pas materiaal toe als aanwezig
        if (playerMaterials != null && playerMaterials.Length > index && playerMaterials[index] != null)
        {
            foreach (Renderer r in renderers)
                r.material = playerMaterials[index];
        }

        // Player scripts
        var inventory = player.GetComponent<PlayerInventory>();
        var movement = player.GetComponent<PlayerMovement>();

        if (inventory != null)
            inventory.itemSlotImage = index == 0 ? itemSlot1 : itemSlot2;

        if (index == 0)
        {
            playerOneInventory = inventory;
            playerOneMovement = movement;
        }
        else
        {
            playerTwoInventory = inventory;
            playerTwoMovement = movement;
        }

        // Spawn position
        player.transform.position = spawnPoints[index].position;

        // UI 3D display
        ui3DManager?.SetupDisplayForPlayer(player.gameObject, index);

        // Link beide spelers
        if (playerOneInventory != null && playerTwoInventory != null)
        {
            playerOneInventory.SetOtherMoveScript(playerTwoMovement);
            playerTwoInventory.SetOtherMoveScript(playerOneMovement);
        }

        // GameManager
        gameManager?.SetPlayersInventory(playerOneInventory, playerTwoInventory);
    }
}
