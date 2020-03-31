using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using Photon.Pun;

public class VRMFacialMotionPlayer : MonoBehaviour
{
	private PhotonView m_PhotonView = null;
	private VRMBlendShapeProxy m_BlendShape = null;
	private Dictionary<string, BlendShapeSettings> m_BlendShapeMap = new Dictionary<string, BlendShapeSettings>()
	{
		{ "Neutral", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.Neutral ), 0.2f ) },
		{ "Think", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.LookUp ), 0.2f ) },
		{ "Good", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.Fun ), 0.2f ) },
		{ "Laugh", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.Fun ), 0.2f ) },
		{ "Happy", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.Joy ), 0.2f ) },
		{ "Nod", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.Neutral ), 0.2f ) },
		{ "Surprise", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.A ), 0.2f ) },
		{ "Bow", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.Joy ), 0.2f ) },
		{ "RaidsHand", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.Neutral ), 0.2f ) },
		{ "Yes", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.Fun ), 0.2f ) },
		{ "No", new BlendShapeSettings( new BlendShapeKey(BlendShapePreset.Sorrow ), 0.2f ) }
	};

	private string m_CurrentFacial = null;

	private void Awake()
	{
		m_BlendShape = GetComponent<VRMBlendShapeProxy>();
		m_PhotonView = GetComponent<PhotonView>();
	}

	public void Play( string key )
	{
		ResetFacial();
		m_PhotonView.RPC( "PlayNetwork", RpcTarget.All, new object[] { m_PhotonView.ViewID, key } );
	}

	[PunRPC]
	public void PlayNetwork( object[] data )
	{
		int sender_id = (int)data[0];

		if( m_PhotonView.ViewID != sender_id )
			return;
		
		// タイミング的にAwakeだと間に合わないのでメンドイから毎回取得
		VRMLipSyncMorphTarget lipsync = GetComponent<VRMLipSyncMorphTarget>();
		if( lipsync != null )
		{
			// ローカルプレイヤーはnullなので普通にあり得るケース
			lipsync.enabled = false;
		}

		string key = (string)data[1];
		m_CurrentFacial = key;
		StartCoroutine( EaseIn( key, 0f, 1f ) );
	}

	public void ResetFacial()
	{
		m_PhotonView.RPC( "ResetNetwork", RpcTarget.All, m_PhotonView.ViewID );
	}

	[PunRPC]
	public void ResetNetwork( int sender_id )
	{
		if( m_PhotonView.ViewID != sender_id )
			return;

		foreach( KeyValuePair<string, BlendShapeSettings> pair in m_BlendShapeMap )
		{
			if( m_BlendShape.GetValue( pair.Value.m_Key ) > 0 )
			{
				StartCoroutine( EaseOut( pair.Key, m_BlendShape.GetValue( pair.Value.m_Key ), 0f ) );
			}
		}
	}

	private IEnumerator EaseIn( string key, float from, float to )
	{
		var setting = m_BlendShapeMap[key];
		float time = 0f;
		float progress = from;
		while( time < setting.m_EaseInTime )
		{
			time += Time.deltaTime;
			progress += Time.deltaTime / setting.m_EaseInTime;

			m_BlendShape.SetValue( setting.m_Key, progress );
			yield return null;
		}

		m_BlendShape.SetValue( setting.m_Key, to );
	}

	private IEnumerator EaseOut( string key, float from, float to )
	{
		var setting = m_BlendShapeMap[key];
		float time = 0f;
		float progress = from;
		
		while( time < setting.m_EaseInTime )
		{
			time += Time.deltaTime;
			progress -= Time.deltaTime / setting.m_EaseInTime;

			m_BlendShape.SetValue( setting.m_Key, progress );
			yield return null;
		}

		m_BlendShape.SetValue( setting.m_Key, to );

		VRMLipSyncMorphTarget lipsync = GetComponent<VRMLipSyncMorphTarget>();
		if( lipsync != null )
		{
			// ローカルプレイヤーはnullなので普通にあり得るケース
			lipsync.enabled = true;
		}
	}
}

public class BlendShapeSettings
{
	public BlendShapeKey m_Key;
	public float m_EaseInTime = 0f;

	public BlendShapeSettings( BlendShapeKey key, float ease_in_time )
	{
		m_Key = key;
		m_EaseInTime = ease_in_time;
	}
}
