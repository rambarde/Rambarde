using System.Collections;
using System.Collections.Generic;
using Rambarde.SceneManagement;
using UnityEngine;

public class SceneManagerHelper : MonoBehaviour
{
    public void LoadScene(int scene)
    {
        SceneManager.Instance?.LoadScene(scene);
    }

    public void LoadScene(string scene)
    {
        SceneManager.Instance?.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}