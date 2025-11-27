using System.Collections.Generic;
using UnityEngine;

public class MoveRoad : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //roadspeed gets updated in gameManager singleton
        transform.position -= new Vector3(0, 0, GameManager.Instance.roadSpeed) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}
