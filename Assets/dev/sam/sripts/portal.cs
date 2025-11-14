using UnityEngine;
using UnityEngine.SceneManagement;

public class portal : MonoBehaviour
{

    public int counter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (counter == 2)
        {

            SceneManager.LoadScene("//next scene name");
                
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        counter++;
    }
}
