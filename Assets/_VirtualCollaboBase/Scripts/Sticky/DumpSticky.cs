using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DumpSticky : MonoBehaviour
{
    void Start()
    {
        ClickTrashBox.OnClickTrashBox.Subscribe(_ => DumpStickySync()).AddTo(this);
    }

    public void DumpStickySync()
    {
        Debug.Log("##############On Use################");
        if (HoldingStickyState.isHolding.Value)
        {
            PhotonNetwork.Destroy(HoldingStickyState.targetSticky);
            HoldingStickyState.isHolding.Value = false;
        }
    }
}
