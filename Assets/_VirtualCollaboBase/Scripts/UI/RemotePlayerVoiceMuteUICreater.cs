using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerVoiceMuteUICreater : MonoBehaviour
{
    [SerializeField] private GameObject MutePlayerUI;
    [SerializeField] private GameObject MutePlayerUIHolder;
   


    public void RefreshMutePlayerPanel()
    {
        var playerInfo = SharingRoot.m_sharingRoot.GetComponentsInChildren<PlayerInfo>(false);

        foreach (Transform ui in MutePlayerUIHolder.transform)
        {
            Destroy(ui.gameObject);
        }

        foreach (PlayerInfo info in playerInfo)
        {
            if (info.VoiceMute != null)
            {
                GameObject ui = Instantiate(MutePlayerUI);
                MutePlayerUIPresenter mutePlayerUIPresenter = ui.GetComponent<MutePlayerUIPresenter>();
                mutePlayerUIPresenter.SetData(info);
                ui.transform.parent = MutePlayerUIHolder.transform;
                ui.transform.localPosition = Vector3.zero;
                ui.transform.localRotation = Quaternion.identity;
            }

        }

    }


}
