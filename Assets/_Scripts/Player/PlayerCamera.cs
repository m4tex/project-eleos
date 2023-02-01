using System;
using System.Collections;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera Main;
        
        [Header("Mouse Look")]
        public Transform snapPoint;
        public float mouseSensitivity = 1f;
    
        private float _xRotation = 0, _yRotation = 0;
        private Camera _camera;

        [Header("Dynamic FOV")] 
        public float walkingFovAmount = 5;
        public float sprintingFovAmount = 10;
        public float fovTransitionSmooth = 4f;
        
        [HideInInspector] 
        public bool weaponZoom = false;
        [HideInInspector]
        public float weaponZoomFovAmount;
        private float _initialFov;
        private Rigidbody _rb;
        
        private void Awake()
        {
            Main = this;
            Cursor.lockState = CursorLockMode.Locked;
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            _initialFov = _camera.fieldOfView;
            _rb = PlayerMovement.Main.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (UIManager.ControllsLock) return;

            transform.position = snapPoint.position;
            
            _yRotation += Input.GetAxis("Mouse X") * mouseSensitivity;
            _xRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            _xRotation = Mathf.Clamp(_xRotation, -89f, 89f);
            transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            
            UpdateFov();
        }

        private void UpdateFov()
        {
            var target = _initialFov;

            target -= PlayerMovement.Main.isRunning ? sprintingFovAmount : _rb.velocity.magnitude > 0.1f ? walkingFovAmount : 0;
            target -= weaponZoom ? weaponZoomFovAmount : 0;

            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, target, Time.deltaTime * fovTransitionSmooth);
        }
    }
}