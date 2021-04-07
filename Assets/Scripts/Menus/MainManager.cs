﻿/*
 * Author: Johan A.
 */

using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Menus
{
    public class MainManager : MonoBehaviour
    {
        public void QuitSimulator()
        {
            Debug.Log("Quitting");
            Application.Quit();
        }

        public void ChangeToSimulationScene()
        {
            SimulationSettings.instance.preview = false;
            SceneManager.LoadScene(1);
        }
    }
}
