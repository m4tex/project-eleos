using System;
using _Scripts.Player;
using _Scripts.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Zombie : MonoBehaviour
    {
        private NavMeshAgent _navAgent;

        public int health = 100;
        public float speed = 3f;

        [Header("Damage Heights")] //Minimal height for a region to be damaged
        public Transform bodyHeight;
        public Transform headHeight;

        private static Transform _playerTransform;
        
        // Start is called before the first frame update
        private void Start()
        {
            _playerTransform = PlayerMovement.Main.transform;
            _navAgent = GetComponent<NavMeshAgent>();
            _navAgent.speed = speed;
        }
        
        private void Update()
        {
            _navAgent.destination = _playerTransform.position;
        }

        public void TakeDamage(int damage, float height)
        {
            float multiplier;

            if (height >= headHeight.position.y)
                multiplier = Firearm.HeadShotMultiplier;
            else if (height >= bodyHeight.position.y)
                multiplier = Firearm.BodyShotMultiplier;
            else
                multiplier = Firearm.LegShotMultiplier;
                
            health -= (int)(damage * multiplier);
        }
    }
}