using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace MecanimIKPlus
{
    [RequireComponent(typeof(IK_Head_Linkage_CS))]
	public class IK_CS : MonoBehaviour
	{
		public Transform lookAtTarget;
		public Transform bodyTarget;
		public Transform leftHandTarget;
		public Transform rightHandTarget;
		public Transform leftFootTarget;
		public Transform rightFootTarget;

		public float lookAtWeight = 1.0f;
		public float leftHandPosWeight = 1.0f;
		public float leftHandRotWeight = 0.5f;
		public float rightHandPosWeight = 1.0f;
		public float rightHandRotWeight = 0.5f;
		public float leftFootPosWeight = 1.0f;
		public float leftFootRotWeight = 1.0f;
		public float rightFootPosWeight = 1.0f;
		public float rightFootRotWeight = 1.0f;

		Animator animator;

        private void Reset()
        {
            lookAtTarget = transform.root.Find("VRIKHeadTarget").GetChild(0);
            bodyTarget = transform.root.Find("IKBodyTarget");
            leftHandTarget = transform.root.Find("VRIKLeftArmTarget");
            rightHandTarget = transform.root.Find("VRIKRightArmTarget");
        }


        void Start ()
		{
			animator = GetComponent <Animator> ();
		}

		void OnAnimatorIK ()
		{	
			animator.SetLookAtWeight (lookAtWeight, 0.0f, 1.0f, 1.0f, 0.0f);
			animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, leftHandPosWeight);
			animator.SetIKRotationWeight (AvatarIKGoal.LeftHand, leftHandRotWeight);
			animator.SetIKPositionWeight (AvatarIKGoal.RightHand, rightHandPosWeight);
			animator.SetIKRotationWeight (AvatarIKGoal.RightHand, rightHandRotWeight);
			animator.SetIKPositionWeight (AvatarIKGoal.LeftFoot, leftFootPosWeight);
			animator.SetIKRotationWeight (AvatarIKGoal.LeftFoot, leftFootRotWeight);
			animator.SetIKPositionWeight (AvatarIKGoal.RightFoot, rightFootPosWeight);
			animator.SetIKRotationWeight (AvatarIKGoal.RightFoot, rightFootRotWeight);
			if (lookAtTarget != null) {
				animator.SetLookAtPosition (lookAtTarget.position);
			}				
			if (bodyTarget != null) {
				animator.bodyPosition = bodyTarget.position;
				animator.bodyRotation = bodyTarget.rotation;
			}
			if (leftHandTarget != null) {
				animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHandTarget.position);
				animator.SetIKRotation (AvatarIKGoal.LeftHand, leftHandTarget.rotation);
			}				
			if (rightHandTarget != null) {
				animator.SetIKPosition (AvatarIKGoal.RightHand, rightHandTarget.position);
				animator.SetIKRotation (AvatarIKGoal.RightHand, rightHandTarget.rotation);
			}				
			if (leftFootTarget != null) {
				animator.SetIKPosition (AvatarIKGoal.LeftFoot, leftFootTarget.position);
				animator.SetIKRotation (AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
			}				
			if (rightFootTarget != null) {
				animator.SetIKPosition (AvatarIKGoal.RightFoot, rightFootTarget.position);
				animator.SetIKRotation (AvatarIKGoal.RightFoot, rightFootTarget.rotation);
			}				
		}

	}

}
