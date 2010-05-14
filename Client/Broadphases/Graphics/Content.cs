using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Kannon.EntitySystem;

namespace Kannon.Broadphases
{
    /// <summary>
    /// Keeps track of anything that implements IContent and makes sure they get loaded.
    /// </summary>
    class Content : IBroadphase
    {
        // Stores Components, and a ContentManager.
        List<Components.IContent> m_Components;
        ContentManager m_ContentManager;

        // Have we already loaded?
        // The reason I store this, is that when a component gets
        // registered with me, if we haven't loaded yet, don't call it's load
        // function.  That way, in early setup, everything gets batch loaded,
        // not on creation.  If we HAVE loaded, though, this new object needs
        // to get it's load function called.
        private bool Loaded = false;

        public Content(ContentManager cm)
        {
            m_Components = new List<Components.IContent>();
            m_ContentManager = cm;

            // Register a callback with the component factory for all IContent components.
            // When an IContent component gets created, ComponentFactory calls our "RegisterComponent" function.
            ComponentFactory.RegisterCreatedCallback<Components.IContent>(RegisterComponent);

            // Load every time the XNA game does.
            XNAGame.Instance.LoadEvent += Load;
        }

        /// <summary>
        /// Load everything.
        /// </summary>
        public void Load()
        {
            foreach (Components.IContent c in m_Components)
                c.Load(m_ContentManager);
        }

        /// <summary>
        /// Register a component.
        /// </summary>
        /// <param name="c"></param>
        public void RegisterComponent(Component c)
        {
            // If we've already loaded, load this component too.
            // Otherwise, it can wait until everything loads.
            if (Loaded)
                (c as Components.IContent).Load(m_ContentManager);
            m_Components.Add(c as Components.IContent);
        }

        public void RemoveComponent(Component c)
        {
            m_Components.Remove(c as Components.IContent);
        }

        /// <summary>
        /// Every broadphase needs a Do function, but this one does nothing.
        /// </summary>
        /// <param name="elapsedTime"></param>
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
