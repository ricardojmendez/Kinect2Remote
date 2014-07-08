﻿using System.Linq;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.BodyProcessor
{
    public class RightArmAbiguityProcessor : IBodyProcessor
    {
        bool IBodyProcessor.ProcessBody(KinectBodyData body)
        {
            // If at least four of the arm joints are inferred, as assume there's no arm
            var inferredCount = body.Joints.Count(x =>
                    x.TrackingState == KinectJointTrackingState.Inferred &&
                    (x.JointType == KinectJointType.ElbowRight ||
                     x.JointType == KinectJointType.WristRight ||
                     x.JointType == KinectJointType.HandRight ||
                     x.JointType == KinectJointType.HandTipRight ||
                     x.JointType == KinectJointType.ThumbRight));

            var isMissing = inferredCount >= 4;
            if (isMissing)
            {
                body.Ambiguity |= BodyAmbiguity.MissingLeftArm;
            }
            return isMissing;
        }
    }
}