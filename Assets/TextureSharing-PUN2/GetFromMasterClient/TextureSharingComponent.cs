using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UniRx;
using SimpleDrawing;
using System;

namespace TextureSharing
{
    public enum StreamingBytesEventCode
    {
        BeginStream = 10,
        Streaming = 11,
    }

    public class TextureSharingComponent : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField]
        int messagePerSecond = 100; // 100 Messages / Second

        int bytePerMessage = 1000; // 1KBytes / Message
        Texture2D texture; // ★ Readable texture ★
        private PhotonView m_PhotonView = null;
        private DrawableCanvas m_DrawableCanvas = null;
        bool isReceiving;
        byte[] receiveBuffer;
        int totalDataSize;
        int currentReceivedDataSize;
        int receivedMessageCount;

        private Subject<Texture2D> m_OnReceivedRawTexture = new Subject<Texture2D>();
        public IObservable<Texture2D> OnReceivedRawTexture { get { return m_OnReceivedRawTexture; } }

        void Start()
        {
            m_PhotonView = GetComponent<PhotonView>();

        }

        public void GetRawTextureDataFromMasterClient(Texture2D texture2D)
        {
            Debug.Log(" GetRawTextureDataFromMasterClient");

            texture = texture2D;
            try
            {
                texture.GetPixels32();
            }
            catch (UnityException e)
            {
                Debug.LogError("!! This texture is not readable !!");
            }
            if (!PhotonNetwork.IsMasterClient)
            {
                m_PhotonView.RPC("GetRawTextureDataRPC", RpcTarget.MasterClient);
            }


        }

        public void SetDrawableCanvas(DrawableCanvas drawableCanvas)
        {
            m_DrawableCanvas = drawableCanvas;
        }

        //**************************************************************************
        // Client -> MasterClient (These methods are executed by the master client)
        //**************************************************************************
        [PunRPC]
        public void GetRawTextureDataRPC(PhotonMessageInfo info)
        {
            //ホワイトボードの場合は最新のテクスチャを取得する。
            if (m_DrawableCanvas != null)
            {
                texture = m_DrawableCanvas.GetTexture2D();
            }

            byte[] rawTextureData = texture.EncodeToPNG();

            int width = texture.width;
            int height = texture.height;
            int dataSize = rawTextureData.Length;
            int viewId = m_PhotonView.ViewID;

            Debug.Log("*************************");
            Debug.Log(" GetRawTextureDataRPC");
            Debug.Log(" RPC sender: " + info.Sender);
            Debug.Log(" Texture size: " + width + "x" + height + " = " + width*height + "px");
            Debug.Log(" RawTextureData: " + rawTextureData.Length + "bytes");
            Debug.Log("*************************");

            StreamTextureDataToRequestSender(rawTextureData, width, height, dataSize, viewId, info.Sender);
        }

        void StreamTextureDataToRequestSender(byte[] rawTextureData, int width, int height, int dataSize, int viewId, Player requestSender)
        {
            Debug.Log("***********************************");
            Debug.Log(" StreamTextureDataToRequestSender  ");
            Debug.Log("***********************************");
            
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others,
                TargetActors = new int[]{ requestSender.ActorNumber },
            };

            SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions
            {
                Reliability = true,
            };

            // Send info
            int[] textureInfo = new int[4];
            textureInfo[0] = viewId;
            textureInfo[1] = width;
            textureInfo[2] = height;
            textureInfo[3] = dataSize;
            PhotonNetwork.RaiseEvent((byte)StreamingBytesEventCode.BeginStream, textureInfo, raiseEventOptions, sendOptions);

            // Send raw data
            // The SlowDown operator is not necessary if you ignore the limit on the number of messages per second of Photon Cloud.
            rawTextureData.ToObservable()
                .Buffer(bytePerMessage)
                //.SlowDown(1.0f/messagePerSecond)
                .Subscribe(byteSubList =>
                {
                    byte[] sendData = new byte[byteSubList.Count];
                    byteSubList.CopyTo(sendData, 0);
                    PhotonNetwork.RaiseEvent((byte)StreamingBytesEventCode.Streaming, sendData, raiseEventOptions, sendOptions);
                }).AddTo(this);
        }

        //***************************************************************************
        // MasterClient -> Client (These methods are executed by the master client)
        //***************************************************************************
        public void OnEvent(ExitGames.Client.Photon.EventData photonEvent)
        {
            if(photonEvent.Code == (byte)StreamingBytesEventCode.BeginStream)
            {
                int[] data = (int[])photonEvent.Parameters[ParameterCode.Data];
                OnReceivedTextureInfo(data);
            }
            if(photonEvent.Code == (byte)StreamingBytesEventCode.Streaming)
            {
                byte[] data = (byte[])photonEvent.Parameters[ParameterCode.Data];
                OnReceivedRawTextureDataStream(data);
            }
        }

        void OnReceivedTextureInfo(int[] data)
        {
            int viewId = data[0];
            if (viewId != this.photonView.ViewID)
            {
                this.isReceiving = false;
                this.totalDataSize = 0;
                this.currentReceivedDataSize = 0;
                this.receivedMessageCount = 0;
                return;
            }

            this.isReceiving = true;
            this.currentReceivedDataSize = 0;
            this.receivedMessageCount = 0;

            int width = data[1];
            int height = data[2];
            int dataSize = data[3];
            this.totalDataSize = dataSize;
            this.receiveBuffer = new byte[dataSize];

            Debug.Log("*************************");
            Debug.Log(" OnReceivedTextureInfo");
            Debug.Log(" Texture size: " + width + "x" + height + "px");
            Debug.Log(" RawTextureDataSize: " + dataSize);
            Debug.Log("*************************");
        }

        void OnReceivedRawTextureDataStream(byte[] data)
        {
            if (this.isReceiving)
            {
                data.CopyTo(this.receiveBuffer, this.currentReceivedDataSize);
                this.currentReceivedDataSize += data.Length;
                this.receivedMessageCount++;

                if (this.currentReceivedDataSize >= (this.totalDataSize))
                {
                    this.isReceiving = false;
                    this.currentReceivedDataSize = 0;
                    this.receivedMessageCount = 0;

                    OnReceivedRawTextureData();
                }
            }
        }

        void OnReceivedRawTextureData()
        {
            Debug.Log("********************************");
            Debug.Log(" OnReceivedRawTextureData ");
            Debug.Log("********************************");

            texture.LoadImage(this.receiveBuffer);
            texture.Apply();

            m_OnReceivedRawTexture.OnNext(texture);
        }
    }
}