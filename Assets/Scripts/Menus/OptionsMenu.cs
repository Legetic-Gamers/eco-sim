/*
 * Author: Johan A.
 */

using System.Diagnostics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Menus
{
    public class OptionsMenu : MonoBehaviour
    {
        private static OptionsMenu _instance;

        public AudioMixer audioMixer;
 
        public static OptionsMenu instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<OptionsMenu>();
                }
                DontDestroyOnLoad(_instance.gameObject);
                Debug.Log("Dont destroy at instance");
                return _instance;
            }
        }
 
        void Awake() 
        {
            if(_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
                Debug.Log("Dont destroy at awake");
            }
            else
            {
                if(this != _instance) Destroy(this.gameObject);
            }
            if(audioMixer !=null ) audioMixer.SetFloat("MasterVolume", Mathf.Log10(0.003f)*20);
        }
        
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume)*20);
        }
    }
}