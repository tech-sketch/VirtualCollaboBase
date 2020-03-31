using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using VRTK;
using Recollab.SceneManagement;
using System;
using UniRx.Triggers;
using Recollab.Network;

public interface IPlayerAvatarPresenter
{
	string AvatarPath { get; set; }

	IObservable<GameObject> OnLoadModelCompleted { get; }

}


public class PlayerAvatarPresenter : MonoBehaviour, IPlayerAvatarPresenter
{
	[Inject(Id = ControllerHand.RIGHT)] private VRTK_ControllerEvents m_RightControllerEvents = null;
	[Inject(Id = ControllerHand.LEFT)] private VRTK_ControllerEvents m_LeftControllerEvents = null;

	[SerializeField] private VRMRuntimeLoader m_RuntimeModelLoader = null;
	[SerializeField] private string m_SpawnPosTag = "Respawn";


	private GameObject m_AvatarModel = null;
	private PlayerAvatarSpawner m_AvatarSpawner = null;

	private Subject<GameObject> m_LoadModelCompleted  = new Subject<GameObject>();
	public IObservable<GameObject> OnLoadModelCompleted { get { return m_LoadModelCompleted; } }


	public string AvatarPath { get; set; }

	[Inject]
	private void Initialize( IPlayerLocator player_locator, ISceneManager scene_manager, INetworkEngineConnector network_connector )
	{
		scene_manager.OnRoomLoaded
			.Subscribe( _ =>
			{
				SubscribeEvents();
			} )
			.AddTo( this );

		network_connector.OnJoinedRoomAsObservable
			.Subscribe( _ =>
			{
				m_AvatarSpawner = new PlayerAvatarSpawner( m_RuntimeModelLoader, player_locator );
				m_AvatarSpawner.Spawn( AvatarPath, m_SpawnPosTag );
			} )
			.AddTo( this );

		network_connector.OnLeftRoomAsObservable
			.Subscribe( _ =>
			{
				UnsubscribeEvents();
				DestroyAvatar();
			} )
			.AddTo( this );


		m_RuntimeModelLoader.OnLoadModelCompleted
			.Subscribe( model =>
			{
				m_AvatarModel = model;
				m_LoadModelCompleted.OnNext( model );
			} )
			.AddTo( this );

	}

	private void OnDestroy()
	{
		UnsubscribeEvents();
	}

	private void SubscribeEvents()
	{
		m_RightControllerEvents.TouchpadTouchStart += TouchpadTouchStart;
		m_LeftControllerEvents.TouchpadTouchStart += TouchpadTouchStart;
		m_RightControllerEvents.TouchpadTouchEnd += TouchpadTouchEnd;
		m_LeftControllerEvents.TouchpadTouchEnd += TouchpadTouchEnd;
	}

	private void UnsubscribeEvents()
	{
		m_RightControllerEvents.TouchpadTouchStart -= TouchpadTouchStart;
		m_LeftControllerEvents.TouchpadTouchStart -= TouchpadTouchStart;
		m_RightControllerEvents.TouchpadTouchEnd -= TouchpadTouchEnd;
		m_LeftControllerEvents.TouchpadTouchEnd -= TouchpadTouchEnd;
	}

	private void ToggleHand()
	{

	}

	private void TouchpadTouchStart( object sender, ControllerInteractionEventArgs e )
	{
	}

	private void TouchpadTouchEnd( object sender, ControllerInteractionEventArgs e )
	{
	}

	private void DestroyAvatar()
	{
		if( m_AvatarModel == null )
			return;

		Photon.Pun.PhotonNetwork.Destroy( m_AvatarModel.GetComponentInParent<PlayerInfo>().gameObject );
	}
}
