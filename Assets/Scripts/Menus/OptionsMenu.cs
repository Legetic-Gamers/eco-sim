/*
 * Author: Johan A.
 */

using System;
using System.Diagnostics;
using ICSharpCode.NRefactory.Ast;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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