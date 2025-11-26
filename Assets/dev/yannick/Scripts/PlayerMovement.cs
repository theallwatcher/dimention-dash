using System.Collections;
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
    private InputAction pauseAction;
    private Vector3 startPos;

    public enum PlayerLane
    {
        Left,
        Middle,
        Right,
    }

    public PlayerLane CurrentLane;
    

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


    //powerups
    float controlDirection = 1f;
    bool forceLaneSwitch = false;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inventory = GetComponent<PlayerInventory>();
        rb = GetComponent<Rigidbody>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        duckAction = playerInput.actions["Duck"];
        powerupAction = playerInput.actions["PowerUps"];

        pauseAction = playerInput.actions["Pause"];
    }
    private void Start()
    {
        startPos = transform.position;

        //players start in middle lane
        CurrentLane = PlayerLane.Middle;

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

        powerupAction = playerInput.actions["PowerUps"];
        powerupAction.performed += OnPowerupUse;

        pauseAction = playerInput.actions["Pause"];
        pauseAction.performed += OnPause;
    }
    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        duckAction.Disable();
        powerupAction.Disable();
    }
    #endregion
    #region Input
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!isGrounded) return;

        isGrounded = false;
        rb.AddForce(Vector3.up * _playerSO.JumpForce, ForceMode.Impulse);
        Debug.Log("jump");
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

    public void OnPause(InputAction.CallbackContext context)
    {
        GameManager.Instance.Pause();
    }

    #endregion
    private void FixedUpdate()
    {
       

        MoveLeftRight();
        SwitchLane();
        if (isMovingZ) HandleForwardBackwardMovement();
        HandleSlide();
    }

    public void MoveLeftRight()
    {
        if (isMovingX || forceLaneSwitch) return;

        //when reverse input powerup is active
        float horizontal = moveDirection.x * controlDirection;

            ///LEFT
            if (horizontal < -.1f)
            {
                targetPosX = new Vector3(rb.position.x - _playerSO.LaneOffset, rb.position.y, rb.position.z);

                //when player is right move to middle pos
                if(CurrentLane == PlayerLane.Right)
                {
                    CurrentLane = PlayerLane.Middle;
                }
                //when player is right move to middle pos
                else if (CurrentLane == PlayerLane.Middle)
                {
                    CurrentLane = PlayerLane.Left;
                }
                //when is player is left dont change
                else if(CurrentLane == PlayerLane.Left)
                {
                    CurrentLane = PlayerLane.Left;
                }
            }

            //RIGHT
            else if (horizontal > .1f)
            {
                targetPosX = new Vector3(rb.position.x + _playerSO.LaneOffset, rb.position.y, rb.position.z);

                //when player is left move to middle pos
                if (CurrentLane == PlayerLane.Left)
                {
                    CurrentLane = PlayerLane.Middle;
                }
                //when player is middle move to right pos
                else if (CurrentLane == PlayerLane.Middle)
                {
                    CurrentLane = PlayerLane.Right;
                }
                //when is player is right dont change
                else if (CurrentLane == PlayerLane.Right)
                {
                    CurrentLane = PlayerLane.Right;
                }
            }

            //CLAMP X POSITION
            targetPosX.x = Mathf.Clamp(targetPosX.x,startPos.x  -_playerSO.LaneOffset, startPos.x + _playerSO.LaneOffset);

            // start movement if new target detected
            if (horizontal > 0.1f || horizontal < -0.1f)
                isMovingX = true;
    }
    private void SwitchLane()
    {

        //dont execute when moving
        if (!isMovingX) return;

        Vector3 currentPos = rb.position;

        //Lerp current position to new target position 
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

    #region powerups

    public IEnumerator SwitchControls()
    {
        controlDirection = -1f;

        yield return new WaitForSeconds(5);
        controlDirection = 1f;
    }

    public void ForceSwitchLane()
    {
        if (isMovingX) return;
        forceLaneSwitch = true;
        // Move RIGHT if possible
        if (CurrentLane == PlayerLane.Left)
        {
            CurrentLane = PlayerLane.Middle;
            targetPosX = new Vector3(rb.position.x + _playerSO.LaneOffset, rb.position.y, rb.position.z);
        }
        else if (CurrentLane == PlayerLane.Middle)
        {
            // Random: left or right
            if (Random.value > 0.5f)
            {
                CurrentLane = PlayerLane.Left;
                targetPosX = new Vector3(rb.position.x - _playerSO.LaneOffset, rb.position.y, rb.position.z);
            }
            else
            {
                CurrentLane = PlayerLane.Right;
                targetPosX = new Vector3(rb.position.x + _playerSO.LaneOffset, rb.position.y, rb.position.z);
            }
        }
        else if (CurrentLane == PlayerLane.Right)
        {
            CurrentLane = PlayerLane.Middle;
            targetPosX = new Vector3(rb.position.x - _playerSO.LaneOffset, rb.position.y, rb.position.z);
        }

        isMovingX = true; // start movement next FixedUpdate
    }
    public void SmoothSwapZ(PlayerMovement otherPlayer)
    {
        if (otherPlayer == null) return;

        StartCoroutine(SmoothSwapCoroutine(otherPlayer));
    }

    private IEnumerator SmoothSwapCoroutine(PlayerMovement otherPlayer)
    {
        // stop current movement
        isMovingZ = false;
        otherPlayer.isMovingZ = false;

        rb.linearVelocity = Vector3.zero;
        otherPlayer.rb.linearVelocity = Vector3.zero;

        rb.isKinematic = true;
        otherPlayer.rb.isKinematic = true;

        float duration = 0.4f;     // how long the swap lasts
        float t = 0f;

        Vector3 myStart = transform.position;
        Vector3 otherStart = otherPlayer.transform.position;

        Vector3 myTarget = new Vector3(myStart.x, myStart.y, otherStart.z);
        Vector3 otherTarget = new Vector3(otherStart.x, otherStart.y, myStart.z);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            transform.position = Vector3.Lerp(myStart, myTarget, t);
            otherPlayer.transform.position = Vector3.Lerp(otherStart, otherTarget, t);

            yield return null;
        }

        // finalize snap to exact spot
        transform.position = myTarget;
        otherPlayer.transform.position = otherTarget;

        rb.isKinematic = false;
        otherPlayer.rb.isKinematic = false;
    }

    #endregion
}


