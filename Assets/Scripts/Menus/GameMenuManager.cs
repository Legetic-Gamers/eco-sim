/*
 * Author Johan A.
 */

using System;
using DataCollection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus
{
    public class GameMenuManager : MonoBehaviour
    {
        public static bool isPaused;
        public static bool isEnded;
        
        public GameObject pauseMenu;
        public GameObject endMenu;
        
        private float lastGameSpeed = 1f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isEnded)
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
            SceneManager.LoadScene("Main");
            Debug.Log("Load menu");
            isEnded = false;
            isPaused = false;
        }

        public void End()
        {
            Debug.Log("GAME IS ENDED");
            if (!isEnded)
            {
                isEnded = true;
                //make sure pause menu is inactive
                pauseMenu.SetActive(false);
                endMenu.SetActive(true);
                Time.timeScale = 0f;    
            }
        }

        public void Restart()
        {
            Debug.Log("Restart");
            //Restart scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
            isEnded = false;
        }

        public void Start()
        {
            DataHandler dh = FindObjectOfType<DataHandler>();
            if (dh)
            {
                //Bind End to action that triggers when all animals are dead
                dh.c.onAllExtinct += End;
            }
        }
    }
}