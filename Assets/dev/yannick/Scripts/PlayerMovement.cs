using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _laneOffset;
    [SerializeField] private float _switchSpeed;
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float jumpHeight = 5f;

    public InputSystem_Actions PlayerControls;

    private Vector2 moveDirection;
    private InputAction jump;
    private InputAction duck;
    private InputAction move;

    private bool isMoving = false;
    private float direction;
    private Vector3 targetPos;
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
        if(isMoving) return;

        //check if moving left
        if (moveDirection.x < -.1f)
        {
            targetPos = new Vector3(transform.position.x + -_laneOffset, transform.position.y, transform.position.z);
            isMoving = true;
        }
        //check if moving right
        else if (moveDirection.x > .1f)
        {
            //set target pos
            targetPos = new Vector3(transform.position.x + _laneOffset, transform.position.y, transform.position.z);
            isMoving = true;
        }
    }
    private void Update()
    {
        //dont execute when moving
        if (!isMoving) return;

        //save player transform
        Transform t = transform;

        //update player position
        t.position = Vector3.Lerp(t.position, targetPos, _switchSpeed * Time.deltaTime);

        //check if target pos is reached
        if(Mathf.Abs(t.position.x - targetPos.x) < 0.01f)
        {
            t.position = targetPos;
            direction = 0;
            isMoving = false;
        } 
    }
}
