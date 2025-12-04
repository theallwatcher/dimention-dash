using System.Collections.Generic;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    [SerializeField] private List<GameObject> _roadSectionList;

    [SerializeField] Transform spawnPoint;
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("RoadSectionTrigger"))
        {
            Instantiate(_roadSectionList[Random.Range(0, _roadSectionList.Count)], spawnPoint.position, spawnPoint.rotation);
        }
    }
}
