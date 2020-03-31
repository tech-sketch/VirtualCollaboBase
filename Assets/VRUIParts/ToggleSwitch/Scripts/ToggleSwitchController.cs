using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VRUIParts
{
    public class ToggleSwitchController : MonoBehaviour
    {

        public bool isOn;

        public GameObject Image_On;
        public GameObject Image_Off;

        private bool switching = false;

        public UnityEvent ToggleOn = new UnityEvent();
        public UnityEvent ToggleOff = new UnityEvent();
        void Start()
        {
            if (isOn)
            {
                ToggleOn.Invoke();
                Image_On.SetActive(true);
                Image_Off.SetActive(false);
            }
            else
            {
                ToggleOff.Invoke();
                Image_On.SetActive(false);
                Image_Off.SetActive(true);
            }
        }


        public void Switching()
        {
            Toggle(isOn);
        }

        public void Toggle(bool toggleStatus)
        {
            if (!Image_On.activeSelf || !Image_Off.activeSelf)
            {
                Image_On.SetActive(true);
                Image_Off.SetActive(true);
            }

            if (toggleStatus)
            {
                ToggleOff.Invoke();
                Image_On.SetActive(false);
                Image_Off.SetActive(true);
                isOn = false;
            }
            else
            {
                ToggleOn.Invoke();
                Image_On.SetActive(true);
                Image_Off.SetActive(false);
                isOn = true;
            }
        }
    }
}
