using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSwitch : MonoBehaviour
{
    public static bool isEnglish = true;
    [SerializeField]
    Sprite SpriteEnglish;
    [SerializeField]
    Sprite SpriteJapanese;
    public GameObject JapaneseCharacter;
    Image LanguageImage;
    // Start is called before the first frame update
    private static Subject<Unit> m_OnLanguageChange= new Subject<Unit>();
    public static IObservable<Unit> OnLanguageChange { get { return m_OnLanguageChange; } }
    void Start()
    {
       LanguageImage = this.GetComponent<Image>();


}
void Update()
    {
        if (!LanguageSwitch.isEnglish)///かな入力
        {
            JapaneseCharacter.SetActive(true);
        }
        else ///英字入力
        {
            JapaneseCharacter.SetActive(false);
        }
    }
    public void onChangeLanguage()
    {

        isEnglish = !isEnglish;
        LanguageImage.sprite = isEnglish ? SpriteEnglish : SpriteJapanese;
        m_OnLanguageChange.OnNext(Unit.Default);
    }
}
