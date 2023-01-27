using UnityEngine;

namespace _Scripts.Weapons
{
    public class Firearm : MonoBehaviour
    {
        [Header("General")] 
        public float baseDamage;

        public bool isFullAuto;
        public int magazineCapacity, currentMagazine;
        public float reloadDuration;
        private float reloadDurationCounter;
        
        
        
        [Header("Modifiers")]
        [Range(1, 2.5f)] 
        public float headShotMultiplier, bodyShotMultiplier, legShotMultiplier;

        [Header("Wall Weapon Properties")] 
        public bool isWallWeapon;
        public int wallPrice;
        public string weaponName;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
