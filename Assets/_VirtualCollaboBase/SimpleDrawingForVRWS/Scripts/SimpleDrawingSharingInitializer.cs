using UnityEngine;
using Photon.Pun;
using VRTK;
using SimpleDrawing.Sharing;
using UniRx;
using System;

namespace VRWS.SimpleDrawing.Sharing
{
    public class SimpleDrawingSharingInitializer : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        GameObject RemoteDrawerParent;
        [SerializeField]
        GameObject RemoteDrawerPrefab;

        private static Subject<RemoteRayCastDrawer> m_OnPenSpawn = new Subject<RemoteRayCastDrawer>();
        public static IObservable<RemoteRayCastDrawer> OnPenSpawn { get { return m_OnPenSpawn; } }


        public override void OnJoinedRoom()
        {
            InstantiateRemoteDrawer();
        }

        private void InstantiateRemoteDrawer()
        {
            if (RemoteDrawerPrefab != null)
            {
                // Instantiate
                Debug.LogFormat("Instantiate: {0}", this.RemoteDrawerPrefab.name);
                Vector3 initPos = Vector3.zero;
                Quaternion initRot = Quaternion.identity;
                GameObject drawer = PhotonNetwork.Instantiate(this.RemoteDrawerPrefab.name, initPos, initRot);

                if (drawer.GetComponentInChildren<PhotonView>().IsMine)
                {
                    drawer.transform.SetParent(RemoteDrawerParent.transform, false);
                }

                // Initial setup
                SimpleDrawerController drawerController = drawer.GetComponent<SimpleDrawerController>();

				// ここはそのうちやり方を変える

                drawerController.VRController = RemoteDrawerParent.transform.parent.parent.parent.GetComponent<VRTK_ControllerEvents>();
                drawerController.Initialize();

                RemoteRayCastDrawer rayCastDrawer = drawer.GetComponentInChildren<RemoteRayCastDrawer>();

                m_OnPenSpawn.OnNext(rayCastDrawer);
                
            }
        }

    }
}
