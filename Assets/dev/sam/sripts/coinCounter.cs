using UnityEngine;

public class coinCounter : MonoBehaviour
{
    int coins;

    private void OnTriggerEnter(Collider other){

        if (other.gameObject.tag == "Coin"){

            coins++;
        }
    }
}
