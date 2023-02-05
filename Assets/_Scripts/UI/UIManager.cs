using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Player;
using _Scripts.Weapons;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Interaction")] public Transform interactionPrompt;
        private TMP_Text _promptText;

        [Header("Weapon Shop")] public GameObject weaponShopUI;
        // public Button refillAmmoButton;
        public List<Button> weaponButtons = new();

        //UI Shop will be temporary and thus this table, while not very well implemented, does the job.
        private static readonly int[] WeaponPriceLookupTable = 
        {
            500, 1050, 1450, 1700, 2400, 3600, 5200
        };

        private static bool _controllsLock;

        public static bool ControllsLock
        {
            get => _controllsLock;
            private set
            {
                Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.Locked;
                Cursor.visible = value;
                _controllsLock = value;
            }
        }

        [Header("Stats")] 
        public TMP_Text pointsIndicator;

        private static UIManager _ins;

        private void Awake()
        {
            _ins = this;

            _promptText = interactionPrompt.GetComponentInChildren<TMP_Text>();
        }

        public static void UpdatePoints(int points)
        {
            _ins.pointsIndicator.text = $"Points: {points}";
        }

        public static void SetInteractionPrompt(string prompt)
        {
            _ins.interactionPrompt.gameObject.SetActive(true);
            _ins._promptText.text = prompt;
        }

        public static void ClearInteractionPrompt()
        {
            _ins.interactionPrompt.gameObject.SetActive(false);
            _ins._promptText.text = "";
        }

        public static void OpenWeaponShop()
        {
            if (ControllsLock) return;

            ControllsLock = true;
            _ins.weaponShopUI.gameObject.SetActive(true);

            UpdateShop();
        }

        private static void CloseWeaponShop()
        {
            if (!ControllsLock) return;

            ControllsLock = false;
            _ins.weaponShopUI.SetActive(false);
        }

        public static void UpdateShop()
        {
            for (var i = 0; i < WeaponPriceLookupTable.Length; i++)
            {
                _ins.weaponButtons[i].interactable = StatsManager.Points >= WeaponPriceLookupTable[i];
            }
            
            // //TODO add ammo prices to the button text
            //
            // _ins.refillAmmoButton.GetComponentInChildren<TMP_Text>().text = $"Refill ammo ${123132}";
        }
        
        // public static void ShowWeaponShopInsufficientFundsPrompt() //We like em long descriptive names xd
        // {
        //     _ins.StartCoroutine(SetActiveForSeconds(_ins.insufficientFundsPrompt, 1.2f));
        // }
        //
        // private static IEnumerator SetActiveForSeconds(GameObject element, float seconds)
        // {
        //     element.SetActive(true);
        //     yield return new WaitForSeconds(seconds);
        //     element.SetActive(false);
        // }

        private void Update()
        {
            if (ControllsLock && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseWeaponShop();
                //Close pause menu
            }
        }
    }
}