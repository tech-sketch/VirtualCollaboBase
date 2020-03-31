namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class KeyboardInputToInputField : MonoBehaviour
    {
        private InputField input;
        public InputField inputSearchTablet;


        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log(this.gameObject.name + " was selected");
        }
        public void ClickKey(string character)
        {
            input.text += character;
        }

        public void Backspace()
        {
            if (input.text.Length > 0)
            {
                input.text = input.text.Substring(0, input.text.Length - 1);
            }
        }

        public void Enter()
        {
            input.text = "";
            this.gameObject.SetActive(false);
        }

        public void InactiveKeyboard()
        {
            this.gameObject.SetActive(false);
        }

        private void Start()
        {
            input = inputSearchTablet;
        }
    }
}