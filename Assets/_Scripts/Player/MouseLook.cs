using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 1f;
    
    private float _xRotation = 0, _yRotation = 0;

    // public bool cameraLock;
    // private bool _cursorLocked;

    void Start() => Cursor.lockState = CursorLockMode.Locked;

    void Update()
    {
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
    }

    // public void SetXRotation(float value) => _xRotation = value;
}