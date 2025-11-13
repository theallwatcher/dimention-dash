using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _laneOffset;
    [SerializeField] private float _switchSpeed;
    [SerializeField] private float gravity = -10f;

    public InputSystem_Actions PlayerControls;
    private InputAction move;
    private InputAction fire;
    private void Awake()
    {
        PlayerControls = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        move = PlayerControls.Player.Move;
        move.Enable();
    }
    private void OnDisable()
    {
        move.Disable();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
