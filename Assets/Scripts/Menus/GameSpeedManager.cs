using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class GameSpeedManager : MonoBehaviour
    {
        public Slider mainSlider;

        public void ChangeSpeed()
        {
            Time.timeScale = mainSlider.value;
        }
    }
}