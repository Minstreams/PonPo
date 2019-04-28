using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Focusor : MonoBehaviour
{
    PostProcessVolume volume;
    PostProcessProfile profile;
    List<PostProcessEffectSettings> settings;
    DepthOfField depthOfF;
    private void Awake()
    {
        volume = GetComponent<PostProcessVolume>();
        profile = volume.profile;
        settings = profile.settings;
        depthOfF = profile.GetSetting<DepthOfField>();
    }
    private void Update()
    {
        depthOfF.focusDistance.Override(-transform.position.z);
    }
}
