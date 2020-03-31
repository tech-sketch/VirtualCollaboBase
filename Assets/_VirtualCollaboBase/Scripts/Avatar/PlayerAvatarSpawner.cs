using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Photon.Pun;
using System;
using Zenject;

public class PlayerAvatarSpawner
{
	private IRuntimeModelLoader m_ModelLoader = null;
	private IDisposable m_OnLoadModelCompleted = null;
	private IPlayerLocator m_PlayerLocator = null;

	public PlayerAvatarSpawner( IRuntimeModelLoader model_loader, IPlayerLocator player_locator )
	{
		m_ModelLoader = model_loader;
		m_PlayerLocator = player_locator;
	}

	public void Spawn( string path, string spawn_pos_tag )
	{
		if( path == SystemDefines.PLAYABLE_AVATAR_PATH + "HiddenAvatar" )
		{
			if( PhotonNetwork.IsMasterClient == false )
			{
				PhotonNetwork.SetMasterClient( PhotonNetwork.LocalPlayer );
			}
        }
        else
        {
            //HiddenAvatarじゃない通常プレーヤーは席に着く
            m_OnLoadModelCompleted = m_ModelLoader.OnLoadModelCompleted
            .First()
            .Subscribe(_ =>
            {
                MoveSpawnPos(spawn_pos_tag);
            });
        }



		m_ModelLoader.InstantiateModel( path );
	}

	private void MoveSpawnPos( string spawn_pos_tag )
	{
		DestinationPointMover mover
			= new DestinationPointMover( m_PlayerLocator.Player.transform, m_PlayerLocator.View );

		var spawn_pos_array = GameObject.FindGameObjectsWithTag( spawn_pos_tag );
		foreach( GameObject spawn_pos in spawn_pos_array )
		{
			var reserve_obj = spawn_pos.GetComponentInChildren<PhotonReserveObject>();
			if( null == reserve_obj )
			{
				mover.Move( spawn_pos );
				return;
			}
		}
	}
}
