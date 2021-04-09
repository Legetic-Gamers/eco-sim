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

        public bool alwaysShowParameterUI = false;
 
/*
        public static OptionsMenu instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<OptionsMenu>();
                }
                DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
        }
*/
 
        void Awake()
        {
            _instance = this;
            if(audioMixer !=null ) audioMixer.SetFloat("MasterVolume", Mathf.Log10(0.003f)*20);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void RuntimeInit()
        {
            var go = new GameObject { name = "[Options]" };
            go.AddComponent<OptionsMenu>();
            DontDestroyOnLoad(go);
        }        
        
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume)*20);
        }
        
        public void SetShowParameterUI(bool toggle)
        {
            alwaysShowParameterUI = toggle;
        }
    }
}