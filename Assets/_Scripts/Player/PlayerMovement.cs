using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Main { get; set; }

    private Rigidbody _rb;
    private Transform _playerCamera;
    private CapsuleCollider _col;

    public float walkSpeed = 12f;
    // public float jumpForce = 16000f;
    public LayerMask groundScanMask;

    [Header("Sprinting")]
    public float sprintMultiplier = 1.5f;
    private bool _isRunning = false;
    
    public float maxStamina = 5f, fatigueDelay = 3f;
    private float _fatigueDelayCounter, _currentStamina;
    
    [Header("Extra")]
    public float groundScanRange = 0.1f;
    public float friction = 30; //Not exactly friction but I couldn't find a better name for it
    // public float jumpDelay = 0.1f;
    public bool movementLock = false;
    public bool debugRays = true;

    private float _jumpDelayCounter;
    private Vector3 _move;

    private void Start()
    {
        Main = this;
        _rb = GetComponent<Rigidbody>();
        _playerCamera = GetComponentInChildren<Camera>().transform;
        _col = GetComponent<CapsuleCollider>();
    } 
    
    private void Update()
    {
        WalkingAndJumping();
    }

    //optimize.... store precalculated instead of calculating per frame
    private void WalkingAndJumping()
    {
        //Pretty naive ground scan that doesn't account for the actual height of the character
        var position = transform.position;
        var feetPosition = position + Vector3.down;
        var isGrounded = Physics.CheckSphere(feetPosition, groundScanRange,groundScanMask);
        //Raycast for slope check
        Physics.Raycast(feetPosition, Vector3.down,
            out var slopeHit, groundScanRange + 0.2f, groundScanMask);
        
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        _move = _playerCamera.right * x + _playerCamera.forward * z;
        _move.y = 0;

        _move.Normalize();
        _move += _isRunning ? sprintMultiplier * _playerCamera.forward: Vector3.zero;
        _move = Vector3.ProjectOnPlane(_move, slopeHit.normal);

        if (debugRays)
        {
            Debug.DrawLine(feetPosition, feetPosition + _move * 4f, Color.green, 0.1f);
            Debug.DrawRay(position, _playerCamera.forward, Color.blue, 0.1f);
        }
        
        if (!isGrounded || movementLock) return;
        
        var velocity = _rb.velocity;
        
        _rb.velocity = Vector3.MoveTowards( velocity, _move * walkSpeed, 
            Time.deltaTime * friction);

        //Sprint
        if (Input.GetKey(KeyCode.LeftShift) && x + z != 0 && _fatigueDelayCounter <= 0 && !_isRunning)
        {
            // walkSpeed *= sprintMultiplier;
            _isRunning = true;
        }
        else if (_isRunning && x + z == 0)
        {
            // walkSpeed /= sprintMultiplier;
            _isRunning = false;
        }
        else if (_isRunning && _currentStamina <= 0)
        {
            // walkSpeed /= sprintMultiplier;
            _isRunning = false;
            _fatigueDelayCounter = fatigueDelay;
        }

        // Stamina
        if (_isRunning)
            _currentStamina -= Time.deltaTime;
        if (_currentStamina < maxStamina && !_isRunning)
            _currentStamina += Time.deltaTime / 2;
        if (_fatigueDelayCounter > 0)
            _fatigueDelayCounter -= Time.deltaTime;
        if (_jumpDelayCounter > 0)
            _jumpDelayCounter -= Time.deltaTime;
        
        // if ((Input.GetButtonDown("Jump") || Input.mouseScrollDelta.y > 0 ) && jumpDelay <= 0)
        // {
        //     velocity = new Vector3(velocity.x, 0, velocity.z);
        //     _rb.velocity = velocity;
        //     _rb.AddForce(new Vector3(0, jumpForce));
        //     _jumpDelayCounter = jumpDelay;
        // }
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

    // private void OnCollisionStay(Collision collisionInfo)
    // {
    //     
    // }
}