using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static Transform loadingScreen;
    public static void LoadScene(int sceneIndex)
    {
        _loadScene(sceneIndex);
        x
    }

    private static IEnumerable _loadScene(int sceneIndex)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.GetComponentInChildren<Slider>().value = loadOperation.progress / .9f;
        
        
        yield return null;
    }
}
