using System.Collections;
using System.Collections.Generic;
using TextureSharing;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;
using Zenject;
using Recollab.Network;

public class TextureSharing4Sticky : MonoBehaviour
{
    TextureSharingComponent textureSharingComponent;
    TextureBroadcastComponent textureBroadcastComponent;
    RawImage image;
    Sticky sticky;

	[Inject]
    private void Initialize(INetworkEngineConnector networkEngineConnector)
    {
        sticky = gameObject.GetComponent<Sticky>();
        image = sticky.displayImage;
        textureSharingComponent = gameObject.GetComponent<TextureSharingComponent>();
        textureBroadcastComponent = gameObject.GetComponent<TextureBroadcastComponent>();

		textureSharingComponent.OnReceivedRawTexture
			.Subscribe( tex2d => SetTexture( tex2d ) )
			.AddTo( this );

		textureBroadcastComponent.OnReceivedRawTexture
			.Subscribe( tex2d => SetTexture( tex2d ) )
			.AddTo( this );

        networkEngineConnector.OnJoinedRoomAsObservable
            .Where( _ => sticky.displayText.text.Equals( string.Empty ) )
			.Subscribe( _ => StartTextureSharing() )
			.AddTo( this );
    }

    private void StartTextureSharing()
    {
        Texture2D texture2D = image.texture as Texture2D;
        if (texture2D == null)
        {
            texture2D = new Texture2D(SystemDefines.STICKY_TEXTURE_WIDTH, SystemDefines.STICKY_TEXTURE_HEIGHT, TextureFormat.RGBA32, false);
        }
        textureSharingComponent.GetRawTextureDataFromMasterClient(texture2D);
    }

    public void StartTextureBroadcast(Texture2D texture2D)
    {
        Debug.Log("###########     StartTextureBroadcast    ###########");
        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ => textureBroadcastComponent.BroadcastTexture(texture2D));

    }

    public void SetTexture4TextureSharing(Texture2D texture2D)
    {
        textureSharingComponent = gameObject.GetComponent<TextureSharingComponent>();
        textureSharingComponent.GetRawTextureDataFromMasterClient(texture2D);

    }

    private void SetTexture(Texture2D texture2D)
    {
        image.texture = texture2D;
        image.color = new Color(1, 1, 1, 1);
    }
}
