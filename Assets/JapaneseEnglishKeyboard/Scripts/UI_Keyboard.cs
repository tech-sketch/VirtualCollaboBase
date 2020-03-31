
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Text;

public class UI_Keyboard : MonoBehaviour
{
    public static InputField input;
 
    public Text ConvertText;
    public static string tmpJapText = "";
    public static string tmpKanaText = "";
    string tmpText = "";

    void Start()
    {
         //input = GetComponentInChildren<InputField>();
        LanguageSwitch.OnLanguageChange.Subscribe(_ => ChangeInputText()).AddTo(this);
    }
    public void SetInputField(InputField outsideInputField)
    {
        input = outsideInputField;
    }

    private void ChangeInputText()
    {
        tmpText = input.text;
        tmpJapText = "";
        tmpKanaText = "";
        ConvertText.text = "";

    }
    public void ClickKey(string character)
    {

        if (!LanguageSwitch.isEnglish)///かな入力
        {
            tmpKanaText = tmpKanaText + character;
            tmpJapText = RomajiKanaConversion.RomanToKana(tmpKanaText);
  
            if (tmpKanaText == tmpJapText)
            {
                input.text = input.text + character;
               
            }
            else
            {
                input.text = input.text + character;
                StringBuilder sb = new StringBuilder(input.text);
                sb = sb.Replace(tmpKanaText, tmpJapText, input.text.Length - tmpKanaText.Length, tmpKanaText.Length);
                input.text = sb.ToString();
                
                tmpKanaText = tmpJapText;
              
            }
            ConvertText.text = RomajiKanaConversion.RomanToKana(ConvertText.text + character);
     
        }
        else ///英字入力
        {
            input.text = input.text + character;

        }

    }

    public void Backspace()
    {
        if (input.text.Length > 0)
        {
            input.text = input.text.Substring(0, input.text.Length - 1);

        }
        if (ConvertText.text.Length > 0)
        {
            ConvertText.text = ConvertText.text.Substring(0, ConvertText.text.Length - 1);
        }

        if (tmpJapText.Length > 0)
        {
            tmpJapText = tmpJapText.Substring(0, tmpJapText.Length - 1);
            
        }
        if (tmpKanaText.Length > 0)
        {
            tmpKanaText = tmpKanaText.Substring(0, tmpKanaText.Length - 1);
        }
    

    }

    public void Enter()
    {
        this.gameObject.SetActive(false);
        //input.text = "";
        tmpJapText = "";
        tmpKanaText = "";
        ConvertText.text = "";

    }

    public void InactiveKeyboard()
    {
        this.gameObject.SetActive(false);
    }
}
