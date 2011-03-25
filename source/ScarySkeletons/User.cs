using xn;
using System.Diagnostics;

namespace ScarySkeletons
{
    public class User : ViewModelBase
    {

        public User()
        {
            this.Head = new BindablePoint(-1000, -1000, 0);
        }

        uint userId;
        const string UserIdName = "UserId";
        public uint UserId
        {
            get { return this.userId; }
            set
            {
                this.userId = value;
                this.OnPropertyChanged(UserIdName);
            }
        }

        BindablePoint rightHandVector;
        const string RightHandVectorName = "RightHandVector";
        public BindablePoint RightHandVector
        {
            get { return this.rightHandVector; }
        }

        BindablePoint head;
        const string HeadName = "Head";
        public BindablePoint Head
        {
            get { return this.head; }
            set
            {
                this.head = value;
                this.OnPropertyChanged(HeadName);
            }
        }

        BindablePoint neck;
        const string NeckName = "Neck";
        public BindablePoint Neck
        {
            get { return this.neck; }
            set
            {
                this.neck = value;
                this.OnPropertyChanged(NeckName);
            }
        }

        BindablePoint leftHand;
        const string LeftHandName = "LeftHand";
        public BindablePoint LeftHand
        {
            get { return this.leftHand; }
            set
            {
                this.leftHand = value;
                this.OnPropertyChanged(LeftHandName);
            }
        }

        BindablePoint rightHand;
        const string RightHandName = "RightHand";
        public BindablePoint RightHand
        {
            get { return this.rightHand; }
            set
            {
                this.rightHand = value;
                //this.rightHandVector = new BindablePoint(
                //    this.rightHand.X - this.rightElbow.X,
                //    this.rightHand.Y - this.rightElbow.Y,
                //    0);
                this.OnPropertyChanged(RightHandName);
            }
        }

        BindablePoint rightElbow;
        const string RightElbowName = "RightElbow";
        public BindablePoint RightElbow
        {
            get { return this.rightElbow; }
            set
            {
                this.rightElbow = value;
                this.OnPropertyChanged(RightElbowName);
            }
        }

        BindablePoint leftElbow;
        const string LeftElbowName = "LeftElbow";
        public BindablePoint LeftElbow
        {
            get { return this.leftElbow; }
            set
            {
                this.leftElbow = value;
                this.OnPropertyChanged(LeftElbowName);
            }
        }

        BindablePoint leftShoulder;
        const string LeftShoulderName = "LeftShoulder";
        public BindablePoint LeftShoulder
        {
            get { return this.leftShoulder; }
            set
            {
                this.leftShoulder = value;
                this.OnPropertyChanged(LeftShoulderName);
            }
        }


        BindablePoint rightShoulder;
        const string RightShoulderName = "RightShoulder";
        public BindablePoint RightShoulder
        {
            get { return this.rightShoulder; }
            set
            {
                this.rightShoulder = value;
                this.OnPropertyChanged(RightShoulderName);
            }
        }

        BindablePoint torso;
        const string TorsoName = "Torso";
        public BindablePoint Torso
        {
            get { return this.torso; }
            set
            {
                this.torso = value;
                this.OnPropertyChanged(TorsoName);
            }
        }

        BindablePoint leftHip;
        const string LeftHipName = "LeftHip";
        public BindablePoint LeftHip
        {
            get { return this.leftHip; }
            set
            {
                this.leftHip = value;
                this.OnPropertyChanged(LeftHipName);
            }
        }


        BindablePoint rightHip;
        const string RightHipName = "RightHip";
        public BindablePoint RightHip
        {
            get { return this.rightHip; }
            set
            {
                this.rightHip = value;
                this.OnPropertyChanged(RightHipName);
            }
        }

        BindablePoint leftKnee;
        const string LeftKneeName = "LeftKnee";
        public BindablePoint LeftKnee
        {
            get { return this.leftKnee; }
            set
            {
                this.leftKnee = value;
                this.OnPropertyChanged(LeftKneeName);
            }
        }


        BindablePoint rightKnee;
        const string RightKneeName = "RightKnee";
        public BindablePoint RightKnee
        {
            get { return this.rightKnee; }
            set
            {
                this.rightKnee = value;
                this.OnPropertyChanged(RightKneeName);
            }
        }

    }
}
