using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Melodies;
using UniRx;
using UniRx.Triggers;
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
    
    public AudioClip combatPhase1;
    public AudioClip combatPhase2;
    public AudioClip combatPhase3;

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

    public void StartCombatMusic() {
        CombatManager.Instance.combatPhase.Subscribe(phase => {
            switch (phase) {
                case CombatPhase.SelectMelodies :
                    OSTSource[0].clip = combatPhase1;
                    OSTSource[0].Play();
                    break;
                case CombatPhase.ResovleFight :
                    OSTSource[0].clip = combatPhase3;
                    OSTSource[0].Play();
                    break;
                case CombatPhase.RhythmGame :
                    OSTSource[0].clip = combatPhase2;
                    OSTSource[0].Play();
                    break;
                case CombatPhase.ExecMelodies:
                    break;
                default:
                    break;
            }
        });
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

    internal async void PlayMelodies(List<Melody> selectedMelodies, double delay)
    {
        Debug.Log("play melodies");
        /*OSTSource[0].clip = combatPhase2;
        OSTSource[0].Play();*/
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
