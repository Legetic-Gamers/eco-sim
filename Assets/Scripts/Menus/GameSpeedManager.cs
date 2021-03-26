using UnityEngine;

namespace Menus
{
    public class GameSpeedManager : MonoBehaviour
    {
        
        public void SlowDown()
        {
            if (!PauseMenuManager.isPaused) Time.timeScale = 0.5f;
            //Debug.Log("Slowing down");
        }
        
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
    }
}