using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharingRoot : MonoBehaviour
{

    public static GameObject m_sharingRoot;
    [SerializeField] private GameObject tagetSharingRoot;

    void Start()
    {
		
        m_sharingRoot = tagetSharingRoot;


    }
}
