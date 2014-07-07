using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.BodyProcessor
{
    public interface IBodyEvaluator
    {
        /// <summary>
        /// Returns the flag to set if this body processor evaluates to true
        /// </summary>
        BodyAmbiguity FlagToSet
        {
            get; 
        }

        /// <summary>
        /// Evaluates the body to see if it matches the processor's conditions
        /// </summary>
        /// <param name="body">Body to evaluate</param>
        /// <returns>True if the body should be flagged, false if otherwise</returns>
        bool ShouldFlagBody(KinectBodyData body);
    }
}
