using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using Photon.Pun;

public interface IPlayerLocator
{
	GameObject Player { get; }
	GameObject Avatar { get; }
	GameObject Hand { get; }
	PhotonView View { get; }
}

public class PlayerLocator : MonoBehaviour, IPlayerLocator
{
	[SerializeField] private GameObject m_Player = null;
	private PhotonView m_PhotonView = null;


	[Inject]
	private void Initialize( IPlayerAvatarPresenter loader )
	{
		loader.OnLoadModelCompleted
			.Subscribe(     avatar =>
			{
				Avatar = avatar;
                //TODO 手の取得方は後で考える
				//Hand = Avatar.transform.Find( "TPHANDS" ).gameObject;
			} ).AddTo(this);

		m_PhotonView = m_Player.GetComponent<PhotonView>();
	}

	public GameObject Player { get { return m_Player; } }
	public GameObject Avatar { get; private set; } = null;
	public GameObject Hand { get; private set; } = null;
	public PhotonView View { get { return m_PhotonView; } }
}
