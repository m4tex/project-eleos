using System;
using System.Collections;
using _Scripts.Interfaces;
using _Scripts.Player;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Prototypes
{
    public class TestInteractable : MonoBehaviour, IInteractable
    {
        public UnityEvent Interaction { get; set; } = new();
        
        public string IntPrompt { get; set; }
        
        private void Start()
        {
            IntPrompt = "Interact with the Cube of Ultraspeed";
            Interaction.AddListener(() => StatsManager.AddEffect(StatsManager.EffectType.Speed, 5, 5));
        }
    }
}