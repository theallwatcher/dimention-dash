using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class buttons : MonoBehaviour
{

    [SerializeField] private GameObject panel;

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

    public void CredditsOn() {

        panel.SetActive(true);

    }

    public void CredditsOff() { 
        
        panel.SetActive(false);
    }
}
