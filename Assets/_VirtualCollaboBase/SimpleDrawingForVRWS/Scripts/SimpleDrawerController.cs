using UnityEngine;
using VRTK;
using UniRx;
using SimpleDrawing;
using SimpleDrawing.Sharing;
using Zenject;

namespace VRWS.SimpleDrawing.Sharing
{
    public class SimpleDrawerController : MonoBehaviour
    {
        [Inject(Id = ControllerHand.RIGHT)] public VRTK_ControllerEvents VRController = null;
        
        // ColorPickerTriangle colorPicker;
        RemoteRayCastDrawer drawer;

		public void Initialize()
		{
			this.VRController.TriggerPressed += DoTriggerPressed;
			this.VRController.TriggerReleased += DoTriggerReleased;
            this.drawer = transform.GetComponentInChildren<RemoteRayCastDrawer>();
            this.drawer.RayCastEnabled = true;
            OnDrawTargetChange(WhiteBoard.DefaultTargetWhiteboard);
        }

        void Start()
        {
            // colorPicker = Camera.main.GetComponentInChildren<ColorPickerTriangle>();

            // LocalSync
            HandWritingStickeyController.OnHandWritingStickeyClick.Subscribe(target => OnDrawTargetChange(target)).AddTo(this);
            WhiteBoard.OnClickWhiteBoard.Subscribe(target => OnDrawTargetChange(target)).AddTo(this);
            WhiteboardClearButtonEvent.OnWhiteboardClearButtonPressed.Subscribe(_ => OnWhiteBoardClear()).AddTo(this);
            OnCreateLocalSync();
        }   

        void Update()
        {
            // drawer.PenColor = colorPicker.TheColor;
        }

        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (drawer != null)
            {
                drawer.RayCastEnabled = true;
                //drawer.Erase = true;
            }
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (drawer != null)
            {

                //drawer.Erase = false;

            }
        }





        private void OnDrawTargetChange(GameObject target)
        {
         
            if (drawer != null)
            {
                if (drawer.TargetDrawableCanvas != null)
                {
                    string name = drawer.TargetDrawableCanvas.gameObject.name;
                    if (name == "StickeyHandWritingCanvas")
                    {
                        if (drawer.TargetDrawableCanvas.gameObject.transform.parent.gameObject.activeInHierarchy)
                        {
                            return;
                        }

                    }

                }

                    drawer.TargetDrawableCanvas = target.transform.GetComponent<DrawableCanvas>();
                
            }
        }

        private void OnWhiteBoardClear()
        {
            if (drawer != null)
            {
                drawer.ClearWhiteboard();
            }
        }

        private void OnCreateLocalSync()
        {
            if (drawer != null)
            {
                drawer.LocalSyncDraw = true;
                drawer.TargetDrawableCanvas = WhiteBoard.DefaultTargetWhiteboard.transform.GetComponent<DrawableCanvas>();
            }
        }

        private void OnDeleteLocalSync()
        {
            if (drawer != null)
            {
                drawer.LocalSyncDraw = false;
                drawer.TargetDrawableCanvas = WhiteBoard.DefaultTargetWhiteboard.transform.GetComponent<DrawableCanvas>();
            }
        }
    }
}
