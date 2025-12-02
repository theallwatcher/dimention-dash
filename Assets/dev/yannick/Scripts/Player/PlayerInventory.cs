using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<ItemObject> itemObjects = new List<ItemObject>();

    [Header("In scene references")]
    public Image itemSlotImage = null;
    private TextMeshProUGUI itemName;

    [Header("Script links")]
    private PlayerMovement movementScript;
    private PlayerMovement opponentMovementScript;

    [SerializeField] private Transform feetPosition;
    [SerializeField] private Transform powerupSpawnPoint;

    [SerializeField] private GameObject shieldObject;
    private Coroutine shieldRoutine;
    public bool hasShield = false;
    [SerializeField] private ItemObject currentPowerup;
    private Coroutine scrollRoutine = null;

    public enum PlayerPosition 
    {
        FirstPlace,
        Tie,
        LastPlace
    }

    public PlayerPosition currentPosition;

    private void Start()
    {
        movementScript = GetComponent<PlayerMovement>();     
        
        itemName = itemSlotImage.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if(itemName  != null)
        itemName.text = " ";
    }

    private void Update()
    {
        if (shieldObject.gameObject.activeInHierarchy)
        {
            hasShield = true;
        }
        else
        {
            hasShield = false;  
        }
    }

    public void SetPosition(PlayerPosition position)
    {
        currentPosition = position;
    }
    public void PickupRandomItem()
    {
        //empty current item in slot
        if (itemSlotImage != null && currentPowerup != null)
        {
            itemSlotImage.sprite = null;
            currentPowerup = null;
            itemName.text = " ";
        }

        //stop routine if already running
        if(scrollRoutine != null)
        {
            StopCoroutine(scrollRoutine);
        }
        Debug.Log("randomize");

        RandomizeItems();
    }

    public void UsePowerup()
    {
        //set powerup spawnPoint 
        if (currentPowerup == null) return;

        SpawnPowerup(currentPowerup.Type);
        itemSlotImage.sprite = null;
        currentPowerup = null;
    }

    private void SpawnPowerup(ItemObject.ItemType type) //every powerup type spawns in a different way
    {
        GameObject spawnedObject = null;

        switch (type)
        {
            case ItemObject.ItemType.Boost:

                powerupSpawnPoint.position = feetPosition.position + new Vector3(0, 0, 10);
                spawnedObject = Instantiate(currentPowerup.Prefab, powerupSpawnPoint.position, Quaternion.identity);

                break;

            case ItemObject.ItemType.Bomb:

                spawnedObject = Instantiate(currentPowerup.Prefab);
                BombPowerup bombScript = spawnedObject.GetComponentInChildren<BombPowerup>();

                if(bombScript != null)
                {
                    bombScript.Constructor(powerupSpawnPoint, opponentMovementScript.transform, opponentMovementScript.transform.gameObject);
                }

                break;




            case ItemObject.ItemType.Spikes:
                //make a new transform to spawn the spike prefab
                GameObject spawn = new GameObject("Spike SpawnPoint");

                //move position to other player and move forwards
                spawn.transform.position = opponentMovementScript.transform.position + new Vector3(0, 0, 10); 
                spawnedObject = Instantiate(currentPowerup.Prefab, spawn.transform.position, Quaternion.identity);
                break;




            case ItemObject.ItemType.LaneSwitch:

                opponentMovementScript.ForceSwitchLane();
                break;




            case ItemObject.ItemType.InvertControls:

                PowerupManager opponentPowerupManager = opponentMovementScript.GetComponent<PowerupManager>();
                opponentPowerupManager.ActivateInvertControls();
                break;



            case ItemObject.ItemType.Shield:

                if(shieldRoutine != null && !hasShield)
                {
                    StopCoroutine(shieldRoutine);
                }

                StartCoroutine(EnableShield(5));
                break;




            case ItemObject.ItemType.PositionSwitch:

                opponentMovementScript.SmoothSwapZ(movementScript); 
                break;

            case ItemObject.ItemType.Coins:

                coinCounter coinScript = GetComponent<coinCounter>();
                coinScript.AddCoins();
                break;
        }
    }

    private IEnumerator ScrollThroughItems(int finalIndex)
    {
        Debug.Log("scroll");

        float interval = 0.03f;
        float maxInterval = 0.5f;
        float acceleration = 1.12f;
        //start at a random entry in the array
        int index = Random.Range(0, itemObjects.Count-1);

        
        while (interval < maxInterval)
        {
            index++;
            //reset when overflowing
            if (index > itemObjects.Count -1)
                index = 0;

            //switch item sprite
            itemSlotImage.sprite = itemObjects[index].UI_sprite;
            yield return new WaitForSeconds(interval);

            interval *= acceleration;
        }

        //when scroll is over pass in the final items
        itemSlotImage.sprite = itemObjects[finalIndex].UI_sprite ;
        currentPowerup = itemObjects[finalIndex];
        itemName.text = itemObjects[finalIndex].Type.ToString();
    } 

    public void SetOtherMoveScript(PlayerMovement movement)
    {
        opponentMovementScript = movement;
    }

    IEnumerator EnableShield(float t)
    {
        shieldObject.SetActive(true);
        hasShield = true;
        yield return new WaitForSeconds(t);
        hasShield = false;
        shieldObject.SetActive(false);
    }

    public void DestroyShield()
    {
        shieldObject.SetActive(false);
        hasShield = false;
    }

    private void RandomizeItems()
    {
        float roll = Random.Range(0, 100);

         #region FirstPlace
        if (currentPosition == PlayerPosition.FirstPlace)
        {
            if (roll < 12f)     //Bomb [12%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(0)); 
            }
            else if (roll < 0)  //Boost [0%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(1)); 
            }
            else if (roll >= 12f && roll < 19f) //Invert controls [7%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(2)); 
            }
            else if (roll >= 19f && roll < 31.5)//Lane switch [12.5%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(3)); 
            }
            else if (roll >= 31.5f && roll < 51.5f)//Pos switch [20%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(4)); 
            }
            else if (roll >= 51.5f && roll < 57.5f) //Shield [6%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(5)); 
            }
            else if (roll >= 57.5f && roll < 70) //Spikes [12.5%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(6)); 
            }
            else if (roll >= 70f && roll <= 100) //Coins [30%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(7)); 
            }
        }
        #endregion
        #region Tie
        if (currentPosition == PlayerPosition.Tie) //when player is tied with opponent all spawn chances are equal
        {
            if (roll < 12f)     //Bomb [12%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(0));
            }
            else if (roll >= 12 && roll < 22)  //Boost [10%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(1));
            }
            else if (roll >= 22f && roll < 29f) //Invert controls [7%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(2));
            }
            else if (roll >= 29f && roll < 29)//Lane switch [0%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(3));
            }
            else if (roll >= 29f && roll < 49f)//Pos switch [20%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(4));
            }
            else if (roll >= 55f && roll < 61f) //Shield [6%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(5));
            }
            else if (roll >= 63.5f && roll < 86) //Spikes [12.5%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(6));
            }
            else if (roll >= 86f && roll <= 100) //Coins [30%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(7));
            }
        }
        #endregion
         #region Last
        if (currentPosition == PlayerPosition.LastPlace)
        {
            if (roll < 12f)     //Bomb [12%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(0));
            }
            else if (roll < 0)  //Boost [0%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(1));
            }
            else if (roll >= 12f && roll < 19f) //Invert controls [7%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(2));
            }
            else if (roll >= 19f && roll < 31.5)//Lane switch [12.5%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(3));
            }
            else if (roll >= 31.5f && roll < 51.5f)//Pos switch [20%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(4));
            }
            else if (roll >= 51.5f && roll < 57.5f) //Shield [6%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(5));
            }
            else if (roll >= 57.5f && roll < 70) //Spikes [12.5%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(6));
            }
            else if (roll >= 70f && roll <= 100) //Coins [30%]
            {
                scrollRoutine = StartCoroutine(ScrollThroughItems(7));
            }
        }
        #endregion
    }
}
