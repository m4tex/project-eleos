using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Main;

    private Rigidbody _rb;
    private Transform _playerCamera;

    public float walkSpeed = 12f;
    public float jumpForce = 16000f;
    public LayerMask groundScanMask;

    private bool _isGrounded;

    [Header("Sprinting")]
    public float sprintMultiplier = 1.5f;
    private bool _isRunning;
    
    public float maxStamina = 5f, fatigueDelay = 3f;
    private float _fatigueDelayCounter, _currentStamina;
    
    [Header("Extra")]
    public float groundScanRange = 0.1f;
    public float friction = 30; //Not exactly friction but I couldn't find a better name for it
    public float strafeAcceleration = 0.1f;
    public float jumpDelay = 0.1f;
    public bool movementLock;

    private float _jumpDelayCounter;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerCamera = GetComponentInChildren<Camera>().transform;
    } 
    
    private void Update()
    {
        // if (!movementLock)
        Crouching();
        WalkingAndJumping();
    }

    
    //optimize.... store precalculated instead of calculating per frame
    private void WalkingAndJumping()
    {
        //Pretty naive ground scan that doesn't account for the actual height of the character
        _isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), 
            groundScanRange,groundScanMask);
        
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        Vector3 move = _playerCamera.right * x + _playerCamera.forward * z;
        move = Vector3.Normalize(move);

        if (_isGrounded && !movementLock)
        {
            var velocity = _rb.velocity;
            _rb.velocity = Vector3.MoveTowards( velocity, move * walkSpeed + new Vector3(0, velocity.y, 0), 
                Time.deltaTime * friction);
            if ((Input.GetButtonDown("Jump") || Input.mouseScrollDelta.y > 0 ) && jumpDelay <= 0)
            {
                velocity = new Vector3(velocity.x, 0, velocity.z);
                _rb.velocity = velocity;
                _rb.AddForce(new Vector3(0, jumpForce));
                _jumpDelayCounter = jumpDelay;
            }
        }

        //Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift) && x + z != 0 && _fatigueDelayCounter <= 0 && !_isRunning)
        {
            walkSpeed *= sprintMultiplier;
            _isRunning = true;
        }
        else if (_isRunning && x + z == 0)
        {
            walkSpeed /= sprintMultiplier;
            _isRunning = false;
        }
        else if (_isRunning && _currentStamina <= 0)
        {
            walkSpeed /= sprintMultiplier;
            _isRunning = false;
            _fatigueDelayCounter = fatigueDelay;
        }

        //Stamina
        if (_isRunning)
            _currentStamina -= Time.deltaTime;
        if (_currentStamina < maxStamina && !_isRunning)
            _currentStamina += Time.deltaTime / 2;
        if (_fatigueDelayCounter > 0)
            _fatigueDelayCounter -= Time.deltaTime;
        if (_jumpDelayCounter > 0)
            _jumpDelayCounter -= Time.deltaTime;
    }

    private void Crouching()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            Crouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StandUp();
    }

    private void Crouch()
    {
        print("crouching");
    }

    private void StandUp()
    {
        print("standing up");
    }

    // private void OnDrawGizmos()
    // {
    //     if (_col) 
    //         Gizmos.DrawSphere();
    // }
}