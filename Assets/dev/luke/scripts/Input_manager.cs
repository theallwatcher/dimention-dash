using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum PlayerType { Player1, Player2 }

public class Input_Manager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private Image itemSlot1, itemSlot2;

    [Header("Player Materials")]
    [SerializeField] private Material player1Material;
    [SerializeField] private Material player2Material;

    bool wasdJoined = false;
    bool arrowsJoined = false;
    bool gamepadJoined = false;

    private PlayerMovement playerOneMovement;
    private PlayerMovement playerTwoMovement;

    private PlayerInventory playerOneInventory;
    private PlayerInventory playerTwoInventory;

    GameManager gameManager;
    [SerializeField] private UI3DObjectManager ui3DManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        // Spawn on WASD input
        if (!wasdJoined && Keyboard.current != null)
        {
            bool wasdPressed = Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame ||
                               Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame;
            if (wasdPressed)
            {
                var player = PlayerInput.Instantiate(playerPrefab,
                    controlScheme: "WASD",
                    pairWithDevice: Keyboard.current);

                //link ui image
                playerOneInventory = player.GetComponent<PlayerInventory>();
                playerOneMovement = player.GetComponent<PlayerMovement>();

                if (playerOneInventory != null)
                {
                    playerOneInventory.itemSlotImage = itemSlot1;
                }

                // Disable movement script
                if (playerOneMovement != null)
                    playerOneMovement.enabled = false;

                // Disable gravity
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.useGravity = false;

                // Apply material
                if (player1Material != null)
                {
                    Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                        renderer.material = player1Material;
                }

                player.transform.position = spawnPoints[0].position;
                wasdJoined = true;

                // Setup 3D display for this player
                if (ui3DManager != null)
                    ui3DManager.SetupDisplayForPlayer(player.gameObject, 0);
            }
        }

        // Spawn on Arrow keys input
        if (!arrowsJoined && Keyboard.current != null)
        {
            bool arrowsPressed = Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame ||
                                 Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame;
            if (arrowsPressed)
            {
                var player2 = PlayerInput.Instantiate(playerPrefab,
                    controlScheme: "Arrows",
                    pairWithDevice: Keyboard.current);

                //link ui image
                playerTwoInventory = player2.GetComponent<PlayerInventory>();
                playerTwoMovement = player2.GetComponent<PlayerMovement>();

                if (playerTwoInventory != null)
                {
                    playerTwoInventory.itemSlotImage = itemSlot2;
                }

                // Disable movement script
                if (playerTwoMovement != null)
                    playerTwoMovement.enabled = false;

                // Disable gravity
                Rigidbody rb2 = player2.GetComponent<Rigidbody>();
                if (rb2 != null)
                    rb2.useGravity = false;

                // Apply material
                if (player2Material != null)
                {
                    Renderer[] renderers = player2.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                        renderer.material = player2Material;
                }

                player2.transform.position = spawnPoints[1].position;
                arrowsJoined = true;

                // Setup 3D display for player 2
                if (ui3DManager != null)
                    ui3DManager.SetupDisplayForPlayer(player2.gameObject, 1);
            }
        }

        // Spawn on Gamepad input
        if (!gamepadJoined)
        {
            foreach (var gp in Gamepad.all)
            {
                if (gp == null) continue;

                bool input = gp.leftStick.ReadValue().sqrMagnitude > 0.1f ||
                             gp.dpad.ReadValue().sqrMagnitude > 0.1f ||
                             gp.buttonSouth.wasPressedThisFrame;

                if (!input) continue;

                int index = !wasdJoined && !arrowsJoined ? 0 : (!arrowsJoined ? 1 : -1);
                if (index == 0)
                {
                    var player = PlayerInput.Instantiate(playerPrefab,
                        controlScheme: "Gamepad",
                        pairWithDevice: gp);

                    playerOneInventory = player.GetComponent<PlayerInventory>();
                    playerOneMovement = player.GetComponent<PlayerMovement>();

                    if (playerOneInventory != null)
                    {
                        playerOneInventory.itemSlotImage = itemSlot1;
                    }

                    // Disable movement script
                    if (playerOneMovement != null)
                        playerOneMovement.enabled = false;

                    // Disable gravity
                    Rigidbody rbGp = player.GetComponent<Rigidbody>();
                    if (rbGp != null)
                        rbGp.useGravity = false;

                    // Apply material
                    if (player1Material != null)
                    {
                        Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in renderers)
                            renderer.material = player1Material;
                    }

                    player.transform.position = spawnPoints[0].position;

                    // Setup 3D display for gamepad player 1
                    if (ui3DManager != null)
                        ui3DManager.SetupDisplayForPlayer(player.gameObject, 0);
                }
                else if (index == 1)
                {
                    var player2 = PlayerInput.Instantiate(playerPrefab,
                        controlScheme: "Gamepad",
                        pairWithDevice: gp);

                    playerTwoInventory = player2.GetComponent<PlayerInventory>();
                    playerTwoMovement = player2.GetComponent<PlayerMovement>();

                    if (playerTwoInventory != null)
                    {
                        playerTwoInventory.itemSlotImage = itemSlot2;
                    }

                    // Disable movement script
                    if (playerTwoMovement != null)
                        playerTwoMovement.enabled = false;

                    // Disable gravity
                    Rigidbody rbGp2 = player2.GetComponent<Rigidbody>();
                    if (rbGp2 != null)
                        rbGp2.useGravity = false;

                    // Apply material
                    if (player2Material != null)
                    {
                        Renderer[] renderers = player2.GetComponentsInChildren<Renderer>();
                        foreach (Renderer renderer in renderers)
                            renderer.material = player2Material;
                    }

                    player2.transform.position = spawnPoints[1].position;

                    // Setup 3D display for gamepad player 2
                    if (ui3DManager != null)
                        ui3DManager.SetupDisplayForPlayer(player2.gameObject, 1);
                }

                gamepadJoined = true;
                break;
            }
        }

        // Update positions if both players exist
        if (playerOneInventory != null && playerTwoInventory != null)
        {
            Transform pos1 = playerOneInventory.transform;
            Transform pos2 = playerTwoInventory.transform;

            gameManager.SetupPlayersPositions(pos1, pos2);

            //set links to each player script
            playerOneInventory.SetOtherMoveScript(playerTwoMovement);
            playerTwoInventory.SetOtherMoveScript(playerOneMovement);
            gameManager.SetPlayersInventory(playerOneInventory, playerTwoInventory);
        }
    }

}
