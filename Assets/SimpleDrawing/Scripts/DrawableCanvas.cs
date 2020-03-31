using System;
using UnityEngine;
using UniRx;
using System.IO;

namespace SimpleDrawing
{
    public class DrawParams
    {
        public Vector2 currentTexCoord;
        public Vector2 previousTexCoord;
        public Color color;
        public int thickness;

        public DrawParams(Vector2 currentTexCoord, Vector2 previousTexCoord, Color color, int thickness)
        {
            this.currentTexCoord = currentTexCoord;
            this.previousTexCoord = previousTexCoord;
            this.color = color;
            this.thickness = thickness;
        }
    }

	[RequireComponent(typeof(Renderer))]
	[DisallowMultipleComponent]
    public class DrawableCanvas : MonoBehaviour
    {
        public bool ResetCanvasOnPlay = true;
        public Color ResetColor = new Color(1, 1, 1, 1);

        private Material drawLineMaterial = null;
        private Material singleColorMaterial = null;
        private RenderTexture drawableRenderTexture;

        #region ShaderPropertyID
		private int mainTexturePropertyID;
		private int lineColorPropertyID;
		private int thicknessPropertyID;
		private int startPositionUVPropertyID;
		private int endPositionUVPropertyID;
		private int singleColorPropertyID;
        #endregion

        private Subject<Unit> m_onReset = new Subject<Unit>();
        public IObservable<Unit> OnReset { get { return m_onReset; } }

        private Subject<DrawParams> m_onDraw = new Subject<DrawParams>();
        public IObservable<DrawParams> OnDraw { get { return m_onDraw; } }
        public CanvasType m_CanvasType;


    private void Awake()
        {
            drawLineMaterial = new Material(Resources.Load<Material>("SimpleDrawing.DrawLine"));
            singleColorMaterial = new Material(Resources.Load<Material>("SimpleDrawing.SingleColor"));
            InitializePropertyID();
        }

        private void Start()
        {
            InitializeDrawCanvas();
        }

        private void InitializePropertyID()
        {
            // DrawLine.shader
			mainTexturePropertyID = Shader.PropertyToID("_MainTex");
			lineColorPropertyID = Shader.PropertyToID("_LineColor");
			thicknessPropertyID = Shader.PropertyToID("_Thickness");
			startPositionUVPropertyID = Shader.PropertyToID("_StartPositionUV");
			endPositionUVPropertyID = Shader.PropertyToID("_EndPositionUV");

            // SingleColor.shader
            singleColorPropertyID = Shader.PropertyToID("_SingleColor");
        }

        private void InitializeDrawCanvas()
        {
            if (drawableRenderTexture == null)
            {
                Material material = GetComponent<Renderer>().material;
                Texture2D mainTexture = (Texture2D) material.mainTexture;

                drawableRenderTexture = new RenderTexture(mainTexture.width, mainTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                drawableRenderTexture.filterMode = mainTexture.filterMode;
                Graphics.Blit(mainTexture, drawableRenderTexture);
                material.mainTexture = drawableRenderTexture;

                if (ResetCanvasOnPlay)
                {
                    ResetCanvas();
                }
            }
        }

        public void Draw(Vector2 currentTexCoord, Vector2 previousTexCoord, int thickness, Color color)
        {
            if (drawableRenderTexture == null)
            {
                InitializeDrawCanvas();
            }

            if (m_CanvasType == CanvasType.Whiteboard)
            {
    

                currentTexCoord.x *= RoomTabletPresenter.m_TilingValue;
                currentTexCoord.y *= RoomTabletPresenter.m_TilingValue;
                previousTexCoord.x *= RoomTabletPresenter.m_TilingValue;
                previousTexCoord.y *= RoomTabletPresenter.m_TilingValue;

                currentTexCoord.x += RoomTabletPresenter.m_OffsetgValueX;
                currentTexCoord.y += RoomTabletPresenter.m_OffsetgValueY;
                previousTexCoord.x += RoomTabletPresenter.m_OffsetgValueX;
                previousTexCoord.y += RoomTabletPresenter.m_OffsetgValueY;

              //  Debug.Log("previousTexCoord.x , previousTexCoord.y は" + previousTexCoord.x + "　" + previousTexCoord.y);
               // Debug.Log("currentTexCoord.x ,currentTexCoord.y は" + currentTexCoord.x + "　" + currentTexCoord.y);
            }
            drawLineMaterial.SetVector(startPositionUVPropertyID, previousTexCoord);
            drawLineMaterial.SetVector(endPositionUVPropertyID, currentTexCoord);
            drawLineMaterial.SetInt(thicknessPropertyID, thickness);
            drawLineMaterial.SetVector(lineColorPropertyID, color);

            var mainPaintTextureBuffer = RenderTexture.GetTemporary(drawableRenderTexture.width, drawableRenderTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            Graphics.Blit(drawableRenderTexture, mainPaintTextureBuffer, drawLineMaterial);
            Graphics.Blit(mainPaintTextureBuffer, drawableRenderTexture);
            RenderTexture.ReleaseTemporary(mainPaintTextureBuffer);

            m_onDraw.OnNext(new DrawParams(currentTexCoord, previousTexCoord, color, thickness));
        }

        public void Erase(Vector2 currentTexCoord, Vector2 previousTexCoord, int thickness)
        {
            Draw(currentTexCoord, previousTexCoord, thickness, ResetColor);
        }

        public void ResetCanvas()
        {
            singleColorMaterial.SetVector(singleColorPropertyID, ResetColor);

			var mainPaintTextureBuffer = RenderTexture.GetTemporary(drawableRenderTexture.width, drawableRenderTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            Graphics.Blit(drawableRenderTexture, mainPaintTextureBuffer, singleColorMaterial);
            Graphics.Blit(mainPaintTextureBuffer, drawableRenderTexture);
            RenderTexture.ReleaseTemporary(mainPaintTextureBuffer);

            m_onReset.OnNext(Unit.Default);
        }


        public Texture2D GetTexture2D()
        {
            if (drawableRenderTexture == null)
            {
                InitializeDrawCanvas();
            }
            Texture2D tex2D = new Texture2D(drawableRenderTexture.width, drawableRenderTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = drawableRenderTexture;
            tex2D.ReadPixels(new Rect(0, 0, drawableRenderTexture.width, drawableRenderTexture.height), 0, 0);
            tex2D.Apply();
            return tex2D;
            
        }

        public void SetTexture2D(Texture2D texture)
        {
            Material material = GetComponent<Renderer>().material;
            RenderTexture rt = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            RenderTexture.active = rt;
            // Copy your texture ref to the render texture
            Graphics.Blit(texture, rt);
            drawableRenderTexture = rt;
            material.mainTexture = drawableRenderTexture;
        }

        public enum CanvasType
        {
            Whiteboard,
            Stickey,
        }
    }
}
