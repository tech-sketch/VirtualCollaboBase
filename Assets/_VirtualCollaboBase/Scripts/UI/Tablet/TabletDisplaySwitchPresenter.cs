using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VRTK;
using Zenject;
using Recollab.SceneManagement;
using UnityEngine.UI;

public class TabletDisplaySwitchPresenter : MonoBehaviour
{
	[Inject(Id = ControllerHand.RIGHT)] private VRTK_ControllerEvents VRController = null;
	[SerializeField] private GameObject TabletRoot;
    [SerializeField] private GameObject TabletforRoom;
    [SerializeField] private GameObject ObjectAnchor;


    private static Subject<Unit> m_OnTabletActive = new Subject<Unit>();
    public static IObservable<Unit> OnTabletActive { get { return m_OnTabletActive; } }

    private static Subject<Unit> m_OnTabletInActive = new Subject<Unit>();
    public static IObservable<Unit> OnTabletInActive { get { return m_OnTabletInActive; } }

    private bool isRoom = false;

	[Inject]
	private void Initialize( ISceneManager scene_manager )
	{
		scene_manager.OnRoomLoaded
			.Subscribe( _ =>
			{
				isRoom = true;
				SwitchTablet();
			} )
			.AddTo( this );

		scene_manager.OnLobbyLoaded
			.Subscribe( _ =>
			{
				isRoom = false;
				SwitchTablet();
			} )
			.AddTo( this );

		this.VRController.ButtonTwoPressed += DoButtonTwoPressed;

    }

	private void Update()
	{
		if( Input.GetKeyDown( KeyCode.B ) )
			DoButtonTwoPressed( null, new ControllerInteractionEventArgs() );
	}

	private void SwitchTablet()
    {
        TabletRoot.SetActive(false);
        TabletforRoom.SetActive(true);
    }

    private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (isRoom)
        {
            TabletRoot.SetActive(!TabletRoot.activeInHierarchy);

            if (TabletRoot.activeInHierarchy)
            {

                Debug.Log(Camera.main.transform.rotation.eulerAngles);
                Vector3 rot = Camera.main.transform.rotation.eulerAngles;
                TabletRoot.transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
                ObjectAnchor.SetActive(true);
                m_OnTabletActive.OnNext(Unit.Default);

            }

            else
            {
                ObjectAnchor.SetActive(false);
                m_OnTabletInActive.OnNext(Unit.Default);
            }
        }
    }

}
