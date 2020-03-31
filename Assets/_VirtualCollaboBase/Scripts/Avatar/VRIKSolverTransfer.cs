using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MecanimIKPlus;

public enum IKSoverReference
{
    None,
    AsSource,
    AsDestination,
}

public class VRIKSolverTransfer : MonoBehaviour
{
    public IKSoverReference IkSolverRef = IKSoverReference.AsSource;
    public VRIKSolverTransfer Source;

    public Transform HeadTarget;
    public Transform LeftArmTarget;
    public Transform RightArmTarget;

    void Awake()
    {
        IK_Head_Linkage_CS ik_Head = this.GetComponentInChildren<IK_Head_Linkage_CS>();
        IK_CS ik_arm = this.GetComponentInChildren<IK_CS>();
        if (ik_Head == null)
        {
            IkSolverRef = IKSoverReference.None;
        }
        else
        {
            if (IkSolverRef == IKSoverReference.AsSource)
            {
                HeadTarget = ik_Head.eyeTransform;
                LeftArmTarget = ik_arm.leftHandTarget;
                RightArmTarget = ik_arm.rightHandTarget;
            }
            else if (IkSolverRef == IKSoverReference.AsDestination)
            {
                ik_Head.eyeTransform = HeadTarget;
                ik_arm.leftHandTarget = LeftArmTarget;
                ik_arm.rightHandTarget = RightArmTarget;
            }
        }
    }

    void Update()
    {
        if (Source != null)
        {
            HeadTarget.position = Source.HeadTarget.position;
            HeadTarget.rotation = Source.HeadTarget.rotation;
            LeftArmTarget.position = Source.LeftArmTarget.position;
            LeftArmTarget.rotation = Source.LeftArmTarget.rotation;
            RightArmTarget.position = Source.RightArmTarget.position;
            RightArmTarget.rotation = Source.RightArmTarget.rotation;
        }
    }
}
