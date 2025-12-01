using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    private PlayerMovement movementScript;

    private void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
    }

    public void ActivateInvertControls()
    {
        StartCoroutine(movementScript.SwitchControls());
    }
}
