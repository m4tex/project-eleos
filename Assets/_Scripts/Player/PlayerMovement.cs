using System;
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

    private GameObject _test;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerCamera = GetComponentInChildren<Camera>().transform;
        _col = GetComponent<CapsuleCollider>();
        _test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    } 
    
    private void Update()
    {
        // if (!movementLock)
        Crouching();
        WalkingAndJumping();
        _test.transform.parent = transform;
        _test.transform.localPosition = -new Vector3(0, _col.height / 2 - _col.radius + groundScanRange / 2, 0);
    }

    
    //optimize.... store precalculated instead of calculating per frame
    private void WalkingAndJumping()
    {
        //Pretty naive ground scan that doesn't account for the center point of the collider
        _isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, _col.height / 2 + groundScanRange + 0.01f - _col.radius, 0), 
            _col.radius-0.01f, groundScanMask);
        
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        Vector3 move = _playerCamera.right * x + _playerCamera.forward * z;
        move.y = 0;
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