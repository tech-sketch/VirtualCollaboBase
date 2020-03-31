using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MoveSticky : MonoBehaviour
{

    
    [SerializeField] private Transform StickyAnchor;


    // Use this for initialization
    void Start()
    {
        Sticky.OnClickSticky.Subscribe(target => ClickSticky(target)).AddTo(this);
        WhiteBoard.OnClickWhiteBoard.Subscribe(target => ClickWhiteBoard(target)).AddTo(this);


    }


    public void ClickSticky(GameObject target)
    {

        if (!HoldingStickyState.isHolding.Value)
        {
            HoldingStickyState.targetSticky = target;
            PhotonView view = target.GetComponent<PhotonView>();
            if (!view.IsMine)
            {
                view.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            }
            target.transform.parent = StickyAnchor;
            Sticky sticky = target.GetComponent<Sticky>();
            sticky.RemoveParent();
            target.transform.SetPositionAndRotation(StickyAnchor.transform.position, Quaternion.identity);


            

            HoldingStickyState.isHolding.Value = true;

        }

    }

    public void ClickWhiteBoard(GameObject target)
    {
        if (HoldingStickyState.isHolding.Value)
        {
            Sticky sticky = HoldingStickyState.targetSticky.GetComponent<Sticky>();
            sticky.SetParent(target.transform.parent.transform.parent.gameObject);
            HoldingStickyState.targetSticky.transform.SetPositionAndRotation(DestinationMarker.DestinationMarkerPos, target.transform.rotation);
            HoldingStickyState.targetSticky.transform.position = HoldingStickyState.targetSticky.transform.TransformPoint(new Vector3(0f,0f,-0.2f));
            HoldingStickyState.targetSticky = null;
            HoldingStickyState.isHolding.Value = false;
        }

    }


}
