using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    private PlayerMovement movementScript;
    private bool switchLaneActive = false;




    private bool positionSwitchActive = false;

    private void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
    }

    public void ActivatePositionSwitch()
    {
        positionSwitchActive = true;
    }

    public void ActivateInvertControls()
    {
        StartCoroutine(movementScript.SwitchControls());
    }

    public void ActiveSwitchLane()
    {

    }
}
