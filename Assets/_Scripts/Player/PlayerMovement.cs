using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Main;

    private Rigidbody _rb;
    private Transform _playerCamera;
    private CapsuleCollider _col;

    public float walkSpeed = 12f;
    // public float jumpForce = 16000f;
    public LayerMask groundScanMask;

    [Header("Sprinting")]
    public float sprintMultiplier = 1.5f;
    private bool _isRunning;
    
    public float maxStamina = 5f, fatigueDelay = 3f;
    private float _fatigueDelayCounter, _currentStamina;
    
    [Header("Extra")]
    public float groundScanRange = 0.1f;
    public float friction = 30; //Not exactly friction but I couldn't find a better name for it
    // public float jumpDelay = 0.1f;
    public bool movementLock;

    private float _jumpDelayCounter;
    private Vector3 _move;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerCamera = GetComponentInChildren<Camera>().transform;
        _playerCamera.transform.name = "AAAAAAAAAA";
        _col = GetComponent<CapsuleCollider>();
    } 
    
    private void Update()
    {
        // if (!movementLock)
        // Crouching();
        WalkingAndJumping();
    }

    //optimize.... store precalculated instead of calculating per frame
    private void WalkingAndJumping()
    {
        //Pretty naive ground scan that doesn't account for the actual height of the character
        var position = transform.position;
        var isGrounded = Physics.CheckSphere(position - new Vector3(0, 1, 0), groundScanRange,groundScanMask);
        //Raycast for slope check
        Physics.Raycast(position - new Vector3(0, 1, 0), Vector3.down,
            out var slopeHit, groundScanRange + 0.2f, groundScanMask);
        
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        _move = _playerCamera.right * x + _playerCamera.forward * z;
        _move.y = 0;
        _move = Vector3.ProjectOnPlane(_move, slopeHit.normal);
        _move.Normalize();

        Debug.DrawLine(position + Vector3.down, position + _move * 4f, Color.green, 0.25f);

        if (!isGrounded || movementLock) return;
        
        var velocity = _rb.velocity;
        _rb.velocity = Vector3.MoveTowards( velocity, _move * walkSpeed + new Vector3(0, velocity.y, 0), 
            Time.deltaTime * friction);
        
        // if ((Input.GetButtonDown("Jump") || Input.mouseScrollDelta.y > 0 ) && jumpDelay <= 0)
        // {
        //     velocity = new Vector3(velocity.x, 0, velocity.z);
        //     _rb.velocity = velocity;
        //     _rb.AddForce(new Vector3(0, jumpForce));
        //     _jumpDelayCounter = jumpDelay;
        // }

        // MOVE TO ANOTHER FUNCTION
        //Sprint
        // if (Input.GetKeyDown(KeyCode.LeftShift) && x + z != 0 && _fatigueDelayCounter <= 0 && !_isRunning)
        // {
        //     walkSpeed *= sprintMultiplier;
        //     _isRunning = true;
        // }
        // else if (_isRunning && x + z == 0)
        // {
        //     walkSpeed /= sprintMultiplier;
        //     _isRunning = false;
        // }
        // else if (_isRunning && _currentStamina <= 0)
        // {
        //     walkSpeed /= sprintMultiplier;
        //     _isRunning = false;
        //     _fatigueDelayCounter = fatigueDelay;
        // }

        //Stamina
        // if (_isRunning)
        //     _currentStamina -= Time.deltaTime;
        // if (_currentStamina < maxStamina && !_isRunning)
        //     _currentStamina += Time.deltaTime / 2;
        // if (_fatigueDelayCounter > 0)
        //     _fatigueDelayCounter -= Time.deltaTime;
        // if (_jumpDelayCounter > 0)
        //     _jumpDelayCounter -= Time.deltaTime;
    }

    // private void Crouching()
    // {
    //     if (Input.GetKeyDown(KeyCode.LeftControl))
    //         Crouch();
    //     if (Input.GetKeyUp(KeyCode.LeftControl))
    //         StandUp();
    // }
    //
    // private void Crouch()
    // {
    //     print("crouching");
    // }
    //
    // private void StandUp()
    // {
    //     print("standing up");
    // }

    // private void OnCollisionStay(Collision collisionInfo)
    // {
    //     foreach (var p in collisionInfo.contacts)
    //     {
    //         var bottom = transform.position - new Vector3(0, 1, 0);
    //         var curve = bottom + Vector3.up * _col.radius;
    //
    //         var dir = curve - p.point;
    //
    //         print("Collision");
    //
    //         if (p.point.y < curve.y)
    //             Debug.DrawLine(p.point, curve, Color.blue, 0.25f);
    //     }
    // }
}