using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Interaction")] 
        public Transform interactionPrompt;
        private TMP_Text _promptText;

        private static UIManager _ins;
        private void Awake()
        {
            _ins = this;
            
            _promptText = interactionPrompt.GetComponentInChildren<TMP_Text>();
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
    }
}