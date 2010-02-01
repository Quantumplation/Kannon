using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{
    public class GlobalProperties : IPropertyContainer
    {
        /// <summary>
        /// Returns the Instance of this Singleton.
        /// </summary>
        /// <seealso cref="Singleton<T>"/>
        private static GlobalProperties instance;
        public static GlobalProperties Instance
        {
            get
            {
                // ?? is shorthand for (a == null ? a : b)
                // Therefore, this returns either instance, or, if instance is null, returns new T();
                return instance ?? (instance = new GlobalProperties());
            }
        }

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
        /// Constructor
        /// </summary>
        public GlobalProperties()
        {
            m_Properties = new Dictionary<string, IProperty>();
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
            m_Properties.Add(name, new Property<T>(defaultValue));
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
    }
}
