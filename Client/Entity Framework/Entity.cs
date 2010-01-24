using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{
    public class Entity : IPropertyContainer, IComponentContainer, IEventContainer
    {
        public Entity()
        {
            m_Events = new Dictionary<string, EntityEvent>();
            m_Components = new Dictionary<string, Component>();
            m_Properties = new Dictionary<string, IProperty>();
        }

        #region Components

        Dictionary<string, Component> m_Components;
        public Dictionary<string, Component> Components
        {
            get
            {
                return m_Components;
            }
        }

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

        public Component GetComponent(string name)
        {
            if( m_Components.ContainsKey(name))
                return m_Components[name];
            return null;
        }

        public bool HasComponent(string name)
        {
            return m_Components.ContainsKey(name);
        }

        public void RemoveComponent(string name)
        {
            if (m_Components.ContainsKey(name))
                m_Components.Remove(name);
        }

        #endregion

        #region Events

        Dictionary<String, EntityEvent> m_Events;
        public Dictionary<string, EntityEvent> Events
        {
            get
            {
                return m_Events;
            }
        }

        public EntityEvent AddEvent(String name, EntityEvent callback)
        {
            if (m_Events.ContainsKey(name))
                return m_Events[name];
            EntityEvent e = new EntityEvent(callback);
            m_Events.Add(name, e);
            return e;
        }

        public EntityEvent GetEvent(String name)
        {
            if (m_Events.ContainsKey(name))
                return m_Events[name];
            return null;
        }

        public bool HasEvent(String name)
        {
            return m_Events.ContainsKey(name);
        }

        public void InvokeEvent(String name)
        {
            if (m_Events.ContainsKey(name))
                m_Events[name]();
        }

        public void RemoveEvent(String name)
        {
            if (m_Events.ContainsKey(name))
                m_Events.Remove(name);
        }

        #endregion

        #region Properties

        Dictionary<String, IProperty> m_Properties;
        public Dictionary<string, IProperty> Properties
        {
            get
            {
                return m_Properties;
            }
        }

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
        public IProperty AddIProperty(String name, IProperty property, bool Overwrite = false)
        {
            if (Overwrite)
                m_Properties.Remove(name);

            if (!m_Properties.ContainsKey(name))
                m_Properties.Add(name, property);

            return property;
        }

        public Property<T> GetProperty<T>(String name)
        {
            if (m_Properties.ContainsKey(name) && m_Properties[name].Type == typeof(T))
                return m_Properties[name] as Property<T>;
            return null;
        }
        public IProperty GetIProperty(String name)
        {
            if (m_Properties.ContainsKey(name))
                return m_Properties[name];
            return null;
        }

        public bool HasProperty<T>(String name)
        {
            return m_Properties.ContainsKey(name) && m_Properties[name].Type == typeof(T);
        }
        public bool HasIProperty(String name)
        {
            return m_Properties.ContainsKey(name);
        }

        public void RemoveProperty<T>(String name)
        {
            if (m_Properties.ContainsKey(name) && m_Properties[name].Type == typeof(T))
                m_Properties.Remove(name);
        }
        public void RemoveIProperty(String name)
        {
            if (m_Properties.ContainsKey(name))
                m_Properties.Remove(name);
        }

        #endregion
    }
}
