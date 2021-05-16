using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus
{
    public class GameSpeedManager : MonoBehaviour
    {
        public Slider mainSlider;
        public TextMeshProUGUI text;

        public void ChangeSpeed()
        {
            Time.timeScale = mainSlider.value;
            text.text = "Simulation Speed " + Time.timeScale;
        }
    }
}