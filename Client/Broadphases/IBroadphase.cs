using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;

namespace Kannon
{
    public interface IBroadphase
    {
        /// <summary>
        /// Register a component with this broadphase.
        /// </summary>
        /// <param name="c">Component to register.</param>
        void RegisterComponent(Component c);

        /// <summary>
        /// Remove a component from this broadphase.
        /// </summary>
        /// <param name="c">Component to remove.</param>
        void RemoveComponent(Component c);

        /// <summary>
        /// Perform the broadphase logic.
        /// </summary>
        /// <param name="elapsedTime">Time that's elapsed since the last frame.</param>
        void Do(float elapsedTime);

        /// <summary>
        /// Represents how often the input should be updated.
        /// </summary>
        float ExecutionFrequency
        {
            get;
            set;
        }

        /// <summary>
        /// Represents whether it is getting too little execution time (less than half as often as it's execution
        /// frequency)
        /// </summary>
        bool ExecutingTooSlowly
        {
            get;
        }
    }
}
