using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UniRx;
using System;

namespace SimpleDrawing.Sharing
{
    [RequireComponent(typeof(PhotonView))]
    public class RemoteRayCastDrawer : MonoBehaviour, IPunObservable
    {
        private enum RayDirectionType
        {
            TransformForward,
            TransformBackward,
            TransformRight,
            TransformLeft,
            TransformUp,
            TransformDown,
        }

        public bool RayCastEnabled = true;
        public Color PenColor = Color.black;
        public int PenWidth = 4;
        public bool Erase = false;

        public bool LocalSyncDraw = false;
        public DrawableCanvas TargetDrawableCanvas;

        [SerializeField]
        RayDirectionType directionType = RayDirectionType.TransformDown;
        [SerializeField]
        float rayDistance = 0.02f;
        [SerializeField]
        float rayOffset = 0.065f;

        [SerializeField]
        private GameObject Cone;

        //[SerializeField]
        //private Button m_BlueButton = null;

        Vector2 defaultTexCoord = new Vector2(-1,-1);

        Material material;
        PhotonView photonView;

        int targetCanvasId;
        Vector2 currentTexCoord;
        Vector2 previousTexCoord;

        bool receivedRayCastEnabled;
        float receivedPenColorR;
        float receivedPenColorG;
        float receivedPenColorB;
        float receivedPenColorA;
        int receivedPenWidth;
        bool receivedErase;
        bool isDrawing;
        int receivedTargetCanvasId;
        Vector2 receivedCurrentTexCoord;
        Vector2 receivedPreviousTexCoord;

        private static Subject<Unit> m_OnDrawStart = new Subject<Unit>();
        public static IObservable<Unit> OnDrawStart { get { return m_OnDrawStart; } }

        private static Subject<Unit> m_OnDrawEnd = new Subject<Unit>();
        public static IObservable<Unit> OnDrawEnd { get { return m_OnDrawEnd; } }

        void Start()
		{
            photonView = GetComponent<PhotonView>();
           
		}


        void Update()
        {
            //ペン先の色を変える
            Cone.GetComponent<Renderer>().material.color = PenColor;

            // シングルプレイヤーの場合!photonView.IsMineがTrue判定なので雑にはじく
            if (!photonView.IsMine && !PhotonNetwork.OfflineMode)
            {
                RayCastEnabled = receivedRayCastEnabled;
                PenColor.r = receivedPenColorR;
                PenColor.g = receivedPenColorG;
                PenColor.b = receivedPenColorB;
                PenColor.a = receivedPenColorA;
                PenWidth = receivedPenWidth;
                Erase = receivedErase;
                targetCanvasId = receivedTargetCanvasId;
                currentTexCoord = receivedCurrentTexCoord;
                previousTexCoord = receivedPreviousTexCoord;
               
                if (RayCastEnabled)
                {
                    var target = PhotonView.Find(targetCanvasId);
                    if (target != null)
                    {
                        var drawObject = target.transform.GetComponent<DrawableCanvas>();
                        if (drawObject != null)
                        {
                            if (!previousTexCoord.Equals(defaultTexCoord))
                            {
                                if (Erase)
                                {
                                    drawObject.Erase(currentTexCoord, previousTexCoord, PenWidth);
                                }
                                else
                                {
                                    drawObject.Draw(currentTexCoord, previousTexCoord, PenWidth, PenColor);
                                }
                            }
                        }
                    }
                }
            }
            else
            {

                // シングルプレイヤーなら無条件で書く
                if (RayCastEnabled || PhotonNetwork.OfflineMode)
                {
                    Vector3 dir = GetCurrentDirection();

                    var ray = new Ray(this.transform.position + dir * rayOffset, dir);
                    RaycastHit hitInfo;
                    if(Physics.Raycast(ray, out hitInfo, rayDistance))
                    {
                     
                        if(hitInfo.collider != null && hitInfo.collider is MeshCollider)
                        {
                            if (!LocalSyncDraw)
                            {
                                TargetDrawableCanvas = hitInfo.transform.GetComponent<DrawableCanvas>();
                            }
                            if (TargetDrawableCanvas != null)
                            {
                                var target = TargetDrawableCanvas.transform.GetComponent<PhotonView>();
                                targetCanvasId = (target != null) ? target.ViewID : -1;

                                previousTexCoord = currentTexCoord;  
                                currentTexCoord = hitInfo.textureCoord;
                               
                                //Debug.Log("currentTexCoordは" + currentTexCoord);

                                if (!previousTexCoord.Equals(defaultTexCoord))
                                {
                                    if (Erase)
                                    {
                                        TargetDrawableCanvas.Erase(currentTexCoord, previousTexCoord, PenWidth);
                                    }
                                    else
                                    {
                                        TargetDrawableCanvas.Draw(currentTexCoord, previousTexCoord, PenWidth, PenColor);
                                    }

                                    if (!isDrawing)
                                    {
                                        m_OnDrawStart.OnNext(Unit.Default);
                                    }
                                    
                                    isDrawing = true;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning("If you want to draw using a RaycastHit, need set MeshCollider for object.");
                        }
                    }
                    else
                    {
                        if (isDrawing)
                        {
                            m_OnDrawEnd.OnNext(Unit.Default);
                            isDrawing = false;
                        }
                        targetCanvasId = -1;
                        currentTexCoord = defaultTexCoord;
                    }
                }
                else
                {
                    targetCanvasId = -1;
                    currentTexCoord = defaultTexCoord;
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(RayCastEnabled);
                stream.SendNext(PenColor.r);
                stream.SendNext(PenColor.g);
                stream.SendNext(PenColor.b);
                stream.SendNext(PenColor.a);
                stream.SendNext(PenWidth);
                stream.SendNext(Erase);
                stream.SendNext(targetCanvasId);
                stream.SendNext(currentTexCoord);
                // stream.SendNext(previousTexCoord);
            }
            else
            {
                
                receivedPreviousTexCoord = receivedCurrentTexCoord;

                receivedRayCastEnabled = (bool)stream.ReceiveNext();
                receivedPenColorR = (float)stream.ReceiveNext();
                receivedPenColorG = (float)stream.ReceiveNext();
                receivedPenColorB = (float)stream.ReceiveNext();
                receivedPenColorA = (float)stream.ReceiveNext();
                receivedPenWidth = (int)stream.ReceiveNext();
                receivedErase = (bool)stream.ReceiveNext();
                receivedTargetCanvasId = (int)stream.ReceiveNext();
                receivedCurrentTexCoord = (Vector2)stream.ReceiveNext();
                // receivedPreviousTexCoord = (Vector2)stream.ReceiveNext();
            }
        }

        private Vector3 GetCurrentDirection()
        {
            Vector3 direction = Vector3.zero;
            switch(directionType)
            {
                case RayDirectionType.TransformForward:
                    direction =  this.transform.forward;
                    break;
                case RayDirectionType.TransformBackward:
                    direction = -this.transform.forward;
                    break;
                case RayDirectionType.TransformRight:
                    direction =  this.transform.right;
                    break;
                case RayDirectionType.TransformLeft:
                    direction = -this.transform.right;
                    break;
                case RayDirectionType.TransformUp:
                    direction =  this.transform.up;
                    break;
                case RayDirectionType.TransformDown:
                    direction = -this.transform.up;
                    break;
            }
            return direction;
        }


        public void ClearWhiteboard()
        {
            var targetPhotonView = TargetDrawableCanvas.transform.GetComponent<PhotonView>();
            var targetPhotonViewId = (targetPhotonView != null) ? targetPhotonView.ViewID : -1;
            photonView.RPC("ClearWhiteboardRPC", RpcTarget.All, targetPhotonViewId);

        }

        [PunRPC]
        private void ClearWhiteboardRPC(int targetPhotonViewId)
        {
            var target = PhotonView.Find(targetPhotonViewId);
            if (target != null)
            {
                var drawObject = target.transform.GetComponent<DrawableCanvas>();
                if (drawObject != null)
                {
                    drawObject.ResetCanvas();
                }
            }
        }

        public static implicit operator RemoteRayCastDrawer(Unit v)
        {
            throw new NotImplementedException();
        }
    }
}
