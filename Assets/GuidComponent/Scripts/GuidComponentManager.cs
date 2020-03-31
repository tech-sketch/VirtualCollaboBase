using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Recollab.SceneManagement;
using Zenject;
using UniRx;

namespace GuidBasedReference
{
    public class GuidComponentManager : MonoBehaviour, ISerializationCallbackReceiver
    {
        // Singleton interface
        private static GuidComponentManager instance;
        public static GuidComponentManager Instance
        {
            get
            {
                if(instance == null){
                    GuidComponentManager[] objects = FindObjectsOfType<GuidComponentManager>();
                    if(objects.Length == 1)
                    {
                        instance = objects[0];
                        DontDestroyOnLoad(instance.gameObject);
                    }
                }
                return instance;
            }
        }

        private Dictionary<string, GameObject> guidToPrefabMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> GuidToPrefabMap
        {
            get{ return guidToPrefabMap; }
        }

        private Dictionary<string, GameObject> guidToObjectMap = new Dictionary<string, GameObject>();
        public Dictionary<string, GameObject> GuidToSceneObjectMap
        {
            get{ return guidToObjectMap; }
        }

		[Header("Debug Info")]
        [SerializeField] private List<GameObject> guidPrefabList;
        [SerializeField] private List<GameObject> guidSceneObjectList;
        [SerializeField] private List<GameObject> duplicateGuidObjectList = Enumerable.Empty<GameObject>().ToList();

        [Inject]
        private void Initialize(ISceneManager manager)
        {
            manager.OnRoomLoaded.Subscribe(_ => UpdateSceneObjectMap()).AddTo(this);
            manager.OnLobbyLoaded.Subscribe(_ => InitializeSceneObjectMap()).AddTo(this);
        }

        void Awake()
        {
            duplicateGuidObjectList.Clear();
            guidToPrefabMap.Clear();
            guidToObjectMap.Clear();

            UpdatePrefabMap();
            UpdateSceneObjectMap();

            guidPrefabList = guidToPrefabMap.Values.ToList();
            guidSceneObjectList = guidToObjectMap.Values.ToList();
        }

		public void OnAfterDeserialize()
        {

        }

        public void OnBeforeSerialize()
        {
            duplicateGuidObjectList.Clear();
            guidToPrefabMap.Clear();
            guidToObjectMap.Clear();

            UpdatePrefabMap();
            UpdateSceneObjectMap();

            guidPrefabList = guidToPrefabMap.Values.ToList();
            guidSceneObjectList = guidToObjectMap.Values.ToList();
        }

        private void OnValidate()
        {
            if (duplicateGuidObjectList != null && duplicateGuidObjectList.Count > 0)
            {
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                string objectName = this.gameObject.name;
                string message = "*** Duplicate GUID exists in current scene (" + sceneName + "). GuidManager is \"" + objectName + "\". ***";
                Debug.LogError(message);
            }
        }


        private void UpdatePrefabMap()
        {
            GameObject[] gameObjects = GameObjectUtils.FindAllNotInScene();

            foreach (var go in gameObjects)
            {
                GuidComponent guidComp = go.GetComponent<GuidComponent>();
                if (guidComp != null)
                {
                    string guid = guidComp.GetGuid().ToString();
                    if (!guidToPrefabMap.ContainsKey(guid) && !guidToObjectMap.ContainsKey(guid))
                    {
                        guidToPrefabMap.Add(guid, guidComp.gameObject);
                    }
                    else 
                    {
                        bool existsInAsset = (guidToPrefabMap.ContainsKey(guid) && guidToPrefabMap[guid] != null && guidToPrefabMap[guid] != guidComp.gameObject);
                        bool existsInScene = (guidToObjectMap.ContainsKey(guid) && guidToObjectMap[guid] != null && guidToObjectMap[guid] != guidComp.gameObject);
                        if (existsInAsset || existsInScene)
                        {
                            duplicateGuidObjectList.Add(guidComp.gameObject);
                        }
                    }
                }
            }
        }

        private void UpdateSceneObjectMap()
        {
     
            GameObject[] gameObjects = GameObjectUtils.FindAllInScene();
  
            foreach (var go in gameObjects)
            {
                GuidComponent guidComp = go.GetComponent<GuidComponent>();
                if (guidComp != null)
                {
                    string guid = guidComp.GetGuid().ToString();
                    if (!guidToObjectMap.ContainsKey(guid) && !guidToPrefabMap.ContainsKey(guid))
                    {
                        guidToObjectMap.Add(guid, guidComp.gameObject);
                    }
                    else 
                    {
                        bool existsInScene = (guidToObjectMap.ContainsKey(guid) && guidToObjectMap[guid] != null && guidToObjectMap[guid] != guidComp.gameObject);
                        bool existsInAsset = (guidToPrefabMap.ContainsKey(guid) && guidToPrefabMap[guid] != null && guidToPrefabMap[guid] != guidComp.gameObject);
                        if (existsInScene || existsInAsset)
                        {
                            duplicateGuidObjectList.Add(guidComp.gameObject);
                        }
                    }
                }
            }
        }

        private void InitializeSceneObjectMap()
        {
            guidToObjectMap = new Dictionary<string, GameObject>();
        }
    }
}
