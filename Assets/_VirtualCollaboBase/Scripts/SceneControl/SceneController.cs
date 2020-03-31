using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Async;
using Zenject;
using Recollab.Network;
using System;

namespace Recollab.SceneManagement
{
	public interface ISceneManager
	{
		void TransitionLobbyScene();
		void TransitionRoomScene( string scene, string room_name );

		IObservable<Unit> OnLobbyLoaded { get; }
        IObservable<Unit> OnRoomLoadStart { get; }
        IObservable<Unit> OnRoomLoaded { get; }
	}

	// ★★ここのInterface継承は仮。もう一枚上が必要
	public class SceneController : MonoBehaviour, ISceneManager
	{
		private INetworkEngineConnector m_Connector = null;
		private SceneMapFactory SceneMapFactory = null;

		private Subject<Unit> m_OnLobbyLoadedSubject = new Subject<Unit>();
		public IObservable<Unit> OnLobbyLoaded { get { return m_OnLobbyLoadedSubject; } }

		private Subject<Unit> m_OnRoomLoadedSubject = new Subject<Unit>();
		public IObservable<Unit> OnRoomLoaded { get { return m_OnRoomLoadedSubject; } }

        private Subject<Unit> m_OnRoomLoadStartSubject = new Subject<Unit>();
        public IObservable<Unit> OnRoomLoadStart { get { return m_OnRoomLoadStartSubject; } }

        [Inject]
		private void Initialize( INetworkEngineConnector connector )
		{
			SceneMapFactory = new SceneMapFactory();
			m_Connector = connector;

			m_Connector.OnJoinedLobbyAsObservable
				.First()
				.Subscribe( _ => TransitionLobbyScene() )
				.AddTo( this );

		}

		// ステージシーンはビルド時に組み込み済みの設計とする。
		// 将来的にはモデルをアセットバンドルでネットワークから持ってくる実装が考えられるが現時点では考慮しない。
		public async void LoadInitialScene()
		{
			await LoadSceneAdditiveAsync( SystemDefines.SCENE_NAME_LOBBY_STAGE_01 );
			await LoadSceneAdditiveAsync( SystemDefines.SCENE_NAME_LOBBY );

			SceneManager.SetActiveScene( SceneManager.GetSceneByName( SystemDefines.SCENE_NAME_LOBBY_STAGE_01 ) );
            m_OnLobbyLoadedSubject.OnNext(Unit.Default);

        }

        public async void TransitionRoomScene( string scene, string room_name )
		{
			Debug.Log( "TransitionRoomScene " + scene );

			m_OnRoomLoadStartSubject.OnNext( Unit.Default );

			SceneManager.SetActiveScene( SceneManager.GetSceneByName( SystemDefines.SCENE_NAME_CORE ) );

			EscapeControllerObject();

			foreach( Scene lobbyScene in SceneMapFactory.GetLoadedLobbyScenes() )
			{
				if( lobbyScene.IsValid() == false )
					continue;

				await UnloadSceneAsync( lobbyScene );
			}

			foreach( Scene roomScene in SceneMapFactory.GetLoadedRoomScenes() )
			{
				if( roomScene.IsValid() == false )
					continue;

				await UnloadSceneAsync( roomScene );
			}

			foreach( string stage in SceneMapFactory.GetRoomSceneNames( scene ) )
			{
				await LoadSceneAdditiveAsync( stage );
			}

			SceneManager.SetActiveScene( SceneManager.GetSceneByName( scene ) );
            m_OnRoomLoadedSubject.OnNext(Unit.Default);

            if ( m_Connector.InRoom == false)
            {
                m_Connector.JoinRoom(room_name);
            }
				


		}

		public async void TransitionLobbyScene()
		{
			if( m_Connector.InRoom == true )
				m_Connector.LeaveRoom();

            if (m_Connector.IsOfflineMode() == true)
                m_Connector.ConnectMasterServer();

            EscapeControllerObject();

			foreach( Scene roomScene in SceneMapFactory.GetLoadedRoomScenes() )
			{
				if( roomScene.IsValid() == false )
					continue;

				await UnloadSceneAsync( roomScene );
			}

			foreach( Scene lobbyScene in SceneMapFactory.GetLoadedLobbyScenes() )
			{
				if( lobbyScene.IsValid() == false )
					continue;

				await UnloadSceneAsync( lobbyScene );
			}

			foreach( string stage in SceneMapFactory.GetLobbySceneNames( SystemDefines.SCENE_NAME_LOBBY_STAGE_01 ) )
			{
				await LoadSceneAdditiveAsync( stage );
			}

			SceneManager.SetActiveScene( SceneManager.GetSceneByName( SystemDefines.SCENE_NAME_LOBBY_STAGE_01 ) );
            m_OnLobbyLoadedSubject.OnNext( Unit.Default );
		}

		private AsyncOperation UnloadSceneAsync( Scene scene )
		{
			Debug.Log( "UnloadSceneAsync -> " + scene.name );

			return SceneManager.UnloadSceneAsync( scene );
		}

		private AsyncOperation UnloadSceneAsync( string scene )
		{
			Debug.Log( "UnloadSceneAsync -> " + scene );
			return SceneManager.UnloadSceneAsync( scene );
		}

		private AsyncOperation LoadSceneAdditiveAsync( string scene )
		{
			Debug.Log( "LoadSceneAsync -> " + scene );
			return SceneManager.LoadSceneAsync( scene, LoadSceneMode.Additive );
		}

		/// <summary>
		/// 現実世界のコントローラのトラッキングをロストした際にVRTKのコントローラオブジェクトが消滅し、
		/// トラッキングが復帰したときに再生成される仕様のため、再生成されるタイミングによっては、
		/// アンロードするシーンにオブジェクトが生成される。
		/// そのため、シーン内を全検索してオブジェクトを退避させる必要がある
		/// </summary>
		private void EscapeControllerObject()
		{
			var r_controller = GameObject.Find( "[VRTK][AUTOGEN][RightControllerScriptAlias][BasePointerRenderer_Origin_Smoothed]" );
			var l_controller = GameObject.Find( "[VRTK][AUTOGEN][LeftControllerScriptAlias][BasePointerRenderer_Origin_Smoothed]" );

			if( r_controller != null )
				SceneManager.MoveGameObjectToScene( r_controller, SceneManager.GetSceneByName( SystemDefines.SCENE_NAME_CORE ) );

			if( l_controller != null )
				SceneManager.MoveGameObjectToScene( l_controller, SceneManager.GetSceneByName( SystemDefines.SCENE_NAME_CORE ) );
		}
	}


	public class SceneMapFactory
	{
		private readonly static Dictionary<string,List<string>> LobbyMap = new Dictionary<string, List<string>>
		{
			{
				SystemDefines.SCENE_NAME_LOBBY_STAGE_01, new List<string>
				{
					SystemDefines.SCENE_NAME_LOBBY,
					SystemDefines.SCENE_NAME_LOBBY_STAGE_01
				}
			},
			{
				SystemDefines.SCENE_NAME_LOBBY_STAGE_02, new List<string>
				{
					SystemDefines.SCENE_NAME_LOBBY,
					SystemDefines.SCENE_NAME_LOBBY_STAGE_02
				}
			}
		};

		private readonly static  Dictionary<string,List<string>> RoomMap = new Dictionary<string, List<string>>
		{
			{
				SystemDefines.SCENE_NAME_ROOM_STAGE_01, new List<string>
				{
					SystemDefines.SCENE_NAME_ROOM,
					SystemDefines.SCENE_NAME_ROOM_STAGE_01
				}
			},
			{
				SystemDefines.SCENE_NAME_ROOM_STAGE_02, new List<string>
				{
					SystemDefines.SCENE_NAME_ROOM,
					SystemDefines.SCENE_NAME_ROOM_STAGE_02
				}
			}
		};

		public Scene GetCoreScene()
		{
			return SceneManager.GetSceneByName( SystemDefines.SCENE_NAME_CORE );
		}

		public List<string> GetLobbySceneNames( string stage_name )
		{
			return LobbyMap[stage_name];
		}

		public List<string> GetRoomSceneNames( string stage_name )
		{
			return RoomMap[stage_name];
		}

		public List<Scene> GetLoadedLobbyScenes()
		{
			return GetLoadedScenes( LobbyMap );
		}

		public List<Scene> GetLoadedRoomScenes()
		{
			return GetLoadedScenes( RoomMap );
		}

		private List<Scene> GetLoadedScenes( Dictionary<string, List<string>> sceneMap )
		{
			List<Scene> loadedSceneList = new List<Scene>();

			foreach( var map in sceneMap )
			{
				foreach( var sceneName in map.Value )
				{
					Scene scene = SceneManager.GetSceneByName( sceneName );
					if( scene.isLoaded )
					{
						loadedSceneList.Add( scene );
					}
				}
			}

			return loadedSceneList;
		}
	}
}

