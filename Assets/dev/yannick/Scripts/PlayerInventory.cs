using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<Image> itemImages = new List<Image>();

    [SerializeField] private Image itemSlotImage = null;

    public void PickupRandomItem()
    {
        itemSlotImage = itemImages[Random.Range(0, itemImages.Count)];
    }
}
