using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<ItemObject> itemImages = new List<ItemObject>();
    [SerializeField] private Image itemSlotImage = null;
    private bool _isLeader;
    public void IsLeader(bool isLeader)
    {
        _isLeader = isLeader;
    }
    public void PickupRandomItem()
    {
        if (_isLeader)
        {

        }
        else
        {

        }
    }
}
