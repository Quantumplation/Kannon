using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{

    /// <summary>
    /// An IEventContainer provides facilities for the storage of Events.
    /// </summary>
    public interface IEventContainer
    {
        /// <summary>
        /// Events registered.
        /// </summary>
        Dictionary<string, EntityEvent> Events
        {
            get;
        }

        /// <summary>
        /// Add an event to the entity.
        /// </summary>
        /// <param name="name">Name of the event to add.</param>
        /// <param name="callback">Default callback.</param>
        /// <returns>Delegate representing the event.</returns>
        EntityEvent AddEvent(String name, EntityEvent callback);

        /// <summary>
        /// Retrieve an event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Either the event with @name, or null.</returns>
        EntityEvent GetEvent(String name);

        /// <summary>
        /// Determine whether the event is registered with this entity.
        /// </summary>
        /// <param name="name">Event to look for.</param>
        /// <returns>True or false, depending on wehther the entity contains an event with @name.</returns>
        bool HasEvent(String name);

        /// <summary>
        /// Cause the eent to be invoked, triggering all registered callbacks.
        /// </summary>
        /// <param name="name">Name of the event to invoke.</param>
        /// <param name="data">Data to pass to the event.</param>
        void InvokeEvent(String name, object data);

        /// <summary>
        /// Remove an event from this entity.
        /// </summary>
        /// <param name="name">Name of the event to remove.</param>
        void RemoveEvent(String name);
    }

    /// <summary>
    /// An event registered with an IEventContainer.  This is something that Can Happen, either TO the entity or BY the entity.
    /// </summary>
    /// <param name="Data">Data passed along with the event.</param>
    public delegate void EntityEvent(object Data);
}
