using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class portalBreaker : MonoBehaviour
{

    BoxCollider Box;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Box = GetComponent<BoxCollider>(); 
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {

           Box.enabled = false;
        }
    }
}
