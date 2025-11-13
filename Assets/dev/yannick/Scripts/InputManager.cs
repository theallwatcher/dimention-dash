using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("jump");
    }
}
