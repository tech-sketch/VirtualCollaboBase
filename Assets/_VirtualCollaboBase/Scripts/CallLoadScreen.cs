using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using Zenject;
using Recollab.SceneManagement;
using Recollab.Network;
using UniRx;
using System;

public class CallLoadScreen : MonoBehaviour
{
    public OVROverlay cubemapOverlay;
    public OVROverlay loadingTextQuadOverlay;
    public float distanceFromCamToLoadText;

    [Inject]
    private void Initialize( ISceneManager manager, INetworkEngineConnector networkEngineConnector)
    {

        manager.OnLobbyLoaded.Subscribe(_ => LoadingLayerOFF()).AddTo(this);
        manager.OnRoomLoadStart.Subscribe(_ => LoadingLayerON()).AddTo(this);
        networkEngineConnector.OnJoinedRoomAsObservable.Delay(TimeSpan.FromSeconds(2)).Subscribe(_ => LoadingLayerOFF()).AddTo(this);


    }

    void Start()
    {

        LoadingLayerON();

    }

    private void LoadingLayerON()
    {
        if (Camera.main != null)
        {
            Transform camTransform = Camera.main.transform;
            Transform uiTextOverlayTrasnform = loadingTextQuadOverlay.transform;
            Vector3 newPos = camTransform.position + camTransform.forward * distanceFromCamToLoadText;
            newPos.y = camTransform.position.y;
            uiTextOverlayTrasnform.position = newPos;
        }

        cubemapOverlay.enabled = true;
        loadingTextQuadOverlay.enabled = true;
    }

    private void LoadingLayerOFF()
    {
        cubemapOverlay.enabled = false;
        loadingTextQuadOverlay.enabled = false;
    }

}
