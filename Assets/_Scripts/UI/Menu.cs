using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [BoxGroup("Menu Panels")]
    public Transform newGamePanel, creditsPanel, savesPanel, optionsPanel, coopPanel;

    #region Button Functions
    
    public void OnClick_NewGame()
    {
        // SaveManager.NewGame();
        // LoadingManager.LoadScene()
    }
    public void OnClick_Play()
    {
        
    }
    public void OnClick_Coop()
    {
        
    }
    public void OnClick_Saves()
    {
        
    }
    public void OnClick_Options()
    {
        
    }
    public void OnClick_Credits()
    {
        
    }
    public void OnClick_Quit()
    {
        
    }
    
    #endregion
}