using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<ItemObject> itemObjects = new List<ItemObject>();
    [SerializeField] private Image itemSlotImage = null;

    private GameObject currentPowerup = null;
    private Coroutine scrollRoutine = null;

    private bool _isLeader;
    private bool _tie;
    public void IsLeader(bool isLeader)
    {
        _isLeader = isLeader;
    }
    public void PickupRandomItem()
    {
        if (itemSlotImage != null && currentPowerup != null)
        {
            itemSlotImage.sprite = null;
            currentPowerup = null;
        }

        if (_isLeader)
        {
            //only spawn nurfed powerups   [index 0 to 2]
            scrollRoutine = StartCoroutine(ScrollThroughItems(Random.Range(0, 2)));
        }
        if(!_isLeader && !_tie)
        {
            //spawn helpfull powerups      [index 3, 4]
            scrollRoutine = StartCoroutine(ScrollThroughItems(Random.Range(3, 4)));
        }
        else if (!_isLeader && _tie) 
        {
            //spawn any of the powerups
            scrollRoutine = StartCoroutine(ScrollThroughItems(Random.Range(0, itemObjects.Count)));
        }
    }

    public void UsePowerup()
    {
        Instantiate(currentPowerup);
        itemSlotImage.sprite = null;
        currentPowerup = null;
    }

    private IEnumerator ScrollThroughItems(int finalIndex)
    {
        float interval = 0.01f;

        //start at a random entry in the array
        int index = Random.Range(0, itemObjects.Count);

        
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
        currentPowerup = itemObjects[finalIndex].Prefab;
    }
}
