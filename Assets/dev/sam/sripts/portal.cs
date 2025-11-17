using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class portal : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = -500;
    public int counter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update(){

        if (Keyboard.current.shiftKey.wasPressedThisFrame) {

            rb.AddForce(new Vector3(0, 0, speed));
        }

        if (counter == 2)
        {

            SceneManager.LoadScene("//next scene name");
                
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "player") {
            Debug.Log("portal hit 1");
            counter++;
        }
    }
}
