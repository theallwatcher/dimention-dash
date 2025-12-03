using UnityEngine;
using TMPro;
public class coinCounter : MonoBehaviour
{
    public int coins;
    public TextMeshProUGUI counter;
    [SerializeField] GameObject coinPartical;

    private void Update()
    {
        counter.text = "Coins: " + coins;
    }

    private void OnTriggerEnter(Collider other){

        if (other.gameObject.tag == "Coin"){
            GameObject partical = Instantiate(coinPartical, transform.position, transform.rotation);
            coins++;
        }
    }

    public void AddCoins(){

        GameObject partical = Instantiate(coinPartical,transform.position, transform.rotation);
        coins += Random.Range(2, 5);
    }
}
