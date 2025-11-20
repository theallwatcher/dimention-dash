using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private Image itemSlot1, itemSlot2;

    bool wasdJoined = false;
    bool arrowsJoined = false;
    bool gamepadJoined = false;

    private void Start()
    {
        if (Keyboard.current == null) return;

        //if theres a controller connected 
        if (Gamepad.all.Count > 0)
        {
            var player = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: Gamepad.all[0]);

            //link ui image
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.itemSlotImage = itemSlot1;
            }

            player.transform.position = spawnPoints[0].position;
        }
        else
        {
            var player = PlayerInput.Instantiate(playerPrefab,
                controlScheme: "WASD",
                pairWithDevice: Keyboard.current);

            //link ui image
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.itemSlotImage = itemSlot1;
            }

            player.transform.position = spawnPoints[0].position;


        }

        if (Gamepad.all.Count > 1)
        {
            var player2 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: Gamepad.all[1]);

            //link ui image
            PlayerInventory inventory = player2.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.itemSlotImage = itemSlot2;
            }

            player2.transform.position = spawnPoints[1].position;
        }
        else
        {
            var player2 = PlayerInput.Instantiate(playerPrefab,
                controlScheme: "Arrows",
                pairWithDevice: Keyboard.current);

            //link ui image
            PlayerInventory inventory = player2.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.itemSlotImage = itemSlot2;
            }

            player2.transform.position = spawnPoints[1].position;

        }

    }


    //private void Update()
    //{
    //    if (Keyboard.current == null) return;

    //    if(!wasdJoined && Keyboard.current.spaceKey.wasPressedThisFrame)
    //    {
    //        var player = PlayerInput.Instantiate(playerPrefab,
    //            controlScheme: "WASD",
    //            pairWithDevice: Keyboard.current );

    //        if(spawnPoints.Length > 0)
    //        {
    //            player.transform.position = spawnPoints[0].position;
    //        }
    //        wasdJoined = true;
    //    }


    //    if (!arrowsJoined && Keyboard.current.rightShiftKey.wasPressedThisFrame)
    //    {
    //        var player = PlayerInput.Instantiate(playerPrefab,
    //            controlScheme: "Arrows",
    //            pairWithDevice: Keyboard.current);

    //        if (spawnPoints.Length > 0)
    //        {
    //            player.transform.position = spawnPoints[1].position;
    //        }
    //        arrowsJoined = true;
    //    }

    //    foreach(var gamepad in Gamepad.all)
    //    {
    //        if (gamepad.buttonSouth.wasPressedThisFrame)
    //        {
    //            PlayerInput.Instantiate(playerPrefab,
    //            controlScheme: "Gamepad",
    //            pairWithDevice: gamepad);
    //            gamepadJoined = true;

    //        }
    //    }


    //}

}
