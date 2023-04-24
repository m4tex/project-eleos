using System.Collections;
using _Scripts.Enemies;
using UnityEngine;

namespace _Scripts.Environment
{
    [RequireComponent(typeof(Rigidbody))]
    public class SetZombieState : MonoBehaviour
    {
        public ZState state;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.TryGetComponent(out Zombie zombie))
                zombie.SetState(state);
        }
    }
}
