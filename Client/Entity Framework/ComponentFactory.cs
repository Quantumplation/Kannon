using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{
    /// <summary>
    /// Component Creation delegate.  Used in order to create a Component.
    /// </summary>
    /// <param name="ent">Entity to create the component on.</param>
    /// <param name="name">Name of the component.</param>
    /// <returns>Newly created component.</returns>
    public delegate Component ComponentCreation(Entity ent, string name);
    /// <summary>
    /// Component CreaTED delegate.  Used WHEN a component is created.
    /// </summary>
    /// <param name="c"></param>
    public delegate void ComponentCreated(Component c);

    namespace Components
    {
        /// <summary>
        /// Component Creator attribute for the automatic extraction of "Creation" methods.
        /// </summary>
        public class ComponentCreator : Attribute
        { }
    }

    public static class ComponentFactory
    {

        static Dictionary<String, ComponentCreation> m_FactoryMethods;
        static Dictionary<Type, ComponentCreated> m_CreationCallbacks;

        /// <summary>
        /// Constructor.
        /// </summary>
        static ComponentFactory()
        {
            m_FactoryMethods = new Dictionary<string, ComponentCreation>();
            m_CreationCallbacks = new Dictionary<Type, ComponentCreated>();
        }

        /// <summary>
        /// Create a component for @ent of @type, with @name.
        /// </summary>
        /// <param name="ent">Entity to create the component for.</param>
        /// <param name="type">Type of the component to create.</param>
        /// <param name="name">Name of the component to create, by default the same as type.</param>
        /// <returns>The newly created component, or null.</returns>
        public static Component Create(Entity ent, String type, String name = "")
        {
            if( name == "" )
                name = type;
            if (m_FactoryMethods.ContainsKey(type))
            {
                Component c = m_FactoryMethods[type](ent, name);
                foreach (Type t in m_CreationCallbacks.Keys)
                    if (c.GetType().GetInterface(t.Name) != null)
                        m_CreationCallbacks[t](c);
            }
            return null;
        }

        /// <summary>
        /// Register a factory method with the component factory.
        /// </summary>
        /// <param name="name">Name of the component type.</param>
        /// <param name="factory">Factory method.</param>
        public static void RegisterComponentType(String name, ComponentCreation factory, bool Overwrite = false)
        {
            if (Overwrite)
                m_FactoryMethods.Remove(name);
            if (!m_FactoryMethods.ContainsKey(name))
                m_FactoryMethods.Add(name, factory);
        }

        /// <summary>
        /// Register a callback for when a component which subclasses t gets created.  This allows, for example, Graphics to automatically pick up IRenderable components.
        /// </summary>
        /// <param name="t">Type to listen for.</param>
        /// <param name="callback">Callback to invoke when a component sublcassing t gets created.</param>
        public static void RegisterCreatedCallback<T>(ComponentCreated callback)
        {
            if (m_CreationCallbacks.ContainsKey(typeof(T)))
                m_CreationCallbacks[typeof(T)] += callback;
            else
                m_CreationCallbacks.Add(typeof(T), callback);
        }

        /// <summary>
        /// Register a type with the Component Factory.
        /// </summary>
        /// <param name="component">Type to strip for creation methods.</param>
        public static void RegisterComponentType(Type component)
        {
            // Make sure we're dealing with a Component type.
            if (component.IsSubclassOf(typeof(Component)))
            {
                // Check each method to see if it's a creator.
                foreach (MethodInfo m in component.GetMethods())
                {
                    // To be a creator, it must be static
                    if (m.IsStatic)
                        // And have a component creator attribute.
                        foreach (object o in m.GetCustomAttributes(false))
                            if (o is Components.ComponentCreator)
                            {
                                // So, register it, and return.
                                RegisterComponentType(component.Name, Delegate.CreateDelegate(typeof(ComponentCreation), m) as ComponentCreation);
                                return;
                            }
                }
            }
        }
    }
}
