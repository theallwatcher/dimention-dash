using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInventory inventory;
    private PlayerAnimator animator;

    [SerializeField] PlayerObject _playerSO;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject dizzyParticals;
    //movement
    private Vector2 moveDirection;
    private InputAction jumpAction;
    private InputAction duckAction;
    private InputAction moveAction;
    private InputAction powerupAction;
    private InputAction pauseAction;
    private Vector3 startPos;

    public bool gameStarted = false;
    public enum PlayerLane
    {
        Left,
        Middle,
        Right,
    }

    public PlayerLane CurrentLane;
    

    //jump
    private bool isGrounded = true;
    private bool isJumping = false;

    private bool isMovingX = false;
    private Vector3 targetPosX;

    private bool isMovingZ = false;
    private Vector3 targetPosZ;

    //slide
    private bool isSliding = false;
    private float slideTimer = 0f;
    [SerializeField] private CapsuleCollider standingCollider, slidingCollider;
    [SerializeField] private Transform slidePosition;
    private float slideStartY;
    private float slideTargetY;
    //powerups
    float controlDirection = 1f;
    bool forceLaneSwitch = false;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inventory = GetComponent<PlayerInventory>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<PlayerAnimator>();


        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        duckAction = playerInput.actions["Duck"];
        powerupAction = playerInput.actions["PowerUps"];

        pauseAction = playerInput.actions["Pause"];

        //only in the first level the players need to wait
        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Main")
        {
            gameStarted = false;
        }
        else
        {
            gameStarted = true;
        }
    }
    private void Start()
    {
        startPos = transform.position;

        //setup slide Y offsets
        slideStartY = slidePosition.position.y;
        slideTargetY = slidePosition.position.y - 1;

        //players start in middle lane
        CurrentLane = PlayerLane.Middle;
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
        if(controlDirection > 0)
        {
            Jump();
        }
        else
        {
            StartSlide();
        }
    }

    private void Jump()
    {
        if (!gameStarted) return;

        if (!isGrounded) return;

        isJumping = true;
        isGrounded = false;
        animator.SetIsGrounded(false);
        if (isSliding)
        {
            EndSlide();
        }


        rb.AddForce(Vector3.up * _playerSO.JumpHeight, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
     {
         if (collision.gameObject.CompareTag("Ground") && isJumping)
         {
             isJumping = false;
             isGrounded = true; 
             animator.SetIsGrounded(true);
         }
     }

    public void OnDuck(InputAction.CallbackContext context)
    {
        if (!gameStarted) return;

        if (controlDirection > 0)
        {
            StartSlide();
        }
        else
        {
            Jump();
        }
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
        if (!gameStarted) return;
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
        if (!isGrounded || isSliding) return;

        isSliding = true;
        animator.SetIsSliding(true);

        standingCollider.enabled = false;
        slidingCollider.enabled = true;

        slideTimer = 0f;

    }


        private void HandleSlide()
        {
            if (!isSliding) return;

            slideTimer += Time.fixedDeltaTime;

            //keep x and y positions 
            Vector3 standY = new Vector3(slidePosition.position.x, slideStartY, slidePosition.position.z);
            Vector3 slideY = new Vector3(slidePosition.position.x, slideTargetY, slidePosition.position.z);

            //check the progression of the slide
            float twoThirds = (_playerSO.SlideDuration / 3) * 2;
            float t;
            float newY;

            if (slideTimer < twoThirds) //move model slightly down 
            {
                t = slideTimer / twoThirds;

                newY = Mathf.Lerp(slideStartY, slideTargetY, t);
            }
            else//move model up when slide is halfway
            {
               t = (slideTimer - twoThirds) / twoThirds;

               newY = Mathf.Lerp(slideTargetY, slideStartY, t);
            }

            //update final y position
            slidePosition.position = new Vector3(slidePosition.position.x, newY, slidePosition.position.z);

            //end sliding
            if (slideTimer >= _playerSO.SlideDuration)
            {
                 EndSlide();
            }
        }

        private void EndSlide()
        {
            //reset y position
            slidePosition.position = new Vector3(slidePosition.position.x, slideStartY, slidePosition.position.z);

            //update colliders
            standingCollider.enabled = true;
            slidingCollider.enabled = false;

            
            //change animation
            animator.SetIsSliding(false);
            isSliding = false;
    }
    #endregion

    #region powerups

    public IEnumerator SwitchControls()
    {
        controlDirection = -1f;
        Vector3 particalPosition = new Vector3(transform.position.x,transform.position.y+6,transform.position.z);
        GameObject dizzy = Instantiate(dizzyParticals,particalPosition, Quaternion.Euler(new Vector3(-90, 0, 0)),this.transform);

        yield return new WaitForSeconds(5);
        controlDirection = 1f;
    }

    public void ForceSwitchLane()
    {
       // if (isMovingX) return;
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
        forceLaneSwitch = false;
        isMovingX = true;
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

        float duration = 0.4f;    
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


