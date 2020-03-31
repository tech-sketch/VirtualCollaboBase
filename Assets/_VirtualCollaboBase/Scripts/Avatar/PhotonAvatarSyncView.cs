using UnityEngine;
using Photon.Pun;

namespace AvatarSync
{
    [RequireComponent(typeof(PhotonView))]
    public class PhotonAvatarSyncView : MonoBehaviour, IPunObservable
    {
        private PhotonView photonView;
        private VRIKSolverTransfer ikSolverTransfer;

        private Vector3 networkHeadTargetPosition;
        private Vector3 networkLeftArmTargetPosition;
        private Vector3 networkRightArmTargetPosition;
        private Quaternion networkHeadTargetRotation;
        private Quaternion networkLeftArmTargetRotation;
        private Quaternion networkRightArmTargetRotation;

        bool firstTake = false;

        private float headPosDistance;
        private float leftArmPosDistance;
        private float rightArmPosDistance;
        private float headPosAngle;
        private float leftArmPosAngle;
        private float rightArmPosAngle;
        private Vector3 headPosDirection;
        private Vector3 leftArmPosDirection;
        private Vector3 rightArmPosDirection;
        private Vector3 storedHeadPos;
        private Vector3 storedLeftArmPos;
        private Vector3 storedRightArmPos;
        
        void OnEnable()
        {
            firstTake = true;
        }

		void Awake()
		{
            photonView = GetComponent<PhotonView>();
            ikSolverTransfer = GetComponent<VRIKSolverTransfer>();
		}

        void Update()
        {
            if (!photonView.IsMine)
            {
                if (ikSolverTransfer != null)
                {
                    ikSolverTransfer.HeadTarget.position = networkHeadTargetPosition;
                    ikSolverTransfer.HeadTarget.rotation = networkHeadTargetRotation;
                    ikSolverTransfer.LeftArmTarget.position = networkLeftArmTargetPosition;
                    ikSolverTransfer.LeftArmTarget.rotation = networkLeftArmTargetRotation;
                    ikSolverTransfer.RightArmTarget.position = networkRightArmTargetPosition;
                    ikSolverTransfer.RightArmTarget.rotation = networkRightArmTargetRotation;

                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(ikSolverTransfer.HeadTarget.position);
                stream.SendNext(ikSolverTransfer.HeadTarget.rotation);
                stream.SendNext(ikSolverTransfer.LeftArmTarget.position);
                stream.SendNext(ikSolverTransfer.LeftArmTarget.rotation);
                stream.SendNext(ikSolverTransfer.RightArmTarget.position);
                stream.SendNext(ikSolverTransfer.RightArmTarget.rotation);

                // Smoothing
                this.headPosDirection = ikSolverTransfer.HeadTarget.position - this.storedHeadPos;
                this.storedHeadPos = ikSolverTransfer.HeadTarget.position;
                stream.SendNext(this.headPosDirection);

                this.leftArmPosDirection = ikSolverTransfer.LeftArmTarget.position - this.storedLeftArmPos;
                this.storedLeftArmPos = ikSolverTransfer.LeftArmTarget.position;
                stream.SendNext(this.leftArmPosDirection);

                this.rightArmPosDirection = ikSolverTransfer.RightArmTarget.position - this.storedRightArmPos;
                this.storedRightArmPos = ikSolverTransfer.RightArmTarget.position;
                stream.SendNext(this.rightArmPosDirection);
            }
            else
            {
                networkHeadTargetPosition = (Vector3)stream.ReceiveNext();
                networkHeadTargetRotation = (Quaternion)stream.ReceiveNext();
                networkLeftArmTargetPosition = (Vector3)stream.ReceiveNext();
                networkLeftArmTargetRotation = (Quaternion)stream.ReceiveNext();
                networkRightArmTargetPosition = (Vector3)stream.ReceiveNext();
                networkRightArmTargetRotation = (Quaternion)stream.ReceiveNext();

                // Smoothing
                this.headPosDirection = (Vector3)stream.ReceiveNext();
                this.leftArmPosDirection = (Vector3)stream.ReceiveNext();
                this.rightArmPosDirection = (Vector3)stream.ReceiveNext();

                if (firstTake)
                {
                    if (ikSolverTransfer.HeadTarget != null && ikSolverTransfer.LeftArmTarget != null && ikSolverTransfer.RightArmTarget != null)
                    {
                        // Position
                        ikSolverTransfer.HeadTarget.position = this.networkHeadTargetPosition;
                        this.headPosDistance = 0f;
                        ikSolverTransfer.LeftArmTarget.position = this.networkLeftArmTargetPosition;
                        this.leftArmPosDistance = 0f;
                        ikSolverTransfer.RightArmTarget.position = this.networkRightArmTargetPosition;
                        this.rightArmPosDistance = 0f;

                        // Rotation
                        this.headPosAngle = 0f;
                        ikSolverTransfer.HeadTarget.rotation = this.networkHeadTargetRotation;
                        this.leftArmPosAngle = 0f;
                        ikSolverTransfer.LeftArmTarget.rotation = this.networkLeftArmTargetRotation;
                        this.rightArmPosAngle = 0f;
                        ikSolverTransfer.RightArmTarget.rotation = this.networkRightArmTargetRotation;

                        firstTake = false;
                    }
                }
                else
                {
                    // Position
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
                    this.networkHeadTargetPosition += this.headPosDirection * lag;
                    this.headPosDistance = Vector3.Distance(ikSolverTransfer.HeadTarget.position, this.networkHeadTargetPosition);
                    this.networkLeftArmTargetPosition += this.leftArmPosDirection * lag;
                    this.leftArmPosDistance = Vector3.Distance(ikSolverTransfer.LeftArmTarget.position, this.networkLeftArmTargetPosition);
                    this.networkRightArmTargetPosition += this.rightArmPosDirection * lag;
                    this.rightArmPosDistance = Vector3.Distance(ikSolverTransfer.RightArmTarget.position, this.networkRightArmTargetPosition);

                    // Rotation
                    this.headPosAngle = Quaternion.Angle(ikSolverTransfer.HeadTarget.rotation, this.networkHeadTargetRotation);
                    this.leftArmPosAngle = Quaternion.Angle(ikSolverTransfer.LeftArmTarget.rotation, this.networkLeftArmTargetRotation);
                    this.rightArmPosAngle = Quaternion.Angle(ikSolverTransfer.RightArmTarget.rotation, this.networkRightArmTargetRotation);
                }
            }
        }
    }
}
