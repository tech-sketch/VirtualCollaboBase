using System;
using UniRx;
using UnityEngine;

namespace VRWS.SimpleDrawing.Sharing
{
    [RequireComponent(typeof(Renderer))]
    [DisallowMultipleComponent]
    public class LocalSyncBoard : MonoBehaviour
    {
        Material material;
        RenderTexture whiteboardRT;
        WhiteBoard prevWhiteBoard;

        void Start()
        {
            material = GetComponent<Renderer>().material;
            WhiteBoard.OnClickWhiteBoard.Subscribe(target => OnClickWhiteBoard(target)).AddTo(this);
            OnClickWhiteBoard(WhiteBoard.DefaultTargetWhiteboard);

            
        }

        private void OnClickWhiteBoard(GameObject localSyncSrc)
        {
            if(prevWhiteBoard != null)
            {
                prevWhiteBoard.SetWhiteboardUICameraOff();
            }

            WhiteBoard whiteBoard = localSyncSrc.GetComponent<WhiteBoard>();
            whiteboardRT = whiteBoard.GetWhiteboardUICameraTexture();
            material.mainTexture = whiteboardRT;
            prevWhiteBoard = whiteBoard;

        }

        public void SetWhiteboard()
        {
            Observable.TimerFrame(1).Subscribe(_ => prevWhiteBoard.ClickWhiteBoard()).AddTo(this);
            
            Debug.Log("prevWhiteBoard??"+ prevWhiteBoard.gameObject.transform.parent.transform.parent.gameObject.name);
        }
        
        public Texture2D GetCurrentWhiteboradAsTexture2D()
        {
            var currentActiveRT = RenderTexture.active;

            RenderTexture.active = whiteboardRT;
            Texture2D tex2d = new Texture2D(whiteboardRT.width, whiteboardRT.height, TextureFormat.ARGB32, false, false);
            tex2d.ReadPixels(new Rect(0, 0, whiteboardRT.width, whiteboardRT.height), 0, 0);
            tex2d.Apply();

            RenderTexture.active = currentActiveRT;

            return tex2d;
        }
    }
}