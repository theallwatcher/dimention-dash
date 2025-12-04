using System.Collections.Generic;
using UnityEngine;

public class MoveRoad : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    // Update is called once per frame
    void FixedUpdate()
    {
        //roadspeed gets updated in gameManager singleton
        rb.position -= new Vector3(0, 0, GameManager.Instance.roadSpeed) * Time.deltaTime;
       // transform.position -= v
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}
