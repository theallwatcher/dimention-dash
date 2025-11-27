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

        portals1[portal1Random].tag = "Untagged";
        portals2[portal2Random].tag = "Untagged";

        Vector3 portalPosition1 = new Vector3(portals1[portal1Random].transform.position.x, portals1[portal1Random].transform.position.y + 5, portals1[portal1Random].transform.position.z);
        Vector3 portalPosition2 = new Vector3(portals2[portal2Random].transform.position.x, portals2[portal2Random].transform.position.y + 5, portals2[portal2Random].transform.position.z);


        GameObject indicator1 = Instantiate(corectIndicatorPrefab, portalPosition1, portals1[portal1Random].transform.rotation, portals1[portal1Random].transform);
        GameObject indicator2 = Instantiate(corectIndicatorPrefab, portalPosition2, portals2[portal2Random].transform.rotation, portals2[portal2Random].transform);

    }

}
