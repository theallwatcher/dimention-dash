using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab1, playerPrefab2;
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

    private bool player1Spawned = false;
    private bool player2Spawned = false;
    private bool bothJoined = false;

    //checks if players are using keyboard or controllers
    private bool usingKeys1 = false;
    private bool usingKeys2 = false;

    private Transform p1Pos;
    private Transform p2Pos;

    [SerializeField] private GameObject countDownUI
        ;
    private void Start()
    {
        if (Keyboard.current == null) return;

        //if theres a controller connected 
        if (Gamepad.all.Count > 0)
        {
            var player = PlayerInput.Instantiate(playerPrefab1,
            controlScheme: "Gamepad",
            pairWithDevice: Gamepad.all[0]);

            SetupUIElements(player, true);

            player.transform.position = spawnPoints[0].position;
            player1Spawned = true;
        }
        else
        {
            var player = PlayerInput.Instantiate(playerPrefab1,
                controlScheme: "WASD",
                pairWithDevice: Keyboard.current);

            SetupUIElements(player, true);


            player.transform.position = spawnPoints[0].position;
            player1Spawned = true;
            usingKeys1 = true;
        }

        if (Gamepad.all.Count > 1)
        {
            var player2 = PlayerInput.Instantiate(playerPrefab2,
            controlScheme: "Gamepad",
            pairWithDevice: Gamepad.all[1]);

            SetupUIElements(player2, false);


            player2.transform.position = spawnPoints[1].position;
            player2Spawned = true;
        }
        else
        {
            var player2 = PlayerInput.Instantiate(playerPrefab2,
                controlScheme: "Arrows",
                pairWithDevice: Keyboard.current);

            SetupUIElements(player2, false);


            player2.transform.position = spawnPoints[1].position;
            player2Spawned = true;
            usingKeys2 = true;
        }

        //set links to each player script
        playerOneInventory.SetOtherMoveScript(playerTwoMovement);
        playerTwoInventory.SetOtherMoveScript(playerOneMovement);

        gameManager = GameManager.Instance;
        gameManager.SetPlayersInventory(playerOneInventory, playerTwoInventory);
    }

    private void Update()
    {

         p1Pos = playerOneInventory.transform;
         p2Pos = playerTwoInventory.transform;

        gameManager.SetupPlayersPositions(p1Pos, p2Pos);

        if(player1Spawned && player2Spawned && !bothJoined)
        {
            if(countDownUI != null)
            {
                ShowControls showControlScript = countDownUI.GetComponentInChildren<ShowControls>();
                showControlScript.SetupPlayerControls(usingKeys1, usingKeys2);
                bothJoined = true;
            }
        }

        //when entering a new scene update the players
        if (!gameManager.PlayersFound())
        {
            Debug.Log("Added players");
            gameManager.SetPlayersInventory(playerOneInventory, playerTwoInventory);
            gameManager.SetupPlayersPositions(p1Pos, p2Pos);
        }
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

    public void SetupPlayers()
    {
        Debug.Log("Added players");
        gameManager.SetPlayersInventory(playerOneInventory, playerTwoInventory);
        gameManager.SetupPlayersPositions(p1Pos, p2Pos);
    }
}
