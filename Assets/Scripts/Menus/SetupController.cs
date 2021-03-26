/*
 * Author: Johan A.
 */

using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Menus
{
    public class SetupController : MonoBehaviour
    {
        public void PlaySimulator()
        {
            Debug.Log("Load simulator...");
            //SceneManager.LoadScene("Main");
        }
    }
}