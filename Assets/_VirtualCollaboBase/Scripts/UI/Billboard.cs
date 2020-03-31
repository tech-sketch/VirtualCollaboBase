using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using VRTK;

public class Billboard : MonoBehaviour
{
	[SerializeField]private PhotonView m_PhotonView = null; 

	private void Awake()
	{
        if (m_PhotonView == null)
        {
            m_PhotonView = GetComponent<PhotonView>();
        }
		
	}

	private void Update()
	{
		if(m_PhotonView != null && m_PhotonView.IsMine == false )
		{
			Transform cam_transform = VRTK_DeviceFinder.HeadsetCamera();
			Vector3 angle = Quaternion.LookRotation( cam_transform.forward, cam_transform.up ).eulerAngles;
			angle.x = transform.eulerAngles.x;
			angle.z = transform.eulerAngles.z;

			transform.eulerAngles = angle;
		}
	}
}
