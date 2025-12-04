using UnityEngine;
using UnityEngine.Rendering;

public class PickupPowerupBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = other.gameObject.GetComponentInParent<PlayerInventory>();

           // if(inventory != null) 
            inventory.EmptyItemSlot();
            Destroy(gameObject);
        }
    }
}
