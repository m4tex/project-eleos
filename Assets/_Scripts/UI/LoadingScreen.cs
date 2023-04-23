using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.UI
{
    public class LoadingScreen: MonoBehaviour
    {
        // public static LoadingScreen _instance;
        //
        // private void Awake()
        // {
        //     _instance = this;
        // }

        public static void LoadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}