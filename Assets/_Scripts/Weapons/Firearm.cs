using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Weapons
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Animator))]
    public class Firearm : MonoBehaviour
    {
        [Header("General")] 
        public float baseDamage = 20f;

        public float fireRate = 0.2f;
        public bool isFullAuto = true;
        public int magazineCapacity = 30;
        public float reloadDuration = 3.2f;
        public float accuracy = 0.85f;
        public float spread = 2f;
        //public List<Attachment> attachments = new();
        
        //Counters
        private float _reloadDurationCounter, _fireRateCounter, _currentMagazine;
        
        [Header("Modifiers")]
        [Range(0.25f, 2.5f)] 
        public float headShotMultiplier = 1.65f;
        
        [Range(0.25f, 2.5f)]
        public float bodyShotMultiplier = 1f, legShotMultiplier = 0.75f;

        [Header("Wall Weapon Properties")] 
        // public bool isWallWeapon = false;
        public int wallPrice = 1200;
        public string weaponName = "Weapon Name";

        [Header("Aiming")] 
        public float scopingSpeed = 0.6f;

        [Header("Animation")] 
        private Animator _animator;

        public AnimationClip shootingAnimation;
        public AnimationClip reloadingAnimation;

        [Header("SFX")] 
        public List<AudioClip> soundClips;
        
        private AudioSource _audioSource;
        
        // Start is called before the first frame update
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
