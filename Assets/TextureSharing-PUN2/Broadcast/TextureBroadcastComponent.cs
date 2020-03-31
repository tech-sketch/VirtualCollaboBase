using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UniRx;
using System;

namespace TextureSharing
{

    public enum BroadcastingBytesEventCode
    {
        BeginStream = 12,
        Streaming = 13,
    }

    public class TextureBroadcastComponent : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField]
        int messagePerSecond = 100; // 100 Messages / Second

        int bytePerMessage = 1000; // 1KBytes / Message
     
        Texture2D texture; // ★ Readable texture ★
        bool isReceiving;
        byte[] receiveBuffer;
        int totalDataSize;
        int currentReceivedDataSize;
        int receivedMessageCount;

        private Subject<Texture2D> m_OnReceivedRawTexture = new Subject<Texture2D>();
        public IObservable<Texture2D> OnReceivedRawTexture { get { return m_OnReceivedRawTexture; } }

        void Start()
        {

        }

        #region sender methods

        public void BroadcastTexture(Texture2D texture2D)
        {
            texture = texture2D;
            try
            {
                texture.GetPixels32();
            }
            catch (UnityException e)
            {
                Debug.LogError("!! This texture is not readable !!");
            }

            byte[] rawTextureData = texture.EncodeToPNG();

            int width = texture.width;
            int height = texture.height;
            int dataSize = rawTextureData.Length;
            int viewId = this.photonView.ViewID;

            Debug.Log("*************************");
            Debug.Log(" BroadcastTexture");
            Debug.Log(" Texture size: " + width + "x" + height + " = " + width*height + "px");
            Debug.Log(" RawTextureData: " + rawTextureData.Length + "bytes");
            Debug.Log("*************************");

            StreamTextureDataToOtherClients(rawTextureData, width, height, dataSize, viewId);
        }

        void StreamTextureDataToOtherClients(byte[] rawTextureData, int width, int height, int dataSize, int viewId)
        {
            Debug.Log("***********************************");
            Debug.Log(" StreamTextureDataToOthers  ");
            Debug.Log("***********************************");
            
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others,
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
            PhotonNetwork.RaiseEvent((byte)BroadcastingBytesEventCode.BeginStream, textureInfo, raiseEventOptions, sendOptions);

            // Send raw data
            // The SlowDown operator is not necessary if you ignore the limit on the number of messages per second of Photon Cloud.
            rawTextureData.ToObservable()
                .Buffer(bytePerMessage)
                // .SlowDown(1.0f/messagePerSecond)
                .Subscribe(byteSubList =>
                {
                    byte[] sendData = new byte[byteSubList.Count];
                    byteSubList.CopyTo(sendData, 0);
                    PhotonNetwork.RaiseEvent((byte)BroadcastingBytesEventCode.Streaming, sendData, raiseEventOptions, sendOptions);
                }).AddTo(this);
        }

        #endregion

        #region receiver methods

        //**************************************************
        //  These methods are executed in receiver clients
        //**************************************************
        public void OnEvent(ExitGames.Client.Photon.EventData photonEvent)
        {
            if(photonEvent.Code == (byte)BroadcastingBytesEventCode.BeginStream)
            {
                int[] data = (int[])photonEvent.Parameters[ParameterCode.Data];
                OnReceivedTextureInfo(data);
            }
            if(photonEvent.Code == (byte)BroadcastingBytesEventCode.Streaming)
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

            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

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

        #endregion
    }
}