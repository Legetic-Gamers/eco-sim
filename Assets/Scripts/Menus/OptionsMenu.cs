/*
 * Author: Johan A.
 */

using UnityEngine;

namespace Menus
{
    public class OptionsMenu : MonoBehaviour
    {
        public static bool alwaysShowParameterUI;
        
        void Awake()
        {
            alwaysShowParameterUI = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void RuntimeInit()
        {
            var go = new GameObject { name = "[Options]" };
            go.AddComponent<OptionsMenu>();
            DontDestroyOnLoad(go);
        }
        
        public void SetShowParameterUI(bool toggle)
        {
            alwaysShowParameterUI = toggle;
        }
    }
}