using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DestinationMarker : MonoBehaviour {

    public static Vector3 DestinationMarkerPos = Vector3.zero;
    private VRTK_UIPointer VRTK_UIPointer;
    // Use this for initialization
    void Start () {
        GetComponent<VRTK_DestinationMarker>().DestinationMarkerHover += new DestinationMarkerEventHandler(DoPointerHover);
        VRTK_UIPointer = GetComponent<VRTK_UIPointer>();
    }

    // Update is called once per frame
    private void DoPointerHover(object sender, DestinationMarkerEventArgs e)
    {
        
        DestinationMarkerPos = e.destinationPosition;
        VRTK_UIPointer.maximumLength = Vector3.Distance(gameObject.transform.position,DestinationMarkerPos) + 0.1f;

    }
}
