using UnityEngine;

public class buildingmover : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = 1;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.AddForce (new Vector3 (0, 0, speed));
    }

    // Update is called once per frame
    void Update(){

        if (gameObject.transform.position.z < endPoint.position.z) { 
        
            gameObject.transform.position = startPoint.position;
        }
        
    }
}
