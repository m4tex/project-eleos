using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement main;

    private Rigidbody _rb;
    private Transform _playerCamera;

    public float walkSpeed = 12f;
    public float jumpForce = 500f;
    
    public float sprintMultiplier = 1.5f;
    private bool _isRunning;
    
    public float maxStamina = 5f, fatigueDelay = 3f;
    private float _fatigueDelayCounter, _currentStamina;

    public Transform groundCheck;
    public float groundScanRange = 0.1f;
    public LayerMask groundMask;
    private bool _isGrounded;
    
    public bool movementLock;

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

    private void WalkingAndJumping()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundScanRange, groundMask);

        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        
        Vector3 move = _playerCamera.right * x + _playerCamera.forward * z;
        move.y = 0;

        if (_isGrounded)
        {
            _rb.velocity = movementLock ? _rb.velocity : move * walkSpeed + new Vector3(0, _rb.velocity.y, 0);
            if(Input.GetButtonDown("Jump") && !movementLock)
                _rb.AddForce(new Vector3(0, jumpForce));
        }

        //Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift) && x + z != 0 && _fatigueDelayCounter <= 0 && !_isRunning)
        {
            walkSpeed *= sprintMultiplier;
            _isRunning = true;
        }
        if (_isRunning && x + z == 0)
        {
            walkSpeed /= sprintMultiplier;
            _isRunning = false;
        }
        if (_isRunning && _currentStamina <= 0)
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
    }

    void Crouching()
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
}