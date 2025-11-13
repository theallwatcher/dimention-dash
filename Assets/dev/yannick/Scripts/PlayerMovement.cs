using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _laneOffset;
    [SerializeField] private float _switchSpeed;
    [SerializeField] private float gravity = -10f;

    public InputSystem_Actions PlayerControls;
    private Vector2 moveDirection;
    private InputAction jump;
    private InputAction duck;
    private InputAction move;
    private bool isMoving = false;

    private void Awake()
    {
        PlayerControls = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        //enable all movement buttons
        move = PlayerControls.Player.Move;
        move.Enable();

        jump = PlayerControls.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        duck = PlayerControls.Player.Duck;
        duck.Enable();
        duck.performed += Duck;
    }
    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        duck.Disable();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
    }
    private void Duck(InputAction.CallbackContext context)
    {
        Debug.Log("Duck");
    }

    private void FixedUpdate()
    {
        //read value from input system
        moveDirection = PlayerControls.Player.Move.ReadValue<Vector2>();

        //check if moving
        if (moveDirection.x == Vector2.zero.x)
        {
            isMoving = false;
        }
        //check if moving left
        else if (moveDirection.x < -.1f)
        {
            Debug.Log("move left");
            MoveToTargetPos(-1);
            isMoving = true;
        }
        //check if moving right
        else if (moveDirection.x > .1f)
        {
            Debug.Log("move right");
            MoveToTargetPos(1);
            isMoving = true;
        }
    }

    private void MoveToTargetPos(float dir)
    {
        Transform t = transform;
        Vector3 targetPos = new Vector3(t.position.x + (dir * _laneOffset) , t.position.y, t.position.z);
        t.position = Vector3.Lerp(t.position, targetPos, _switchSpeed * Time.deltaTime);
        isMoving = false;
    }
 
}
