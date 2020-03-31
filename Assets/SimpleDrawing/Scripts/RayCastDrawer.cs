using UnityEngine;

namespace SimpleDrawing
{
    [RequireComponent(typeof(MeshRenderer))]
    public class RayCastDrawer : MonoBehaviour
    {   
        private enum RayDirectionType
        {
            TransformForward,
            TransformBackward,
            TransformRight,
            TransformLeft,
            TransformUp,
            TransformDown,
        }

        public bool RayCastEnabled = true;
        public float RayDistance = 5.0f;

        [SerializeField]
        RayDirectionType directionType = RayDirectionType.TransformDown;

        [SerializeField]
        Color penColor = Color.cyan;

        [SerializeField]
        int penWidth = 3;

        [SerializeField]
        bool erase = false;

        Vector2 defaultTexCoord = Vector2.zero;

        Vector2 currentTexCoord;
        Vector2 previousTexCoord;

		void Start()
		{
			GetComponent<MeshRenderer>().material.color = penColor;
		}

        void Update()
        {
            if (RayCastEnabled)
            {
                var ray = new Ray(this.transform.position, GetCurrentDirection());
                RaycastHit hitInfo;
				if(Physics.Raycast(ray, out hitInfo, RayDistance))
                {
                    if(hitInfo.collider != null && hitInfo.collider is MeshCollider)
                    {
                        var drawObject = hitInfo.transform.GetComponent<DrawableCanvas>();
                        if (drawObject != null)
                        {
                            previousTexCoord = currentTexCoord;
                            currentTexCoord = hitInfo.textureCoord;
                            if (erase)
                            {
                                drawObject.Erase(currentTexCoord, previousTexCoord, penWidth);
                            }
                            else
                            {
                                drawObject.Draw(currentTexCoord, previousTexCoord, penWidth, penColor);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("If you want to draw using a RaycastHit, need set MeshCollider for object.");
                    }
                }
                else
                {
                    currentTexCoord = defaultTexCoord;
                }
            }
            else if (!RayCastEnabled)
            {
                currentTexCoord = defaultTexCoord;
            }
        }

        private Vector3 GetCurrentDirection()
        {
            Vector3 direction = Vector3.zero;
            switch(directionType)
            {
                case RayDirectionType.TransformForward:
                    direction =  this.transform.forward;
                    break;
                case RayDirectionType.TransformBackward:
                    direction = -this.transform.forward;
                    break;
                case RayDirectionType.TransformRight:
                    direction =  this.transform.right;
                    break;
                case RayDirectionType.TransformLeft:
                    direction = -this.transform.right;
                    break;
                case RayDirectionType.TransformUp:
                    direction =  this.transform.up;
                    break;
                case RayDirectionType.TransformDown:
                    direction = -this.transform.up;
                    break;
            }
            return direction;
        }
    }
}