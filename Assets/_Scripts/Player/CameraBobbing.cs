using System;
using UnityEngine;

namespace _Scripts.Player
{
    public class CameraBobbing : MonoBehaviour
    {


        private void Start() => _rb = GetComponent<Rigidbody>();

        private void Update()
        {
            if (Math.Abs(_rb.velocity.magnitude) > 0.01f)
            {
                print("test");
            }
        }
    }
}