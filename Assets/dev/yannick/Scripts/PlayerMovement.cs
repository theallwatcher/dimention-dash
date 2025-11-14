using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputSystem_Actions PlayerControls;
    [SerializeField] PlayerObject _playerSO;

    //jump
    private bool isGrounded = true;
    private Rigidbody rb;
    private Vector3 startPos;

    //movement
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
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
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
        if (!isGrounded) return;

        isGrounded = false;
        rb.AddForce(Vector3.up * _playerSO.JumpForce, ForceMode.Impulse);

    }
    private void Duck(InputAction.CallbackContext context)
    {
        Debug.Log("Duck");
    }

    private void FixedUpdate()
    {
        if (!isMoving)
        {
            //read value from input system
            moveDirection = PlayerControls.Player.Move.ReadValue<Vector2>();


            ///LEFT
            if (moveDirection.x < -.1f)
                targetPos = new Vector3(rb.position.x - _playerSO.LaneOffset, rb.position.y, rb.position.z);

            //RIGHT
            else if (moveDirection.x > .1f)
                targetPos = new Vector3(rb.position.x + _playerSO.LaneOffset, rb.position.y, rb.position.z);

            //CLAMP X POSITION
            targetPos.x = Mathf.Clamp(targetPos.x, -_playerSO.LaneOffset, _playerSO.LaneOffset);

            // start movement if new target detected
            if (moveDirection.x > 0.1f || moveDirection.x < -0.1f)
                isMoving = true;
        }

        HandleMovement();
    }
    private void Update()
    {
            Debug.Log(isMoving);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void HandleMovement()
    {

        //dont execute when moving
        if (!isMoving) return;

        Vector3 currentPos = rb.position;

        float newX = Mathf.Lerp(
            currentPos.x,
            targetPos.x,
            _playerSO.MovementSpeed * Time.fixedDeltaTime
            );

        //update player position
        Vector3 newPos = new Vector3(
          newX, currentPos.y, currentPos.z);

        rb.MovePosition(newPos);

        //check if target pos is reached
        if (Mathf.Abs(newX - targetPos.x) < 0.01f)
        {
            rb.MovePosition(new Vector3(targetPos.x, currentPos.y, currentPos.z));
            isMoving = false;
        }
    }
}
