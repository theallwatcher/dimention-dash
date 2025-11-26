using UnityEngine;

public class buildingmover : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = 1;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    void Start(){
        rb = GetComponent<Rigidbody>();
       // rb.AddForce (new Vector3 (0, 0, speed));
    }

    // Update is called once per frame
    void Update(){

        //roadspeed gets updated in gameManager singleton
        transform.position -= new Vector3(0, 0, GameManager.Instance.roadSpeed) * Time.deltaTime;

        if (gameObject.transform.position.z < endPoint.position.z) { 
        
            gameObject.transform.position = startPoint.position;
        }
        
    }
}
