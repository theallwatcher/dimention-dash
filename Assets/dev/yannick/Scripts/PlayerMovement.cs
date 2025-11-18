using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //public InputSystem_Actions PlayerControls;
    private PlayerInput playerInput;
    private PlayerInventory inventory;
    [SerializeField] PlayerObject _playerSO;
    [SerializeField] Rigidbody rb;
    //movement
    private Vector2 moveDirection;
    private InputAction jumpAction;
    private InputAction duckAction;
    private InputAction moveAction;
    private InputAction powerupAction;
    private Vector3 startPos;
    //jump
    private bool isGrounded = true;


    private bool isMovingX = false;
    private Vector3 targetPosX;

    private bool isMovingZ = false;
    private Vector3 targetPosZ;

    //slide
    private bool isSliding = false;
    private float slideTimer = 0f;
    [SerializeField] private CapsuleCollider playerCollider;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        duckAction = playerInput.actions["Duck"];
        powerupAction = playerInput.actions["PowerUps"];
    }
    private void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        inventory = GetComponent<PlayerInventory>();

        if (playerCollider != null)
        {
            originalColliderHeight = playerCollider.height;
            originalColliderCenter = playerCollider.center;
        }
    }
    #region onEnableAndDisable
    private void OnEnable()
    {
        moveAction = playerInput.actions["Move"];
        moveAction.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveDirection = Vector2.zero;

        jumpAction = playerInput.actions["Jump"];
        jumpAction.performed += OnJump;

        duckAction = playerInput.actions["Duck"];
        duckAction.performed += OnDuck;

        powerupAction.performed += OnPowerupUse;

        /*//enable all movement buttons
        moveAction.Enable();

        jumpAction.Enable();
        jumpAction.performed += OnJump;

       // duckAction.Enable();
        duckAction.performed += OnDuck;*/
    }
    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        duckAction.Disable();
    }
    #endregion
    #region Input
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isGrounded) return;

        isGrounded = false;
        rb.AddForce(Vector3.up * _playerSO.JumpForce, ForceMode.Impulse);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;  
        }
    }
    public void OnDuck(InputAction.CallbackContext context)
    {
        StartSlide();
    }

    public void OnPowerupUse(InputAction.CallbackContext context)
    {
        inventory.UsePowerup();
    }

    #endregion
    private void FixedUpdate()
    {
       

        Move();
        SwitchLane();
        if (isMovingZ) HandleForwardBackwardMovement();
        HandleSlide();
    }

    public void Move()
    {
        if (!isMovingX)
        {
            //read value from input system

            ///LEFT
            if (moveDirection.x < -.1f)
                targetPosX = new Vector3(rb.position.x - _playerSO.LaneOffset, rb.position.y, rb.position.z);

            //RIGHT
            else if (moveDirection.x > .1f)
                targetPosX = new Vector3(rb.position.x + _playerSO.LaneOffset, rb.position.y, rb.position.z);

            //CLAMP X POSITION
            targetPosX.x = Mathf.Clamp(targetPosX.x,startPos.x  -_playerSO.LaneOffset, startPos.x + _playerSO.LaneOffset);

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

        //targetPosZ.z = Mathf.Clamp(targetPosZ.z, startPos.z - 5, 50);

        rb.MovePosition(newPos);

            if (Mathf.Abs(newZ - targetPosZ.z) < 0.01f)
            {
                rb.MovePosition(targetPosZ);
                isMovingZ = false;
            }
    }

    public void MovePlayerZ(float amount)
    {
        if (isMovingZ) return;

        targetPosZ = new Vector3(
            rb.position.x,
            rb.position.y,
            rb.position.z - amount);
        isMovingZ = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "obstacle") 
        {
            MovePlayerZ(3);
        }
    }
    #endregion


    #region slide movement
    private void StartSlide()
    {
        if (isSliding || playerCollider == null) return;

        isSliding = true;
        slideTimer = 0f;

        // Lower collider
        playerCollider.height = originalColliderHeight * _playerSO.slideHeight;
        playerCollider.center = originalColliderCenter - new Vector3(0, originalColliderHeight * (1 - _playerSO.slideHeight) / 2f, 0);
    }


    private void HandleSlide()
    {
        if (!isSliding) return;

        slideTimer += Time.fixedDeltaTime;
        if (slideTimer >= _playerSO.SlideDuration)
        {
            EndSlide();
        }
    }

    private void EndSlide()
    {
        isSliding = false;

        // Reset collider
        playerCollider.height = originalColliderHeight;
        playerCollider.center = originalColliderCenter;
    }
    #endregion
}


