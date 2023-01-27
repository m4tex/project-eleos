using System;
using UnityEngine;

public enum LoadoutItem
{
    Firearm, Grenade, Special
}

//Responsible for tracking the current loadout setup and weapon switching
namespace _Scripts.Weapons
{
    public class LoadoutManager : MonoBehaviour
    {
        public static LoadoutManager Instance;
        
        private int _currentWeaponIndex = 0;
        private int _lastWeaponIndex = 0;

        public Firearm primary;
        //Doesn't necessarily mean a pistol
        public Firearm secondary;
        
        //public Grenade fragGrenades;
        //public Grenade stunGrenades;
        //public Grenade specialGrenades; IDK
        //private Grenade[] grenades; FOR CYCLING BETWEEN GRENADES (MAKES IT EASIER I GUESS)
        
        //public List<Special> 
        
        private Component[] _loadout;

        [Header("Weapon Switching")] 
        public float weaponSwitchDelay = 0.2f;
        private float _weaponSwitchCounter;
        
        // Start is called before the first frame update
        private void Start()
        {
            Instance = this;
            
            _loadout = new Component[2]; //Add grenades.length
            primary = GetComponentInChildren<Firearm>();
            _loadout[0] = primary;
        }

        private void UpdateLoadout()
        {
            _loadout[0] = primary;
            _loadout[1] = secondary;
            //Grenades
        }
        
        private void Update()
        {
            Inputs();
            
            //Counters
            if (_weaponSwitchCounter > 0)
            {
                _weaponSwitchCounter -= Time.deltaTime;
            }
        }

        private void ToggleItem(int index)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison //Not triggered all that often to matter
            if (_loadout[index] == null || index == _currentWeaponIndex)
                return;
            
            foreach (var item in _loadout)
                item.gameObject.SetActive(false);
            
            _loadout[index].gameObject.SetActive(true);

            _lastWeaponIndex = _currentWeaponIndex;
            _currentWeaponIndex = index;
            _weaponSwitchCounter = weaponSwitchDelay;
        }

        public void NewWeapon(GameObject weapon, LoadoutItem item)
        {
            switch (item)
            {
                case LoadoutItem.Firearm:
                    var weaponComponent = weapon.GetComponent<Firearm>();
                    if (secondary != null && (_currentWeaponIndex == 0 || (_currentWeaponIndex != 1 && _lastWeaponIndex == 0)))
                        primary = weaponComponent;
                    else
                        secondary = weaponComponent;
                    break;
                case LoadoutItem.Grenade:
                    
                    break;
                
                case LoadoutItem.Special:
                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item, null);
            }
            
            UpdateLoadout();
        }
        
        private void Inputs()
        {
            if (_weaponSwitchCounter > 0)
                return;
                
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToggleItem(0);
            } 
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ToggleItem(1);
            } 
            else if (Input.GetKeyDown(KeyCode.G))
            {
                //Cycle through grenades
            }
        }
    }
}