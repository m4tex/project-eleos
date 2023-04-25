using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement Main { get; private set; }

        private Rigidbody _rb;
        private Transform _playerCamera;

        [Header("General")]
        public float walkSpeed = 12f;
        public float jumpForce;
        public float jumpDelay;
        public LayerMask groundScanMask;

        [Space]
        
        [Header("Sprinting")]
        public float sprintMultiplier = 1.5f;
        
        public bool isRunning { get; private set; }

        public float maxStamina = 5f, fatigueDelay = 3f;
        private float _fatigueDelayCounter, _currentStamina;

        [Space] 
        
        [Header("Vaulting")] 
        public float vaultDuration;
        // public float vaultHeight;
        public float vaultDistance;
        private bool _isVaulting;

        [Header("Audio")] 
        public List<AudioClip> footstepSoundClips;
        public float footstepInterval;
        public float minimumVelocity;
        public float pitchVariation = .4f;
        
        private AudioSource _audio;
        private float _footstepIntervalCounter = 0.0f;

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
        public float walkingFovAmount = 65;
        public float runningFOV = 75;
        public float fovTransitionSmooth = 10f;
        
        private Camera _playerCameraComponent;
        private float _initialCameraFOV;
        
        [Space]
        
        [Header("Extra")]
        public float groundScanRange = 0.1f;
        public float friction = 30f; //Not exactly friction but I couldn't find a better name for it
        public bool movementLock = false;
        
        private float _jumpDelayCounter;
        private Vector3 _move;
        
        private void Awake() => Main = this;
        
        private void Start()
        {
            if (Camera.main == null) throw new Exception("Main Camera doesn't exist!!!");

            _audio = GetComponent<AudioSource>();
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
            // DynamicFOV();
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
            
            if (!isGrounded || movementLock || _isVaulting) return;

            // if (_jumpDelayCounter <= 0 && Input.GetKeyDown(KeyCode.Space))
            // {
            //     _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //     _jumpDelayCounter = jumpDelay;
            // }

            if (_rb.velocity.magnitude > minimumVelocity && _footstepIntervalCounter <= 0)
            {
                int index = Random.Range(0, footstepSoundClips.Count);
                float pitch = Random.Range(0, pitchVariation);
                _audio.pitch = 1 - pitch;
                _audio.PlayOneShot(footstepSoundClips[index]);
                _footstepIntervalCounter = footstepInterval / (isRunning ? 1.5f : 1);
            }

            //Raycast for slope check
            Physics.Raycast(feetPosition, Vector3.down,
                out var slopeHit, groundScanRange + 0.2f, groundScanMask);

            _move = _playerCamera.right * x + _playerCamera.forward * z;
            _move.y = 0;

            _move.Normalize();
            
            _move += (isRunning || _isVaulting) ? sprintMultiplier * _playerCamera.forward : Vector3.zero;
            _move = Vector3.ProjectOnPlane(_move, slopeHit.normal);
            
            _rb.velocity = Vector3.MoveTowards( _rb.velocity, _move * (walkSpeed * StatsManager.SpeedFactor), 
                Time.deltaTime * friction);

            #region Sprinting

            if (Input.GetKey(KeyCode.LeftShift) && z > 0 && _fatigueDelayCounter <= 0 && !isRunning)
            {
                isRunning = true;
            }
            else switch (isRunning)
            {
                case true when z <= 0:
                    isRunning = false;
                    break;
                case true when _currentStamina <= 0:
                    isRunning = false;
                    _fatigueDelayCounter = fatigueDelay;
                    break;
            }
            
            #endregion
        }

        private void SnapPointBobbing(float inputX, float inputZ, bool isGrounded)
        {
            var yOffset = Mathf.Sin(_bobbingElapsed * frequency / 1000 * Mathf.PI * (isRunning ? sprintBobbingFrequencyFactor : 1)) * amplitude;
            
            cameraSnapPoint.localPosition = Vector3.Lerp(cameraSnapPoint.localPosition,
                _initialSnapPointPos + new Vector3(0, yOffset, 0), Time.deltaTime * smoothBobbing);

            if (inputX + inputZ == 0 || !isGrounded) _bobbingElapsed = 0;
            else _bobbingElapsed += Time.time;
        }

        public void Vault()
        {
            StartCoroutine(VaultAnim());
        }

        private IEnumerator VaultAnim()
        {
            _isVaulting = true;
            isRunning = false;
            float elapsed = 0f;
            while (elapsed < vaultDuration)
            {
                _rb.velocity = Vector3.zero;
                transform.position += (_playerCamera.forward + _playerCamera.up*2) * (Time.deltaTime * vaultDistance);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _isVaulting = false;
        }
        
        // private void DynamicFOV()
        // {
        //     var targetFOV = isRunning ? runningFOV : _rb.velocity.magnitude > 0.1f ? walkingFOV : _initialCameraFOV;
        //     
        //     _playerCameraComponent.fieldOfView = Mathf.Lerp(_playerCameraComponent.fieldOfView, targetFOV,
        //         fovTransitionSmooth * Time.deltaTime);
        // }
        
        private void Counters()
        {
            if (isRunning)
                _currentStamina -= Time.deltaTime;
            if (_currentStamina < maxStamina && !isRunning)
                _currentStamina += Time.deltaTime / 2;
            if (_fatigueDelayCounter > 0)
                _fatigueDelayCounter -= Time.deltaTime;
            if (_jumpDelayCounter > 0)
                _jumpDelayCounter -= Time.deltaTime;
            if (_footstepIntervalCounter > 0)
                _footstepIntervalCounter -= Time.deltaTime;
        }
    }
}