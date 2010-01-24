using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{
    public delegate Component ComponentCreation(Entity ent, string name);

    public class ComponentCreator : Attribute
    {}

    public static class ComponentFactory
    {
        static Dictionary<String, ComponentCreation> m_FactoryMethods;

        static ComponentFactory()
        {
            m_FactoryMethods = new Dictionary<string, ComponentCreation>();
        }

        public static Component Create(Entity ent, String type, String name = "")
        {
            if( name == "" )
                name = type;
            if (m_FactoryMethods.ContainsKey(type))
                return m_FactoryMethods[type](ent, name);
            return null;
        }

        public static void RegisterComponentType(String name, ComponentCreation factory)
        {
            if (!m_FactoryMethods.ContainsKey(name))
                m_FactoryMethods.Add(name, factory);
        }

        public static void RegisterComponentType(Type component)
        {
            if (component.IsSubclassOf(typeof(Component)))
            {
                foreach (MethodInfo m in component.GetMethods())
                {
                    if (m.IsStatic)
                        foreach (object o in m.GetCustomAttributes(false))
                            if (o is ComponentCreator)
                            {
                                RegisterComponentType(component.Name, Delegate.CreateDelegate(component, m) as ComponentCreation);
                                return;
                            }
                }
            }
        }
    }
}
