namespace Eden.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Serialization;
    using UnitySceneManger = UnityEngine.SceneManagement.SceneManager;

    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup splashScreen;

        private void Start()
        {
            DontDestroyOnLoad(splashScreen);
            DontDestroyOnLoad(this);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        public void LoadScene(int sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            float alpha = splashScreen.alpha;
            splashScreen.blocksRaycasts = true;
            // fadeIn splashScreen
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime)
            {
                splashScreen.alpha = t;
                yield return null;
            }

            splashScreen.alpha = 1.0f;
            AsyncOperation loading = UnitySceneManger.LoadSceneAsync(sceneName);
            while (!loading.isDone)
            {
                yield return null;
            }

            // fadeOut splashScreen
            for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime)
            {
                splashScreen.alpha = t;
                yield return null;
            }
            splashScreen.blocksRaycasts = false;
            splashScreen.alpha = 0.0f;
        }
        IEnumerator LoadSceneAsync(int sceneName)
        {
            float alpha = splashScreen.alpha;

            // fadeIn splashScreen
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime)
            {
                splashScreen.alpha = t;
                yield return null;
            }

            splashScreen.alpha = 1.0f;
            AsyncOperation loading = UnitySceneManger.LoadSceneAsync(sceneName);
            while (!loading.isDone)
            {
                yield return null;
            }

            // fadeOut splashScreen
            for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime)
            {
                splashScreen.alpha = t;
                yield return null;
            }

            splashScreen.alpha = 0.0f;
        }
    }
}
