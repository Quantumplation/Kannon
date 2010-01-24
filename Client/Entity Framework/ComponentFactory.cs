using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{
    /// <summary>
    /// Component Creation
    /// </summary>
    /// <param name="ent">Entity to create the component on.</param>
    /// <param name="name">Name of the component.</param>
    /// <returns>Newly created component.</returns>
    public delegate Component ComponentCreation(Entity ent, string name);

    /// <summary>
    /// Component Creator attribute for the automatic extraction of "Creation" methods.
    /// </summary>
    public class ComponentCreator : Attribute
    {}

    public static class ComponentFactory
    {
        static Dictionary<String, ComponentCreation> m_FactoryMethods;

        /// <summary>
        /// Constructor.
        /// </summary>
        static ComponentFactory()
        {
            m_FactoryMethods = new Dictionary<string, ComponentCreation>();
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
                return m_FactoryMethods[type](ent, name);
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
                            if (o is ComponentCreator)
                            {
                                // So, register it, and return.
                                RegisterComponentType(component.Name, Delegate.CreateDelegate(component, m) as ComponentCreation);
                                return;
                            }
                }
            }
        }
    }
}
