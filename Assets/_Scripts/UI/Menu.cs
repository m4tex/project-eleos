using System;
using UnityEngine;

namespace _Scripts.UI
{
    public class Menu : MonoBehaviour
    {
        public GameObject creditsModal;
        private bool _creditsOpen = false;

        private void Update()
        {
            if (_creditsOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                creditsModal.SetActive(false);
                _creditsOpen = false;
            }
        }

        public void OnClick_Play()
        {
            LoadingScreen.LoadLevel();
        }
        public void OnClick_Credits()
        {
            creditsModal.SetActive(true);
            _creditsOpen = true;
        }
        public void OnClick_Quit()
        {
            Application.Quit();
        }
    }
}