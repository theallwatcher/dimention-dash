using UnityEngine;
using UnityEngine.Rendering;

public class PickupPowerupBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("box");
            PlayerInventory inventory = other.gameObject.GetComponent<PlayerInventory>();

           // if(inventory != null) 
            inventory.PickupRandomItem();
            GameObject parent = gameObject.transform.parent.gameObject;
            Destroy(parent);
        }
    }
}
