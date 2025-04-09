using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    
    // Start is called before the first frame update    
    private float tresholdVolume = -80f;
    [Range(0f,1f)]
    public float masterVolume;
    [Range(0f,1f)]
    public float SFXVolume;
    [Range(0f,1f)]
    public float musicVolume;
    [Range(0f,1f)]
    public float environmentVolume;
    public float voiceVolume;
    // Update is called once per frame
    void Update()
    {
        SetVolume("Master", masterVolume);
        SetVolume("SFX", SFXVolume);
        SetVolume("Music", musicVolume);
        SetVolume("Environment", environmentVolume);
        SetVolume("Voice", voiceVolume);
    }

    public void SetVolume(string exposedParameter, float volume)
    {
        if (volume <= 0.0001f)
            mixer.SetFloat(exposedParameter, -80f); // почти тишина
        else
            mixer.SetFloat(exposedParameter, Mathf.Log10(volume) * 20f);
    }

}
