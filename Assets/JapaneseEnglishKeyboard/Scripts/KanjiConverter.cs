using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

public class KanjiConverter : MonoBehaviour
{
   // public InputField targetInputField;
    public Text convertText;
    private const string url = "http://www.google.com/transliterate?langpair=ja-Hira|ja&text=";
    private const string urlend = ",";
    private char[] splitter = { ',' };
    [SerializeField]
    Transform Content = null;
    [SerializeField]
    RectTransform prefab = null;


    //private static Subject<Unit> m_OnSelectCandidateComplete = new Subject<Unit>();
   // public static UniRx.IObservable<Unit> OnSelectCandidateComplete { get { return m_OnSelectCandidateComplete; } }

    // Use this for initialization
    void Start()
    {
       // DialKeyBoardController.OnKanaKanjiHnekann.Subscribe(_ => GetCandidate());
        ClickCandidate.OnClickCandidateButton.Subscribe(Item => OnSelectCandidate(Item)).AddTo(this);

    }

    private void OnSelectCandidate(Image Item) ///変換候補の中から決定したら
    {
        // targetInputField.text = targetInputField.text.TrimEnd(convertText.text.ToCharArray());
        
        UI_Keyboard.input.text= UI_Keyboard.input.text.TrimEnd(convertText.text.ToCharArray());
        UI_Keyboard.tmpJapText = UI_Keyboard.tmpJapText.TrimEnd(convertText.text.ToCharArray());
        UI_Keyboard.tmpKanaText = UI_Keyboard.tmpKanaText.TrimEnd(convertText.text.ToCharArray());
       
        //targetInputField.text += Item.gameObject.GetComponentInChildren<Text>().text;
        UI_Keyboard.input.text += Item.gameObject.GetComponentInChildren<Text>().text;
        UI_Keyboard.tmpJapText += Item.gameObject.GetComponentInChildren<Text>().text;
        UI_Keyboard.tmpKanaText += Item.gameObject.GetComponentInChildren<Text>().text;


        if (LanguageSwitch.isEnglish)
        {
            //targetInputField.text += " ";
            UI_Keyboard.input.text += " ";
        }

        foreach (Transform n in Content)
        {
            GameObject.Destroy(n.gameObject);
        }
        convertText.text = "";

      //  m_OnSelectCandidateComplete.OnNext(Unit.Default);
    }



    public void GetCandidate()///変換ボタンを押したら
    {


        foreach (Transform n in Content)
        {
            GameObject.Destroy(n.gameObject);
        }

        if (convertText.text != string.Empty)
        {
            ObservableWWW.Get(url + convertText.text + urlend).Subscribe(data => ComposeCandidate(data, convertText.text)).AddTo(this);


        }

        Debug.Log("convertText" + convertText.text);

    }

    private void ComposeCandidate(string value, string convertText)
    {
        Debug.Log(value);
        value = value.Split(splitter, 2)[1];
        value = value.Remove(value.Length - 2);
        Debug.Log(value);

        if (value != "[]")
        {
            GoogleAutoCompleteAPI respose = JsonUtility.FromJson<GoogleAutoCompleteAPI>("{\"candidate\":" + value + "}");

            foreach (string str in respose.candidate)
            {
                var item = Instantiate(prefab) as RectTransform;

                var text = item.GetComponentsInChildren<Text>();

                text[0].text = str;

                item.SetParent(Content, false);
            }

        }

    }

    [Serializable]
    public class GoogleAutoCompleteAPI
    {
        public string[] candidate;

    }
}

