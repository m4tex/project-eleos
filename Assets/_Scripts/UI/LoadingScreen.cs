using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts.UI
{
    public class LoadingScreen: MonoBehaviour
    {
        public static LoadingScreen _instance;

        public List<Image> backgroundImages;
        public Slider progressBar;
        public float fadeLength;

        private Image _bg;
        private void Start()
        {
            _instance = this;

            _bg = backgroundImages[Random.Range(0, backgroundImages.Count)];
        }

        public static void LoadLevel()
        {
            _instance.StartCoroutine(_instance.Load());
        }

        private IEnumerator Load()
        {
            var operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            
            while (!operation.isDone)
            {
                
                
                yield return null;
            }
            
            progressBar.gameObject.SetActive(false);

            Color color = new Color(255, 255, 255, 0);
            _bg.material.MaterialFade(color, fadeLength, this);
        }
    }
}