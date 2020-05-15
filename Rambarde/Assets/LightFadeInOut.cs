using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightFadeInOut : MonoBehaviour
{
    public Light light;
    public float speed;
    private Sequence FadeInOut;
    // Start is called before the first frame update
    void Start()
    {
       FadeInOut = DOTween.Sequence();
       FadeInOut.Append(light.DOIntensity(2.8f, speed));
       FadeInOut.Append(light.DOIntensity(2.2f, speed));
       FadeInOut.SetLoops(-1);
       FadeInOut.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
