using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace Kannon
{
    /// <summary>
    /// IComponentContainers store a multitude of components.
    /// </summary>
    public interface IComponentContainer
    {
        Dictionary<String, Component> Components
        {
            get;
        }

        /// <summary>
        /// Add a component to the entity, of type @type, and with name @name
        /// </summary>
        /// <param name="type">Type of the component to add, as registered with the component factory.</param>
        /// <param name="name">The name of the component, default: same as type</param>
        /// <returns>The newly constructed (and added) component.</returns>
        Component AddComponent(string type, string name = "");

        /// <summary>
        /// Retrieve an already existing component.
        /// </summary>
        /// <param name="name">Name of the component</param>
        /// <returns>Either the registered component, or null.</returns>
        Component GetComponent(string name);

        /// <summary>
        /// Determines whether this entity has a component of @name.
        /// </summary>
        /// <param name="name">Name of the component</param>
        /// <returns>True or False, depending on if the component is registered with this entity.</returns>
        bool HasComponent(string name);

        /// <summary>
        /// Remove a component from this entity.
        /// </summary>
        /// <param name="name">Name of the component to remove.</param>
        void RemoveComponent(string name);
    }

    /// <summary>
    /// A component.  Changes the behavior of an entity in some way.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Entity assosciated with this Component.
        /// </summary>
        public Entity Entity
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of this component.
        /// </summary>
        public String Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Component(Entity ent, String name)
        {
            Entity = ent;
            Name = name;
        }

        /// <summary>
        /// Parse an XMLNode, pulling component specific data out.
        /// </summary>
        public abstract void Parse(XmlNode data);
    }
}
