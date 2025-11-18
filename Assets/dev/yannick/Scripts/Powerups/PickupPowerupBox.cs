using UnityEngine;

public class PickupPowerupBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = other.gameObject.GetComponent<PlayerInventory>();
            inventory.PickupRandomItem();
        }
    }
}
