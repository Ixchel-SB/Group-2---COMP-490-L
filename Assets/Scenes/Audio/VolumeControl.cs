using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    
    void Start()
    {
        Debug.Log("VolumeControl Start() called");
        
        if (audioMixer == null)
        {
            Debug.LogError("Audio Mixer is NOT assigned!");
        }
        else
        {
            Debug.Log("Audio Mixer IS assigned: " + audioMixer.name);
        }
        
        if (musicSlider == null)
        {
            Debug.LogError("Music Slider is NOT assigned!");
        }
        else
        {
            Debug.Log("Music Slider IS assigned: " + musicSlider.name);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        Debug.Log("Saved volume: " + savedMusic);
        
        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
        }
        
        SetMusicVolume(savedMusic);
    }
    
    public void SetMusicVolume(float volume)
    {
        Debug.Log("SetMusicVolume called with volume: " + volume);
        
        float decibels = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        Debug.Log("Decibels: " + decibels);
        
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MusicVolume", decibels);
            Debug.Log("SetFloat called with MusicVolume = " + decibels);
        }
        
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
//For some reason the volume slider is not working as it should "need fixing" (ᵕ⸝⸝• ᴗ •)
