using System;
using _Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Weapons
{
    public class WallWeapon : MonoBehaviour, IInteractable
    {
        public UnityEvent Interaction { get; set; } = new();
        public string IntPrompt { get; set; }

        //The fully scripted weapon object with disabled components
        public GameObject weapon;

        private void Start()
        {
            IntPrompt = $"Buy {weapon.GetComponent<Firearm>().weaponName} for $";
        }
    }
}
