using System;
using UnityEngine;

namespace Cameras
{
    public class CameraController : MonoBehaviour
    {
        public Camera worldCamera;
        public Camera statCamera;
        public GameObject ui;

        private AudioListener worldListener;
        private AudioListener statListener;

        private bool worldCameraActive = true;

        private WorldCamera wc;
        private void Awake()
        {
            worldListener = worldCamera.GetComponent<AudioListener>();
            statListener = statCamera.GetComponent<AudioListener>();
            wc = FindObjectOfType<WorldCamera>();
            statListener.enabled = false;
            worldListener.enabled = true;
            wc.enabled = true;
            worldCamera.enabled = true;
            statCamera.enabled = false;
            ui.SetActive(!worldCameraActive);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (worldCameraActive)
                {
                    var position = worldCamera.transform.position;
                    PlayerPrefs.SetFloat("cameraX", position.x);
                    PlayerPrefs.SetFloat("cameraY", position.y);
                    PlayerPrefs.SetFloat("cameraZ", position.z);
                }
                else
                {
                    var position = worldCamera.transform.position;
                    position.x = PlayerPrefs.GetFloat("cameraX");
                    position.y = PlayerPrefs.GetFloat("cameraY");
                    position.z = PlayerPrefs.GetFloat("cameraZ");
                }

                worldCameraActive = !worldCameraActive;
                ui.SetActive(!worldCameraActive);
                wc.enabled = !wc.enabled;
                
                worldCamera.enabled = !worldCamera.enabled;
                statCamera.enabled = !statCamera.enabled;
                
                worldListener.enabled = !worldListener.enabled;
                statListener.enabled = !statListener.enabled;
            }
        }
    }
}
