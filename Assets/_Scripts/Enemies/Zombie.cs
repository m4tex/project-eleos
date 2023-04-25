using System;
using System.Collections;
using _Scripts.Managers;
using _Scripts.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Scripts.Enemies
{
    public enum ZType
    {
        Regular,
        Spider,
        Crawler,
        Big
    }

    public enum ZState
    {
        crawling,
        woundedCrawl,
        regular,
        climbing
    }
    
    [RequireComponent(typeof(NavMeshAgent))]
    public class Zombie : MonoBehaviour
    {
        private NavMeshAgent _navAgent;
        private Animator _anim;
        private static readonly int CrawlBool = Animator.StringToHash("Crawl");
        private static readonly int CLimbBool = Animator.StringToHash("Climb");
        private static readonly int AttackBool = Animator.StringToHash("Attack");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");


        private int _health = 100;
        public int Health
        {
            get => _health;
            set
            {
                _health = value;
                
                if (_health <= 0) Die();
            }
        }
        public float speed = 3f;
        public float attackRange;
        public float attackSpeed;
        public int attackDamage;

        private bool _attacking = false;
        private bool _isDead = false;
        private bool _legshot = false;

        private static Transform _playerTransform;

        private void Awake() => _anim = GetComponentInChildren<Animator>();
        private void Start()
        {
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

        private void Die()
        {
            _navAgent.enabled = false;
            _isDead = true;
            _anim.SetTrigger(DeathTrigger);
            StopAllCoroutines();
            
            foreach (var componentsInChild in GetComponentsInChildren<BoxCollider>())
            {
                componentsInChild.enabled = false;
            }

            LevelManager.Instance.zombies.Remove(gameObject);
        }

        public void Legshot()
        {
            if (_legshot) return;

            if (Random.Range(0, 7) == 0)
                SetState(ZState.woundedCrawl);
            
            _legshot = true;
        }

        public void SetState(ZState state)
        {
            switch (state)
            {
                case ZState.crawling:
                case ZState.woundedCrawl:
                    _anim.SetBool(CrawlBool, true);
                    _anim.SetBool(CLimbBool, false);
                    break;
                case ZState.regular:
                    _anim.SetBool(CrawlBool, false);
                    _anim.SetBool(CLimbBool, false);
                    break;
                case ZState.climbing:
                    _anim.SetBool(CrawlBool, false);
                    _anim.SetBool(CLimbBool, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Walk"))
                SetState(ZState.regular);
        }
    }
}