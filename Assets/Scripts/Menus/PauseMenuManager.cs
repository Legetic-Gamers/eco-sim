/*
 * Author Johan A.
 */
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus
{
    public class PauseMenuManager : MonoBehaviour
    {
        public static bool isPaused;
        public GameObject pauseMenu;
        private float lastGameSpeed = 1f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) Resume();
                else Pause();
            }
        }

        public void Resume()
        {
            Time.timeScale = lastGameSpeed;
            pauseMenu.SetActive(false);
            isPaused = false;
        }

        public void Pause()
        {
            lastGameSpeed = Time.timeScale;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }

        public void LoadMenu()
        {
            Time.timeScale = 1f;
            //SceneManager.LoadScene("Menu");
            Debug.Log("Load menu");
        }
    }
}