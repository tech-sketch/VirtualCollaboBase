using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public interface IResourceInstantiater
{
	IObservable<InstantiateResourceArgs> OnInstantiateResourceCompleted { get; }

	IEnumerator InstantiateResourceAsync( string path, Vector3 pos, Quaternion rot, Transform parent );
	IEnumerator InstantiateResourceAsyncNetwork( string path, Vector3 pos, Quaternion rot, Transform parent, byte group );
}

public class InstantiateResourceArgs
{
	public GameObject m_Model;

	public InstantiateResourceArgs( GameObject model )
	{
		m_Model = model;
	}
}

public class ResourceInstantiater : IResourceInstantiater
{
	private Subject<InstantiateResourceArgs> m_InstantiateResourceSubject = new Subject<InstantiateResourceArgs>();
	public IObservable<InstantiateResourceArgs> OnInstantiateResourceCompleted { get { return m_InstantiateResourceSubject; } }


	public IEnumerator InstantiateResourceAsync( string path, Vector3 pos, Quaternion rot, Transform parent )
	{
		ResourceRequest request = Resources.LoadAsync( path );

		yield return request;

		GameObject go = GameObject.Instantiate( request.asset, pos, rot, parent ) as GameObject;

		m_InstantiateResourceSubject.OnNext( new InstantiateResourceArgs( go ) );
	}

	public IEnumerator InstantiateResourceAsyncNetwork( string path, Vector3 pos, Quaternion rot, Transform parent, byte group )
	{
		ResourceRequest request = Resources.LoadAsync( path );

		yield return request;

		GameObject go = Photon.Pun.PhotonNetwork.Instantiate( path, pos, rot, group );
		go.transform.SetParent( parent );

		m_InstantiateResourceSubject.OnNext( new InstantiateResourceArgs( go ) );
	}
}


//namespace EmoteVR
//{
//	public enum NetworkType
//	{
//		LOCAL,
//		NETWORK
//	}

//	public interface IInstantiater
//	{
//		UnityEngine.Object Instantiate( string path );
//	}

	
//}