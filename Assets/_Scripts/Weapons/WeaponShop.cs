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

            if (StatsManager.Points >= weaponComponent.wallPrice)
            {
                StatsManager.Points -= weaponComponent.wallPrice;
                LoadoutManager.instance.NewWeapon(weapon, LoadoutItem.Firearm);
            }
            else
            {
                UIManager.ShowWeaponShopInsufficientFundsPrompt();
            }
        }

        public void BuyAmmo()
        {
            var weapon = LoadoutManager.GetCurrentWeapon();
            var ammoPrice = GetAmmoPrice(weapon.ammoType);
            var missingAmmo = weapon.magazineCapacity + weapon.maxReserveAmmo - weapon.currentMagazine -
                              weapon.currentReserveAmmo;

            var ammoToBuy = (int)(StatsManager.Points / (missingAmmo * ammoPrice));

            if (ammoToBuy != 0)
            {
                StatsManager.Points -= (ammoPrice * ammoToBuy);
                weapon.currentReserveAmmo += ammoToBuy;
            }
            else
            {
                UIManager.ShowWeaponShopInsufficientFundsPrompt();
            }
        }

        private int GetAmmoPrice(AmmoType type)
        {
            return type switch
            {
                AmmoType.Pistol => handgunAmmoPrice,
                AmmoType.Rifle => rifleAmmoPrice,
                AmmoType.Shotgun => shotgunAmmoPrice,
                AmmoType.Smg => smgAmmoPrice,
                _ => -1
            };
        }
    }
}