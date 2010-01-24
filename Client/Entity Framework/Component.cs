using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{
    public interface IComponentContainer
    {
        Dictionary<String, Component> Components
        {
            get;
        }

        Component AddComponent(string type, string name = "");

        Component GetComponent(string name);

        bool HasComponent(string name);

        void RemoveComponent(string name);
    }

    public class Component
    {
    }
}
