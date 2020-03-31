using UnityEngine;
using Photon.Pun;

namespace SimpleDrawing.Sharing
{
    public class SharingInitializer : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        int SerializationRate = 30;
        [SerializeField]
        GameObject RemoteDrawerPrefab;
        [SerializeField]
        Transform RemoteDrawerInitialTransform;
        [SerializeField]
        GameObject WhiteBoardPrefab;
        [SerializeField]
        Transform WhiteBoardInitialTransform;

        void Awake()
        {
            // Defines how many times per second OnPhotonSerialize should be called on PhotonViews.
            PhotonNetwork.SendRate = 2 * SerializationRate;
            PhotonNetwork.SerializationRate = SerializationRate;

            Debug.LogFormat("PhotonNetwork.SendRate: {0}", PhotonNetwork.SendRate);
            Debug.LogFormat("PhotonNetwork.SerializationRate: {0}", PhotonNetwork.SerializationRate);
        }

        public override void OnJoinedRoom()
        {
            InstantiateRemoteDrawer();
            InstantiateWhiteBoard();
        }

        private void InstantiateRemoteDrawer()
        {
            if (RemoteDrawerPrefab != null)
            {
                Debug.LogFormat("Instantiate: {0}", this.RemoteDrawerPrefab.name);
                Vector3 initPos = RemoteDrawerInitialTransform.position;
                Quaternion initRot = RemoteDrawerInitialTransform.rotation;
                GameObject drawer = PhotonNetwork.Instantiate(this.RemoteDrawerPrefab.name, initPos, initRot);
                drawer.transform.SetParent(Camera.main.transform, true);
            }
        }

        private void InstantiateWhiteBoard()
        {
            if (PhotonNetwork.IsMasterClient && WhiteBoardPrefab != null)
            {
                Debug.LogFormat("Instantiate: {0}", this.WhiteBoardPrefab.name);
                Vector3 initPos = WhiteBoardInitialTransform.position;
                Quaternion initRot = WhiteBoardInitialTransform.rotation;
                PhotonNetwork.InstantiateSceneObject(this.WhiteBoardPrefab.name, initPos, initRot);
            }
        }
    }
}
