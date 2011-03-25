using System;
using System.Collections.Generic;
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

        Dictionary<uint, Dictionary<SkeletonJoint, SkeletonJointPosition>> joints;

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
            this.joints = new Dictionary<uint, Dictionary<SkeletonJoint, SkeletonJointPosition>>();
            this.userGenerator.StartGenerating();

            MapOutputMode mapMode = this.depth.GetMapOutputMode();

            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();
        }

        void userGenerator_NewUser(ProductionNode node, uint id)
        {
            this.poseDetectionCapability.StartPoseDetection(this.calibPose, id);
        }

        void userGenerator_LostUser(ProductionNode node, uint id)
        {
            this.joints.Remove(id);
        }

        void skeletonCapbility_CalibrationEnd(ProductionNode node, uint id, bool success)
        {
            if (success)
            {
                this.skeletonCapbility.StartTracking(id);
                this.joints.Add(id, new Dictionary<SkeletonJoint, SkeletonJointPosition>());
            }
            else
            {
                this.poseDetectionCapability.StartPoseDetection(calibPose, id);
            }
        }

        void poseDetectionCapability_PoseDetected(ProductionNode node, string pose, uint id)
        {
            this.poseDetectionCapability.StopPoseDetection(id);
            this.skeletonCapbility.RequestCalibration(id, true);
        }

        private unsafe void ReaderThread()
        {
            DepthMetaData depthMD = new DepthMetaData();

            while (this.ShouldRun)
            {
                this.context.WaitOneUpdateAll(this.depth);

                uint[] users = this.userGenerator.GetUsers();
                foreach (uint user in users)
                {
                    var userJoints = this.joints[user];
                    

                }
            }

        }
    }

}

