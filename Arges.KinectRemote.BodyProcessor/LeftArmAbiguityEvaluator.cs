﻿using System.Linq;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.BodyProcessor
{
    public class LeftArmAbiguityEvaluator: IBodyEvaluator
    {
        BodyAmbiguity IBodyEvaluator.FlagToSet
        {
            get { return BodyAmbiguity.MissingLeftArm; }
        }

        bool IBodyEvaluator.ShouldFlagBody(KinectBodyData body)
        {
            // If at least three of the arm joints are inferred, as assume there's no arm
            var inferredCount = body.Joints.Count(x =>
                    x.TrackingState == KinectJointTrackingState.Inferred &&
                    (x.JointType == KinectJointType.ElbowLeft ||
                     x.JointType == KinectJointType.WristLeft ||
                     x.JointType == KinectJointType.HandLeft ||
                     x.JointType == KinectJointType.HandTipLeft ||
                     x.JointType == KinectJointType.ThumbLeft));
            return inferredCount >= 4; 
        }
    }
}