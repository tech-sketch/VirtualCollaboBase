using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarUIController : MonoBehaviour
{
	[SerializeField] private GameObject m_HudPrefab = null;
	[SerializeField] private GameObject m_HudParent = null;

	private void Awake()
	{
		//if( GetComponent<PhotonView>().IsMine == true )
		//{
		//	GameObject hud = PhotonNetwork.Instantiate( m_HudPrefab.name, Vector3.zero, Quaternion.identity );
		//	hud.transform.SetParent( m_HudParent.transform );
		//	hud.transform.localPosition = Vector3.zero;
		//	hud.transform.localRotation = Quaternion.identity;
		//}
	}
}
