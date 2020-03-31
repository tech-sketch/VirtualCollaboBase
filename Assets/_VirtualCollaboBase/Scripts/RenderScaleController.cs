using UnityEngine;

public class RenderScaleController : MonoBehaviour
{
    public float renderScale = 1.25f;    // デフォルトは1.0 上げれば上げるほどVR内の解像度が高くなる(その分描画は重くなる)

    // 起動時に一回だけ設定(動的に変えると描画が壊れる時がある)
    void Start()
    {
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = renderScale;
    }
}