using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enemies;
using _Scripts.Player;
using _Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
        public float impactForce = 200;
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
        // public float aimingSpeed = 0.6f;
        public int aimingZoomAmount = 8;
        public float aimingDistanceOffset;
        public float aimingHeight;

        [FormerlySerializedAs("pushback")] [Header("Animation")] 
        public float recoilPushback = 0.15f;
        public float recoilTorque = 6f;

        [Header("SFX")] 
        public List<AudioClip> soundClips;
        public AudioClip reloadingClip, emptyMagClip;
        private AudioSource _audioSource;

        private bool _isReloading;
        private bool _isAiming;
        private TMP_Text _ammoIndicator;
        private Camera _camera;
        private WeaponSway _weaponSway;

        [Header("Shotgun (leave default for non-shotguns)")]
        public int multishotCount = 1;
        public float multishotSpread = 0;

        // Start is called before the first frame update
        private void Start()
        {
            _camera = PlayerCamera.Main.GetComponent<Camera>();
            _audioSource = GetComponent<AudioSource>();
            _weaponSway = GetComponent<WeaponSway>();
            _weaponSway.aimSwayPos = new Vector3(0, aimingHeight, aimingDistanceOffset);
            
            currentMagazine = magazineCapacity;
            currentReserveAmmo = maxReserveAmmo;

            _ammoIndicator = GetComponentInChildren<TMP_Text>();
            UpdateAmmo();
        }

        private void OnEnable()
        {
            PlayerCamera.Main.weaponZoomFovAmount = aimingZoomAmount;
        }
        

        private void Update()
        {
            if (UIManager.ControllsLock) return;

            var input = isFullAuto ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

            if (_fireRateCounter <= 0 && !PlayerMovement.Main.isRunning)
            {
                if (input && currentMagazine != 0)
                {
                    //Pitch variation?
                    var clip = soundClips[Random.Range(0, soundClips.Count)];
                    _audioSource.PlayOneShot(clip);
                    
                    if (multishotCount == 1)
                    {
                        Shoot(_camera.transform.forward);
                    }
                    else
                    {
                        for (int i = 0; i < multishotCount; i++)
                        {
                            Vector3 direction = _camera.transform.forward;
                            Vector3 offset = new Vector3(Random.Range(-multishotSpread, multishotSpread), Random.Range(-multishotSpread, multishotSpread), 0);

                            direction += offset;
                            
                            Shoot(direction, true);
                        }
                    }
                    currentMagazine--;
                    UpdateAmmo();
                }
                //Auto reload removes that option...
                // else if (Input.GetMouseButtonDown(0))
                //     _audioSource.PlayOneShot(emptyMagClip);
            }

            if (((input && currentMagazine == 0) || Input.GetKeyDown(KeyCode.R)) 
                && currentReserveAmmo != 0 && currentMagazine != magazineCapacity && !_isReloading)
            {
                Reload();
            }

            if (Input.GetMouseButtonDown(1) && !_isReloading)
            {
                StartAiming();
            }

            if (Input.GetMouseButtonUp(1) || (PlayerMovement.Main.isRunning && _isAiming))
            {
                StopAiming();
            }

            Counters();
        }

        private void StartAiming()
        {
            _isAiming = true;
            _weaponSway.isAiming = true;
            PlayerCamera.Main.weaponZoom = true;
        }

        private void StopAiming()
        {
            _isAiming = false;
            _weaponSway.isAiming = false;
            PlayerCamera.Main.weaponZoom = false;
        }
        
        private void Shoot(Vector3 dir, bool drawRays = false)
        {
            var direction = dir;

            var weaponTransform = transform;
            weaponTransform.localPosition -= new Vector3(0, 0, recoilPushback);
            weaponTransform.localEulerAngles -= new Vector3(recoilTorque, 0, 0);

            if (Physics.Raycast(_camera.transform.position, direction, out var hit, range))
            {
                if (hit.transform.root.TryGetComponent<Zombie>(out var zombie))
                {
                    zombie.TakeDamage(baseDamage, hit.point.y);
                } else if (hit.transform.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.AddForce(direction * impactForce);
                }
            }

            if (drawRays)
                Debug.DrawLine(_camera.transform.position, hit.point, Color.green, 8f);

            //TODO bullet holes
            
            _fireRateCounter = fireRate;
        }

        private IEnumerator ReloadCoroutine()
        {
            _audioSource.PlayOneShot(reloadingClip);
            
            _isReloading = true;
            _weaponSway.enabled = false;

            float elapsed = 0;
            float length = reloadDuration / 6;
            Vector3 initialPosition = transform.localPosition;
            Vector3 targetPosition = initialPosition - new Vector3(0, 1.5f, 0);

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

            UpdateAmmo();
            _isReloading = false;
            _weaponSway.enabled = true;
        }

        private void Counters()
        {
            if (_fireRateCounter > 0)
                _fireRateCounter -= Time.deltaTime;
        }

        public void UpdateAmmo()
        {
            _ammoIndicator.text = $"{currentMagazine} / {currentReserveAmmo}";
        }

        public void SmallRefill(int magazineCount)
        {
            currentReserveAmmo += magazineCount * magazineCapacity;
            UpdateAmmo();
        }

        public void Reload()
        {
            StartCoroutine(ReloadCoroutine());
        }
    }
}