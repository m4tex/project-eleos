using System.Collections;
using System.Collections.Generic;
using _Scripts.Interfaces;
using _Scripts.Player;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Weapons
{
    public class WeaponShop : MonoBehaviour, IInteractable
    {
        public UnityEvent Interaction { get; set; } = new UnityEvent();
        public string IntPrompt { get; set; }
        public ConditionFunction Condition { get; set; }

        public int handgunAmmoPrice = 5;
        public int smgAmmoPrice = 8;
        public int shotgunAmmoPrice = 15;
        public int rifleAmmoPrice = 10;
        
        private void Start()
        {
            IntPrompt = "Open Armory";
            Interaction.AddListener(UIManager.OpenWeaponShop);
        }

        public void BuyWeapon(GameObject weapon)
        {
            var weaponComponent = weapon.GetComponent<Firearm>();

            if (StatsManager.Points < weaponComponent.wallPrice) return;
            
            StatsManager.Points -= weaponComponent.wallPrice;
            LoadoutManager.instance.NewWeapon(weapon, LoadoutItem.Firearm);
            
            UIManager.UpdateShop();
        }

        // public void BuyAmmo()
        // {
        //     var weapon = LoadoutManager.GetCurrentWeapon();
        //     var ammoPrice = GetAmmoPrice(weapon.ammoType);
        //     
        //     var missingAmmo = weapon.magazineCapacity + weapon.maxReserveAmmo - weapon.currentMagazine -
        //                       weapon.currentReserveAmmo;
        //
        //     var maxAmount = StatsManager.Points / ammoPrice;
        //
        //     if (maxAmount >= missingAmmo)
        //     {
        //         StatsManager.Points -= missingAmmo * ammoPrice;
        //         weapon.currentReserveAmmo += missingAmmo;
        //         weapon.UpdateAmmo();
        //     }
        //     else if (maxAmount > 0)
        //     {
        //         StatsManager.Points -= maxAmount * ammoPrice;
        //         weapon.currentReserveAmmo += maxAmount;
        //         weapon.UpdateAmmo();
        //     }
        //     
        //     UIManager.UpdateShop();
        // }

        // private int GetAmmoPrice(AmmoType type)
        // {
        //     return type switch
        //     {
        //         AmmoType.Pistol => handgunAmmoPrice,
        //         AmmoType.Rifle => rifleAmmoPrice,
        //         AmmoType.Shotgun => shotgunAmmoPrice,
        //         AmmoType.Smg => smgAmmoPrice,
        //         _ => -1
        //     };
        // }
    }
}