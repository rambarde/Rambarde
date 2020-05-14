using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerHelper : MonoBehaviour
{
    public void PlayUIOneShot(string clipStr)
    {
        MusicManager.Instance?.PlayUIOneShot(clipStr);
    }


    public void PlayUI(string clipStr)
    {
        MusicManager.Instance?.PlayUI(clipStr);
    }
}
