using System.Collections;
using System.Collections.Generic;
using TextureSharing;
using UnityEngine;
using UniRx;
using SimpleDrawing;
using Zenject;
using Recollab.Network;
using System;

public class TextureSharing4Whiteboard : MonoBehaviour
{

    private TextureSharingComponent textureSharingComponent = null;
    private DrawableCanvas drawableCanvas = null;
	private INetworkEngineConnector m_NetworkEngineConnector = null;

	[Inject]
    private void Initialize(INetworkEngineConnector networkEngineConnector)
    {
        m_NetworkEngineConnector = networkEngineConnector;
    }

	private void Start()
	{
		drawableCanvas = gameObject.GetComponent<DrawableCanvas>();
		textureSharingComponent = gameObject.GetComponent<TextureSharingComponent>();
		textureSharingComponent.OnReceivedRawTexture.Subscribe( tex2d => SetTexture( tex2d ) );
	}

	public void StartTextureSharing()
    {

        textureSharingComponent.SetDrawableCanvas(drawableCanvas);
        textureSharingComponent.GetRawTextureDataFromMasterClient(drawableCanvas.GetTexture2D());
        
    }

    private void SetTexture(Texture2D texture2D)
    {
        Debug.Log(" SetTexture " + gameObject.transform.parent.transform.parent.gameObject.name);
        drawableCanvas.SetTexture2D(texture2D);
    }
}
