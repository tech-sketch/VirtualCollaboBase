using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using ExitGames.Client.Photon;

namespace Recollab.Network
{
	public interface INetworkEngineConnector
	{
		void ConnectMasterServer();
		void Disconnect();
		void JoinLobby();
		void LeaveLobby();
		void JoinRoom( string room_name );
		void LeaveRoom( bool becomeInactive = true );
        void SetOfflineMode();
        bool IsOfflineMode();
        bool InRoom { get; }

		IObservable<Unit> OnConnectedAsObservable { get; }
		IObservable<Unit> OnLeftRoomAsObservable { get; }
		IObservable<Player> OnMasterClientSwitchedAsObservable { get; }
		IObservable<FailureReason> OnCreateRoomFailedAsObservable { get; }
		IObservable<FailureReason> OnJoinRoomFailedAsObservable { get; }
		IObservable<Unit> OnCreatedRoomAsObservable { get; }
		IObservable<Unit> OnJoinedLobbyAsObservable { get; }
		IObservable<Unit> OnLeftLobbyAsObservable { get; }
		IObservable<DisconnectCause> OnDisconnectedAsObservable { get; }
		IObservable<RegionHandler> OnRegionListReceivedAsObservable { get; }
		IObservable<List<RoomInfo>> OnRoomListUpdateAsObservable { get; }
		IObservable<Unit> OnJoinedRoomAsObservable { get; }
		IObservable<Player> OnPlayerEnteredRoomAsObservable { get; }
		IObservable<Player> OnPlayerLeftRoomAsObservable { get; }
		IObservable<FailureReason> OnJoinRandomFailedAsObservable { get; }
		IObservable<Unit> OnConnectedToMasterAsObservable { get; }
		IObservable<ExitGames.Client.Photon.Hashtable> OnRoomPropertiesUpdateAsObservable { get; }
		IObservable<PlayerProperties> OnPlayerPropertiesUpdateAsObservable { get; }
		IObservable<List<FriendInfo>> OnFriendListUpdateAsObservable { get; }
		IObservable<Dictionary<string, object>> OnCustomAuthenticationResponseAsObservable { get; }
		IObservable<string> OnCustomAuthenticationFailedAsObservable { get; }
		IObservable<OperationResponse> OnWebRpcResponseAsObservable { get; }
		IObservable<List<TypedLobbyInfo>> OnLobbyStatisticsUpdateAsObservable { get; }
	}

	public struct FailureReason
	{
		public short ErrorCode { get; private set; }
		public string Message { get; private set; }

		public FailureReason( short error_code, string message ) : this()
		{
			ErrorCode = error_code;
			Message = message;
		}

		public override string ToString()
		{
			return string.Format( "[{0}] {1}", ErrorCode, Message );
		}
	}

	public struct PlayerProperties
	{
		public Player Target { get; private set; }
		public ExitGames.Client.Photon.Hashtable ChangedProps { get; private set; }

		public PlayerProperties( Player target, ExitGames.Client.Photon.Hashtable changed_props )
		{
			Target = target;
			ChangedProps = changed_props;
		}
	}



	public class PhotonConnector : MonoBehaviourPunCallbacks, INetworkEngineConnector
	{
		[SerializeField] private string m_GameVersion = "1.0";
		[SerializeField] private byte m_MaxPlayers = 10;
		[SerializeField] private bool m_IsAutoConnectMasterServer = true;
		[SerializeField] private bool m_IsAutoJoinLobby = true;

		#region EventCallbackDefines
		private Subject<Unit> m_OnConnectedSubject = new Subject<Unit>();
		public IObservable<Unit> OnConnectedAsObservable { get { return m_OnConnectedSubject; } }

		private Subject<Unit> m_OnLeftRoomSubject = new Subject<Unit>();
		public IObservable<Unit> OnLeftRoomAsObservable { get { return m_OnLeftRoomSubject; } }

		private Subject<Player> m_OnMasterClientSwitchedSubject = new Subject<Player>();
		public IObservable<Player> OnMasterClientSwitchedAsObservable { get { return m_OnMasterClientSwitchedSubject; } }

		private Subject<FailureReason> m_OnCreateRoomFailedSubject = new Subject<FailureReason>();
		public IObservable<FailureReason> OnCreateRoomFailedAsObservable { get { return m_OnCreateRoomFailedSubject; } }

		private Subject<FailureReason> m_OnJoinRoomFailedSubject = new Subject<FailureReason>();
		public IObservable<FailureReason> OnJoinRoomFailedAsObservable { get { return m_OnJoinRoomFailedSubject; } }

		private Subject<Unit> m_OnCreatedRoomSubject = new Subject<Unit>();
		public IObservable<Unit> OnCreatedRoomAsObservable { get { return m_OnCreatedRoomSubject; } }

		private Subject<Unit> m_OnJoinedLobbySubject = new Subject<Unit>();
		public IObservable<Unit> OnJoinedLobbyAsObservable { get { return m_OnJoinedLobbySubject; } }

		private Subject<Unit> m_OnLeftLobbySubject = new Subject<Unit>();
		public IObservable<Unit> OnLeftLobbyAsObservable { get { return m_OnLeftLobbySubject; } }

		private Subject<DisconnectCause> m_OnDisconnectedSubject = new Subject<DisconnectCause>();
		public IObservable<DisconnectCause> OnDisconnectedAsObservable { get { return m_OnDisconnectedSubject; } }

		private Subject<RegionHandler> m_OnRegionListReceivedSubject = new Subject<RegionHandler>();
		public IObservable<RegionHandler> OnRegionListReceivedAsObservable { get { return m_OnRegionListReceivedSubject; } }

		private Subject<List<RoomInfo>> m_OnRoomListUpdateSubject = new Subject<List<RoomInfo>>();
		public IObservable<List<RoomInfo>> OnRoomListUpdateAsObservable { get { return m_OnRoomListUpdateSubject; } }

		private Subject<Unit> m_OnJoinedRoomSubject = new Subject<Unit>();
		public IObservable<Unit> OnJoinedRoomAsObservable { get { return m_OnJoinedRoomSubject; } }

		private Subject<Player> m_OnPlayerEnteredRoomSubject = new Subject<Player>();
		public IObservable<Player> OnPlayerEnteredRoomAsObservable { get { return m_OnPlayerEnteredRoomSubject; } }

		private Subject<Player> m_OnPlayerLeftRoomSubject = new Subject<Player>();
		public IObservable<Player> OnPlayerLeftRoomAsObservable { get { return m_OnPlayerLeftRoomSubject; } }

		private Subject<FailureReason> m_OnJoinRandomFailedSubject = new Subject<FailureReason>();
		public IObservable<FailureReason> OnJoinRandomFailedAsObservable { get { return m_OnJoinRandomFailedSubject; } }

		private Subject<Unit> m_OnConnectedToMasterSubject = new Subject<Unit>();
		public IObservable<Unit> OnConnectedToMasterAsObservable { get { return m_OnConnectedToMasterSubject; } }

		private Subject<ExitGames.Client.Photon.Hashtable> m_OnRoomPropertiesUpdateSubject = new Subject<ExitGames.Client.Photon.Hashtable>();
		public IObservable<ExitGames.Client.Photon.Hashtable> OnRoomPropertiesUpdateAsObservable { get { return m_OnRoomPropertiesUpdateSubject; } }

		private Subject<PlayerProperties> m_OnPlayerPropertiesUpdateSubject = new Subject<PlayerProperties>();
		public IObservable<PlayerProperties> OnPlayerPropertiesUpdateAsObservable { get { return m_OnPlayerPropertiesUpdateSubject; } }

		private Subject<List<FriendInfo>> m_OnFriendListUpdateSubject = new Subject<List<FriendInfo>>();
		public IObservable<List<FriendInfo>> OnFriendListUpdateAsObservable { get { return m_OnFriendListUpdateSubject; } }

		private Subject<Dictionary<string, object>> m_OnCustomAuthenticationResponseSubject = new Subject<Dictionary<string, object>>();
		public IObservable<Dictionary<string, object>> OnCustomAuthenticationResponseAsObservable { get { return m_OnCustomAuthenticationResponseSubject; } }

		private Subject<string> m_OnCustomAuthenticationFailedSubject = new Subject<string>();
		public IObservable<string> OnCustomAuthenticationFailedAsObservable { get { return m_OnCustomAuthenticationFailedSubject; } }

		private Subject<OperationResponse> m_OnWebRpcResponseSubject = new Subject<OperationResponse>();
		public IObservable<OperationResponse> OnWebRpcResponseAsObservable { get { return m_OnWebRpcResponseSubject; } }

		private Subject<List<TypedLobbyInfo>> m_OnLobbyStatisticsUpdateSubject = new Subject<List<TypedLobbyInfo>>();
		public IObservable<List<TypedLobbyInfo>> OnLobbyStatisticsUpdateAsObservable { get { return m_OnLobbyStatisticsUpdateSubject; } }
		#endregion

		public void Start()
		{
			if( m_IsAutoConnectMasterServer )
			{
				ConnectMasterServer();
			}
		}

		public void ConnectMasterServer()
		{
			Debug.Log( "[Photon] ConnectMasterServer" );

			PhotonNetwork.GameVersion = m_GameVersion;
			PhotonNetwork.ConnectUsingSettings();
		}

		public void Disconnect()
		{
			Debug.Log( "[Photon] Disconnect" );
			PhotonNetwork.Disconnect();
		}

		public void JoinLobby()
		{
			Debug.Log( "[Photon] JoinLobby" );
			PhotonNetwork.JoinLobby();
		}

		public void LeaveLobby()
		{
			Debug.Log( "[Photon] LeaveLobby" );
			PhotonNetwork.LeaveLobby();
		}

		public void JoinRoom( string room_name )
		{
			Debug.Log( "[Photon] JoinRoom" );

			if( string.IsNullOrEmpty( room_name ) == false )
			{
				Debug.Log( "[Photon] JoinRoom() was called at JoinOrCreateRoom(). Room name is " + room_name);
				PhotonNetwork.JoinOrCreateRoom( room_name, VRWSRoomProperties.CreateRoomOptions( room_name, m_MaxPlayers ), TypedLobby.Default );
			}
			else
			{
				Debug.Log( "[Photon] JoinRoom() was called at JoinRandomRoom()." );
				PhotonNetwork.JoinRandomRoom();
			}
		}

		public void LeaveRoom( bool becomeInactive = true )
		{
			Debug.Log( "[Photon] LeaveRoom" );
			PhotonNetwork.LeaveRoom( becomeInactive );
		}

		public bool InRoom
		{
			get { return PhotonNetwork.InRoom; }
		}

        public void SetOfflineMode()
        {
            PhotonNetwork.OfflineMode = true;
        }

        public bool IsOfflineMode()
        {
            return PhotonNetwork.OfflineMode;
        }

        #region Callbacks

        public override void OnConnected()
		{
			Debug.Log( "[Photon] OnConnected" );
			m_OnConnectedSubject.OnNext( Unit.Default );	
		}

		public override void OnLeftRoom()
		{
			Debug.Log( "[Photon] OnLeftRoom" );
			m_OnLeftRoomSubject.OnNext( Unit.Default );
		}

		public override void OnMasterClientSwitched( Player newMasterClient )
		{
			Debug.Log( "[Photon] OnMasterClientSwitched" );
			m_OnMasterClientSwitchedSubject.OnNext( newMasterClient );
		}

		public override void OnCreateRoomFailed( short returnCode, string message )
		{
			Debug.LogFormat( "[Photon] OnCreateRoomFailed returnCode = {0}, message = {1}", returnCode, message );
			m_OnCreateRoomFailedSubject.OnNext( new FailureReason( returnCode, message ) );
		}

		public override void OnJoinRoomFailed( short returnCode, string message )
		{
			Debug.LogFormat( "[Photon] OnJoinRoomFailed returnCode = {0}, message = {1}", returnCode, message );
			m_OnJoinRoomFailedSubject.OnNext( new FailureReason( returnCode, message ) );
		}

		public override void OnCreatedRoom()
		{
			Debug.Log( "[Photon] OnCreatedRoom" );
			m_OnCreatedRoomSubject.OnNext( Unit.Default );
		}

		public override void OnJoinedLobby()
		{
			Debug.Log( "[Photon] OnJoinedLobby" );
			m_OnJoinedLobbySubject.OnNext( Unit.Default );
		}

		public override void OnLeftLobby()
		{
			Debug.Log( "[Photon] OnLeftLobby" );
			m_OnLeftLobbySubject.OnNext( Unit.Default );
		}

		public override void OnDisconnected( DisconnectCause cause )
		{
			Debug.Log( "[Photon] OnDisconnected(" + cause + ")" );

			m_OnDisconnectedSubject.OnNext( cause );

            if (cause == DisconnectCause.DisconnectByClientLogic)
            {
                Debug.Log("再接続不要");

                return;
            }

			if( cause == DisconnectCause.None || cause == DisconnectCause.ClientTimeout)
			{
				ConnectMasterServer();
			}
			else if( cause == DisconnectCause.ExceptionOnConnect )
			{
				PhotonNetwork.OfflineMode = true;
			}
		}

		public override void OnRegionListReceived( RegionHandler regionHandler )
		{
			Debug.Log( "[Photon] OnRegionListReceived" );
			m_OnRegionListReceivedSubject.OnNext( regionHandler );
		}


		public override void OnRoomListUpdate( List<RoomInfo> roomList )
		{
			Debug.Log( "[Photon] OnRoomListUpdate" );
			m_OnRoomListUpdateSubject.OnNext( roomList );
		}


		public override void OnJoinedRoom()
		{
			Debug.LogFormat( "[Photon] OnJoinedRoom() called by PUN." );
			Debug.LogFormat( "[Photon] Current room: {0}", PhotonNetwork.CurrentRoom.Name );

			m_OnJoinedRoomSubject.OnNext( Unit.Default );
		}


		public override void OnPlayerEnteredRoom( Player newPlayer )
		{
			Debug.Log( "[Photon] OnPlayerEnteredRoom" );
			m_OnPlayerEnteredRoomSubject.OnNext( newPlayer );
		}


		public override void OnPlayerLeftRoom( Player otherPlayer )
		{
			Debug.Log( "[Photon] OnPlayerLeftRoom" );
			m_OnPlayerLeftRoomSubject.OnNext( otherPlayer );
		}


		public override void OnJoinRandomFailed( short returnCode, string message )
		{
			Debug.LogFormat( "[Photon] OnJoinRandomFailed returnCode = {0}, message = {1}", returnCode, message );
			m_OnJoinRandomFailedSubject.OnNext( new FailureReason( returnCode, message ) );
		}


		public override void OnConnectedToMaster()
		{
			Debug.Log( "[Photon] OnConnectedToMaster() was called by PUN." );

			m_OnConnectedToMasterSubject.OnNext( Unit.Default );


			if( m_IsAutoJoinLobby && !PhotonNetwork.OfflineMode )
			{
				JoinLobby();
			}
		}


		public override void OnRoomPropertiesUpdate( ExitGames.Client.Photon.Hashtable propertiesThatChanged )
		{
			Debug.Log( "[Photon] OnRoomPropertiesUpdate" );
			m_OnRoomPropertiesUpdateSubject.OnNext( propertiesThatChanged );
		}


		public override void OnPlayerPropertiesUpdate( Player target, ExitGames.Client.Photon.Hashtable changedProps )
		{
			Debug.Log( "[Photon] OnPlayerPropertiesUpdate" );
			m_OnPlayerPropertiesUpdateSubject.OnNext( new PlayerProperties( target, changedProps ) );
		}


		public override void OnFriendListUpdate( List<FriendInfo> friendList )
		{
			Debug.Log( "[Photon] OnFriendListUpdate" );
			m_OnFriendListUpdateSubject.OnNext( friendList );
		}


		public override void OnCustomAuthenticationResponse( Dictionary<string, object> data )
		{
			Debug.Log( "[Photon] OnCustomAuthenticationResponse" );
			m_OnCustomAuthenticationResponseSubject.OnNext( data );
		}

		public override void OnCustomAuthenticationFailed( string debugMessage )
		{
			Debug.Log( "[Photon] OnCustomAuthenticationFailed " + debugMessage );
			m_OnCustomAuthenticationFailedSubject.OnNext( debugMessage );
		}


		public override void OnWebRpcResponse( OperationResponse response )
		{
			Debug.Log( "[Photon] OnWebRpcResponse" );
			m_OnWebRpcResponseSubject.OnNext( response );
		}


		public override void OnLobbyStatisticsUpdate( List<TypedLobbyInfo> lobbyStatistics )
		{
			Debug.Log( "[Photon] OnLobbyStatisticsUpdate" );
			m_OnLobbyStatisticsUpdateSubject.OnNext( lobbyStatistics );
		}

        #endregion
    }
}

