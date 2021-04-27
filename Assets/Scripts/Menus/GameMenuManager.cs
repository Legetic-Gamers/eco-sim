/*
 * Author Johan A.
 */

using System;
using System.Collections;
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

        public static bool isPaused2;

        private float lastGameSpeed = 1f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isEnded && !isPaused2)
            {
                if (isPaused) Resume();
                else Pause();
            }

            if (Input.GetKeyDown(KeyCode.P) && !isPaused)
            {
                if (isPaused2)
                {
                    Time.timeScale = lastGameSpeed;
                }
                else
                {
                    lastGameSpeed = Time.timeScale;
                    Time.timeScale = 0;
                }
                isPaused2 = !isPaused2;
            }
        }

       /* private void FixedUpdate()
        {
            // timer is relation of in-game time and real time, fixedDeltaTime adjusts for how often FixedUpdate is called
            timer += Time.fixedDeltaTime;
            var time = TimeSpan.FromSeconds(timer);
            timerText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
            
            //timerText.text = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        } */
       private void TimerUpdate()
       {
           timer++;
           var time = TimeSpan.FromSeconds(timer);
           timerText.text = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
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
                Time.timeScale = 1f;
                StartCoroutine(DelayEndPopUp());
            }
        }

        IEnumerator DelayEndPopUp()
        {
            yield return new WaitForSeconds(2f);
            //make sure pause menu is inactive
            pauseMenu.SetActive(false);
            endMenu.SetActive(true);
            Time.timeScale = 0f;
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
                tickEventPublisher.OnTimerUpdate += TimerUpdate;
            DataHandler dh = FindObjectOfType<DataHandler>();
            if (dh)
            {
                //Bind End to action that triggers when all animals are dead
                dh.c.onAllExtinct += End;
            }
        }
    }
}