using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class winnerPortal : MonoBehaviour
{

    Rigidbody rb;
    [SerializeField] float speed = -500;
    [SerializeField] List<GameObject> portals1 = new List<GameObject>();
    [SerializeField] List<GameObject> portals2 = new List<GameObject>();
    [SerializeField] GameObject corectIndicatorPrefab;

    void Start(){

        rb = GetComponent<Rigidbody>();
        RandomizePortals();
        rb.AddForce(new Vector3(0, 0, speed));
    }

    private void RandomizePortals(){

        int portal1Random = Random.Range(0, portals1.Count);
        int portal2Random = Random.Range(0, portals2.Count);

        //portals1[portal1Random].tag = "Untagged";
        //portals2[portal2Random].tag = "Untagged";




        GameObject indicator1 = Instantiate(corectIndicatorPrefab, portals1[portal1Random].transform.position, portals1[portal1Random].transform.rotation, this.transform);
        GameObject indicator2 = Instantiate(corectIndicatorPrefab, portals2[portal2Random].transform.position, portals2[portal2Random].transform.rotation, this.transform);
        Destroy(portals1[portal1Random]);
        Destroy(portals2[portal2Random]);

    }

    private void OnTriggerEnter(Collider other){

        if (other.gameObject.tag == "Player") {

            PlayerInput inp = other.gameObject.GetComponentInParent<PlayerInput>();
            if (inp == null) {

                Debug.LogError("did not get ipm");
            }
            int winNum = inp.playerIndex;

            if (winNum == 0) {

                SceneManager.LoadScene("loser1");
            }
            else if (winNum == 1){

                SceneManager.LoadScene("loser0");
            }
        }
    }
}
