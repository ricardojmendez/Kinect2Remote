﻿using Arges.KinectRemote.Data;

namespace Arges.KinectRemote.BodyProcessor
{
    public interface IBodyProcessor
    {
        /// <summary>
        /// Evaluates the body to see if it matches the processor's conditions,
        /// and if so processes it (possibly setting flags or altering other
        /// values)
        /// </summary>
        /// <param name="body">Body to evaluate</param>
        /// <returns>True if the body was altered, false if otherwise</returns>
        bool ProcessBody(KinectBodyData body);
    }
}
