using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class FootSteps : MonoBehaviour
{

    public LayerMask Layers;
    public FootSoundSO[] FootSoundObjects;
    public FootSoundSO DefaultSO;
    public Transform LeftFoot;
    public Transform RightFoot;
    private AudioSource AudioSource;
    private Animator anim;

    public bool UseEvents = false;
    public UnityEvent OnSoundPlay;

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!UseEvents)
            CurveFootStep();
    }

    float RFSC;
    float RightFootStepCurve{
        get { return RFSC;  }
        set {
            if (RFSC > 0 && value < 0)
                PlaySound(RightFoot);

            RFSC = value;
         }
    }
    float LFSC;
    float LeftFootStepCurve
    {
        get { return LFSC; }
        set
        {
            if (LFSC < 0 && value > 0)
                PlaySound(LeftFoot);

            LFSC = value;
         }
    }
    void CurveFootStep()
    {
        RightFootStepCurve = anim.GetFloat("RightFootStep");
        LeftFootStepCurve = anim.GetFloat("LeftFootStep");
    }

    void PlaySound(Transform foot)
    {
        FootSoundSO FootSteps = GetData(foot);
        PlayRandomSound(FootSteps);
    }

    public void LeftFootStep()
    {
        if (!UseEvents)
            return;

        FootSoundSO FootSteps = GetData(LeftFoot);
        PlayRandomSound(FootSteps);
    }

    public void RightFootStep()
    {
        if (!UseEvents)
            return;

        FootSoundSO FootSteps = GetData(RightFoot);
        PlayRandomSound(FootSteps);
    }

    private void PlayRandomSound(FootSoundSO footSteps)
    {
        if(footSteps != null)
        {
            AudioSource.Stop();
            int getRandom = UnityEngine.Random.Range(0, footSteps.sounds.Length - 1);
            AudioSource.volume = footSteps.Volume + UnityEngine.Random.Range(-footSteps.VolumeVariance, footSteps.VolumeVariance);
            AudioSource.pitch = footSteps.Pitch + UnityEngine.Random.Range(-footSteps.PitchVariance, footSteps.PitchVariance);  
            AudioSource.PlayOneShot(footSteps.sounds[getRandom]);
            OnSoundPlay.Invoke();
        }
    }
    
    private FootSoundSO GetData(Transform FromPos)
    {
        var tex = GetTexture(FromPos);
        FootSoundSO SO;

        if (tex != null)
            SO = Array.Find(FootSoundObjects, element => element.texture == tex);
        else
            SO = DefaultSO;

        return SO;
    }

    private Texture GetTexture(Transform FromPos)
    {
        var hit = RayCast(FromPos).transform?.gameObject;
        var layer = hit?.layer;

        Texture texture;
        switch (layer)
        {
            case 8: //Ground
                texture = hit.GetComponent<Renderer>().material.mainTexture;
                break;
            case 9: //Terrain
                int t = GetMainTexture(transform.position);
                texture = hit.GetComponent<Terrain>().terrainData.terrainLayers[t].diffuseTexture;
                break;
            default:
                texture = null;
                break;

        }
        return texture;
        
    }
    
    RaycastHit RayCast(Transform From)
    {
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, From.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity, Layers))
        {
            return hit;
        }
        else
            return default;
    }

    public static float[] GetTextureMix(Vector3 worldPos) {
     // returns an array containing the relative mix of textures
     // on the main terrain at this world position.
     // The number of values in the array will equal the number
     // of textures added to the terrain.
     Terrain terrain = Terrain.activeTerrain;
     TerrainData terrainData = terrain.terrainData;
     Vector3 terrainPos = terrain.transform.position;
     // calculate which splat map cell the worldPos falls within (ignoring y)
     int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
     int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
     // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
     float[,,] splatmapData = terrainData.GetAlphamaps(mapX,mapZ,1,1);
     // extract the 3D array data to a 1D array:
     float[] cellMix = new float[splatmapData.GetUpperBound(2)+1];
     for (int n=0; n < cellMix.Length; ++n)
     {
         cellMix[n] = splatmapData[0,0,n];    
     }
     return cellMix;        
 }
    public static int GetMainTexture(Vector3 worldPos) {
     // returns the zero-based index of the most dominant texture
     // on the main terrain at this world position.
     float[] mix = GetTextureMix(worldPos);
     float maxMix = 0;
     int maxIndex = 0;
     // loop through each mix value and find the maximum
     for (int n=0; n<mix.Length; ++n)
     {
         if (mix[n] > maxMix)
         {
             maxIndex = n;
             maxMix = mix[n];
         }
     }
     return maxIndex;
 }
}
