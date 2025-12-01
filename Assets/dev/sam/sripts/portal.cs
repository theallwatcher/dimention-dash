using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class portal : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = -500;
    public int counter;
    public int hitCount = 2;

    [SerializeField] List<GameObject> portals1 = new List<GameObject>();
    [SerializeField] List<GameObject> portals2 = new List<GameObject>();
    [SerializeField] GameObject corectIndicatorPrefab;
    [SerializeField] string nextSceneName;
    
    void Start(){
        
        rb = GetComponent<Rigidbody>();
        RandomizePortals();
        rb.AddForce(new Vector3(0, 0, speed));
    }

    // Update is called once per frame
    void Update(){

        /*if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {

            rb.AddForce(new Vector3(0, 0, speed));
        }*/

        if (counter >= hitCount)
        {

            SceneManager.LoadScene(nextSceneName);

        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {

            Debug.Log("portal hit 1");
            counter++;
        }
    }

    private void RandomizePortals()
    {

        int portal1Random = Random.Range(0, portals1.Count);
        int portal2Random = Random.Range(0, portals2.Count);

        //portals1[portal1Random].tag = "Untagged";
        //portals2[portal2Random].tag = "Untagged";

       


        GameObject indicator1 = Instantiate(corectIndicatorPrefab, portals1[portal1Random].transform.position, portals1[portal1Random].transform.rotation,this.transform);
        GameObject indicator2 = Instantiate(corectIndicatorPrefab, portals2[portal2Random].transform.position, portals2[portal2Random].transform.rotation,this.transform);
        Destroy(portals1[portal1Random]);
        Destroy(portals2[portal2Random]);

    }

}
