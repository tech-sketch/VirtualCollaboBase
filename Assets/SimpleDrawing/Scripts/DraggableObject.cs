using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    void OnMouseDrag()
    {
        Vector3 objectPointInScreen = Camera.main.WorldToScreenPoint(this.transform.position);

        Vector3 mousePointInScreen = new Vector3(Input.mousePosition.x,
                                                Input.mousePosition.y,
                                                objectPointInScreen.z);

        Vector3 mousePointInWorld = Camera.main.ScreenToWorldPoint(mousePointInScreen);

        this.transform.position = mousePointInWorld;
    }
}
