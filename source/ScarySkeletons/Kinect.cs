using System;
using System.Threading;
using xn;

namespace ScarySkeletons
{
    public class Kinect
    {
        static readonly object RunLock = new object();

        bool shouldRun;
        Context context;
        DepthGenerator depth;
        UserGenerator userGenerator;
        SkeletonCapability skeletonCapbility;
        PoseDetectionCapability poseDetectionCapability;
        string calibPose;
        Thread readerThread;

        public Kinect()
        {
            this.Initialize();
        }

        public bool ShouldRun
        {
            get
            {
                lock (RunLock)
                {
                    return shouldRun;
                }
            }
            set
            {
                lock (RunLock)
                {
                    shouldRun = value;
                }
            }
        }

        void Initialize()
        {
            this.context = new Context("SamplesConfig.xml");
            this.depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            if (this.depth == null)
            {
                throw new Exception("Viewer must have a depth node!");
            }

            this.userGenerator = new UserGenerator(this.context);
            this.skeletonCapbility = new SkeletonCapability(this.userGenerator);
            this.poseDetectionCapability = new PoseDetectionCapability(this.userGenerator);
            this.calibPose = this.skeletonCapbility.GetCalibrationPose();

            this.userGenerator.NewUser += new UserGenerator.NewUserHandler(userGenerator_NewUser);
            this.userGenerator.LostUser += new UserGenerator.LostUserHandler(userGenerator_LostUser);
            this.poseDetectionCapability.PoseDetected += new PoseDetectionCapability.PoseDetectedHandler(poseDetectionCapability_PoseDetected);
            this.skeletonCapbility.CalibrationEnd += new SkeletonCapability.CalibrationEndHandler(skeletonCapbility_CalibrationEnd);

            this.skeletonCapbility.SetSkeletonProfile(SkeletonProfile.All);
            this.userGenerator.StartGenerating();

            App.ViewModel.Status = "Waiting to acquire user";

            this.ShouldRun = true;
            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();
        }

        void userGenerator_NewUser(ProductionNode node, uint id)
        {
            App.ViewModel.Status = "Waiting for pose...";
            App.ViewModel.User.UserId = id;
            this.poseDetectionCapability.StartPoseDetection(this.calibPose, id);
        }

        void userGenerator_LostUser(ProductionNode node, uint id)
        {
            App.ViewModel.Status = "Lost user.";
            App.ViewModel.Tracking = false;
            this.ResetPoints();
        }

        void skeletonCapbility_CalibrationEnd(ProductionNode node, uint id, bool success)
        {
            if (success)
            {
                App.ViewModel.Status = "Tracking user";
                this.skeletonCapbility.StartTracking(id);
                App.ViewModel.Tracking = true;
            }
            else
            {
                App.ViewModel.Status = "Waiting for pose...";
                this.poseDetectionCapability.StartPoseDetection(calibPose, id);
            }
        }

        void poseDetectionCapability_PoseDetected(ProductionNode node, string pose, uint id)
        {
            App.ViewModel.Status = "Pose detected";
            this.poseDetectionCapability.StopPoseDetection(id);
            this.skeletonCapbility.RequestCalibration(id, true);
        }

        void ResetPoints()
        {
            App.ViewModel.User.Head = BindablePoint.Zero;
            App.ViewModel.User.Neck = BindablePoint.Zero;
            App.ViewModel.User.LeftElbow = BindablePoint.Zero;
            App.ViewModel.User.RightElbow = BindablePoint.Zero;
            App.ViewModel.User.LeftHand = BindablePoint.Zero;
            App.ViewModel.User.RightHand = BindablePoint.Zero;
            App.ViewModel.User.RightShoulder = BindablePoint.Zero;
            App.ViewModel.User.LeftShoulder = BindablePoint.Zero;
            App.ViewModel.User.Torso = BindablePoint.Zero;
            App.ViewModel.User.RightHip = BindablePoint.Zero;
            App.ViewModel.User.LeftHip = BindablePoint.Zero;
            App.ViewModel.User.RightKnee = BindablePoint.Zero;
            App.ViewModel.User.LeftKnee = BindablePoint.Zero;

        }

        void ReaderThread()
        {
            while (this.ShouldRun)
            {
                this.context.WaitOneUpdateAll(this.depth);

                var users = this.userGenerator.GetUsers();

                if (users.Length > 0)
                {
                    var firstUser = users[0];
                    if (this.skeletonCapbility.IsTracking(firstUser))
                    {
                        App.ViewModel.User.Head = GetJointPosition(firstUser, SkeletonJoint.Head).position.ToBindablePoint();
                        App.ViewModel.User.Neck = GetJointPosition(firstUser, SkeletonJoint.Neck).position.ToBindablePoint();
                        App.ViewModel.User.LeftElbow = GetJointPosition(firstUser, SkeletonJoint.LeftElbow).position.ToBindablePoint();
                        App.ViewModel.User.RightElbow = GetJointPosition(firstUser, SkeletonJoint.RightElbow).position.ToBindablePoint();
                        App.ViewModel.User.LeftHand = GetJointPosition(firstUser, SkeletonJoint.LeftHand).position.ToBindablePoint();
                        App.ViewModel.User.RightHand = GetJointPosition(firstUser, SkeletonJoint.RightHand).position.ToBindablePoint();
                        App.ViewModel.User.RightShoulder = GetJointPosition(firstUser, SkeletonJoint.RightShoulder).position.ToBindablePoint();
                        App.ViewModel.User.LeftShoulder = GetJointPosition(firstUser, SkeletonJoint.LeftShoulder).position.ToBindablePoint();
                        App.ViewModel.User.Torso = GetJointPosition(firstUser, SkeletonJoint.Torso).position.ToBindablePoint();
                        App.ViewModel.User.RightHip = GetJointPosition(firstUser, SkeletonJoint.RightHip).position.ToBindablePoint();
                        App.ViewModel.User.LeftHip = GetJointPosition(firstUser, SkeletonJoint.LeftHip).position.ToBindablePoint();
                        App.ViewModel.User.RightKnee = GetJointPosition(firstUser, SkeletonJoint.RightKnee).position.ToBindablePoint();
                        App.ViewModel.User.LeftKnee = GetJointPosition(firstUser, SkeletonJoint.LeftKnee).position.ToBindablePoint();

                    }
                }
            }
        }

        SkeletonJointPosition GetJointPosition(uint user, SkeletonJoint joint)
        {
            SkeletonJointPosition pos = new SkeletonJointPosition();
            this.skeletonCapbility.GetSkeletonJointPosition(user, joint, ref pos);
            if (pos.position.Z == 0)
            {
                pos.fConfidence = 0;
            }
            else
            {
                pos.position = this.depth.ConvertRealWorldToProjective(pos.position);
            }
            return pos;
        }
    }
}

