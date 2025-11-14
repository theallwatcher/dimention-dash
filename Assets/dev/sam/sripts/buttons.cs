using UnityEngine;
using UnityEngine.SceneManagement;

public class buttons : MonoBehaviour
{
    public void startLevel(){

        //Laad het huidige level in met de SceneManager.
        SceneManager.LoadScene("sam Test");
        Time.timeScale = 1;
    }

    public void menu(){

        //Laad het huidige level in met de SceneManager.
        SceneManager.LoadScene("start Scene");
        Time.timeScale = 1;
    }

    public void quit() {

        Application.Quit();
        Debug.Log("quit");
    
    }
}
