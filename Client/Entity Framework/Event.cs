using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon
{
    public delegate void EntityEvent();

    public interface IEventContainer
    {
        Dictionary<string, EntityEvent> Events
        {
            get;
        }

        EntityEvent AddEvent(String name, EntityEvent callback);

        EntityEvent GetEvent(String name);

        bool HasEvent(String name);

        void InvokeEvent(String name);

        void RemoveEvent(String name);
    }
}
