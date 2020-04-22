using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRenderMode : MonoBehaviour {

    public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
    void Start() {
        GetComponent<Canvas>().renderMode = renderMode;
    }
}
