using System.Collections;
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
        private Camera _camera;

        private void Awake()
        {
            main = this;
            Cursor.lockState = CursorLockMode.Locked;
            _camera = GetComponent<Camera>();
        } 

        private void Update()
        {
            if (UIManager.ControllsLock) return;

                transform.position = snapPoint.position;
            
            _yRotation += Input.GetAxis("Mouse X") * mouseSensitivity;
            _xRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            _xRotation = Mathf.Clamp(_xRotation, -89f, 89f);
            transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        }

        public void ChangeZoom(float amount, float speed)
        {
            StartCoroutine(ChangeZoomCoroutine(amount, speed));
        }
        
        private IEnumerator ChangeZoomCoroutine(float amount, float speed)
        {
            float elapsed = 0;
            float changed = 0;
            
            while (elapsed < speed)
            {
                elapsed += Time.deltaTime;

                var delta = 1 - Mathf.Pow(elapsed / speed, 4);

                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, amount - changed, delta);
                changed += delta * amount;
                
                yield return null;
            }
        }
    }
}