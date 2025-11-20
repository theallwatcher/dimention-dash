using UnityEngine;

public class coinCounter : MonoBehaviour
{
    public int coins;

    private void OnTriggerEnter(Collider other){

        if (other.gameObject.tag == "Coin"){

            coins++;
        }
    }
}
