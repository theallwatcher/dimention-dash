using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<ItemObject> itemObjects = new List<ItemObject>();

    [Header("In scene references")]
    public Image itemSlotImage = null;

    [Header("Script links")]
    private PlayerMovement movementScript;
    private PlayerMovement apponentMovementScript;

    [SerializeField] private Transform feetPosition;
    [SerializeField] private Transform powerupSpawnPoint;

    [SerializeField] private GameObject shieldObject;
    private Coroutine shieldRoutine;
    private ItemObject currentPowerup = null;
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
        //currentPowerup = itemObjects[0];
    }

    public void SetPosition(PlayerPosition position)
    {
        currentPosition = position;
    }
    public void PickupRandomItem()
    {
        if (itemSlotImage != null && currentPowerup != null)
        {
            itemSlotImage.sprite = null;
            currentPowerup = null;
        }

        //stop routine if already running
        if(scrollRoutine != null)
        {
            StopCoroutine(scrollRoutine);
        }

        //spawn powerups based on the position of player
        //[leader gets less chance of good powerup]


        scrollRoutine = StartCoroutine(ScrollThroughItems(0));
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
                    bombScript.Constructor(powerupSpawnPoint, apponentMovementScript.transform, apponentMovementScript.transform.gameObject);
                }

                break;




            case ItemObject.ItemType.Spikes:
                //make a new transform to spawn the spike prefab
                GameObject spawn = new GameObject("Spike SpawnPoint");

                //move position to other player and move forwards
                spawn.transform.position = apponentMovementScript.transform.position + new Vector3(0, 0, 10); 
                spawnedObject = Instantiate(currentPowerup.Prefab, spawn.transform.position, Quaternion.identity);
                break;




            case ItemObject.ItemType.LaneSwitch:

                apponentMovementScript.ActivateSwitchLanePowerup();
                break;




            case ItemObject.ItemType.InvertControls:

                apponentMovementScript.ActivateInvertControls(5);
                break;



            case ItemObject.ItemType.Shield:

                if(shieldRoutine != null)
                {
                    StopCoroutine(shieldRoutine);
                }

                StartCoroutine(EnableShield(5));
                break;




            case ItemObject.ItemType.PositionSwitch:

               // spawnedObject = Instantiate();
                break;
        }
    }

    private IEnumerator ScrollThroughItems(int finalIndex)
    {
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
    } 

    public void SetOtherMoveScript(PlayerMovement movement)
    {
        apponentMovementScript = movement;
    }

    IEnumerator EnableShield(float t)
    {
        shieldObject.SetActive(true);
        yield return new WaitForSeconds(t);

        shieldObject.SetActive(false);
    }
}
