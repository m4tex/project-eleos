using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum GameScene
{
    Menu = 0,
    Level1 =1,
}

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene()
    {
        SceneManager.LoadSceneAsync();
    }
}
