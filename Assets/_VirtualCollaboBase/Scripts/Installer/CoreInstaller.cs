using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Recollab.Network;
using Recollab.SceneManagement;
using VRTK;

public class CoreInstaller : MonoInstaller<CoreInstaller>
{
    [SerializeField] private AvatarSettingPresenter AvatarSettingPresenter = null;
    [SerializeField] private PhotonConnector NetworkConnector = null;
	[SerializeField] private SceneController SceneManager = null;

    [SerializeField] private VRTK_ControllerEvents RightControllerEvents = null;
	[SerializeField] private VRTK_ControllerEvents LeftControllerEvents = null;
	[SerializeField] private PlayerLocator PlayerLocator = null;
	[SerializeField] private PlayerAvatarPresenter PlayerAvatarPresenter = null;

    public override void InstallBindings()
	{
        Container.Bind<IAvatarSettingPresenter>().FromInstance(AvatarSettingPresenter);
        Container.Bind<INetworkEngineConnector>().FromInstance( NetworkConnector );
		Container.Bind<ISceneManager>().FromInstance( SceneManager );
        Container.Bind<VRTK_ControllerEvents>().WithId( ControllerHand.RIGHT ).FromInstance( RightControllerEvents );
		Container.Bind<VRTK_ControllerEvents>().WithId( ControllerHand.LEFT ).FromInstance( LeftControllerEvents );
		Container.Bind<IPlayerLocator>().FromInstance( PlayerLocator );
		Container.Bind<IPlayerAvatarPresenter>().FromInstance( PlayerAvatarPresenter );
    }
}

public enum ControllerHand
{
	RIGHT,
	LEFT
}
