using UnityEngine;

namespace SimpleDrawing
{
    public class MouseDrawer : MonoBehaviour
    {
        [SerializeField]
        Color penColor = Color.red;

        [SerializeField]
        int penWidth = 3;

        [SerializeField]
        bool erase = false;

        Vector2 defaultTexCoord = Vector2.zero;
        Vector2 previousTexCoord;

        void Update()
        {
            bool mouseDown = Input.GetMouseButton(0);
            if (mouseDown)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
				if(Physics.Raycast(ray, out hitInfo))
                {
                    if(hitInfo.collider != null && hitInfo.collider is MeshCollider)
                    {
                        var drawObject = hitInfo.transform.GetComponent<DrawableCanvas>();
                        if (drawObject != null)
                        {
                            Vector2 currentTexCoord = hitInfo.textureCoord;
                            if (erase)
                            {
                                drawObject.Erase(currentTexCoord, previousTexCoord, penWidth);
                            }
                            else
                            {
                                drawObject.Draw(currentTexCoord, previousTexCoord, penWidth, penColor);
                            }
                            previousTexCoord = currentTexCoord;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("If you want to draw using a RaycastHit, need set MeshCollider for object.");
                    }
                }
                else
                {
                    previousTexCoord = defaultTexCoord;
                }
            }
            else if (!mouseDown) // Mouse is released
            {
                previousTexCoord = defaultTexCoord;
            }
        }
    }
}