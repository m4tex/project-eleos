using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using _Scripts.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts.UI
{
    public delegate void OnLoadEvent();
    
    public class LoadingScreen: MonoBehaviour
    {
        public static LoadingScreen _instance;

        public List<Sprite> backgroundImages;
        public Image background;
        public Slider progressBar;
        public float fadeLength;
        
        private void Start()
        {
            if (_instance != null)
                Destroy(_instance.gameObject);
            
            _instance = this;

            background.sprite = backgroundImages[Random.Range(0, backgroundImages.Count)];
            DontDestroyOnLoad(gameObject);
        }

        public static void LoadLevel(OnLoadEvent whenDone, int index)
        {
            if (!_instance.gameObject.activeSelf)
                _instance.gameObject.SetActive(true);
                
            _instance.StartCoroutine(_instance.Load(whenDone, index));
        }

        private IEnumerator Load(OnLoadEvent whenDone, int index)
        {
            background.gameObject.SetActive(true);
            progressBar.gameObject.SetActive(true);
            
            var operation = SceneManager.LoadSceneAsync(index);
            
            while (!operation.isDone)
            {
                progressBar.value = operation.progress / .9f;
                yield return null;
            }
            
            progressBar.gameObject.SetActive(false);

            for (float i = 0; i < fadeLength; i += Time.deltaTime)
            {
                background.color = new Color(255, 255, 255, 1 - (i / fadeLength));
            }

            gameObject.SetActive(false);
            whenDone();
        }
    }
}