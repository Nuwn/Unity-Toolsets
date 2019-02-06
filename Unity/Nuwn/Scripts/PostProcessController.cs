using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessController : MonoBehaviour
{
    PostProcessVolume volume;
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
    }


    public void SwapPP(PostProcessProfile profile)
    {
        volume.profile = profile;
    }


    IEnumerator Lerp(Action<FloatParameter> val, float from, float to, float duration = 2, Action<bool> Callback = null)
    {
        var i = 0f;
        var rate = 1f / duration;

        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            val(new FloatParameter() { value = Mathf.Lerp(from, to, i) });
            yield return null;
        }
        val(new FloatParameter() { value = to });
        Callback?.Invoke(true);
    }
}
