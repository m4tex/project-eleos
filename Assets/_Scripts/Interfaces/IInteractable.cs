using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Interfaces
{
    public delegate bool ConditionFunction();
    
    public interface IInteractable
    {
        UnityEvent Interaction { get; set; }
        string IntPrompt { get; set; }
        ConditionFunction Condition { get; set; }
    }
}