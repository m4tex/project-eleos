using System;
using System.Collections;
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
        private Animator _anim;
        private static readonly int AttackBool = Animator.StringToHash("Attack");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");

        private int _health = 100;
        public int Health
        {
            get => _health;
            set
            {
                _health = value;
                
                if (_health > 0) return;

                Die();

                // Destroy(gameObject);
            }
        }
        public float speed = 3f;
        public float attackRange;
        public float attackSpeed;
        public int attackDamage;

        private bool _attacking = false;
        private bool _isDead = false;
        
        [Header("Damage Heights")] //Minimal height for a region to be damaged
        public Transform bodyHeight;
        public Transform headHeight;

        private static Transform _playerTransform;

        // Start is called before the first frame update
        private void Start()
        {
            _anim = GetComponentInChildren<Animator>();
            _playerTransform = PlayerMovement.Main.transform;
            _navAgent = GetComponent<NavMeshAgent>();
            _navAgent.speed = speed;
        }
        
        private void Update()
        {
            var playerPosition = _playerTransform.position;

            if (_isDead) return;
            
            _navAgent.destination = _attacking ? transform.position : playerPosition;
            
            var distance = (transform.position - playerPosition).magnitude;

            if (distance <= attackRange)
            {
                _anim.SetBool(AttackBool, true);
                
                if (_attacking) return;
                
                _attacking = true;
                StartCoroutine(Attack());
            } else _anim.SetBool(AttackBool, false);
        }

        private IEnumerator Attack()
        {
            yield return new WaitForSeconds(1 / attackSpeed);
            var distance = (transform.position - _playerTransform.position).magnitude;

            if (distance <= attackRange)
                StatsManager.Health -= attackDamage;

            _attacking = false;
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

            StatsManager.Points += (int)(10 * multiplier);
            Health -= (int)(damage * multiplier);
        }

        private void Die()
        {
            _navAgent.destination = transform.position;
            _isDead = true;
            _anim.SetTrigger(DeathTrigger);
            
            foreach (var componentsInChild in GetComponentsInChildren<BoxCollider>())
            {
                componentsInChild.enabled = false;
            }
        }
    }
}