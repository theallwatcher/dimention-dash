using UnityEngine;
using UnityEngine.SceneManagement;

public class buttons : MonoBehaviour
{
    public void startLevel(){

        //Laad het huidige level in met de SceneManager.
        SceneManager.LoadScene("Main");
        Time.timeScale = 1;
    }

    public void menu(){

        //Laad het huidige level in met de SceneManager.
        SceneManager.LoadScene("start");
        Time.timeScale = 1;
    }

    public void quit() {

        Application.Quit();
        Debug.Log("quit");
    
    }
}
