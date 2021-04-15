/*
 * Author Johan A.
 */

using System;
using System.Timers;
using DataCollection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menus
{
    public class GameMenuManager : MonoBehaviour
    {
        private TickEventPublisher tickEventPublisher;
        public static bool isPaused;
        public static bool isEnded;
        
        public GameObject pauseMenu;
        public GameObject endMenu;
        public Text timerText;
        private float timer = 0f;
        
        private float lastGameSpeed = 1f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isEnded)
            {
                if (isPaused) Resume();
                else Pause();
            }
        }

        private void ShowTime()
        {
            timer += 0.5f;
            var time = TimeSpan.FromSeconds(timer);
            timerText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
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
            tickEventPublisher = FindObjectOfType<TickEventPublisher>();
            if (tickEventPublisher)
                tickEventPublisher.onSenseTickEvent += ShowTime;
            DataHandler dh = FindObjectOfType<DataHandler>();
            if (dh)
            {
                //Bind End to action that triggers when all animals are dead
                dh.c.onAllExtinct += End;
            }
        }
    }
}