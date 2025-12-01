using System.Collections;
using UnityEngine;

public class ShowControls : MonoBehaviour
{
    [SerializeField] GameObject player1Keyboard, player1Controller;
    [SerializeField] GameObject player2Keyboard, player2Controller;

    public void SetupPlayerControls(bool usingKeys1, bool usingKeys2) //checks if players are using keyboards or controller
    {
        DisableAll();

        //player 1 setup
        if (usingKeys1) player1Keyboard.SetActive(true);
        else player1Controller.SetActive(true);

        //player 2 setup
        if (usingKeys2) player2Keyboard.SetActive(true);
        else player2Controller.SetActive(true);
    }
    
    public void DisableAll()
    {
            player1Keyboard.SetActive(false);
            player1Controller.SetActive(false);
            player2Keyboard.SetActive(false);
            player2Controller.SetActive(false);
    }
}
