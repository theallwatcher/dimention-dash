using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 5f;
    private void Update()
    {
        transform.Rotate(0, rotationSpeed, 0, Space.Self);
    }
}
