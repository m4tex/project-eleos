using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Interfaces
{
    public interface IInteractable
    {
        UnityEvent Interaction { get; set; }
        string IntPrompt { get; set; }
    }
}