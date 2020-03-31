using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ClickCandidate : MonoBehaviour
{
    // Start is called before the first frame update
    private static Subject<Image> m_OnClickCandidateButton = new Subject<Image>();
    public static IObservable<Image> OnClickCandidateButton { get { return m_OnClickCandidateButton; } }
    
    public void OnClickCandidate()
    {
        Image candidate_image = this.GetComponent<Image>();
        m_OnClickCandidateButton.OnNext(candidate_image);
    }
}
