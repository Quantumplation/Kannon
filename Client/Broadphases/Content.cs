using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;

namespace Kannon.Broadphases
{
    class Content : IBroadphase
    {
        List<Components.IContentComponent> m_Components;
        ContentManager m_ContentManager;

        private bool Loaded = false;

        public Content(ContentManager cm)
        {
            m_Components = new List<Components.IContentComponent>();
            m_ContentManager = cm;

            ComponentFactory.RegisterCreatedCallback<Components.IContentComponent>(RegisterComponent);

            XNAGame.Instance.LoadEvent += Load;
        }

        public void Load()
        {
            foreach (Components.IContentComponent c in m_Components)
                c.Load(m_ContentManager);
        }

        public void RegisterComponent(Component c)
        {
            if (Loaded)
                (c as Components.IContentComponent).Load(m_ContentManager);
            m_Components.Add(c as Components.IContentComponent);
        }

        public void RemoveComponent(Component c)
        {
            m_Components.Remove(c as Components.IContentComponent);
        }

        public void Do(float elapsedTime)
        {
            return;
        }

        public float ExecutionFrequency
        {
            get;
            set;
        }

        public bool ExecutingTooSlowly
        {
            get;
            protected set;
        }
    }
}
