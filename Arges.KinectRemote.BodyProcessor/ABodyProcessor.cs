using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arges.KinectRemote.BodyProcessor
{
    public abstract class ABodyProcessor
    {
        /// <summary>
        /// Processes a body
        /// </summary>
        /// <param name="body">Body to process</param>
        /// <returns>Returns true if the body was altered</returns>
        protected abstract bool ProcessBody(Data.KinectBody body);

        /// <summary>
        /// Processes a list of bodies
        /// </summary>
        /// <param name="bodies">Enumerable of bodies to process</param>
        public virtual void ProcessBodies(IEnumerable<Data.KinectBody> bodies)
        {
            foreach (var body in bodies)
            {
                ProcessBody(body);
            }
        }
    }
}
