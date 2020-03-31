using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public interface IAvatarSettingPresenter
{
    List<GameObject> UseAvatarList { get; }
}

public class AvatarSettingPresenter : MonoBehaviour, IAvatarSettingPresenter
{

    [SerializeField] private List<GameObject> useAvatarList;

    public List<GameObject> UseAvatarList { get { return useAvatarList; } }


}
