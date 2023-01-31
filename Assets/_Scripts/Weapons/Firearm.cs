using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemies;
using _Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Weapons
{
    public enum AmmoType
    {
        Pistol,
        Smg,
        Shotgun,
        Rifle
    }

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(WeaponSway))]
    public class Firearm : MonoBehaviour
    {
        [Header("General")] 
        public int baseDamage = 20;
        public int range = 200;
        public float fireRate = 0.2f;
        public bool isFullAuto = true;
        public int magazineCapacity = 30;
        public int maxReserveAmmo = 120;
        public AmmoType ammoType = AmmoType.Pistol;
        public float reloadDuration = 3.2f;
        public float accuracy = 0.85f;
        public float spread = 2f;
        //public List<Attachment> attachments = new();

        //Counters
        private float _fireRateCounter;
        [HideInInspector]
        public int currentMagazine, currentReserveAmmo;

        [Header("Modifiers")] 
        public const float HeadShotMultiplier = 1.65f, BodyShotMultiplier = 1f, LegShotMultiplier = 0.75f;

        [Header("Wall Weapon Properties")]
        // public bool isWallWeapon = false;
        public int wallPrice = 1200;
        public string weaponName = "Weapon Name";

        [Header("Aiming")] 
        public float aimingSpeed = 0.6f;
        public int aimingZoomAmount = 8;
        public float aimingDistanceOffset;
        public float aimingHeight;

        [FormerlySerializedAs("pushback")] [Header("Animation")] 
        public float recoilPushback = 0.15f;
        public float recoilTorque = 6f;

        [Header("SFX")] 
        public List<AudioClip> soundClips;
        private AudioSource _audioSource;

        private bool _isReloading;
        private bool _isAiming;
        private TMP_Text _ammoIndicator;
        private Camera _camera;
        private WeaponSway _weaponSway;
        
        // Start is called before the first frame update
        private void Start()
        {
            _camera = PlayerCamera.main.GetComponent<Camera>();
            _audioSource = GetComponent<AudioSource>();
            _weaponSway = GetComponent<WeaponSway>();
            _weaponSway.aimSwayPos = new Vector3(0, aimingHeight, aimingDistanceOffset);
            
            currentMagazine = magazineCapacity;
            currentReserveAmmo = maxReserveAmmo;

            _ammoIndicator = GetComponentInChildren<TMP_Text>();
            _ammoIndicator.text = $"{currentMagazine} / {currentReserveAmmo}";
        }
        
        private void Update()
        {
            var input = isFullAuto ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

            if (input && currentMagazine != 0 && _fireRateCounter <= 0 && !PlayerMovement.Main.IsRunning)
                Shoot();

            if (((input && currentMagazine == 0) || Input.GetKeyDown(KeyCode.R)) 
                && currentReserveAmmo != 0 && currentMagazine != magazineCapacity && !_isReloading)
            {
                StartCoroutine(ReloadCoroutine());
            }

            if (Input.GetMouseButtonDown(1) && !_isReloading)
            {
                StartAiming();
            }

            if (Input.GetMouseButtonUp(1) || (PlayerMovement.Main.IsRunning && _isAiming))
            {
                StopAiming();
            }

            Counters();
        }

        private void StartAiming()
        {
            _isAiming = true;
            _weaponSway.isAiming = true;
            PlayerCamera.main.ChangeZoom(aimingZoomAmount, aimingSpeed);
        }

        private void StopAiming()
        {
            _isAiming = false;
            _weaponSway.isAiming = false;
            PlayerCamera.main.ChangeZoom(-aimingZoomAmount, aimingSpeed);
        }
        
        private void Shoot()
        {
            var weaponTransform = transform;
            weaponTransform.localPosition -= new Vector3(0, 0, recoilPushback);
            weaponTransform.localEulerAngles -= new Vector3(recoilTorque, 0, 0);

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit, range))
            {
                if (hit.transform.TryGetComponent<Zombie>(out var zombie))
                {
                    zombie.TakeDamage(baseDamage, hit.point.y);
                }
            }

            //TODO bullet holes

            currentMagazine--;
            _ammoIndicator.text = $"{currentMagazine} / {currentReserveAmmo}";
            
            _fireRateCounter = fireRate;
        }

        private IEnumerator ReloadCoroutine()
        {
            _isReloading = true;
            _weaponSway.enabled = false;

            float elapsed = 0;
            float length = reloadDuration / 6;
            Vector3 initialPosition = transform.localPosition;
            Vector3 targetPosition = initialPosition - new Vector3(0, 0.5f, 0);

            while (elapsed < length)
            {
                var easedInDelta = Mathf.Pow(elapsed / length, 4);
                elapsed += Time.deltaTime;

                transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, easedInDelta);
                yield return null;
            }
            
            yield return new WaitForSeconds(reloadDuration);

            if (currentReserveAmmo >= magazineCapacity - currentMagazine)
            {
                currentReserveAmmo -= magazineCapacity - currentMagazine;
                currentMagazine = magazineCapacity;
            }
            else
            {
                currentMagazine += currentReserveAmmo;
                currentReserveAmmo = 0;
            }

            _ammoIndicator.text = $"{currentMagazine} / {currentReserveAmmo}";
            _isReloading = false;
            _weaponSway.enabled = true;
        }

        private void Counters()
        {
            if (_fireRateCounter > 0)
                _fireRateCounter -= Time.deltaTime;
        }
    }
}