// Voorbeeld van een ander C# script (b.v. PlayerController.cs)
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            // BELANGRIJK: Roep de Play methode aan via AudioManager.Instance
            AudioManager.Instance.Play("CoinPickup"); // Dit speelt het geluid met de naam "CoinPickup" af.

            Destroy(collision.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Een ander voorbeeld voor een spronggeluid
            AudioManager.Instance.Play("JumpSound");
        }
    }
}