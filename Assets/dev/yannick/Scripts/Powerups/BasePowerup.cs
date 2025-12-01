using UnityEngine;

public abstract class BasePowerup : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Activate(other.GetComponent<PlayerMovement>());

            //add effect?

            Destroy(gameObject);
        }
    }

    protected virtual void Activate(PlayerMovement player)
    {
        //activate powerup
    }

    
}
