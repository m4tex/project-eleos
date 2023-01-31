using System;
using _Scripts.UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Weapons
{
    public class WeaponSway : MonoBehaviour
    {
        [Header("Positional Sway")] 
        public float maxSway = 10;
        public float swayAmount = 4, swaySmooth = 0.5f;
    
        [Header("Rotational Sway")] 
        public float maxTilt;
        public float tiltAmount, tiltSmooth;
        // public bool tiltX, tiltY, tiltZ;

        [Header("Aiming")] 
        [HideInInspector]
        public bool isAiming = false;
        public float aimTiltAmount;
        [HideInInspector]
        public Vector3 aimSwayPos;
        private Vector3 _aimSwayRot = Vector3.zero;
        
        private Vector3 _initialPos, _initialRot;
        private float mouseX, mouseY;
        
        private void Start()
        {
            _initialPos = transform.localPosition;
            _initialRot = transform.localEulerAngles;
        }
    
        private void Update()
        {
            if (!UIManager.ControllsLock)
            {
                mouseX = Mathf.Lerp(mouseX, Input.GetAxis("Mouse X"), Time.deltaTime * 12f);
                mouseY = Mathf.Lerp(mouseY, Input.GetAxis("Mouse Y"), Time.deltaTime * 12f);
            }
    
            //Pos sway
            var moveX = Math.Clamp(mouseX * swayAmount, -maxSway, maxSway);
            var moveY = Math.Clamp(mouseY * swayAmount, -maxSway, maxSway);
            
            var finalPos = isAiming ? aimSwayPos : new Vector3(moveX, 0, moveY) + _initialPos;

            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos, Time.deltaTime * swaySmooth);

            //Rot sway
            var rotX = Math.Clamp(mouseX * (isAiming ? aimTiltAmount : tiltAmount), -maxTilt, maxTilt);
            var rotY = Math.Clamp(mouseY * (isAiming ? aimTiltAmount : tiltAmount), -maxTilt, maxTilt);

            var finalRot = Quaternion.Euler(new Vector3( -(rotY + (isAiming ? 0 : Math.Abs(rotX) * 0.6f)), 0, rotX) + (isAiming ? _aimSwayRot : _initialRot));
            
            transform.localRotation = 
                Quaternion.Slerp(transform.localRotation, finalRot, Time.deltaTime * tiltSmooth);
        }
    }
}