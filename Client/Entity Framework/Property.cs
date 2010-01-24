using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{

    /// <summary>
    /// Provides facilities for storing properties.
    /// </summary>
    public interface IPropertyContainer
    {
        /// <summary>
        /// Properties registered with the Entity.
        /// </summary>
        Dictionary<String, IProperty> Properties
        {
            get;
        }

        /// <summary>
        /// Add a typed property to the entity.
        /// </summary>
        /// <typeparam name="T">Type of data the property will store.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="defaultValue">Default value to use, if the property does not already exist.</param>
        /// <returns>The property with @name.</returns>
        Property<T> AddProperty<T>(String name, T defaultValue);        
        /// <summary>
        /// Add a generic property to the entity.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="property">Property to add.</param>
        /// <param name="Overwrite">If true, an existing property with @name will be overwritten.</param>
        /// <returns>The property with @name attached to this entity.</returns>
        IProperty AddIProperty(String name, IProperty property, bool Overwrite);

        /// <summary>
        /// Retrieve a typed property from this entity.
        /// </summary>
        /// <typeparam name="T">Type of data the property stores.</typeparam>
        /// <param name="name">Name of the property</param>
        /// <returns>The property with name, as a Property{T}, or null."/></returns>
        Property<T> GetProperty<T>(String name);
        /// <summary>
        /// Retrieve a generic property from this entity.
        /// </summary>
        /// <param name="name">Name of the property to retrieve.</param>
        /// <returns>The named property, or null.</returns>
        IProperty GetIProperty(String name);

        /// <summary>
        /// Determine whether this entity contains a typed property with @name, and is of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of data the property stores.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>True if the property exists and is of the correct type, false otherwise.</returns>
        bool HasProperty<T>(String name);
        /// <summary>
        /// Determine whether this entity contains a generic property with @name.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>True if the property is registered with this entity.</returns>
        bool HasIProperty(String name);

        /// <summary>
        /// Remove a property if it exists and is of the correct type.
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="name">Name of the property</param>
        void RemoveProperty<T>(String name);
        /// <summary>
        /// Remove a property if it exists.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        void RemoveIProperty(String name);
    }


    /// <summary>
    /// A generic property.
    /// </summary>
    public interface IProperty
    {
        Type Type
        {
            get;
        }
    }

    /// <summary>
    /// Value Changed delegate.  Invoked when a proeprties value has changed.
    /// </summary>
    /// <typeparam name="T">Type of data the property stores.</typeparam>
    /// <param name="oldValue">Previous value stored.</param>
    /// <param name="newValue">New value stored.</param>
    public delegate void ValueChanged<T>(T oldValue, T newValue);

    /// <summary>
    /// A typed property.  More useful than generic property.
    /// </summary>
    /// <typeparam name="T">Type of data this property holds.</typeparam>
    public class Property<T> : IProperty
    {
        T m_Internal;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Default value</param>
        public Property(T value)
        {
            m_Internal = value;
        }
        
        /// <summary>
        /// Type of data this property stores.
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Value Changed event, invoked when the value of this property is assigned too.
        /// </summary>
        public event ValueChanged<T> ValueChanged;

        /// <summary>
        /// Value stored in this property.
        /// </summary>
        public T Value
        {
            get
            {
                return m_Internal;
            }
            set
            {
                if (ValueChanged != null)
                    ValueChanged(m_Internal, value);
                m_Internal = value;
            }
        }
    }
}
