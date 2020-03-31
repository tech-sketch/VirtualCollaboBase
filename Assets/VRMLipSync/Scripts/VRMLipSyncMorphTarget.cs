using UnityEngine;
using VRM;

public class VRMLipSyncMorphTarget : MonoBehaviour
{
	public VRMBlendShapeProxy blendShapeProxy;
	public OVRLipSyncContext lipsyncContext;

	// smoothing amount
	[Range(1, 100)]
	[Tooltip("Smoothing of 1 will yield only the current predicted viseme, 100 will yield an extremely smooth viseme response.")]
	public int smoothAmount = 80;
    [Range(1, 10)]
    public float gain = 10f;


    OVRLipSync.Viseme[] visemes =
	{
		OVRLipSync.Viseme.aa,
		OVRLipSync.Viseme.E,
		OVRLipSync.Viseme.ih,
		OVRLipSync.Viseme.oh,
		OVRLipSync.Viseme.ou,
	};

	void Start()
	{
		if (lipsyncContext == null)
		{
			Debug.LogError("VRMLipSyncMorphTarget.Start Error: " +
				"No OVRLipSyncContext component on this object!");
		}
		else
		{
			lipsyncContext.Smoothing = smoothAmount;
		}
	}

	void Update()
	{
		// get the current viseme frame
		OVRLipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
		if (frame != null)
		{
			SetVisemeToMorphTarget(frame);
		}

         //Update smoothing value
         //if (smoothAmount != lipsyncContext.Smoothing)
         //{
         //	lipsyncContext.Smoothing = smoothAmount;
         //}
    }

    void SetVisemeToMorphTarget(OVRLipSync.Frame frame)
    {
        foreach (OVRLipSync.Viseme viseme in visemes)
        {
			int index = (int)viseme;
			switch (viseme)
			{
				case OVRLipSync.Viseme.aa:
					blendShapeProxy.SetValue(BlendShapePreset.A, Mathf.Clamp01(frame.Visemes[index] * gain));
					break;
				case OVRLipSync.Viseme.E:
					blendShapeProxy.SetValue(BlendShapePreset.E, Mathf.Clamp01(frame.Visemes[index] * gain));
					break;
				case OVRLipSync.Viseme.ih:
					blendShapeProxy.SetValue(BlendShapePreset.I, Mathf.Clamp01(frame.Visemes[index] * gain));
					break;
				case OVRLipSync.Viseme.oh:
					blendShapeProxy.SetValue(BlendShapePreset.O, Mathf.Clamp01(frame.Visemes[index] * gain));
					break;
				case OVRLipSync.Viseme.ou:
					blendShapeProxy.SetValue(BlendShapePreset.U, Mathf.Clamp01(frame.Visemes[index] * gain));
					break;
				default:
					blendShapeProxy.SetValue(BlendShapePreset.A, 0);
					blendShapeProxy.SetValue(BlendShapePreset.I, 0);
					blendShapeProxy.SetValue(BlendShapePreset.U, 0);
					blendShapeProxy.SetValue(BlendShapePreset.E, 0);
					blendShapeProxy.SetValue(BlendShapePreset.O, 0);
					break;
			}
        }
    }
}
