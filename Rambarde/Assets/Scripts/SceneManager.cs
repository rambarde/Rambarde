namespace Rambarde.SceneManagement
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
        private static SceneManager _instance;
        public static SceneManager Instance => _instance;
        private void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(splashScreen);
                DontDestroyOnLoad(gameObject);
                _instance = this;
            }
        }

        private void Start()
        {
            MusicManager.Instance.PlayOst("Ost" + UnitySceneManger.GetActiveScene().buildIndex);
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
            splashScreen.gameObject.SetActive(true);
            float alpha = splashScreen.alpha;
            splashScreen.blocksRaycasts = true;
            // fadeIn splashScreen
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime)
            {
                splashScreen.alpha = t;
                yield return null;
            }

            splashScreen.alpha = 1.0f;
            AsyncOperation unLoading = UnitySceneManger.UnloadSceneAsync(UnitySceneManger.GetActiveScene().buildIndex);
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
            splashScreen.gameObject.SetActive(false);
        }

        IEnumerator LoadSceneAsync(int sceneName)
        {
            splashScreen.gameObject.SetActive(true);
            float alpha = splashScreen.alpha;
            splashScreen.blocksRaycasts = true;
            // fadeIn splashScreen
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime)
            {
                splashScreen.alpha = t;
                yield return null;
            }

            splashScreen.alpha = 1.0f;
            AsyncOperation unLoading = UnitySceneManger.UnloadSceneAsync(UnitySceneManger.GetActiveScene().buildIndex);
            AsyncOperation loading = UnitySceneManger.LoadSceneAsync(sceneName);
            while (!loading.isDone)
            {
                yield return null;
            }
            MusicManager.Instance.PlaySceneOst(UnitySceneManger.GetActiveScene().buildIndex);
            // fadeOut splashScreen
            for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime)
            {
                splashScreen.alpha = t;
                yield return null;
            }
            splashScreen.blocksRaycasts = false;
            splashScreen.alpha = 0.0f;
            splashScreen.gameObject.SetActive(false);
        }
    }
}
