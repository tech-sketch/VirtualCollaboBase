using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
public class RomajiKanaConversion : MonoBehaviour
{
    static Dictionary<string, string> kanamap = new Dictionary<string, string>() {
  {"a","あ"}, {"i","い"}, {"u","う"}, {"e","え"}, {"o","お"},
  {"ka","か"}, {"ki","き"}, {"ku","く"}, {"ke","け"}, {"ko","こ"},
  {"sa","さ"}, {"si","し"}, {"su","す"}, {"se","せ"}, {"so","そ"},
  {"ta","た"}, {"ti","ち"}, {"tu","つ"}, {"te","て"}, {"to","と"}, {"chi","ち"}, {"tsu","つ"},
  {"na","な"}, {"ni","に"}, {"nu","ぬ"}, {"ne","ね"}, {"no","の"},
  {"ha","は"}, {"hi","ひ"}, {"hu","ふ"}, {"he","へ"}, {"ho","ほ"}, {"fu","ふ"},
  {"ma","ま"}, {"mi","み"}, {"mu","む"}, {"me","め"}, {"mo","も"},
  {"ya","や"}, {"yi","い"}, {"yu","ゆ"}, {"ye","いぇ"}, {"yo","よ"},
  {"ra","ら"}, {"ri","り"}, {"ru","る"}, {"re","れ"}, {"ro","ろ"},
  {"wa","わ"}, {"wyi","ゐ"}, {"wu","う"}, {"wye","ゑ"}, {"wo","を"},
  {"nn","ん"},
  {"ga","が"}, {"gi","ぎ"}, {"gu","ぐ"}, {"ge","げ"}, {"go","ご"},
  {"za","ざ"}, {"zi","じ"}, {"zu","ず"}, {"ze","ぜ"}, {"zo","ぞ"}, {"ji","じ"},
  {"da","だ"}, {"di","ぢ"}, {"du","づ"}, {"de","で"}, {"do","ど"},
  {"ba","ば"}, {"bi","び"}, {"bu","ぶ"}, {"be","べ"}, {"bo","ぼ"},
  {"pa","ぱ"}, {"pi","ぴ"}, {"pu","ぷ"}, {"pe","ぺ"}, {"po","ぽ"},
  {"kya","きゃ"}, {"kyu","きゅ"}, {"kyo","きょ"},
  {"sya","しゃ"}, {"syu","しゅ"}, {"syo","しょ"},
  {"tya","ちゃ"}, {"tyi","ちぃ"}, {"tyu","ちゅ"}, {"tye","ちぇ"}, {"tyo","ちょ"}, {"cha","ちゃ"}, {"chu","ちゅ"}, {"che","ちぇ"}, {"cho","ちょ"},
  {"nya","にゃ"}, {"nyi","にぃ"}, {"nyu","にゅ"}, {"nye","にぇ"}, {"nyo","にょ"},
  {"hya","ひゃ"}, {"hyi","ひぃ"}, {"hyu","ひゅ"}, {"hye","ひぇ"}, {"hyo","ひょ"},
  {"mya","みゃ"}, {"myi","みぃ"}, {"myu","みゅ"}, {"mye","みぇ"}, {"myo","みょ"},
  {"rya","りゃ"}, {"ryi","りぃ"}, {"ryu","りゅ"}, {"rye","りぇ"}, {"ryo","りょ"},
  {"gya","ぎゃ"}, {"gyi","ぎぃ"}, {"gyu","ぎゅ"}, {"gye","ぎぇ"}, {"gyo","ぎょ"},
  {"zya","じゃ"}, {"zyi","じぃ"}, {"zyu","じゅ"}, {"zye","じぇ"}, {"zyo","じょ"},
  {"ja","じゃ"}, {"ju","じゅ"}, {"je","じぇ"}, {"jo","じょ"}, {"jya","じゃ"}, {"jyi","じぃ"}, {"jyu","じゅ"}, {"jye","じぇ"}, {"jyo","じょ"},
  {"dya","ぢゃ"}, {"dyi","ぢぃ"}, {"dyu","ぢゅ"}, {"dye","ぢぇ"}, {"dyo","ぢょ"},
  {"bya","びゃ"}, {"byi","びぃ"}, {"byu","びゅ"}, {"bye","びぇ"}, {"byo","びょ"},
  {"pya","ぴゃ"}, {"pyi","ぴぃ"}, {"pyu","ぴゅ"}, {"pye","ぴぇ"}, {"pyo","ぴょ"},
  {"fa","ふぁ"}, {"fi","ふぃ"}, {"fe","ふぇ"}, {"fo","ふぉ"},
  {"fya","ふゃ"}, {"fyu","ふゅ"}, {"fyo","ふょ"},
  {"xa","ぁ"}, {"xi","ぃ"}, {"xu","ぅ"}, {"xe","ぇ"}, {"xo","ぉ"}, {"la","ぁ"}, {"li","ぃ"}, {"lu","ぅ"}, {"le","ぇ"}, {"lo","ぉ"},
  {"xya","ゃ"}, {"xyu","ゅ"}, {"xyo","ょ"},
  {"xtu","っ"}, {"xtsu","っ"},
  {"wi","うぃ"}, {"we","うぇ"},
  {"va","ヴぁ"}, {"vi","ヴぃ"}, {"vu","ヴ"}, {"ve","ヴぇ"}, {"vo","ヴぉ"}
};
    static Regex regex;
    static Regex LTUregex;
    // Start is called before the first frame update
    void Start()
    {
        regex = new Regex(GetRegexString());
        LTUregex = new Regex(@"^([^n])\1$");
    }
    public static string RomanToKana(string roman)
    {
        string hiragana = string.Empty;
        for (int i = 0; i < roman.Length; ++i)
        {
           
            string targetStr = roman.Substring(i);
            Match match = regex.Match(targetStr);
            if (match.Success)
            {
              
                if (match.Value == "n")
                {
                    hiragana += "ん";
                }
                else if (LTUregex.IsMatch(match.Value))
                {
                    hiragana += "っ";
                    --i;
                   
                }
                else
                {
                    hiragana += kanamap[match.Value];
                   
                }
                i += match.Value.Length - 1;
            }
            else
            {
                hiragana += roman[i];
          
            }
        
        }
        return hiragana;
    }
    private static string GetRegexString()
    {
        string s = "^(?:";
        foreach (string key in kanamap.Keys)
        {
            s += key + '|';
        }
        return s + "(?:n(?![aiueo]|y[aiueo]|$))|" + "([qwrtypsdfghjklzxcvbm])\\1)"; ///aiueon＋ひらがな以外
    }
}
