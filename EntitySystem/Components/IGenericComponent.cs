using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon.EntitySystem.Components
{
    /// <summary>
    /// Represents a component which needs to be updated each frame.
    /// </summary>
    public interface IGenericComponent
    {
        /// <summary>
        /// Update this component.
        /// </summary>
        /// <param name="elapsedTime">Time that's elapsed since last frame.</param>
        void Update(float elapsedTime);
    }
}
