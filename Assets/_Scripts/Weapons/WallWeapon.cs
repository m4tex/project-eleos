using System;
using _Scripts.Interfaces;
using _Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Weapons
{
    public class WallWeapon : MonoBehaviour, IInteractable
    {
        public UnityEvent Interaction { get; set; } = new();
        public string IntPrompt { get; set; }

        //The fully scripted weapon object with disabled components
        private Firearm _weapon;

        private void Start()
        {
            _weapon = GetComponentInChildren<Firearm>();
            IntPrompt = $"Buy {_weapon.weaponName} for ${_weapon.wallPrice}";
            Interaction.AddListener(() =>
            {
                LoadoutManager.instance.NewWeapon(_weapon.gameObject, LoadoutItem.Firearm);
                StatsManager.Points -= _weapon.wallPrice;
            });
        }
    }
}
