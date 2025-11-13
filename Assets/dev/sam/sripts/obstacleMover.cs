using UnityEngine;

public class obstacleMover : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = -500;
    [SerializeField] Transform endPoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){

        rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(0, 0, speed));
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.z < endPoint.position.z)
        {

            Destroy(gameObject);
        }
    }
}
