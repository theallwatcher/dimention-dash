using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class playerKiller : MonoBehaviour
{
    public PlayerInput playerInputObj;

    private void OnCollisionEnter(Collision collision){

        if (collision.gameObject.tag == "Killer"){

            Debug.Log("ded");

            string loseString = "loser" + playerInputObj.playerIndex;
            SceneManager.LoadScene(loseString);

        }
    }
}
