using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    [SerializeField] GameObject sectionPrefab;
    [SerializeField] Transform spawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RoadSectionTrigger"))
        {
            Instantiate(sectionPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
