using System;
using _Scripts.Interfaces;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Player
{
    public class Interaction : MonoBehaviour
    {
        private Transform _playerCamera;

        public float range;
        public KeyCode interactionKey;

        private void Start()
        {
            if (Camera.main == null) throw new Exception("Main Camera doesn't exist!!!");
            
            _playerCamera = Camera.main.transform;
        }

        private bool _promptShown;
        void Update()
        {
            Physics.Raycast(_playerCamera.position, _playerCamera.forward, out var hit, range);
        
            //Shows text when hovering and executes the interaction function when pressing a certain key
            if (hit.transform && hit.transform.TryGetComponent<IInteractable>(out var interactable))
            {
                UIManager.SetInteractionPrompt(interactionKey + " to " + interactable.IntPrompt);

                if (Input.GetKeyDown(interactionKey))
                {
                    interactable.Interaction.Invoke();
                }

                _promptShown = true;
            }
            else if (_promptShown)
            {
                UIManager.ClearInteractionPrompt();
                _promptShown = false;
            }
        }
    }
}