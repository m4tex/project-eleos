using System;
using UnityEngine;

namespace _Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement Main { get; set; }

        private Rigidbody _rb;
        private Transform _playerCamera;

        [Header("General")]
        public float walkSpeed = 12f;
        public LayerMask groundScanMask;

        [Space]
        
        [Header("Sprinting")]
        public float sprintMultiplier = 1.5f;
        private bool _isRunning = false;
    
        public float maxStamina = 5f, fatigueDelay = 3f;
        private float _fatigueDelayCounter, _currentStamina;
    
        [Space]
        
        [Header("Extra")]
        public float groundScanRange = 0.1f;
        public float friction = 30f; //Not exactly friction but I couldn't find a better name for it
        public bool movementLock = false;

        private float _jumpDelayCounter;
        private Vector3 _move;

        [Space] 
        
        [Header("Camera Bobbing")] 
        public float frequency = .5f;
        public float amplitude = 5f;
        public float smoothBobbing = 2f;
        public float sprintBobbingFrequencyFactor = 1.5f;
        public Transform cameraSnapPoint;
        private float _bobbingElapsed;
        private Vector3 _initialSnapPointPos;

        [Space] 
        
        [Header("Dynamic FOV")] 
        public float walkingFOV = 65;
        public float runningFOV = 75;
        public float fovTransitionSmooth = 10f;
        
        private Camera _playerCameraComponent;
        private float _initialCameraFOV;
        
        private void Start()
        {
            if (Camera.main == null) throw new Exception("Main Camera doesn't exist!!!");
            
            Main = this;
            _playerCamera = Camera.main.transform;
            _rb = GetComponent<Rigidbody>();
            _initialSnapPointPos = cameraSnapPoint.localPosition;
            _playerCameraComponent = _playerCamera.GetComponent<Camera>();
            _initialCameraFOV = _playerCameraComponent.fieldOfView;
        } 
    
        private void Update()
        {
            WalkingAndSprint();
            Counters();
            DynamicFOV();
        }

        private void WalkingAndSprint()
        {
            //Pretty naive ground scan that doesn't account for the actual height of the character
            var position = transform.position;
            var feetPosition = position + Vector3.down;
            var isGrounded = Physics.CheckSphere(feetPosition, groundScanRange,groundScanMask);
            
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");

            SnapPointBobbing(x, z, isGrounded);
            
            if (!isGrounded || movementLock) return;
            
            //Raycast for slope check
            Physics.Raycast(feetPosition, Vector3.down,
                out var slopeHit, groundScanRange + 0.2f, groundScanMask);
            
            _move = _playerCamera.right * x + _playerCamera.forward * z;
            _move.y = 0;

            _move.Normalize();
            
            _move += _isRunning ? sprintMultiplier * _playerCamera.forward : Vector3.zero;
            _move = Vector3.ProjectOnPlane(_move, slopeHit.normal);
            
            _rb.velocity = Vector3.MoveTowards( _rb.velocity, _move * walkSpeed, 
                Time.deltaTime * friction);

            #region Sprinting

            if (Input.GetKey(KeyCode.LeftShift) && z > 0 && _fatigueDelayCounter <= 0 && !_isRunning)
            {
                _isRunning = true;
            }
            else switch (_isRunning)
            {
                case true when z == 0:
                    _isRunning = false;
                    break;
                case true when _currentStamina <= 0:
                    _isRunning = false;
                    _fatigueDelayCounter = fatigueDelay;
                    break;
            }
            
            #endregion
        }

        private void SnapPointBobbing(float inputX, float inputZ, bool isGrounded)
        {
            var yOffset = Mathf.Sin(_bobbingElapsed * frequency / 1000 * Mathf.PI * (_isRunning ? sprintBobbingFrequencyFactor : 1)) * amplitude;
            
            cameraSnapPoint.localPosition = Vector3.Lerp(cameraSnapPoint.localPosition,
                _initialSnapPointPos + new Vector3(0, yOffset, 0), Time.deltaTime * smoothBobbing);

            if (inputX + inputZ == 0 || !isGrounded) _bobbingElapsed = 0;
            else _bobbingElapsed += Time.time;
        }

        private void DynamicFOV()
        {
            var targetFOV = _isRunning ? runningFOV : _rb.velocity.magnitude > 0.1f ? walkingFOV : _initialCameraFOV;
            
            _playerCameraComponent.fieldOfView = Mathf.Lerp(_playerCameraComponent.fieldOfView, targetFOV,
                fovTransitionSmooth * Time.deltaTime);
        }
        
        private void Counters()
        {
            if (_isRunning)
                _currentStamina -= Time.deltaTime;
            if (_currentStamina < maxStamina && !_isRunning)
                _currentStamina += Time.deltaTime / 2;
            if (_fatigueDelayCounter > 0)
                _fatigueDelayCounter -= Time.deltaTime;
            if (_jumpDelayCounter > 0)
                _jumpDelayCounter -= Time.deltaTime;
        }
    }
}