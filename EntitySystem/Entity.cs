using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon.EntitySystem
{
    /// <summary>
    /// Representation of something in the game world.
    /// </summary>
    /// <remarks>
    /// An entity holds Components, Events, and Properties.
    /// </remarks>
    public class Entity : IComponentContainer, IEventContainer, IPropertyContainer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Entity()
        {
            m_Events = new Dictionary<string, EntityEvent>();
            m_Components = new Dictionary<string, Component>();
            m_Properties = new Dictionary<string, IProperty>();
        }

        #region Components

        /// <summary>
        /// Components attached to the entity.
        /// </summary>
        Dictionary<string, Component> m_Components;
        public Dictionary<string, Component> Components
        {
            get
            {
                return m_Components;
            }
        }

        /// <summary>
        /// Add a component to the entity, of type @type, and with name @name
        /// </summary>
        /// <param name="type">Type of the component to add, as registered with the component factory.</param>
        /// <param name="name">The name of the component, default: same as type</param>
        /// <returns>The newly constructed (and added) component.</returns>
        public Component AddComponent(string type, string name = "")
        {
            if( name == "" )
                name = type;
            if (m_Components.ContainsKey(name))
                return m_Components[name];

            Component component = ComponentFactory.Create(this, type, name);
            m_Components.Add(name, component);
            return component;
        }

        /// <summary>
        /// Retrieve an already existing component.
        /// </summary>
        /// <param name="name">Name of the component</param>
        /// <returns>Either the registered component, or null.</returns>
        public Component GetComponent(string name)
        {
            if( m_Components.ContainsKey(name))
                return m_Components[name];
            return null;
        }

        /// <summary>
        /// Determines whether this entity has a component of @name.
        /// </summary>
        /// <param name="name">Name of the component</param>
        /// <returns>True or False, depending on if the component is registered with this entity.</returns>
        public bool HasComponent(string name)
        {
            return m_Components.ContainsKey(name);
        }

        /// <summary>
        /// Remove a component from this entity.
        /// </summary>
        /// <param name="name">Name of the component to remove.</param>
        public void RemoveComponent(string name)
        {
            if (m_Components.ContainsKey(name))
                m_Components.Remove(name);
        }

        #endregion

        #region Events

        /// <summary>
        /// Events registered with the entity.
        /// </summary>
        Dictionary<String, EntityEvent> m_Events;
        public Dictionary<string, EntityEvent> Events
        {
            get
            {
                return m_Events;
            }
        }

        /// <summary>
        /// Add an event to the entity.
        /// </summary>
        /// <param name="name">Name of the event to add.</param>
        /// <param name="callback">Default callback.</param>
        /// <returns>Delegate representing the event.</returns>
        public EntityEvent AddEvent(String name, EntityEvent callback)
        {
            if (m_Events.ContainsKey(name))
            {
                m_Events[name] += callback;
                return m_Events[name];
            }
            EntityEvent e = new EntityEvent(callback);
            m_Events.Add(name, e);
            return e;
        }

        /// <summary>
        /// Retrieve an event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>Either the event with @name, or null.</returns>
        public EntityEvent GetEvent(String name)
        {
            if (m_Events.ContainsKey(name))
                return m_Events[name];
            return null;
        }

        /// <summary>
        /// Determine whether the event is registered with this entity.
        /// </summary>
        /// <param name="name">Event to look for.</param>
        /// <returns>True or false, depending on wehther the entity contains an event with @name.</returns>
        public bool HasEvent(String name)
        {
            return m_Events.ContainsKey(name);
        }

        /// <summary>
        /// Cause the eent to be invoked, triggering all registered callbacks.
        /// </summary>
        /// <param name="name">Name of the event to invoke.</param>
        /// <param name="data">Data to pass to the event.</param>
        public void InvokeEvent(String name, Object data)
        {
            if (m_Events.ContainsKey(name))
                m_Events[name](data);
        }

        /// <summary>
        /// Remove an event from this entity.
        /// </summary>
        /// <param name="name">Name of the event to remove.</param>
        public void RemoveEvent(String name)
        {
            if (m_Events.ContainsKey(name))
                m_Events.Remove(name);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Properties registered with the Entity.
        /// </summary>
        Dictionary<String, IProperty> m_Properties;
        public Dictionary<string, IProperty> Properties
        {
            get
            {
                return m_Properties;
            }
        }

        /// <summary>
        /// Add a typed property to the entity.
        /// </summary>
        /// <typeparam name="T">Type of data the property will store.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="defaultValue">Default value to use, if the property does not already exist.</param>
        /// <returns>The property with @name.</returns>
        public Property<T> AddProperty<T>(String name, T defaultValue = default(T))
        {
            if (m_Properties.ContainsKey(name))
            {
                if( m_Properties[name].Type == typeof(T) )
                    return m_Properties[name] as Property<T>;
                throw new Exception("Property exists, but is of a different type.");
            }
            Property<T> property = new Property<T>(defaultValue);
            m_Properties.Add(name, property);
            return property;
        }
        /// <summary>
        /// Add a generic property to the entity.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="property">Property to add.</param>
        /// <param name="Overwrite">If true, an existing property with @name will be overwritten.</param>
        /// <returns>The property with @name attached to this entity.</returns>
        public IProperty AddIProperty(String name, IProperty property, bool Overwrite = false)
        {
            if (Overwrite)
                m_Properties.Remove(name);

            if (!m_Properties.ContainsKey(name))
                m_Properties.Add(name, property);

            return property;
        }

        /// <summary>
        /// Retrieve a typed property from this entity.
        /// </summary>
        /// <typeparam name="T">Type of data the property stores.</typeparam>
        /// <param name="name">Name of the property</param>
        /// <returns>The property with name, as a Property{T}, or null."/></returns>
        public Property<T> GetProperty<T>(String name)
        {
            if (m_Properties.ContainsKey(name) && m_Properties[name].Type == typeof(T))
                return m_Properties[name] as Property<T>;
            return null;
        }
        /// <summary>
        /// Retrieve a generic property from this entity.
        /// </summary>
        /// <param name="name">Name of the property to retrieve.</param>
        /// <returns>The named property, or null.</returns>
        public IProperty GetIProperty(String name)
        {
            if (m_Properties.ContainsKey(name))
                return m_Properties[name];
            return null;
        }

        /// <summary>
        /// Determine whether this entity contains a typed property with @name, and is of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of data the property stores.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>True if the property exists and is of the correct type, false otherwise.</returns>
        public bool HasProperty<T>(String name)
        {
            return m_Properties.ContainsKey(name) && m_Properties[name].Type == typeof(T);
        }
        /// <summary>
        /// Determine whether this entity contains a generic property with @name.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>True if the property is registered with this entity.</returns>
        public bool HasIProperty(String name)
        {
            return m_Properties.ContainsKey(name);
        }

        /// <summary>
        /// Remove a property if it exists and is of the correct type.
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="name">Name of the property</param>
        public void RemoveProperty<T>(String name)
        {
            if (m_Properties.ContainsKey(name) && m_Properties[name].Type == typeof(T))
                m_Properties.Remove(name);
        }
        /// <summary>
        /// Remove a property if it exists.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        public void RemoveIProperty(String name)
        {
            if (m_Properties.ContainsKey(name))
                m_Properties.Remove(name);
        }

        #endregion
    }
}
