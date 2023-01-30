using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera main;
        
        public Transform snapPoint;
        public float mouseSensitivity = 1f;
    
        private float _xRotation = 0, _yRotation = 0;
        // private Transform _body;

        // public bool cameraLock;
        // private bool _cursorLocked;

        private void Awake()
        {
            main = this;
            Cursor.lockState = CursorLockMode.Locked;
            // _body = GetComponentInParent<PlayerMovement>().transform;
        } 

        private void Update()
        {
            if (UIManager.ControllsLock) return;

                transform.position = snapPoint.position;
            
            _yRotation += Input.GetAxis("Mouse X") * mouseSensitivity;
            _xRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            _xRotation = Mathf.Clamp(_xRotation, -89f, 89f);

            // if(cameraLock && _cursorLocked)
            // {
            //     Cursor.lockState = CursorLockMode.Confined;
            //     _cursorLocked = false;
            // }
            // else if(!cameraLock && !_cursorLocked)
            // {
            //     Cursor.lockState = CursorLockMode.Locked;
            //     _cursorLocked = true;
            // }
            // if (!cameraLock)
            // {
            // }
            transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            // _body.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
        }

        // public void SetXRotation(float value) => _xRotation = value;
    }
}