using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/*
 * Handles adjusting audio mixer volumes from options menu
 * 
 * Jeff Stevenson
 * 4.25.25
 */

public class AudioSettings : MonoBehaviour
{
    private const string MASTER_SLIDER_VOL_REF = "masterVolume";
    private const string MUSIC_SLIDER_VOL_REF = "musicVolume";
    private const string SFX_SLIDER_VOL_REF = "sfxVolume";

    [SerializeField]
    private AudioMixer audioMixer;

    [Space]
    [SerializeField]
    private Slider masterSlider;    
    [SerializeField]
    private Slider sfxSlider;    
    [SerializeField]
    private Slider musicSlider;

    // adjusts range for passed in slider values to audio range of -80 to 20
    private float audioGroupDecibelOffset = -80f;

    private void Start()
    {
        AdjustSliderValuesToGroupVolumes();
    }

    public void UpdateMasterVolume()
    {
        UpdateAudioGroupVolume(MASTER_SLIDER_VOL_REF, masterSlider.value);
    }
    public void UpdateSFXVolume()
    {
        UpdateAudioGroupVolume(SFX_SLIDER_VOL_REF, sfxSlider.value);
    }
    public void UpdateMusicVolume()
    {
        UpdateAudioGroupVolume(MUSIC_SLIDER_VOL_REF, musicSlider.value);
    }
    private void UpdateAudioGroupVolume(string audioGroupStrRef, float volume)
    {
        audioMixer.SetFloat(audioGroupStrRef, volume + audioGroupDecibelOffset);
    }

    private void AdjustSliderValuesToGroupVolumes()
    {
        masterSlider.value = GetMixerParamValue(MASTER_SLIDER_VOL_REF) - audioGroupDecibelOffset;
        musicSlider.value = GetMixerParamValue(MUSIC_SLIDER_VOL_REF) - audioGroupDecibelOffset;
        sfxSlider.value = GetMixerParamValue(SFX_SLIDER_VOL_REF) - audioGroupDecibelOffset;
    }

    private float GetMixerParamValue(string volumeRefStr)
    {
        float volume;
        audioMixer.GetFloat(volumeRefStr, out volume);
        Debug.Log(volumeRefStr + " " + volume);
        return volume;
    }
}
