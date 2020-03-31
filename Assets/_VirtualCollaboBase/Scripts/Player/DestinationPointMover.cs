using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Photon.Pun;
using VRTK;

public class DestinationPointMover
{
	private Transform m_PlayerTransform = null;
	private static GameObject m_ReservePoint = null;
	private PhotonView m_PlayerView = null;

	public DestinationPointMover( Transform transform, PhotonView player_view )
    {
		m_PlayerTransform = transform;
		m_PlayerView = player_view;

		VRTK_DestinationPoint4VRWS.OnClickSeat.Subscribe( dest => Move( dest ) );
	}

	public void Move( GameObject dest )
	{
		if( IsReserved( dest ) == true )
			return;

		ReserveDestinationPoint( dest );

		m_PlayerTransform.position = dest.transform.position;

		var offset_pos = VRTK_DeviceFinder.HeadsetTransform().localPosition;
		offset_pos.y = 0f;

		var offset_rot = VRTK_DeviceFinder.HeadsetTransform().localEulerAngles;
		offset_rot.x = 0f;
		offset_rot.z = 0f;
		Vector3 rotation =  dest.transform.eulerAngles - offset_rot;
		m_PlayerTransform.eulerAngles = rotation;

	}

	private bool IsReserved( GameObject dest )
	{
		return ( dest.GetComponentInChildren<PhotonReserveObject>() == null )
			? false
			: true;
	}

	/// <summary>
	/// 移動先のGameObjectの子供としてPhotonReserveObjectを作成し、
	/// 全ユーザーはそのオブジェクトの有無を確認して、移動を行う
	/// </summary>
	/// <param name="dest"></param>
	private void ReserveDestinationPoint( GameObject dest )
	{
		if( m_ReservePoint != null )
		{
			PhotonNetwork.Destroy( m_ReservePoint );

			m_ReservePoint = null;
		}

		m_ReservePoint = PhotonNetwork.Instantiate( "PhotonReserveObject", dest.transform.position, dest.transform.rotation );

		var target_view = m_ReservePoint.GetComponent<PhotonView>();
		var parent_view = dest.GetComponent<PhotonView>();

		m_PlayerView.RPC( "SetParent", RpcTarget.AllBufferedViaServer, new object[] { target_view.ViewID, parent_view.ViewID } );
	}

	public void SetParent( object[] data )
	{
		int target_id = (int)data[0];
		int parent_id = (int)data[1];

		var target = PhotonView.Find( target_id );

		if( null == target )
			return;

		var parent = PhotonView.Find( parent_id ).transform;

		target.transform.SetParent( parent.transform );
	}
}
