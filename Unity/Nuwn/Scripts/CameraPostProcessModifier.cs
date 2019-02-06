using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraPostProcessModifier : MonoBehaviour
{

    [SerializeField] private PostProcessVolume PPMainVolume = default;
    [SerializeField] private PostProcessVolume PPVolume = default;
    [SerializeField] private PostProcessProfile NormalPP = default;
    [SerializeField] private PostProcessProfile HurtPP = default;
    [SerializeField] private PostProcessProfile DeadPP = default;

    public static CameraPostProcessModifier Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private PostProcessProfile PPcurrent {
        get { return PPVolume.profile; }
        set
        {
            if(value == NormalPP)
            {
                StartCoroutine(Lerp(SetWeight, 1, 0, 0.3f));
            }
            else
            {
                PPVolume.profile = value;
                StartCoroutine(Lerp(SetWeight, 0, 1, 0.3f));
            }         
        }
    }
    private PostProcessProfile PPMain
    {
        get { return PPMainVolume.profile; }
        set
        {
            PPMainVolume.profile = value;
        }
    }

    Vignette vignette;
    ChromaticAberration ChromaticAberration = default;

    public void StartDeathEnding()
    {
        PPMain = HurtPP;
        PPcurrent = DeadPP;
    }
    public void SetDamaged()
    {
        PPMain = NormalPP;
        PPcurrent = HurtPP;
    }
    public void SetNormal()
    {
        PPMain = NormalPP;
        PPcurrent = NormalPP;
    }


























    // Functions
    void SetWeight(FloatParameter val)
    {
        PPVolume.weight = val;
    }


    public void PulsateChromaticAberration(float to, float time = 5)
    {
        StartCoroutine(Lerp(SetCAVal, ChromaticAberration.intensity.GetValue<float>(), to, time, (result) => { if (result) PulsateChromaticAberration(-to, time); }));
    }

    
    void SetCAVal(FloatParameter val)
    {
        ChromaticAberration.intensity.Override(val);
        ApplyChange(ChromaticAberration);
    }
    void SetVigVal(FloatParameter val)
    {
        vignette.intensity.Override(val);
        ApplyChange(vignette);
    }
    void ApplyChange(PostProcessEffectSettings obj)
    {
        PPVolume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, obj);
    }




    IEnumerator Lerp(Action<FloatParameter> val, float from, float to , float duration = 2, Action<bool> Callback = null)
    {
        var i = 0f;
        var rate = 1f / duration;

        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            val(new FloatParameter() { value = Mathf.Lerp(from, to, i) });
            yield return null;
        }
        val(new FloatParameter() { value = to } );
        Callback?.Invoke(true);
    }
 

}