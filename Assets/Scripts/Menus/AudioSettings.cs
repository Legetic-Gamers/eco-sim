using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Menus
{
    public class AudioSettings : MonoBehaviour
    {
        public AudioMixer audioMixer;
        
        public Slider slider;
        
        private void Start()
        {
            slider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        }
        
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume)*20);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }
}