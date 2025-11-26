using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private Image itemSlot1, itemSlot2;
    [SerializeField] private TextMeshProUGUI coinCounter1, coinCounter2;
    bool wasdJoined = false;
    bool arrowsJoined = false;
    bool gamepadJoined = false;

    private PlayerMovement playerOneMovement;
    private PlayerMovement playerTwoMovement;

    private PlayerInventory playerOneInventory;
    private PlayerInventory playerTwoInventory;

    private coinCounter playerOneCoinCounter;
    private coinCounter playerTwoCoinCounter;

    GameManager gameManager;
    private void Start()
    {
        if (Keyboard.current == null) return;

        //if theres a controller connected 
        if (Gamepad.all.Count > 0)
        {
            var player = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: Gamepad.all[0]);

            SetupUIElements(player, true);

            player.transform.position = spawnPoints[0].position;
        }
        else
        {
            var player = PlayerInput.Instantiate(playerPrefab,
                controlScheme: "WASD",
                pairWithDevice: Keyboard.current);

            SetupUIElements(player, true);


            player.transform.position = spawnPoints[0].position;


        }

        if (Gamepad.all.Count > 1)
        {
            var player2 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: Gamepad.all[1]);

            SetupUIElements(player2, false);


            player2.transform.position = spawnPoints[1].position;
        }
        else
        {
            var player2 = PlayerInput.Instantiate(playerPrefab,
                controlScheme: "Arrows",
                pairWithDevice: Keyboard.current);

            SetupUIElements(player2, false);


            player2.transform.position = spawnPoints[1].position;
        }

        //set links to each player script
       // playerOneInventory.SetOtherMoveScript(playerTwoMovement);
        //playerTwoInventory.SetOtherMoveScript(playerOneMovement);

        gameManager = GameManager.Instance;
        gameManager.SetPlayersInventory(playerOneInventory, playerTwoInventory);
    }

    private void Update()
    {

        Transform pos1 = playerOneInventory.transform;
        Transform pos2 = playerTwoInventory.transform;

        gameManager.SetupPlayersPositions(pos1, pos2);
    }

    private void SetupUIElements(PlayerInput player, bool isPlayerOne)
    {
        var inventory = player.GetComponent<PlayerInventory>();
        var movement = player.GetComponent<PlayerMovement>();
        var counter = player.GetComponent<coinCounter>();

        if (isPlayerOne)
        {
            playerOneInventory = inventory;
            playerOneMovement = movement;
            playerOneCoinCounter = counter;

            inventory.itemSlotImage = itemSlot1;
            counter.counter = coinCounter1;
        }
        else
        {
            playerTwoInventory = inventory;
            playerTwoMovement = movement;
            playerTwoCoinCounter = counter;

            inventory.itemSlotImage = itemSlot2;
            counter.counter = coinCounter2;
        }
    }
}
