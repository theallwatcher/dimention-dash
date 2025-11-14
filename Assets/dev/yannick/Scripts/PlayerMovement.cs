using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputSystem_Actions PlayerControls;
    [SerializeField] PlayerObject _playerSO;

    //movement
    private Vector2 moveDirection;
    private InputAction jump;
    private InputAction duck;
    private InputAction move;
    
    //jump
    private bool isGrounded = true;
    private Rigidbody rb;


    private bool isMovingX = false;
    private Vector3 targetPosX;

    private bool isMovingZ = false;
    private Vector3 targetPosZ;

    private void Awake()
    {
        PlayerControls = new InputSystem_Actions();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    #region onEnableAndDisable
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
    #endregion
    #region Input
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

    #endregion
    private void FixedUpdate()
    {
        Debug.Log(isMovingZ);
        Move();
        SwitchLane();
        if (isMovingZ) HandleForwardBackwardMovement();
    }
    private void OnCollisionEnter(Collision other)
    {
        //GROUNDCHECK
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void Move()
    {
        if (!isMovingX)
        {
            //read value from input system
            moveDirection = PlayerControls.Player.Move.ReadValue<Vector2>();


            ///LEFT
            if (moveDirection.x < -.1f)
                targetPosX = new Vector3(rb.position.x - _playerSO.LaneOffset, rb.position.y, rb.position.z);

            //RIGHT
            else if (moveDirection.x > .1f)
                targetPosX = new Vector3(rb.position.x + _playerSO.LaneOffset, rb.position.y, rb.position.z);

            //CLAMP X POSITION
            targetPosX.x = Mathf.Clamp(targetPosX.x, -_playerSO.LaneOffset, _playerSO.LaneOffset);

            // start movement if new target detected
            if (moveDirection.x > 0.1f || moveDirection.x < -0.1f)
                isMovingX = true;
        }
    }
    private void SwitchLane()
    {

        //dont execute when moving
        if (!isMovingX) return;

        Vector3 currentPos = rb.position;

        float newX = Mathf.Lerp(
            currentPos.x,
            targetPosX.x,
            _playerSO.MovementSpeed * Time.fixedDeltaTime
            );

        //update player position
        Vector3 newPos = new Vector3(
          newX, currentPos.y, currentPos.z);

        rb.MovePosition(newPos);

        //check if target pos is reached
        if (Mathf.Abs(newX - targetPosX.x) < 0.01f)
        {
            rb.MovePosition(new Vector3(targetPosX.x, currentPos.y, currentPos.z));
            isMovingX = false;
        }
    }
    #region Z movement
    private void HandleForwardBackwardMovement()
    {
            Vector3 currentPos = rb.position;

            float newZ = Mathf.Lerp(
                currentPos.z,
                targetPosZ.z,
                _playerSO.MovementSpeed * Time.fixedDeltaTime);

            Vector3 newPos = new Vector3(
                currentPos.x, currentPos.y, newZ);

            rb.MovePosition(newPos);

            if (Mathf.Abs(newZ - targetPosZ.z) < 0.01f)
            {
                rb.MovePosition(targetPosZ);
                isMovingZ = false;
            }
    }

    public void DamagePlayer()
    {
        if (isMovingZ) return;

        targetPosZ = new Vector3(
            rb.position.x,
            rb.position.y,
            rb.position.z - _playerSO.DamageOffset);
        isMovingZ = true;
    }

    public void BoostPlayer()
    {
        if (isMovingZ) return;

        targetPosZ = new Vector3(
            rb.position.x,
            rb.position.y,
            rb.position.z + _playerSO.BoostOffset);
        isMovingZ = true;
    }
    #endregion
}
