using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Melodies;
using UniRx;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance => _instance;

    public AudioSource melodySource;
    public AudioSource SFXSource;
    public AudioSource UISource;
    public AudioSource[] OSTSource;
    public AudioClip melodyDefault;
    public AudioClip buzzClip;

    public void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }

    }



    public void PlayBuzz()
    {
        melodySource.volume = 0;

        SFXSource.PlayOneShot(buzzClip, 1);
        melodySource.DOFade(1, buzzClip.length);
    }

    public void PlaySfx(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }
    public async Task PlayUIOneShotTask(string clipStr)
    {
        AudioClip clip = await Utils.LoadResource<AudioClip>("Sound/" + clipStr);
        if (clip)
        {
            UISource.PlayOneShot(clip, 1);
        }
    }
    public void PlayUIOneShot(string clipStr)
    {
        _ = PlayUIOneShotTask(clipStr);
    }

    public async Task PlayUI(string clipStr)
    {
        AudioClip clip = await Utils.LoadResource<AudioClip>("Sound/" + clipStr);
        if (clip)
        {
            if (UISource.isPlaying)
            {
                UISource.Stop();
            }
            UISource.clip = clip;
            UISource.Play();
        }
    }

    internal async Task PlayMelodies(ReactiveCollection<Melody> selectedMelodies, double delay)
    {
        Debug.Log("play melodies");
        await Utils.AwaitObservable(Observable.Timer(TimeSpan.FromSeconds(delay)));
        foreach (var melody in selectedMelodies)
        {
            //Debug.Log(melody.clip.name ?? "default");
            melodySource.clip = melody.clip != null ? melody.clip : melodyDefault;
            melodySource.Play();
            await Utils.AwaitObservable(Observable.Timer(TimeSpan.FromSeconds(melodySource.clip.length)));
        }

    }
}
