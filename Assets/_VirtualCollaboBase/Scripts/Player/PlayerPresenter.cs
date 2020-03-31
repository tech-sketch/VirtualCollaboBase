using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Zenject;
using UniRx;
using Recollab.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class PlayerPresenter : MonoBehaviour
{
	private DestinationPointMover m_Mover = null;
	private PhotonView m_PhotonView = null;

	[Inject]
	private void Initialize( ISceneManager scene_manager, IPlayerLocator player_locator )
	{
		scene_manager.OnLobbyLoaded
			.Subscribe( _ =>
			{
				player_locator.Player.transform.position = Vector3.zero;
				player_locator.Player.transform.rotation = Quaternion.identity;
			} )
			.AddTo( this );
	}

	private void Awake()
	{
		m_PhotonView = GetComponent<PhotonView>();
		m_Mover = new DestinationPointMover( transform, m_PhotonView );
	}

	[PunRPC]
	private void SetParent( object[] data )
	{
		m_Mover.SetParent( data );
	}
}
