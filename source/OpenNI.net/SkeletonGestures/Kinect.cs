using System;
using System.Threading;
using xn;
using System.Diagnostics;

namespace SkeletonGestures
{
    public class Kinect
    {
        static readonly object RunLock = new object();

        bool shouldRun;
        string calibPose;

        Context context;
        DepthGenerator depth;
        UserGenerator userGenerator;
        SkeletonCapability skeletonCapbility;
        PoseDetectionCapability poseDetectionCapability;
        Thread readerThread;

        Swipe leftSwipe;
        Swipe rightSwipe;
        Push push;

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

            this.leftSwipe = new Swipe();
            this.leftSwipe.Initialize();
            this.leftSwipe.SwipeCaptured += new EventHandler<SwipeEventArgs>(leftSwipe_SwipeCaptured);

            this.rightSwipe = new Swipe();
            this.rightSwipe.Initialize();
            this.rightSwipe.SwipeCaptured += new EventHandler<SwipeEventArgs>(rightSwipe_SwipeCaptured);

            this.push = new Push();
            this.push.Initialize();
            this.push.PushCaptured += new EventHandler<EventArgs>(push_PushCaptured);

            this.ShouldRun = true;
            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();
        }

        void push_PushCaptured(object sender, EventArgs e)
        {
            this.OnPushDetected();
        }

        void rightSwipe_SwipeCaptured(object sender, SwipeEventArgs e)
        {
            this.OnSwipeDetected(e.Direction, Hand.Right);
        }

        void leftSwipe_SwipeCaptured(object sender, SwipeEventArgs e)
        {
            this.OnSwipeDetected(e.Direction, Hand.Left);
        }

        void OnSwipeDetected(SwipeDirection direction, Hand hand)
        {
            App.ViewModel.SwipeInfo = "Swiped " + hand.ToString() + " hand, to the " + direction.ToString();
        }

        void OnPushDetected()
        {
            App.ViewModel.SwipeInfo = "Pushed!!!";
        }

        void userGenerator_NewUser(ProductionNode node, uint id)
        {
            App.ViewModel.Status = "User identified. Waiting for pose...";
            this.poseDetectionCapability.StartPoseDetection(this.calibPose, id);
        }

        void userGenerator_LostUser(ProductionNode node, uint id)
        {
            App.ViewModel.Status = "Lost user.";
            App.ViewModel.Tracking = false;
        }

        void skeletonCapbility_CalibrationEnd(ProductionNode node, uint id, bool success)
        {
            if (success)
            {
                App.ViewModel.Status = "Tracking user";
                this.skeletonCapbility.StartTracking(id);
                App.ViewModel.Tracking = true;
                this.leftSwipe.Initialize();
                this.rightSwipe.Initialize();
            }
            else
            {
                App.ViewModel.Status = "Calibration failed...  Waiting for another pose...";
                this.poseDetectionCapability.StartPoseDetection(calibPose, id);
            }
        }

        void poseDetectionCapability_PoseDetected(ProductionNode node, string pose, uint id)
        {
            App.ViewModel.Status = "Pose detected";
            this.poseDetectionCapability.StopPoseDetection(id);
            this.skeletonCapbility.RequestCalibration(id, true);
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
                        var rightHand = this.GetJointPosition(firstUser, SkeletonJoint.RightHand);
                        var rightPoint =                             
                        new TimePoint()
                        {
                            X = rightHand.position.X,
                            Y = rightHand.position.Y,
                            Z = rightHand.position.Z,
                            Time = DateTime.Now
                        };

                        this.rightSwipe.AddSwipePoint(rightPoint);
                        this.push.AddPushPoint(rightPoint);

                        var leftHand = this.GetJointPosition(firstUser, SkeletonJoint.LeftHand);

                        this.leftSwipe.AddSwipePoint(
                            new TimePoint()
                            {
                                X = leftHand.position.X,
                                Y = leftHand.position.Y,
                                Z = leftHand.position.Z,
                                Time = DateTime.Now
                            });
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

