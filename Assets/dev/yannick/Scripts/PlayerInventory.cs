using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<ItemObject> itemObjects = new List<ItemObject>();

    [Header("In scene references")]
    [SerializeField] private Image itemSlotImage = null;
    [SerializeField] PlayerMovement otherPlayer;
    [SerializeField] PlayerMovement thisMovement;
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
        thisMovement = GetComponent<PlayerMovement>();
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

        if (currentPosition == PlayerPosition.FirstPlace)
        {
            //only spawn nurfed powerups   [index 0 to 2]
            scrollRoutine = StartCoroutine(ScrollThroughItems(Random.Range(0, 2)));
        }
        if(currentPosition == PlayerPosition.LastPlace)
        {
            //spawn helpfull powerups      [index 3, 4]
            scrollRoutine = StartCoroutine(ScrollThroughItems(Random.Range(3, 4)));
        }
        else if (currentPosition == PlayerPosition.Tie) 
        {
            //spawn any of the powerups
            scrollRoutine = StartCoroutine(ScrollThroughItems(Random.Range(0, itemObjects.Count)));
        }
    }

    public void UsePowerup()
    {
        //set powerup spawnPoint 

        SpawnPowerup(currentPowerup.Type);
        itemSlotImage.sprite = null;
        currentPowerup = null;
    }

    private void SpawnPowerup(ItemObject.ItemType type)
    {
        GameObject spawnedObject = null;

        switch (type)
        {
            case ItemObject.ItemType.Boost:
                
                

                break;

            case ItemObject.ItemType.Bomb:

                spawnedObject = Instantiate(spawnedObject);
                break;




            case ItemObject.ItemType.Spikes:

                spawnedObject = Instantiate(spawnedObject);
                break;




            case ItemObject.ItemType.LaneSwitch:

                spawnedObject = Instantiate(spawnedObject);
                break;




            case ItemObject.ItemType.InvertControls:

                spawnedObject = Instantiate(spawnedObject);
                break;



            case ItemObject.ItemType.Shield:

                spawnedObject = Instantiate(spawnedObject);
                break;




            case ItemObject.ItemType.PositionSwitch:

                spawnedObject = Instantiate(spawnedObject);
                break;
        }
    }

    private IEnumerator ScrollThroughItems(int finalIndex)
    {
        float interval = 0.01f;

        //start at a random entry in the array
        int index = Random.Range(0, itemObjects.Count-1);

        
        while (interval < 0.6f)
        {
            interval += 0.05f;
            index++;
            //reset when overflowing
            if (index > itemObjects.Count -1)
                index = 0;

            //switch item sprite
            itemSlotImage.sprite = itemObjects[index].UI_sprite;
            yield return new WaitForSeconds(interval);
        }

        //when scroll is over pass in the final items
        itemSlotImage.sprite = itemObjects[finalIndex].UI_sprite ;
        currentPowerup = itemObjects[finalIndex];
    }
}
