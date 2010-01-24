using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{

    public interface IPropertyContainer
    {
        Dictionary<String, IProperty> Properties
        {
            get;
        }

        Property<T> AddProperty<T>(String name, T defaultValue);
        IProperty AddIProperty(String name, IProperty property, bool Overwrite);

        Property<T> GetProperty<T>(String name);
        IProperty GetIProperty(String name);

        bool HasProperty<T>(String name);
        bool HasIProperty(String name);

        void RemoveProperty<T>(String name);
        void RemoveIProperty(String name);
    }

    public delegate void ValueChanged<T>(T oldValue, T newValue);

    public interface IProperty
    {
        Type Type
        {
            get;
        }
    }


    public class Property<T> : IProperty
    {
        T m_Internal;

        public Property(T value)
        {
            m_Internal = value;
        }
        
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        public event ValueChanged<T> ValueChanged;

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
