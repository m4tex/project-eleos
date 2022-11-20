using _Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Prototypes
{
    public class TestInteractable : MonoBehaviour, IInteractable
    {
        
        public UnityEvent Interaction { get; set; } = new UnityEvent();
        
        public string IntPrompt { get; set; }

        private void Start()
        {
            IntPrompt = "Interact with Cube";
            Interaction.AddListener(Hello);
        }

        private void Hello()
        {
            print("Hello, World!");
        }
    }
}