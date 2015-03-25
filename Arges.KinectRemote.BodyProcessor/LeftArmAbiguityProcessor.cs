using System.Linq;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.BodyProcessor
{
    public class LeftArmAbiguityProcessor: ABodyProcessor
    {
        protected override bool ProcessBody(KinectBody body)
        {
            // If at least three of the arm joints are inferred, as assume there's no arm
            var inferredCount = body.Joints.Count(x =>
                    x.TrackingState == KinectTrackingState.Inferred &&
                    (x.JointType == KinectJointType.ElbowLeft ||
                     x.JointType == KinectJointType.WristLeft ||
                     x.JointType == KinectJointType.HandLeft ||
                     x.JointType == KinectJointType.HandTipLeft ||
                     x.JointType == KinectJointType.ThumbLeft));

            var isMissing = inferredCount >= 4;
            if (isMissing)
            {
                body.Tags.Add("LeftArmMissing");
            }
            return isMissing;
        }
    }
}
