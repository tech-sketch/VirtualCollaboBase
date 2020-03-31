using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DropdownStickyTypeChangerPresenter : MonoBehaviour
{

    [SerializeField] private Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        dropdown.OnValueChangedAsObservable().Subscribe(index => StickyTypeChanger.OnStickyTypeChange.OnNext(index)).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
