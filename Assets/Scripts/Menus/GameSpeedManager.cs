using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class GameSpeedManager : MonoBehaviour
    {
        public Slider mainSlider;

        public void SetStandardSpeed()
        {
            if (!PauseMenuManager.isPaused) Time.timeScale = 1f;
            //Debug.Log("Setting standard speed");
        }
        
        public void FastForward()
        {
            if (!PauseMenuManager.isPaused) Time.timeScale = 2f;
            //Debug.Log("Speeding up");
        }

        public void ChangeSpeed()
        {
            Time.timeScale = mainSlider.value;
        }
    }
}