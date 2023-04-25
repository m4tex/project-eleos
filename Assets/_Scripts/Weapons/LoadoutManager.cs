using System;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Weapons
{
    public enum LoadoutItem
    {
        Firearm, Grenade, Special
    }

//Responsible for tracking the current loadout setup and weapon switching
    public class LoadoutManager : MonoBehaviour
    {
        public static LoadoutManager instance;
        
        private int _currentWeaponIndex = 0;
        private int _lastWeaponIndex = 0;

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
            instance = this;
                
            
            _loadout = new Component[2]; //Add grenades.length
            _loadout[0] = GetComponentInChildren<Firearm>();
        }

        private void Update()
        {
            Inputs();
            
            //Counters
            if (_weaponSwitchCounter > 0)
                _weaponSwitchCounter -= Time.deltaTime;
        }

        private void ToggleItem(int index)
        {
            if (_loadout[index] == null || index == _currentWeaponIndex)
                return;

            GetCurrentWeapon().AbortReload();
            
            foreach (var item in _loadout)
            {
                item.gameObject.SetActive(false);
            }
            
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
                    if (_loadout[1] != null &&
                        (_currentWeaponIndex == 0 || (_currentWeaponIndex != 1 && _lastWeaponIndex == 0)))
                    {
                        SetWeapon(weapon, 0);
                    }
                    else
                    {
                        SetWeapon(weapon, 1);
                        ToggleItem(1);
                    }
                    
                    break;
                case LoadoutItem.Grenade:
                    
                    break;
                
                case LoadoutItem.Special:
                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item, null);
            }
        }

        private void SetWeapon(GameObject weaponObject, int index)
        {
            if (_loadout[index] != null)
                Destroy(_loadout[index].gameObject);

            GameObject weapon = Instantiate(weaponObject, transform);
            _loadout[index] = weapon.GetComponent<Firearm>();
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

        public static Firearm GetCurrentWeapon()
        {
            return instance._loadout[instance._currentWeaponIndex].GetComponent<Firearm>();
        }

        public static void RefillAllWeapons()
        {
            foreach (Component weapon in instance._loadout)
            {
                if (weapon == null || weapon.GetType() != typeof(Firearm))
                    continue;

                Firearm component = (Firearm) weapon;


                if (component.currentReserveAmmo <
                    component.maxReserveAmmo + component.magazineCapacity - component.currentMagazine)
                {
                    component.currentReserveAmmo = component.maxReserveAmmo + component.magazineCapacity -
                                                   component.currentReserveAmmo;
                
                    GetCurrentWeapon().Reload();
                }
                
                component.UpdateAmmo();
            }
        }
    }
}