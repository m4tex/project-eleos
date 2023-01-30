using System;
using System.Collections;
using _Scripts.Player;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Interaction")] public Transform interactionPrompt;
        private TMP_Text _promptText;

        [Header("Weapon Shop")] public GameObject weaponShopUI;
        public GameObject insufficientFundsPrompt;
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
        }

        private static void CloseWeaponShop()
        {
            if (!ControllsLock) return;

            ControllsLock = false;
            _ins.weaponShopUI.SetActive(false);
            _ins.insufficientFundsPrompt.SetActive(false);
        }
        
        public static void ShowWeaponShopInsufficientFundsPrompt() //We like em long descriptive names xd
        {
            _ins.insufficientFundsPrompt.SetActive(true);
        }
        
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