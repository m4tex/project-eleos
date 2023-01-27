using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum WeaponType
{
    Regular, Shotgun
}

namespace _Scripts.Weapons
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Animator))]
    public class Weapon : MonoBehaviour
    {
        [Header("General")]
        public WeaponType weaponType;
        
        public float baseDamage;
        public float fireRate; 
        public bool automatic;
        public float reloadTime;
        public float capacity;
        //public List<Attachment> attachments = new();

        private float _currentMagazine;
        private float _fireRateCounter;
        private float _reloadTimeCounter;

        [Header("Aiming")] 
        public float aimSpeed;
        
        
        [Header("Animation")]
        private Animator _animator;

        public AnimationClip shootingAnimation;
        public AnimationClip reloadingAnimation;
        

        [Header("SFX")] 
        public List<AudioClip> soundClips;
        private AudioSource _audioSource;
        
        private void Update()
        {
            if ((automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0)) && _currentMagazine > 0 && _fireRateCounter == 0)
            {
                
            }
        }
    }
}
