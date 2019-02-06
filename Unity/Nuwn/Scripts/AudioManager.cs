using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public Slider VolumeSlider;
    public Slider MusicSlider;
    public Slider EffectsSlider;
    public AudioMixer MasterMixer;

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    public void setMasterVolume(float masterlvl)
    {
        MasterMixer.SetFloat("Master", masterlvl);
        PlayerPrefs.SetFloat("Master", masterlvl);
        PlayerPrefs.Save();
    }




    void Start()
    {
        //Get the saved music volume, standard = 10f
        float master = PlayerPrefs.GetFloat("Master", 10f);

        //Set the music volume to the saved volume
        AdjustMusicVolume(master);
    }

    private void AdjustMusicVolume(float master)
    {
        MasterMixer.SetFloat("Master", master);
        VolumeSlider.value = master;
    }
}
